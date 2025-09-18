using Microsoft.Extensions.Logging;
using FhirIntegrationService.ValidationPipeline.Models;
using System.Diagnostics;

namespace FhirIntegrationService.ValidationPipeline.Services;

/// <summary>
/// Orchestrates the complete FHIR validation pipeline execution
/// </summary>
public interface IValidationPipelineOrchestrator
{
    /// <summary>
    /// Executes the complete validation pipeline
    /// </summary>
    Task<ValidationPipelineResult> ExecutePipelineAsync(
        ValidationPipelineConfiguration config,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of validation pipeline orchestrator
/// </summary>
public class ValidationPipelineOrchestrator : IValidationPipelineOrchestrator
{
    private readonly ILogger<ValidationPipelineOrchestrator> _logger;
    private readonly IFhirProfileValidator _profileValidator;
    private readonly IResourceValidator _resourceValidator;
    private readonly IContentValidator _contentValidator;
    private readonly ISecurityValidator _securityValidator;
    private readonly IPublicationValidator _publicationValidator;
    private readonly IQualityGateValidator _qualityGateValidator;

    public ValidationPipelineOrchestrator(
        ILogger<ValidationPipelineOrchestrator> logger,
        IFhirProfileValidator profileValidator,
        IResourceValidator resourceValidator,
        IContentValidator contentValidator,
        ISecurityValidator securityValidator,
        IPublicationValidator publicationValidator,
        IQualityGateValidator qualityGateValidator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _profileValidator = profileValidator ?? throw new ArgumentNullException(nameof(profileValidator));
        _resourceValidator = resourceValidator ?? throw new ArgumentNullException(nameof(resourceValidator));
        _contentValidator = contentValidator ?? throw new ArgumentNullException(nameof(contentValidator));
        _securityValidator = securityValidator ?? throw new ArgumentNullException(nameof(securityValidator));
        _publicationValidator = publicationValidator ?? throw new ArgumentNullException(nameof(publicationValidator));
        _qualityGateValidator = qualityGateValidator ?? throw new ArgumentNullException(nameof(qualityGateValidator));
    }

    /// <inheritdoc/>
    public async Task<ValidationPipelineResult> ExecutePipelineAsync(
        ValidationPipelineConfiguration config,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting FHIR validation pipeline execution");

        var result = new ValidationPipelineResult
        {
            ExecutionStartTime = DateTime.UtcNow
        };

        var overallStopwatch = Stopwatch.StartNew();

        try
        {
            // Ensure output directory exists
            if (!Directory.Exists(config.OutputDirectory))
            {
                Directory.CreateDirectory(config.OutputDirectory);
                _logger.LogInformation("Created output directory: {OutputDirectory}", config.OutputDirectory);
            }

            // Phase 1: FHIR Technical Validation
            _logger.LogInformation("Phase 1: Executing FHIR Technical Validation");
            using (var activity = Activity.StartActivity("FHIR Technical Validation"))
            {
                result.TechnicalValidation = await _profileValidator
                    .ValidateProfilesAsync(config.ProfileDirectory, cancellationToken);

                _logger.LogInformation(
                    "FHIR Technical Validation completed with status: {Status}",
                    result.TechnicalValidation.Status);

                activity?.SetTag("validation.status", result.TechnicalValidation.Status.ToString());
                activity?.SetTag("validation.profile_count", result.TechnicalValidation.ProfileValidationResults.Count);
            }

            // Phase 2: Resource Validation
            _logger.LogInformation("Phase 2: Executing Resource Validation");
            using (var activity = Activity.StartActivity("Resource Validation"))
            {
                result.ResourceValidation = await _resourceValidator
                    .ValidateResourcesAsync(config.ExampleResourceDirectory, cancellationToken);

                _logger.LogInformation(
                    "Resource Validation completed with status: {Status}",
                    result.ResourceValidation.Status);

                activity?.SetTag("validation.status", result.ResourceValidation.Status.ToString());
                activity?.SetTag("validation.resource_count", result.ResourceValidation.ResourceValidationResults.Count);
            }

            // Phase 3: Content Validation
            _logger.LogInformation("Phase 3: Executing Content Validation");
            using (var activity = Activity.StartActivity("Content Validation"))
            {
                result.ContentValidation = await _contentValidator
                    .ValidateImplementationGuideAsync(config.ImplementationGuideDirectory, cancellationToken);

                _logger.LogInformation(
                    "Content Validation completed with status: {Status}",
                    result.ContentValidation.Status);

                activity?.SetTag("validation.status", result.ContentValidation.Status.ToString());
            }

            // Phase 4: Security Validation
            _logger.LogInformation("Phase 4: Executing Security Validation");
            using (var activity = Activity.StartActivity("Security Validation"))
            {
                result.SecurityValidation = await _securityValidator
                    .ValidateSecurityComplianceAsync(config, cancellationToken);

                _logger.LogInformation(
                    "Security Validation completed with status: {Status}",
                    result.SecurityValidation.Status);

                activity?.SetTag("validation.status", result.SecurityValidation.Status.ToString());
            }

            // Phase 5: Publication Validation
            _logger.LogInformation("Phase 5: Executing Publication Validation");
            using (var activity = Activity.StartActivity("Publication Validation"))
            {
                result.PublicationValidation = await _publicationValidator
                    .ValidatePublicationReadinessAsync(config, cancellationToken);

                _logger.LogInformation(
                    "Publication Validation completed with status: {Status}",
                    result.PublicationValidation.Status);

                activity?.SetTag("validation.status", result.PublicationValidation.Status.ToString());
            }

            // Phase 6: Quality Gate Validation
            _logger.LogInformation("Phase 6: Executing Quality Gate Validation");
            using (var activity = Activity.StartActivity("Quality Gate Validation"))
            {
                result.QualityGateCompliance = await _qualityGateValidator
                    .ValidateQualityGatesAsync(result, config.QualityGateConfigPath, cancellationToken);

                _logger.LogInformation(
                    "Quality Gate Validation completed. Overall compliance: {Compliance}, Blocking failures: {BlockingFailures}",
                    result.QualityGateCompliance.OverallCompliance,
                    result.QualityGateCompliance.BlockingFailures.Count);

                activity?.SetTag("validation.compliance", result.QualityGateCompliance.OverallCompliance);
                activity?.SetTag("validation.blocking_failures", result.QualityGateCompliance.BlockingFailures.Count);
            }

            // Aggregate Results and Generate Recommendations
            result.OverallStatus = DetermineOverallStatus(result);
            result.Recommendations = GenerateRecommendations(result);
            result.Metrics = CalculateMetrics(result, overallStopwatch.Elapsed);

            overallStopwatch.Stop();
            result.ExecutionEndTime = DateTime.UtcNow;

            _logger.LogInformation(
                "FHIR validation pipeline completed in {Duration:F2}s with overall status: {Status}",
                overallStopwatch.Elapsed.TotalSeconds,
                result.OverallStatus);

            // Save validation report
            await SaveValidationReportAsync(result, config.OutputDirectory, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error during validation pipeline execution");

            overallStopwatch.Stop();
            result.ExecutionEndTime = DateTime.UtcNow;
            result.OverallStatus = ValidationStatus.Failed;

            result.Recommendations.Add(new ValidationRecommendation
            {
                Type = RecommendationType.Technical,
                Priority = RecommendationPriority.Critical,
                Title = "Pipeline Execution Failed",
                Description = $"The validation pipeline failed with a critical error: {ex.Message}",
                ActionItems = new List<string>
                {
                    "Review pipeline logs for detailed error information",
                    "Verify all input directories and files exist",
                    "Check system resources and permissions",
                    "Contact technical support if the issue persists"
                }
            });
        }

        return result;
    }

    private static ValidationStatus DetermineOverallStatus(ValidationPipelineResult result)
    {
        var statuses = new[]
        {
            result.TechnicalValidation.Status,
            result.ResourceValidation.Status,
            result.ContentValidation.Status,
            result.SecurityValidation.Status,
            result.PublicationValidation.Status
        };

        // Check for critical failures first
        if (statuses.Any(s => s == ValidationStatus.Failed || s == ValidationStatus.Error))
        {
            return ValidationStatus.Failed;
        }

        // Check quality gate compliance
        if (!result.QualityGateCompliance.OverallCompliance)
        {
            return ValidationStatus.Failed;
        }

        // Check for warnings
        if (statuses.Any(s => s == ValidationStatus.Warning))
        {
            return ValidationStatus.Warning;
        }

        return ValidationStatus.Success;
    }

    private static List<ValidationRecommendation> GenerateRecommendations(ValidationPipelineResult result)
    {
        var recommendations = new List<ValidationRecommendation>();

        // Technical validation recommendations
        if (result.TechnicalValidation.Status != ValidationStatus.Success)
        {
            recommendations.Add(new ValidationRecommendation
            {
                Type = RecommendationType.Technical,
                Priority = RecommendationPriority.High,
                Title = "FHIR Technical Validation Issues",
                Description = "Technical validation has identified issues that need to be addressed",
                ActionItems = new List<string>
                {
                    "Review FHIR profile validation errors",
                    "Fix canonical URL issues",
                    "Correct profile metadata",
                    "Validate constraint expressions"
                }
            });
        }

        // Resource validation recommendations
        if (result.ResourceValidation.Status != ValidationStatus.Success)
        {
            recommendations.Add(new ValidationRecommendation
            {
                Type = RecommendationType.Technical,
                Priority = RecommendationPriority.Medium,
                Title = "Resource Validation Issues",
                Description = "Example resources do not conform to defined profiles",
                ActionItems = new List<string>
                {
                    "Review example resource validation errors",
                    "Update resources to conform to profiles",
                    "Verify data types and constraints",
                    "Test resources against updated profiles"
                }
            });
        }

        // Security validation recommendations
        if (result.SecurityValidation.Status != ValidationStatus.Success)
        {
            recommendations.Add(new ValidationRecommendation
            {
                Type = RecommendationType.Security,
                Priority = RecommendationPriority.Critical,
                Title = "Security Compliance Issues",
                Description = "Security validation has identified compliance issues",
                ActionItems = new List<string>
                {
                    "Remove any PHI from documentation",
                    "Review access control configurations",
                    "Update security documentation",
                    "Implement audit trail mechanisms"
                }
            });
        }

        // Publication validation recommendations
        if (result.PublicationValidation.Status != ValidationStatus.Success)
        {
            recommendations.Add(new ValidationRecommendation
            {
                Type = RecommendationType.Publication,
                Priority = RecommendationPriority.Medium,
                Title = "Publication Readiness Issues",
                Description = "Publication validation has identified readiness issues",
                ActionItems = new List<string>
                {
                    "Complete FHIR package structure",
                    "Verify Simplifier.net configuration",
                    "Test external accessibility",
                    "Update versioning information"
                }
            });
        }

        // Quality gate recommendations
        foreach (var blockingFailure in result.QualityGateCompliance.BlockingFailures)
        {
            recommendations.Add(new ValidationRecommendation
            {
                Type = RecommendationType.Technical,
                Priority = RecommendationPriority.Critical,
                Title = $"Quality Gate Failure: {blockingFailure.GateName}",
                Description = $"Blocking quality gate '{blockingFailure.GateName}' failed with score {blockingFailure.Score:F1}% (required: {blockingFailure.PassThreshold:F1}%)",
                ActionItems = blockingFailure.Criteria
                    .Where(c => c.Status != ValidationStatus.Success)
                    .Select(c => $"Address criterion: {c.Name}")
                    .ToList()
            });
        }

        return recommendations;
    }

    private static ValidationMetrics CalculateMetrics(ValidationPipelineResult result, TimeSpan totalDuration)
    {
        var metrics = new ValidationMetrics
        {
            TotalProfilesValidated = result.TechnicalValidation.ProfileValidationResults.Count,
            TotalResourcesValidated = result.ResourceValidation.ResourceValidationResults.Count,
            AverageValidationTime = totalDuration,
            MemoryUsage = GC.GetTotalMemory(false)
        };

        // Count issues across all validation phases
        var allIssues = new List<ValidationIssue>();

        result.TechnicalValidation.ProfileValidationResults.ForEach(p => allIssues.AddRange(p.ValidationIssues));
        result.ResourceValidation.ResourceValidationResults.ForEach(r => allIssues.AddRange(r.ValidationIssues));
        allIssues.AddRange(result.ContentValidation.ImplementationGuideValidation.Issues);
        allIssues.AddRange(result.SecurityValidation.PhiExposureCheck.Issues);
        allIssues.AddRange(result.PublicationValidation.PackageStructureValidation.Issues);

        metrics.TotalValidationIssues = allIssues.Count;
        metrics.CriticalIssues = allIssues.Count(i =>
            i.Severity.Equals("Error", StringComparison.OrdinalIgnoreCase) ||
            i.Severity.Equals("Fatal", StringComparison.OrdinalIgnoreCase));
        metrics.WarningIssues = allIssues.Count(i =>
            i.Severity.Equals("Warning", StringComparison.OrdinalIgnoreCase));

        return metrics;
    }

    private async Task SaveValidationReportAsync(
        ValidationPipelineResult result,
        string outputDirectory,
        CancellationToken cancellationToken)
    {
        try
        {
            var reportPath = Path.Combine(outputDirectory, $"validation-report-{DateTime.UtcNow:yyyyMMdd-HHmmss}.json");

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonContent = JsonSerializer.Serialize(result, options);
            await File.WriteAllTextAsync(reportPath, jsonContent, cancellationToken);

            _logger.LogInformation("Validation report saved to: {ReportPath}", reportPath);

            // Also save a copy as latest report
            var latestReportPath = Path.Combine(outputDirectory, "validation-report-latest.json");
            await File.WriteAllTextAsync(latestReportPath, jsonContent, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving validation report");
        }
    }
}