using System.Text.Json;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Validation;
using Microsoft.Extensions.Logging;
using FhirIntegrationService.ValidationPipeline.Models;
using System.Diagnostics;

namespace FhirIntegrationService.ValidationPipeline.Services;

/// <summary>
/// Service for validating FHIR profiles and structure definitions
/// </summary>
public interface IFhirProfileValidator
{
    /// <summary>
    /// Validates all FHIR profiles in the specified directory
    /// </summary>
    Task<TechnicalValidationResult> ValidateProfilesAsync(
        string profileDirectory,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a single FHIR profile
    /// </summary>
    Task<ProfileValidationResult> ValidateProfileAsync(
        string profilePath,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of FHIR profile validator using Firely .NET SDK
/// </summary>
public class FhirProfileValidator : IFhirProfileValidator
{
    private readonly ILogger<FhirProfileValidator> _logger;
    private readonly FhirJsonParser _jsonParser;
    private readonly FhirXmlParser _xmlParser;
    private readonly Validator _validator;

    public FhirProfileValidator(ILogger<FhirProfileValidator> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Initialize FHIR parsers with default settings
        _jsonParser = new FhirJsonParser();
        _xmlParser = new FhirXmlParser();

        // Initialize validator with FHIR R4 specification
        _validator = new Validator(ValidationSettings.CreateDefault());

        _logger.LogInformation("FhirProfileValidator initialized with FHIR R4 validation");
    }

    /// <inheritdoc/>
    public async Task<TechnicalValidationResult> ValidateProfilesAsync(
        string profileDirectory,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting FHIR profile validation in directory: {ProfileDirectory}", profileDirectory);

        var result = new TechnicalValidationResult();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            if (!Directory.Exists(profileDirectory))
            {
                _logger.LogError("Profile directory does not exist: {ProfileDirectory}", profileDirectory);
                result.Status = ValidationStatus.Error;
                return result;
            }

            // Find all FHIR profile files
            var profileFiles = Directory.GetFiles(profileDirectory, "*.json", SearchOption.AllDirectories)
                .Concat(Directory.GetFiles(profileDirectory, "*.xml", SearchOption.AllDirectories))
                .Where(IsProfileFile)
                .ToList();

            _logger.LogInformation("Found {ProfileCount} profile files to validate", profileFiles.Count);

            // Validate each profile
            var validationTasks = profileFiles.Select(async profilePath =>
            {
                try
                {
                    return await ValidateProfileAsync(profilePath, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error validating profile: {ProfilePath}", profilePath);
                    return new ProfileValidationResult
                    {
                        ProfilePath = profilePath,
                        Status = ValidationStatus.Error,
                        ValidationIssues = new List<ValidationIssue>
                        {
                            new()
                            {
                                Severity = "Error",
                                Code = "VALIDATION_EXCEPTION",
                                Description = $"Validation failed with exception: {ex.Message}",
                                Location = profilePath,
                                Recommendation = "Check file format and content"
                            }
                        }
                    };
                }
            });

            result.ProfileValidationResults = (await Task.WhenAll(validationTasks)).ToList();

            // Perform additional validations
            result.CanonicalUrlValidation = await ValidateCanonicalUrlsAsync(result.ProfileValidationResults, cancellationToken);
            result.MetadataValidation = await ValidateMetadataAsync(result.ProfileValidationResults, cancellationToken);
            result.ConstraintValidation = await ValidateConstraintsAsync(result.ProfileValidationResults, cancellationToken);

            // Determine overall status
            result.Status = DetermineOverallStatus(result);

            stopwatch.Stop();
            _logger.LogInformation(
                "FHIR profile validation completed in {Duration}ms. Status: {Status}",
                stopwatch.ElapsedMilliseconds,
                result.Status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error during profile validation");
            result.Status = ValidationStatus.Failed;
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<ProfileValidationResult> ValidateProfileAsync(
        string profilePath,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Validating profile: {ProfilePath}", profilePath);

        var stopwatch = Stopwatch.StartNew();
        var result = new ProfileValidationResult
        {
            ProfilePath = profilePath
        };

        try
        {
            if (!File.Exists(profilePath))
            {
                result.Status = ValidationStatus.Error;
                result.ValidationIssues.Add(new ValidationIssue
                {
                    Severity = "Error",
                    Code = "FILE_NOT_FOUND",
                    Description = "Profile file does not exist",
                    Location = profilePath,
                    Recommendation = "Verify the file path is correct"
                });
                return result;
            }

            // Read and parse the profile
            var profileContent = await File.ReadAllTextAsync(profilePath, cancellationToken);
            var structureDefinition = await ParseStructureDefinitionAsync(profileContent, profilePath);

            if (structureDefinition == null)
            {
                result.Status = ValidationStatus.Error;
                result.ValidationIssues.Add(new ValidationIssue
                {
                    Severity = "Error",
                    Code = "PARSE_ERROR",
                    Description = "Failed to parse StructureDefinition",
                    Location = profilePath,
                    Recommendation = "Verify the file contains valid FHIR JSON or XML"
                });
                return result;
            }

            // Extract profile metadata
            result.ProfileName = structureDefinition.Name ?? Path.GetFileNameWithoutExtension(profilePath);
            result.CanonicalUrl = structureDefinition.Url ?? string.Empty;

            // Validate the structure definition
            var validationResult = await _validator.ValidateAsync(structureDefinition);

            // Convert validation issues
            result.ValidationIssues = ConvertValidationIssues(validationResult.Issue);

            // Determine validation status
            result.Status = DetermineValidationStatus(result.ValidationIssues);

            stopwatch.Stop();
            result.ValidationDuration = stopwatch.Elapsed;

            _logger.LogDebug(
                "Profile validation completed: {ProfileName} - Status: {Status} - Issues: {IssueCount}",
                result.ProfileName,
                result.Status,
                result.ValidationIssues.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating profile: {ProfilePath}", profilePath);
            result.Status = ValidationStatus.Error;
            result.ValidationIssues.Add(new ValidationIssue
            {
                Severity = "Error",
                Code = "VALIDATION_EXCEPTION",
                Description = $"Validation failed with exception: {ex.Message}",
                Location = profilePath,
                Recommendation = "Check file format and content"
            });
        }

        return result;
    }

    private async Task<StructureDefinition?> ParseStructureDefinitionAsync(string content, string filePath)
    {
        try
        {
            // Determine file format and parse accordingly
            if (filePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                return await Task.FromResult(_jsonParser.Parse<StructureDefinition>(content));
            }
            else if (filePath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                return await Task.FromResult(_xmlParser.Parse<StructureDefinition>(content));
            }
            else
            {
                // Try JSON first, then XML
                try
                {
                    return await Task.FromResult(_jsonParser.Parse<StructureDefinition>(content));
                }
                catch
                {
                    return await Task.FromResult(_xmlParser.Parse<StructureDefinition>(content));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse StructureDefinition from file: {FilePath}", filePath);
            return null;
        }
    }

    private static bool IsProfileFile(string filePath)
    {
        // Check if file is likely a FHIR profile based on path or naming conventions
        var fileName = Path.GetFileName(filePath).ToLowerInvariant();
        var directory = Path.GetDirectoryName(filePath)?.ToLowerInvariant() ?? string.Empty;

        return fileName.Contains("structuredefinition") ||
               fileName.Contains("profile") ||
               directory.Contains("profiles") ||
               directory.Contains("structure") ||
               HasStructureDefinitionContent(filePath);
    }

    private static bool HasStructureDefinitionContent(string filePath)
    {
        try
        {
            var content = File.ReadAllText(filePath);
            return content.Contains("\"resourceType\": \"StructureDefinition\"") ||
                   content.Contains("<resourceType value=\"StructureDefinition\"");
        }
        catch
        {
            return false;
        }
    }

    private List<ValidationIssue> ConvertValidationIssues(List<OperationOutcome.IssueComponent>? issues)
    {
        if (issues == null) return new List<ValidationIssue>();

        return issues.Select(issue => new ValidationIssue
        {
            Severity = issue.Severity?.ToString() ?? "Unknown",
            Code = issue.Code?.ToString() ?? "Unknown",
            Description = issue.Diagnostics ?? issue.Details?.Text ?? "No description",
            Location = string.Join(", ", issue.Expression ?? new List<string>()),
            Recommendation = GenerateRecommendation(issue)
        }).ToList();
    }

    private static string GenerateRecommendation(OperationOutcome.IssueComponent issue)
    {
        return issue.Code?.ToString() switch
        {
            "required" => "Add the missing required element",
            "cardinality" => "Check element cardinality constraints",
            "invalid" => "Verify element value is valid for the specified type",
            "structure" => "Review profile structure and constraints",
            _ => "Review the FHIR specification for guidance"
        };
    }

    private static ValidationStatus DetermineValidationStatus(List<ValidationIssue> issues)
    {
        if (!issues.Any()) return ValidationStatus.Success;

        var hasErrors = issues.Any(i => i.Severity.Equals("Error", StringComparison.OrdinalIgnoreCase) ||
                                       i.Severity.Equals("Fatal", StringComparison.OrdinalIgnoreCase));
        if (hasErrors) return ValidationStatus.Error;

        var hasWarnings = issues.Any(i => i.Severity.Equals("Warning", StringComparison.OrdinalIgnoreCase));
        if (hasWarnings) return ValidationStatus.Warning;

        return ValidationStatus.Success;
    }

    private async Task<CanonicalUrlValidationResult> ValidateCanonicalUrlsAsync(
        List<ProfileValidationResult> profileResults,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Validating canonical URLs");

        var result = new CanonicalUrlValidationResult();
        var canonicalUrls = new HashSet<string>();
        var duplicates = new List<string>();

        foreach (var profileResult in profileResults)
        {
            if (string.IsNullOrEmpty(profileResult.CanonicalUrl))
            {
                result.Issues.Add(new ValidationIssue
                {
                    Severity = "Error",
                    Code = "MISSING_CANONICAL_URL",
                    Description = "Profile is missing canonical URL",
                    Location = profileResult.ProfilePath,
                    Recommendation = "Add a canonical URL to the StructureDefinition"
                });
                continue;
            }

            if (!canonicalUrls.Add(profileResult.CanonicalUrl))
            {
                duplicates.Add(profileResult.CanonicalUrl);
            }

            // Validate URL format
            if (!Uri.TryCreate(profileResult.CanonicalUrl, UriKind.Absolute, out _))
            {
                result.Issues.Add(new ValidationIssue
                {
                    Severity = "Error",
                    Code = "INVALID_CANONICAL_URL",
                    Description = "Canonical URL is not a valid URI",
                    Location = profileResult.ProfilePath,
                    Recommendation = "Ensure canonical URL is a valid absolute URI"
                });
            }
        }

        // Report duplicates
        foreach (var duplicate in duplicates.Distinct())
        {
            result.Issues.Add(new ValidationIssue
            {
                Severity = "Error",
                Code = "DUPLICATE_CANONICAL_URL",
                Description = $"Canonical URL '{duplicate}' is used by multiple profiles",
                Location = "Multiple files",
                Recommendation = "Ensure each profile has a unique canonical URL"
            });
        }

        result.Status = result.Issues.Any(i => i.Severity.Equals("Error", StringComparison.OrdinalIgnoreCase))
            ? ValidationStatus.Error
            : ValidationStatus.Success;

        await Task.CompletedTask; // Placeholder for async work
        return result;
    }

    private async Task<MetadataValidationResult> ValidateMetadataAsync(
        List<ProfileValidationResult> profileResults,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Validating profile metadata");

        var result = new MetadataValidationResult();

        foreach (var profileResult in profileResults)
        {
            if (string.IsNullOrEmpty(profileResult.ProfileName))
            {
                result.Issues.Add(new ValidationIssue
                {
                    Severity = "Warning",
                    Code = "MISSING_PROFILE_NAME",
                    Description = "Profile is missing a name",
                    Location = profileResult.ProfilePath,
                    Recommendation = "Add a name to the StructureDefinition for better identification"
                });
            }
        }

        result.Status = result.Issues.Any(i => i.Severity.Equals("Error", StringComparison.OrdinalIgnoreCase))
            ? ValidationStatus.Error
            : result.Issues.Any(i => i.Severity.Equals("Warning", StringComparison.OrdinalIgnoreCase))
                ? ValidationStatus.Warning
                : ValidationStatus.Success;

        await Task.CompletedTask; // Placeholder for async work
        return result;
    }

    private async Task<ConstraintValidationResult> ValidateConstraintsAsync(
        List<ProfileValidationResult> profileResults,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Validating profile constraints");

        var result = new ConstraintValidationResult
        {
            Status = ValidationStatus.Success
        };

        // Additional constraint validation logic would go here
        // For now, we'll assume constraints are validated as part of the main profile validation

        await Task.CompletedTask; // Placeholder for async work
        return result;
    }

    private static ValidationStatus DetermineOverallStatus(TechnicalValidationResult result)
    {
        var allResults = new List<ValidationStatus>
        {
            result.CanonicalUrlValidation.Status,
            result.MetadataValidation.Status,
            result.ConstraintValidation.Status
        };

        allResults.AddRange(result.ProfileValidationResults.Select(r => r.Status));

        if (allResults.Any(s => s == ValidationStatus.Error || s == ValidationStatus.Failed))
            return ValidationStatus.Error;

        if (allResults.Any(s => s == ValidationStatus.Warning))
            return ValidationStatus.Warning;

        return ValidationStatus.Success;
    }
}