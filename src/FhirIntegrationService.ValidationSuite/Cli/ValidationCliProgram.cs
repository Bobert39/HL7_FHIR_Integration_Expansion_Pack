using FhirIntegrationService.ValidationSuite.Interfaces;
using FhirIntegrationService.ValidationSuite.Models;
using FhirIntegrationService.ValidationSuite.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace FhirIntegrationService.ValidationSuite.Cli;

/// <summary>
/// Command-line interface for FHIR validation suite
/// </summary>
public class ValidationCliProgram
{
    private static async Task<int> Main(string[] args)
    {
        // Set up dependency injection
        var services = new ServiceCollection();
        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();

        // Create root command
        var rootCommand = new RootCommand("FHIR Validation Suite - Automated validation of FHIR resources")
        {
            Name = "fhir-validate"
        };

        // Add validate command
        var validateCommand = CreateValidateCommand(serviceProvider);
        rootCommand.AddCommand(validateCommand);

        // Add validate-directory command
        var validateDirectoryCommand = CreateValidateDirectoryCommand(serviceProvider);
        rootCommand.AddCommand(validateDirectoryCommand);

        // Execute command
        return await rootCommand.InvokeAsync(args);
    }

    /// <summary>
    /// Configure dependency injection services
    /// </summary>
    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        services.AddScoped<IFhirResourceValidator, FhirResourceValidator>();
        services.AddScoped<IValidationReportGenerator, ValidationReportGenerator>();
    }

    /// <summary>
    /// Create validate command for single resources
    /// </summary>
    private static Command CreateValidateCommand(ServiceProvider serviceProvider)
    {
        var validateCommand = new Command("validate", "Validate a single FHIR resource");

        var resourceOption = new Option<string>(
            name: "--resource",
            description: "Path to the FHIR resource file (JSON or XML)")
        {
            IsRequired = true
        };

        var profileOption = new Option<string[]>(
            name: "--profiles",
            description: "Profile URLs to validate against (optional)")
        {
            AllowMultipleArgumentsPerToken = true
        };

        var outputOption = new Option<string>(
            name: "--output",
            description: "Output directory for validation reports (default: ./validation-output)",
            getDefaultValue: () => "./validation-output");

        var formatOption = new Option<string[]>(
            name: "--formats",
            description: "Report formats to generate: html, json, csv, console (default: console)",
            getDefaultValue: () => new[] { "console" })
        {
            AllowMultipleArgumentsPerToken = true
        };

        var verboseOption = new Option<bool>(
            name: "--verbose",
            description: "Include detailed validation information",
            getDefaultValue: () => false);

        validateCommand.AddOption(resourceOption);
        validateCommand.AddOption(profileOption);
        validateCommand.AddOption(outputOption);
        validateCommand.AddOption(formatOption);
        validateCommand.AddOption(verboseOption);

        validateCommand.SetHandler(async (string resource, string[] profiles, string output, string[] formats, bool verbose) =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<ValidationCliProgram>>();
            var validator = serviceProvider.GetRequiredService<IFhirResourceValidator>();
            var reportGenerator = serviceProvider.GetRequiredService<IValidationReportGenerator>();

            try
            {
                logger.LogInformation("Starting validation for resource: {Resource}", resource);

                if (!File.Exists(resource))
                {
                    logger.LogError("Resource file not found: {Resource}", resource);
                    Environment.Exit(1);
                }

                // Read resource content
                var content = await File.ReadAllTextAsync(resource);
                ValidationResult result;

                if (resource.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    result = await validator.ValidateResourceFromJsonAsync(content, profiles);
                }
                else if (resource.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    result = await validator.ValidateResourceFromXmlAsync(content, profiles);
                }
                else
                {
                    logger.LogError("Unsupported file format. Only .json and .xml files are supported.");
                    Environment.Exit(1);
                    return;
                }

                // Create batch report for consistency
                var batchReport = new BatchValidationReport
                {
                    BatchName = $"Single Resource Validation: {Path.GetFileName(resource)}",
                    Results = new List<ValidationResult> { result },
                    ValidationStartTime = DateTime.UtcNow,
                    ValidationEndTime = DateTime.UtcNow,
                    TotalValidationDuration = result.ValidationDuration
                };

                batchReport.Summary = new ValidationSummary
                {
                    TotalResources = 1,
                    PassedResources = result.IsValid ? 1 : 0,
                    FailedResources = result.IsValid ? 0 : 1,
                    WarningResources = result.HasWarnings ? 1 : 0,
                    TotalIssues = result.Issues.Count,
                    OverallSuccess = result.IsValid
                };

                // Generate reports
                await GenerateReports(batchReport, output, formats, verbose, logger, reportGenerator);

                // Set exit code
                Environment.Exit(result.IsValid ? 0 : 1);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during validation");
                Environment.Exit(1);
            }
        }, resourceOption, profileOption, outputOption, formatOption, verboseOption);

        return validateCommand;
    }

    /// <summary>
    /// Create validate-directory command for batch validation
    /// </summary>
    private static Command CreateValidateDirectoryCommand(ServiceProvider serviceProvider)
    {
        var validateDirCommand = new Command("validate-directory", "Validate all FHIR resources in a directory");

        var directoryOption = new Option<string>(
            name: "--directory",
            description: "Path to directory containing FHIR resources")
        {
            IsRequired = true
        };

        var patternOption = new Option<string>(
            name: "--pattern",
            description: "File pattern to match (default: *.*)",
            getDefaultValue: () => "*.*");

        var profileOption = new Option<string[]>(
            name: "--profiles",
            description: "Profile URLs to validate against (optional)")
        {
            AllowMultipleArgumentsPerToken = true
        };

        var outputOption = new Option<string>(
            name: "--output",
            description: "Output directory for validation reports (default: ./validation-output)",
            getDefaultValue: () => "./validation-output");

        var formatOption = new Option<string[]>(
            name: "--formats",
            description: "Report formats to generate: html, json, csv, console (default: console)",
            getDefaultValue: () => new[] { "console" })
        {
            AllowMultipleArgumentsPerToken = true
        };

        var verboseOption = new Option<bool>(
            name: "--verbose",
            description: "Include detailed validation information",
            getDefaultValue: () => false);

        var thresholdOption = new Option<double>(
            name: "--pass-threshold",
            description: "Minimum pass rate threshold for success (default: 95.0)",
            getDefaultValue: () => 95.0);

        validateDirCommand.AddOption(directoryOption);
        validateDirCommand.AddOption(patternOption);
        validateDirCommand.AddOption(profileOption);
        validateDirCommand.AddOption(outputOption);
        validateDirCommand.AddOption(formatOption);
        validateDirCommand.AddOption(verboseOption);
        validateDirCommand.AddOption(thresholdOption);

        validateDirCommand.SetHandler(async (string directory, string pattern, string[] profiles, string output, string[] formats, bool verbose, double threshold) =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<ValidationCliProgram>>();
            var validator = serviceProvider.GetRequiredService<IFhirResourceValidator>();
            var reportGenerator = serviceProvider.GetRequiredService<IValidationReportGenerator>();

            try
            {
                logger.LogInformation("Starting batch validation for directory: {Directory}", directory);

                if (!Directory.Exists(directory))
                {
                    logger.LogError("Directory not found: {Directory}", directory);
                    Environment.Exit(1);
                }

                // Set up progress reporting
                var progress = new Progress<BatchValidationProgress>(p =>
                {
                    if (verbose)
                    {
                        logger.LogInformation("Progress: {Current}/{Total} ({Percentage:F1}%) - {Resource}",
                            p.CurrentResource, p.TotalResources, p.ProgressPercentage, p.CurrentResourceName);
                    }
                });

                // Perform batch validation
                var report = await validator.ValidateDirectoryAsync(directory, pattern, profiles, progress);

                // Update configuration with threshold
                report.Configuration.MinimumPassRateThreshold = threshold;
                report.Summary.OverallSuccess = report.Summary.PassRate >= threshold;

                // Generate reports
                await GenerateReports(report, output, formats, verbose, logger, reportGenerator);

                // Generate CI summary
                var ciSummary = await reportGenerator.GenerateCiSummaryAsync(report);
                logger.LogInformation("CI Summary: {Summary}", ciSummary.Summary);

                // Set exit code
                Environment.Exit(ciSummary.ExitCode);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during batch validation");
                Environment.Exit(1);
            }
        }, directoryOption, patternOption, profileOption, outputOption, formatOption, verboseOption, thresholdOption);

        return validateDirCommand;
    }

    /// <summary>
    /// Generate validation reports in specified formats
    /// </summary>
    private static async Task GenerateReports(BatchValidationReport report, string outputPath, string[] formats, bool verbose, ILogger logger, IValidationReportGenerator reportGenerator)
    {
        // Ensure output directory exists
        Directory.CreateDirectory(outputPath);

        var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        var baseFileName = $"validation-report-{timestamp}";

        foreach (var format in formats)
        {
            try
            {
                switch (format.ToLower())
                {
                    case "html":
                        var htmlPath = Path.Combine(outputPath, $"{baseFileName}.html");
                        await reportGenerator.GenerateHtmlReportAsync(report, htmlPath);
                        logger.LogInformation("HTML report generated: {Path}", htmlPath);
                        break;

                    case "json":
                        var jsonPath = Path.Combine(outputPath, $"{baseFileName}.json");
                        await reportGenerator.GenerateJsonReportAsync(report, jsonPath);
                        logger.LogInformation("JSON report generated: {Path}", jsonPath);
                        break;

                    case "csv":
                        var csvPath = Path.Combine(outputPath, $"{baseFileName}.csv");
                        await reportGenerator.GenerateCsvReportAsync(report, csvPath);
                        logger.LogInformation("CSV report generated: {Path}", csvPath);
                        break;

                    case "console":
                        var consoleReport = await reportGenerator.GenerateConsoleReportAsync(report, verbose);
                        Console.WriteLine(consoleReport);
                        break;

                    default:
                        logger.LogWarning("Unknown report format: {Format}", format);
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error generating {Format} report", format);
            }
        }
    }
}