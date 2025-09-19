using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FhirIntegrationService.ValidationPipeline.Models;
using FhirIntegrationService.ValidationPipeline.Services;
using System.Text.Json;
using System.Diagnostics;

namespace FhirIntegrationService.ValidationPipeline;

/// <summary>
/// Main program for FHIR validation pipeline execution
/// </summary>
public class Program
{
    private static async Task<int> Main(string[] args)
    {
        // Set up console encoding for proper output
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        try
        {
            var result = await Parser.Default.ParseArguments<ValidationCommandLineOptions>(args)
                .MapResult(
                    async options => await ExecuteValidationPipelineAsync(options),
                    errors => Task.FromResult(1)
                );

            return result;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"💥 Critical error: {ex.Message}");
            Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
            return 1;
        }
    }

    private static async Task<int> ExecuteValidationPipelineAsync(ValidationCommandLineOptions options)
    {
        var stopwatch = Stopwatch.StartNew();

        // Create host builder with dependency injection
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                    if (options.VerboseLogging)
                    {
                        builder.SetMinimumLevel(LogLevel.Debug);
                    }
                    else
                    {
                        builder.SetMinimumLevel(LogLevel.Information);
                    }
                });

                // Register validation services
                services.AddSingleton<IFhirProfileValidator, FhirProfileValidator>();
                services.AddSingleton<IResourceValidator, ResourceValidator>();
                services.AddSingleton<IContentValidator, ContentValidator>();
                services.AddSingleton<ISecurityValidator, SecurityValidator>();
                services.AddSingleton<IPublicationValidator, PublicationValidator>();
                services.AddSingleton<IQualityGateValidator, QualityGateValidator>();
                services.AddSingleton<IValidationPipelineOrchestrator, ValidationPipelineOrchestrator>();
            })
            .Build();

        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        var orchestrator = host.Services.GetRequiredService<IValidationPipelineOrchestrator>();

        try
        {
            // Display startup banner
            DisplayStartupBanner(logger, options);

            // Handle special commands
            if (!string.IsNullOrEmpty(options.CheckGatesReportPath))
            {
                return await CheckQualityGatesAsync(options.CheckGatesReportPath, options.QualityGateConfigPath, logger);
            }

            // Create validation configuration
            var config = CreateValidationConfiguration(options);

            // Validate configuration
            var configValidation = ValidateConfiguration(config, logger);
            if (!configValidation.IsValid)
            {
                logger.LogError("❌ Configuration validation failed:");
                foreach (var error in configValidation.Errors)
                {
                    logger.LogError("  - {Error}", error);
                }
                return 1;
            }

            // Execute validation pipeline
            logger.LogInformation("🚀 Starting FHIR validation pipeline execution...");
            var result = await orchestrator.ExecutePipelineAsync(config);

            stopwatch.Stop();

            // Display results summary
            DisplayResultsSummary(result, stopwatch.Elapsed, logger);

            // Determine exit code
            return DetermineExitCode(result, options);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "💥 Critical error during validation pipeline execution");
            return 1;
        }
    }

    private static void DisplayStartupBanner(ILogger<Program> logger, ValidationCommandLineOptions options)
    {
        logger.LogInformation("═══════════════════════════════════════════════════════════════");
        logger.LogInformation("🔍 FHIR Integration Service - Validation Pipeline");
        logger.LogInformation("═══════════════════════════════════════════════════════════════");
        logger.LogInformation("📁 Profile Directory: {ProfileDirectory}", options.ProfileDirectory ?? "src/");
        logger.LogInformation("📄 Example Resources: {ExampleDirectory}", options.ExampleResourceDirectory ?? "docs/examples/");
        logger.LogInformation("📚 Implementation Guide: {IgDirectory}", options.ImplementationGuideDirectory ?? "docs/implementation-guide/");
        logger.LogInformation("📊 Output Directory: {OutputDirectory}", options.OutputDirectory ?? "validation-output/");
        logger.LogInformation("⚙️  Verbose Logging: {VerboseLogging}", options.VerboseLogging);
        logger.LogInformation("⚡ Parallel Validation: {ParallelValidation}", options.EnableParallelValidation);
        logger.LogInformation("⏱️  Timeout: {Timeout}s", options.ValidationTimeoutSeconds);
        logger.LogInformation("═══════════════════════════════════════════════════════════════");
    }

    private static ValidationPipelineConfiguration CreateValidationConfiguration(ValidationCommandLineOptions options)
    {
        var config = new ValidationPipelineConfiguration();

        // Override defaults with command line options
        if (!string.IsNullOrEmpty(options.ProfileDirectory))
            config.ProfileDirectory = options.ProfileDirectory;

        if (!string.IsNullOrEmpty(options.ExampleResourceDirectory))
            config.ExampleResourceDirectory = options.ExampleResourceDirectory;

        if (!string.IsNullOrEmpty(options.ImplementationGuideDirectory))
            config.ImplementationGuideDirectory = options.ImplementationGuideDirectory;

        if (!string.IsNullOrEmpty(options.OutputDirectory))
            config.OutputDirectory = options.OutputDirectory;

        if (!string.IsNullOrEmpty(options.QualityGateConfigPath))
            config.QualityGateConfigPath = options.QualityGateConfigPath;

        config.VerboseLogging = options.VerboseLogging;
        config.EnableParallelValidation = options.EnableParallelValidation;
        config.FailOnWarnings = options.FailOnWarnings;
        config.ValidationTimeoutSeconds = options.ValidationTimeoutSeconds;

        return config;
    }

    private static (bool IsValid, List<string> Errors) ValidateConfiguration(
        ValidationPipelineConfiguration config,
        ILogger<Program> logger)
    {
        var errors = new List<string>();

        // Validate required directories exist
        if (!Directory.Exists(config.ProfileDirectory))
        {
            errors.Add($"Profile directory does not exist: {config.ProfileDirectory}");
        }

        if (!string.IsNullOrEmpty(config.ExampleResourceDirectory) && !Directory.Exists(config.ExampleResourceDirectory))
        {
            logger.LogWarning("⚠️ Example resource directory does not exist: {Directory}", config.ExampleResourceDirectory);
        }

        if (!Directory.Exists(config.ImplementationGuideDirectory))
        {
            logger.LogWarning("⚠️ Implementation Guide directory does not exist: {Directory}", config.ImplementationGuideDirectory);
        }

        // Validate quality gate config exists if specified
        if (!string.IsNullOrEmpty(config.QualityGateConfigPath) && !File.Exists(config.QualityGateConfigPath))
        {
            logger.LogWarning("⚠️ Quality gate config file does not exist: {File}. Using default configuration.", config.QualityGateConfigPath);
        }

        // Validate timeout value
        if (config.ValidationTimeoutSeconds <= 0 || config.ValidationTimeoutSeconds > 3600)
        {
            errors.Add($"Invalid validation timeout: {config.ValidationTimeoutSeconds}s. Must be between 1 and 3600 seconds.");
        }

        return (errors.Count == 0, errors);
    }

    private static void DisplayResultsSummary(ValidationPipelineResult result, TimeSpan duration, ILogger<Program> logger)
    {
        logger.LogInformation("═══════════════════════════════════════════════════════════════");
        logger.LogInformation("📊 FHIR Validation Pipeline Results Summary");
        logger.LogInformation("═══════════════════════════════════════════════════════════════");

        // Overall status with emoji
        var statusEmoji = result.OverallStatus switch
        {
            ValidationStatus.Success => "✅",
            ValidationStatus.Warning => "⚠️",
            ValidationStatus.Error => "❌",
            ValidationStatus.Failed => "💥",
            _ => "❓"
        };

        logger.LogInformation("{StatusEmoji} Overall Status: {Status}", statusEmoji, result.OverallStatus);
        logger.LogInformation("⏱️  Execution Duration: {Duration:F2}s", duration.TotalSeconds);

        // Validation metrics
        logger.LogInformation("📈 Validation Metrics:");
        logger.LogInformation("  • Profiles Validated: {ProfileCount}", result.Metrics.TotalProfilesValidated);
        logger.LogInformation("  • Resources Validated: {ResourceCount}", result.Metrics.TotalResourcesValidated);
        logger.LogInformation("  • Total Issues: {TotalIssues}", result.Metrics.TotalValidationIssues);
        logger.LogInformation("  • Critical Issues: {CriticalIssues}", result.Metrics.CriticalIssues);
        logger.LogInformation("  • Warning Issues: {WarningIssues}", result.Metrics.WarningIssues);

        // Quality gate results
        logger.LogInformation("🚦 Quality Gate Compliance: {Compliance}",
            result.QualityGateCompliance.OverallCompliance ? "✅ PASS" : "❌ FAIL");

        if (result.QualityGateCompliance.BlockingFailures.Any())
        {
            logger.LogInformation("🚫 Blocking Quality Gate Failures:");
            foreach (var failure in result.QualityGateCompliance.BlockingFailures)
            {
                logger.LogInformation("  • {GateName}: {Score:F1}% (Required: {Threshold:F1}%)",
                    failure.GateName, failure.Score, failure.PassThreshold);
            }
        }

        // Phase-specific status
        logger.LogInformation("📋 Phase Results:");
        logger.LogInformation("  • Technical Validation: {Status}", GetStatusEmoji(result.TechnicalValidation.Status));
        logger.LogInformation("  • Resource Validation: {Status}", GetStatusEmoji(result.ResourceValidation.Status));
        logger.LogInformation("  • Content Validation: {Status}", GetStatusEmoji(result.ContentValidation.Status));
        logger.LogInformation("  • Security Validation: {Status}", GetStatusEmoji(result.SecurityValidation.Status));
        logger.LogInformation("  • Publication Validation: {Status}", GetStatusEmoji(result.PublicationValidation.Status));

        // Recommendations summary
        if (result.Recommendations.Any())
        {
            var criticalCount = result.Recommendations.Count(r => r.Priority == RecommendationPriority.Critical);
            var highCount = result.Recommendations.Count(r => r.Priority == RecommendationPriority.High);

            logger.LogInformation("💡 Recommendations: {Total} total ({Critical} critical, {High} high priority)",
                result.Recommendations.Count, criticalCount, highCount);
        }

        logger.LogInformation("═══════════════════════════════════════════════════════════════");
    }

    private static string GetStatusEmoji(ValidationStatus status)
    {
        return status switch
        {
            ValidationStatus.Success => "✅ Success",
            ValidationStatus.Warning => "⚠️ Warning",
            ValidationStatus.Error => "❌ Error",
            ValidationStatus.Failed => "💥 Failed",
            _ => "❓ Unknown"
        };
    }

    private static int DetermineExitCode(ValidationPipelineResult result, ValidationCommandLineOptions options)
    {
        // Check for critical failures
        if (result.OverallStatus == ValidationStatus.Failed || result.OverallStatus == ValidationStatus.Error)
        {
            return 1;
        }

        // Check quality gate compliance
        if (!result.QualityGateCompliance.OverallCompliance)
        {
            return 1;
        }

        // Check fail on warnings setting
        if (options.FailOnWarnings && result.OverallStatus == ValidationStatus.Warning)
        {
            return 1;
        }

        return 0;
    }

    private static async Task<int> CheckQualityGatesAsync(string reportPath, string? gateConfigPath, ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation("🚦 Checking quality gates against validation report: {ReportPath}", reportPath);

            if (!File.Exists(reportPath))
            {
                logger.LogError("❌ Validation report file not found: {ReportPath}", reportPath);
                return 1;
            }

            var reportContent = await File.ReadAllTextAsync(reportPath);
            var validationResult = JsonSerializer.Deserialize<ValidationPipelineResult>(reportContent);

            if (validationResult == null)
            {
                logger.LogError("❌ Failed to parse validation report");
                return 1;
            }

            // Re-validate quality gates if configuration is provided
            if (!string.IsNullOrEmpty(gateConfigPath) && File.Exists(gateConfigPath))
            {
                var qualityGateValidator = new QualityGateValidator(logger);
                var gateResult = await qualityGateValidator.ValidateQualityGatesAsync(validationResult, gateConfigPath);

                validationResult.QualityGateCompliance = gateResult;
            }

            // Display quality gate results
            DisplayQualityGateResults(validationResult.QualityGateCompliance, logger);

            return validationResult.QualityGateCompliance.OverallCompliance ? 0 : 1;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "💥 Error checking quality gates");
            return 1;
        }
    }

    private static void DisplayQualityGateResults(QualityGateComplianceResult compliance, ILogger<Program> logger)
    {
        logger.LogInformation("═══════════════════════════════════════════════════════════════");
        logger.LogInformation("🚦 Quality Gate Validation Results");
        logger.LogInformation("═══════════════════════════════════════════════════════════════");

        logger.LogInformation("Overall Compliance: {Compliance}",
            compliance.OverallCompliance ? "✅ PASS" : "❌ FAIL");

        foreach (var gate in compliance.GateResults)
        {
            var gateEmoji = gate.Status == ValidationStatus.Success ? "✅" : "❌";
            var blockingIndicator = gate.Blocking ? "🚫" : "ℹ️";

            logger.LogInformation("{Emoji} {Blocking} {GateName}: {Score:F1}% (Required: {Threshold:F1}%)",
                gateEmoji, blockingIndicator, gate.GateName, gate.Score, gate.PassThreshold);

            if (gate.Status != ValidationStatus.Success && gate.Criteria.Any())
            {
                foreach (var criterion in gate.Criteria.Where(c => c.Status != ValidationStatus.Success))
                {
                    logger.LogInformation("    • {CriterionName}: {ActualValue:F1}% (Required: {RequiredValue:F1}%)",
                        criterion.Name, criterion.ActualValue, criterion.RequiredValue);
                }
            }
        }

        if (compliance.BlockingFailures.Any())
        {
            logger.LogInformation("🚫 Blocking Failures: {Count}", compliance.BlockingFailures.Count);
            foreach (var failure in compliance.BlockingFailures)
            {
                logger.LogInformation("  • {GateName}", failure.GateName);
            }
        }

        logger.LogInformation("═══════════════════════════════════════════════════════════════");
    }
}

// Additional required service implementations (minimal for compilation)
public class ResourceValidator : IResourceValidator
{
    private readonly ILogger<ResourceValidator> _logger;

    public ResourceValidator(ILogger<ResourceValidator> logger)
    {
        _logger = logger;
    }

    public async Task<ResourceValidationResult> ValidateResourcesAsync(string resourceDirectory, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validating resources in directory: {Directory}", resourceDirectory);
        await Task.CompletedTask;
        return new ResourceValidationResult { Status = ValidationStatus.Success };
    }
}

public class ContentValidator : IContentValidator
{
    private readonly ILogger<ContentValidator> _logger;

    public ContentValidator(ILogger<ContentValidator> logger)
    {
        _logger = logger;
    }

    public async Task<ContentValidationResult> ValidateImplementationGuideAsync(string igDirectory, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validating Implementation Guide in directory: {Directory}", igDirectory);
        await Task.CompletedTask;
        return new ContentValidationResult { Status = ValidationStatus.Success };
    }
}

public class SecurityValidator : ISecurityValidator
{
    private readonly ILogger<SecurityValidator> _logger;

    public SecurityValidator(ILogger<SecurityValidator> logger)
    {
        _logger = logger;
    }

    public async Task<SecurityValidationResult> ValidateSecurityComplianceAsync(ValidationPipelineConfiguration config, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validating security compliance");
        await Task.CompletedTask;
        return new SecurityValidationResult { Status = ValidationStatus.Success };
    }
}

public class PublicationValidator : IPublicationValidator
{
    private readonly ILogger<PublicationValidator> _logger;

    public PublicationValidator(ILogger<PublicationValidator> logger)
    {
        _logger = logger;
    }

    public async Task<PublicationValidationResult> ValidatePublicationReadinessAsync(ValidationPipelineConfiguration config, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validating publication readiness");
        await Task.CompletedTask;
        return new PublicationValidationResult { Status = ValidationStatus.Success };
    }
}

// Interface definitions for services
public interface IResourceValidator
{
    Task<ResourceValidationResult> ValidateResourcesAsync(string resourceDirectory, CancellationToken cancellationToken = default);
}

public interface IContentValidator
{
    Task<ContentValidationResult> ValidateImplementationGuideAsync(string igDirectory, CancellationToken cancellationToken = default);
}

public interface ISecurityValidator
{
    Task<SecurityValidationResult> ValidateSecurityComplianceAsync(ValidationPipelineConfiguration config, CancellationToken cancellationToken = default);
}

public interface IPublicationValidator
{
    Task<PublicationValidationResult> ValidatePublicationReadinessAsync(ValidationPipelineConfiguration config, CancellationToken cancellationToken = default);
}