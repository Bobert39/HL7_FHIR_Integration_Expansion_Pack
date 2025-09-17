using FhirIntegrationService.ValidationSuite.Interfaces;
using FhirIntegrationService.ValidationSuite.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Validation;
using Microsoft.Extensions.Logging;
using Polly;
using System.Diagnostics;
using System.Text.Json;

namespace FhirIntegrationService.ValidationSuite.Services;

/// <summary>
/// FHIR resource validator implementation using Firely .NET SDK
/// </summary>
public class FhirResourceValidator : IFhirResourceValidator
{
    private readonly ILogger<FhirResourceValidator> _logger;
    private readonly FhirJsonParser _jsonParser;
    private readonly FhirXmlParser _xmlParser;
    private readonly Validator _validator;
    private readonly ResilienceStrategy _resilienceStrategy;

    public FhirResourceValidator(ILogger<FhirResourceValidator> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jsonParser = new FhirJsonParser();
        _xmlParser = new FhirXmlParser();
        _validator = new Validator();

        // Configure Polly resilience strategy for file operations
        _resilienceStrategy = new ResiliencePipelineBuilder()
            .AddRetry(new()
            {
                ShouldHandle = new PredicateBuilder().Handle<IOException>().Handle<UnauthorizedAccessException>(),
                Delay = TimeSpan.FromMilliseconds(100),
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential
            })
            .AddTimeout(TimeSpan.FromSeconds(30))
            .Build();
    }

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateResourceAsync(Resource resource, IEnumerable<string>? profileUrls = null, CancellationToken cancellationToken = default)
    {
        if (resource == null)
            throw new ArgumentNullException(nameof(resource));

        var stopwatch = Stopwatch.StartNew();
        var result = new ValidationResult
        {
            ResourceName = resource.Id ?? "Unknown",
            ResourceType = resource.TypeName
        };

        try
        {
            _logger.LogDebug("Starting validation for {ResourceType} resource: {ResourceId}",
                resource.TypeName, resource.Id);

            // Perform validation using Firely SDK
            var validationResult = await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return _validator.Validate(resource);
            }, cancellationToken);

            // Convert Firely validation result to our model
            result.Issues = ConvertValidationIssues(validationResult);
            result.IsValid = !result.HasErrors;
            result.ValidatedProfiles = profileUrls?.ToList() ?? new List<string>();

            _logger.LogDebug("Validation completed for {ResourceType} resource: {ResourceId}. Valid: {IsValid}, Issues: {IssueCount}",
                resource.TypeName, resource.Id, result.IsValid, result.Issues.Count);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Validation cancelled for {ResourceType} resource: {ResourceId}",
                resource.TypeName, resource.Id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating {ResourceType} resource: {ResourceId}",
                resource.TypeName, resource.Id);

            result.Issues.Add(new ValidationIssue
            {
                Severity = IssueSeverity.Fatal,
                Code = "VALIDATION_ERROR",
                Description = $"Validation failed with exception: {ex.Message}",
                ElementPath = "Resource",
                Details = { ["Exception"] = ex.GetType().Name }
            });
            result.IsValid = false;
        }
        finally
        {
            stopwatch.Stop();
            result.ValidationDuration = stopwatch.Elapsed;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateResourceFromJsonAsync(string resourceJson, IEnumerable<string>? profileUrls = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(resourceJson))
            throw new ArgumentException("Resource JSON cannot be null or empty", nameof(resourceJson));

        try
        {
            var resource = await Task.Run(() => _jsonParser.Parse<Resource>(resourceJson), cancellationToken);
            return await ValidateResourceAsync(resource, profileUrls, cancellationToken);
        }
        catch (Exception ex) when (!(ex is OperationCanceledException))
        {
            _logger.LogError(ex, "Error parsing JSON resource for validation");

            return new ValidationResult
            {
                ResourceName = "Unknown",
                ResourceType = "Unknown",
                IsValid = false,
                Issues = new List<ValidationIssue>
                {
                    new()
                    {
                        Severity = IssueSeverity.Fatal,
                        Code = "PARSE_ERROR",
                        Description = $"Failed to parse JSON resource: {ex.Message}",
                        ElementPath = "Resource",
                        Details = { ["Exception"] = ex.GetType().Name }
                    }
                }
            };
        }
    }

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateResourceFromXmlAsync(string resourceXml, IEnumerable<string>? profileUrls = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(resourceXml))
            throw new ArgumentException("Resource XML cannot be null or empty", nameof(resourceXml));

        try
        {
            var resource = await Task.Run(() => _xmlParser.Parse<Resource>(resourceXml), cancellationToken);
            return await ValidateResourceAsync(resource, profileUrls, cancellationToken);
        }
        catch (Exception ex) when (!(ex is OperationCanceledException))
        {
            _logger.LogError(ex, "Error parsing XML resource for validation");

            return new ValidationResult
            {
                ResourceName = "Unknown",
                ResourceType = "Unknown",
                IsValid = false,
                Issues = new List<ValidationIssue>
                {
                    new()
                    {
                        Severity = IssueSeverity.Fatal,
                        Code = "PARSE_ERROR",
                        Description = $"Failed to parse XML resource: {ex.Message}",
                        ElementPath = "Resource",
                        Details = { ["Exception"] = ex.GetType().Name }
                    }
                }
            };
        }
    }

    /// <inheritdoc />
    public async Task<BatchValidationReport> ValidateDirectoryAsync(string directoryPath, string filePattern = "*.*", IEnumerable<string>? profileUrls = null, IProgress<BatchValidationProgress>? progressCallback = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
            throw new ArgumentException("Directory path cannot be null or empty", nameof(directoryPath));

        if (!Directory.Exists(directoryPath))
            throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

        var report = new BatchValidationReport
        {
            BatchName = $"Directory Validation: {directoryPath}",
            ValidationStartTime = DateTime.UtcNow,
            Configuration = new ValidationConfiguration
            {
                ProfileUrls = profileUrls?.ToList() ?? new List<string>()
            }
        };

        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Get all FHIR resource files
            var files = await _resilienceStrategy.ExecuteAsync(async _ =>
            {
                return await Task.Run(() =>
                {
                    var allFiles = Directory.GetFiles(directoryPath, filePattern, SearchOption.AllDirectories);
                    return allFiles.Where(f => f.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ||
                                             f.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)).ToArray();
                }, cancellationToken);
            }, cancellationToken);

            _logger.LogInformation("Found {FileCount} FHIR resource files in directory: {DirectoryPath}",
                files.Length, directoryPath);

            var totalFiles = files.Length;
            var processedFiles = 0;

            // Process files in batches for better performance
            var batchSize = Math.Min(10, Math.Max(1, totalFiles / 4));
            var batches = files.Chunk(batchSize);

            foreach (var batch in batches)
            {
                var batchTasks = batch.Select(async file =>
                {
                    try
                    {
                        var content = await _resilienceStrategy.ExecuteAsync(async _ =>
                            await File.ReadAllTextAsync(file, cancellationToken), cancellationToken);

                        ValidationResult result;
                        if (file.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                        {
                            result = await ValidateResourceFromJsonAsync(content, profileUrls, cancellationToken);
                        }
                        else
                        {
                            result = await ValidateResourceFromXmlAsync(content, profileUrls, cancellationToken);
                        }

                        result.ResourceName = Path.GetFileName(file);

                        // Update progress
                        Interlocked.Increment(ref processedFiles);
                        progressCallback?.Report(new BatchValidationProgress
                        {
                            CurrentResource = processedFiles,
                            TotalResources = totalFiles,
                            CurrentResourceName = Path.GetFileName(file),
                            CurrentStage = "Validating files"
                        });

                        return result;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing file: {FilePath}", file);

                        Interlocked.Increment(ref processedFiles);
                        return new ValidationResult
                        {
                            ResourceName = Path.GetFileName(file),
                            ResourceType = "Unknown",
                            IsValid = false,
                            Issues = new List<ValidationIssue>
                            {
                                new()
                                {
                                    Severity = IssueSeverity.Fatal,
                                    Code = "FILE_PROCESSING_ERROR",
                                    Description = $"Error processing file: {ex.Message}",
                                    ElementPath = "File",
                                    Details = { ["FilePath"] = file, ["Exception"] = ex.GetType().Name }
                                }
                            }
                        };
                    }
                });

                var batchResults = await Task.WhenAll(batchTasks);
                report.Results.AddRange(batchResults);
            }

            // Generate summary
            report.Summary = GenerateValidationSummary(report.Results, report.Configuration);
            report.PerformanceMetrics = CalculatePerformanceMetrics(report.Results, stopwatch.Elapsed);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Directory validation cancelled for: {DirectoryPath}", directoryPath);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during directory validation: {DirectoryPath}", directoryPath);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            report.TotalValidationDuration = stopwatch.Elapsed;
            report.ValidationEndTime = DateTime.UtcNow;
        }

        _logger.LogInformation("Directory validation completed. Total: {TotalResources}, Passed: {PassedResources}, Failed: {FailedResources}",
            report.Summary.TotalResources, report.Summary.PassedResources, report.Summary.FailedResources);

        return report;
    }

    /// <inheritdoc />
    public async Task<BatchValidationReport> ValidateBatchAsync(IEnumerable<Resource> resources, IEnumerable<string>? profileUrls = null, IProgress<BatchValidationProgress>? progressCallback = null, CancellationToken cancellationToken = default)
    {
        if (resources == null)
            throw new ArgumentNullException(nameof(resources));

        var resourceList = resources.ToList();
        var report = new BatchValidationReport
        {
            BatchName = $"Batch Validation: {resourceList.Count} resources",
            ValidationStartTime = DateTime.UtcNow,
            Configuration = new ValidationConfiguration
            {
                ProfileUrls = profileUrls?.ToList() ?? new List<string>()
            }
        };

        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Starting batch validation for {ResourceCount} resources", resourceList.Count);

            var totalResources = resourceList.Count;
            var processedResources = 0;

            // Process resources with controlled concurrency
            var semaphore = new SemaphoreSlim(Environment.ProcessorCount);
            var tasks = resourceList.Select(async resource =>
            {
                await semaphore.WaitAsync(cancellationToken);
                try
                {
                    var result = await ValidateResourceAsync(resource, profileUrls, cancellationToken);

                    Interlocked.Increment(ref processedResources);
                    progressCallback?.Report(new BatchValidationProgress
                    {
                        CurrentResource = processedResources,
                        TotalResources = totalResources,
                        CurrentResourceName = resource.Id ?? resource.TypeName,
                        CurrentStage = "Validating resources"
                    });

                    return result;
                }
                finally
                {
                    semaphore.Release();
                }
            });

            var results = await Task.WhenAll(tasks);
            report.Results.AddRange(results);

            // Generate summary
            report.Summary = GenerateValidationSummary(report.Results, report.Configuration);
            report.PerformanceMetrics = CalculatePerformanceMetrics(report.Results, stopwatch.Elapsed);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Batch validation cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during batch validation");
            throw;
        }
        finally
        {
            stopwatch.Stop();
            report.TotalValidationDuration = stopwatch.Elapsed;
            report.ValidationEndTime = DateTime.UtcNow;
        }

        _logger.LogInformation("Batch validation completed. Total: {TotalResources}, Passed: {PassedResources}, Failed: {FailedResources}",
            report.Summary.TotalResources, report.Summary.PassedResources, report.Summary.FailedResources);

        return report;
    }

    /// <summary>
    /// Converts Firely SDK validation results to our validation issue model
    /// </summary>
    private List<ValidationIssue> ConvertValidationIssues(OperationOutcome validationResult)
    {
        var issues = new List<ValidationIssue>();

        if (validationResult?.Issue != null)
        {
            foreach (var issue in validationResult.Issue)
            {
                issues.Add(new ValidationIssue
                {
                    Severity = ConvertSeverity(issue.Severity),
                    Code = issue.Code?.ToString() ?? "UNKNOWN",
                    Description = issue.Diagnostics ?? "No description available",
                    ElementPath = string.Join(", ", issue.Location ?? new List<string>()),
                    Location = string.Join(", ", issue.Expression ?? new List<string>()),
                    Details = new Dictionary<string, object>
                    {
                        ["IssueType"] = issue.Code?.ToString() ?? "Unknown"
                    }
                });
            }
        }

        return issues;
    }

    /// <summary>
    /// Converts Firely SDK issue severity to our severity enum
    /// </summary>
    private IssueSeverity ConvertSeverity(OperationOutcome.IssueSeverity? severity)
    {
        return severity switch
        {
            OperationOutcome.IssueSeverity.Information => IssueSeverity.Information,
            OperationOutcome.IssueSeverity.Warning => IssueSeverity.Warning,
            OperationOutcome.IssueSeverity.Error => IssueSeverity.Error,
            OperationOutcome.IssueSeverity.Fatal => IssueSeverity.Fatal,
            _ => IssueSeverity.Error
        };
    }

    /// <summary>
    /// Generates validation summary from results
    /// </summary>
    private ValidationSummary GenerateValidationSummary(List<ValidationResult> results, ValidationConfiguration configuration)
    {
        var summary = new ValidationSummary
        {
            TotalResources = results.Count,
            PassedResources = results.Count(r => r.IsValid),
            FailedResources = results.Count(r => !r.IsValid),
            WarningResources = results.Count(r => r.HasWarnings),
            TotalIssues = results.Sum(r => r.Issues.Count)
        };

        // Group issues by severity
        summary.IssuesBySeverity = results
            .SelectMany(r => r.Issues)
            .GroupBy(i => i.Severity.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        // Group issues by resource type
        summary.IssuesByResourceType = results
            .GroupBy(r => r.ResourceType)
            .ToDictionary(g => g.Key, g => g.Sum(r => r.Issues.Count));

        // Determine overall success based on configuration thresholds
        var fatalErrorCount = summary.IssuesBySeverity.GetValueOrDefault("Fatal", 0);
        summary.OverallSuccess = summary.PassRate >= configuration.MinimumPassRateThreshold &&
                                fatalErrorCount <= configuration.MaximumFatalErrors;

        return summary;
    }

    /// <summary>
    /// Calculates performance metrics for validation operations
    /// </summary>
    private ValidationPerformanceMetrics CalculatePerformanceMetrics(List<ValidationResult> results, TimeSpan totalDuration)
    {
        var validationTimes = results.Select(r => r.ValidationDuration).Where(t => t > TimeSpan.Zero).ToList();

        return new ValidationPerformanceMetrics
        {
            AverageValidationTime = validationTimes.Any() ?
                TimeSpan.FromTicks((long)validationTimes.Average(t => t.Ticks)) : TimeSpan.Zero,
            MinimumValidationTime = validationTimes.Any() ? validationTimes.Min() : TimeSpan.Zero,
            MaximumValidationTime = validationTimes.Any() ? validationTimes.Max() : TimeSpan.Zero,
            ResourcesPerSecond = totalDuration.TotalSeconds > 0 ? results.Count / totalDuration.TotalSeconds : 0,
            MemoryUsageBytes = GC.GetTotalMemory(false),
            ConcurrentOperations = Environment.ProcessorCount
        };
    }
}