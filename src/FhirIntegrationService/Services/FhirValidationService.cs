using FhirIntegrationService.Exceptions;
using FhirIntegrationService.Services.Interfaces;
using Hl7.Fhir.Model;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Validation;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace FhirIntegrationService.Services;

/// <summary>
/// Implementation of FHIR validation service using Firely SDK
/// </summary>
public class FhirValidationService : IFhirValidationService
{
    private readonly ILogger<FhirValidationService> _logger;
    private readonly FhirValidationConfiguration _configuration;
    private readonly Validator _validator;
    private readonly IResourceResolver _resolver;

    /// <summary>
    /// Initializes a new instance of FhirValidationService
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="configuration">FHIR validation configuration</param>
    /// <param name="resolver">Resource resolver for profiles</param>
    public FhirValidationService(
        ILogger<FhirValidationService> logger,
        IOptions<FhirValidationConfiguration> configuration,
        IResourceResolver resolver)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
        _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));

        // Initialize Firely SDK validator
        var settings = new ValidationSettings
        {
            ResourceResolver = _resolver,
            GenerateSnapshot = true,
            EnableXsdValidation = true,
            Trace = false
        };

        _validator = new Validator(settings);
    }

    /// <inheritdoc />
    public async Task<FhirValidationResult> ValidateResourceAsync(Resource resource, string profileUrl, CancellationToken cancellationToken = default)
    {
        if (resource == null)
            throw new ArgumentNullException(nameof(resource));

        if (string.IsNullOrWhiteSpace(profileUrl))
            throw new ArgumentException("Profile URL cannot be null or empty", nameof(profileUrl));

        var stopwatch = Stopwatch.StartNew();
        var result = new FhirValidationResult
        {
            ProfileUrl = profileUrl,
            ValidationTimestamp = DateTime.UtcNow
        };

        try
        {
            _logger.LogDebug("Starting FHIR validation for {ResourceType} against profile {ProfileUrl}",
                resource.TypeName, profileUrl);

            // Ensure the resource has the profile declared
            if (resource.Meta == null)
            {
                resource.Meta = new Meta();
            }

            if (!resource.Meta.Profile.Contains(profileUrl))
            {
                resource.Meta.Profile.Add(profileUrl);
            }

            // Perform validation
            var validationResult = await Task.Run(() =>
            {
                try
                {
                    return _validator.Validate(resource);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during FHIR validation process");
                    throw new FhirValidationException(
                        "FHIR validation process failed",
                        resource.TypeName,
                        profileUrl,
                        new List<string> { ex.Message },
                        ex);
                }
            }, cancellationToken);

            // Process validation results
            var hasErrors = false;
            foreach (var issue in validationResult.Issue)
            {
                var validationIssue = new ValidationIssue
                {
                    Severity = MapSeverity(issue.Severity),
                    Code = issue.Details?.Coding?.FirstOrDefault()?.Code,
                    Description = issue.Diagnostics,
                    Location = string.Join(", ", issue.Location ?? new List<string>())
                };

                result.Issues.Add(validationIssue);

                // Check if this is a blocking error
                if (issue.Severity == OperationOutcome.IssueSeverity.Error || issue.Severity == OperationOutcome.IssueSeverity.Fatal)
                {
                    hasErrors = true;
                }
            }

            result.IsValid = !hasErrors && (validationResult.Issue?.Count == 0 ||
                validationResult.Issue.All(i => i.Severity == OperationOutcome.IssueSeverity.Warning ||
                                                i.Severity == OperationOutcome.IssueSeverity.Information));

            stopwatch.Stop();
            result.ValidationDuration = stopwatch.Elapsed;

            _logger.LogDebug("FHIR validation completed for {ResourceType}. Valid: {IsValid}, Issues: {IssueCount}, Duration: {Duration}ms",
                resource.TypeName, result.IsValid, result.Issues.Count, stopwatch.ElapsedMilliseconds);

            // Log validation issues if any
            if (result.Issues.Any())
            {
                var errorCount = result.Issues.Count(i => i.Severity == IssueSeverity.Error || i.Severity == IssueSeverity.Fatal);
                var warningCount = result.Issues.Count(i => i.Severity == IssueSeverity.Warning);

                if (errorCount > 0)
                {
                    _logger.LogWarning("FHIR validation found {ErrorCount} errors and {WarningCount} warnings for {ResourceType}",
                        errorCount, warningCount, resource.TypeName);
                }
                else if (warningCount > 0)
                {
                    _logger.LogInformation("FHIR validation found {WarningCount} warnings for {ResourceType}",
                        warningCount, resource.TypeName);
                }
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("FHIR validation was cancelled for {ResourceType}", resource.TypeName);
            throw;
        }
        catch (FhirValidationException)
        {
            // Re-throw FHIR validation exceptions as-is
            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Unexpected error during FHIR validation for {ResourceType}. Duration: {Duration}ms",
                resource.TypeName, stopwatch.ElapsedMilliseconds);

            throw new FhirValidationException(
                "Unexpected error occurred during FHIR validation",
                resource.TypeName,
                profileUrl,
                new List<string> { ex.Message },
                ex);
        }
    }

    /// <inheritdoc />
    public async Task<FhirValidationResult> ValidateResourceAsync(Resource resource, CancellationToken cancellationToken = default)
    {
        if (resource == null)
            throw new ArgumentNullException(nameof(resource));

        var defaultProfileUrl = GetDefaultProfileUrl(resource.TypeName);
        return await ValidateResourceAsync(resource, defaultProfileUrl, cancellationToken);
    }

    /// <inheritdoc />
    public OperationOutcome CreateOperationOutcome(FhirValidationResult validationResult)
    {
        if (validationResult == null)
            throw new ArgumentNullException(nameof(validationResult));

        var operationOutcome = new OperationOutcome();

        foreach (var issue in validationResult.Issues)
        {
            var outcomeIssue = new OperationOutcome.IssueComponent
            {
                Severity = MapToFhirSeverity(issue.Severity),
                Code = GetIssueType(issue.Code),
                Diagnostics = issue.Description
            };

            if (!string.IsNullOrEmpty(issue.Location))
            {
                outcomeIssue.Location.Add(issue.Location);
            }

            if (!string.IsNullOrEmpty(issue.Code))
            {
                outcomeIssue.Details = new CodeableConcept
                {
                    Text = issue.Code
                };
            }

            operationOutcome.Issue.Add(outcomeIssue);
        }

        // Add summary issue if no specific issues were found but validation failed
        if (!validationResult.IsValid && !operationOutcome.Issue.Any())
        {
            operationOutcome.Issue.Add(new OperationOutcome.IssueComponent
            {
                Severity = OperationOutcome.IssueSeverity.Error,
                Code = OperationOutcome.IssueType.Invalid,
                Diagnostics = "Resource validation failed but no specific issues were identified"
            });
        }

        return operationOutcome;
    }

    /// <inheritdoc />
    public string GetDefaultProfileUrl(string resourceType)
    {
        if (string.IsNullOrWhiteSpace(resourceType))
            throw new ArgumentException("Resource type cannot be null or empty", nameof(resourceType));

        // Check if custom profile is configured
        if (_configuration.DefaultProfiles.TryGetValue(resourceType, out var customProfile))
        {
            return customProfile;
        }

        // Return base FHIR profile URL
        return $"http://hl7.org/fhir/StructureDefinition/{resourceType}";
    }

    /// <summary>
    /// Maps Firely SDK issue severity to internal severity
    /// </summary>
    private static IssueSeverity MapSeverity(OperationOutcome.IssueSeverity? severity)
    {
        return severity switch
        {
            OperationOutcome.IssueSeverity.Fatal => IssueSeverity.Fatal,
            OperationOutcome.IssueSeverity.Error => IssueSeverity.Error,
            OperationOutcome.IssueSeverity.Warning => IssueSeverity.Warning,
            OperationOutcome.IssueSeverity.Information => IssueSeverity.Information,
            _ => IssueSeverity.Information
        };
    }

    /// <summary>
    /// Maps internal severity to FHIR OperationOutcome severity
    /// </summary>
    private static OperationOutcome.IssueSeverity MapToFhirSeverity(IssueSeverity severity)
    {
        return severity switch
        {
            IssueSeverity.Fatal => OperationOutcome.IssueSeverity.Fatal,
            IssueSeverity.Error => OperationOutcome.IssueSeverity.Error,
            IssueSeverity.Warning => OperationOutcome.IssueSeverity.Warning,
            IssueSeverity.Information => OperationOutcome.IssueSeverity.Information,
            _ => OperationOutcome.IssueSeverity.Information
        };
    }

    /// <summary>
    /// Gets appropriate FHIR issue type based on error code
    /// </summary>
    private static OperationOutcome.IssueType GetIssueType(string? code)
    {
        if (string.IsNullOrEmpty(code))
            return OperationOutcome.IssueType.Invalid;

        return code.ToLowerInvariant() switch
        {
            "required" or "missing" => OperationOutcome.IssueType.Required,
            "value" or "invalid-value" => OperationOutcome.IssueType.Value,
            "structure" or "invalid-structure" => OperationOutcome.IssueType.Structure,
            "cardinality" => OperationOutcome.IssueType.Invariant,
            "profile" => OperationOutcome.IssueType.Invalid,
            _ => OperationOutcome.IssueType.Invalid
        };
    }
}

/// <summary>
/// Configuration for FHIR validation service
/// </summary>
public class FhirValidationConfiguration
{
    /// <summary>
    /// Dictionary of default profiles for each resource type
    /// </summary>
    public Dictionary<string, string> DefaultProfiles { get; set; } = new();

    /// <summary>
    /// Whether to enable strict validation mode
    /// </summary>
    public bool StrictValidation { get; set; } = true;

    /// <summary>
    /// Whether to generate snapshots during validation
    /// </summary>
    public bool GenerateSnapshot { get; set; } = true;

    /// <summary>
    /// Whether to enable XSD validation
    /// </summary>
    public bool EnableXsdValidation { get; set; } = true;

    /// <summary>
    /// Maximum validation time in milliseconds
    /// </summary>
    public int MaxValidationTimeMs { get; set; } = 5000;
}