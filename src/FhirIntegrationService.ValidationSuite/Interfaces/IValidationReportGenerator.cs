using FhirIntegrationService.ValidationSuite.Models;

namespace FhirIntegrationService.ValidationSuite.Interfaces;

/// <summary>
/// Interface for generating validation reports in different formats
/// </summary>
public interface IValidationReportGenerator
{
    /// <summary>
    /// Generates an HTML validation report
    /// </summary>
    /// <param name="report">The batch validation report to generate from</param>
    /// <param name="outputPath">Output file path for the HTML report</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Path to the generated HTML report</returns>
    Task<string> GenerateHtmlReportAsync(BatchValidationReport report, string outputPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a JSON validation report
    /// </summary>
    /// <param name="report">The batch validation report to generate from</param>
    /// <param name="outputPath">Output file path for the JSON report</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Path to the generated JSON report</returns>
    Task<string> GenerateJsonReportAsync(BatchValidationReport report, string outputPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a CSV validation report
    /// </summary>
    /// <param name="report">The batch validation report to generate from</param>
    /// <param name="outputPath">Output file path for the CSV report</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Path to the generated CSV report</returns>
    Task<string> GenerateCsvReportAsync(BatchValidationReport report, string outputPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a console-formatted validation report
    /// </summary>
    /// <param name="report">The batch validation report to generate from</param>
    /// <param name="includeDetails">Whether to include detailed issue information</param>
    /// <returns>Formatted console output string</returns>
    Task<string> GenerateConsoleReportAsync(BatchValidationReport report, bool includeDetails = false);

    /// <summary>
    /// Generates a validation summary report for CI/CD integration
    /// </summary>
    /// <param name="report">The batch validation report to generate from</param>
    /// <returns>CI/CD compatible summary</returns>
    Task<ValidationCiSummary> GenerateCiSummaryAsync(BatchValidationReport report);
}

/// <summary>
/// CI/CD integration summary for validation results
/// </summary>
public class ValidationCiSummary
{
    /// <summary>
    /// Overall pass/fail status
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Exit code for CI/CD systems
    /// </summary>
    public int ExitCode { get; set; }

    /// <summary>
    /// Brief summary message
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Detailed breakdown for CI/CD logs
    /// </summary>
    public string Details { get; set; } = string.Empty;

    /// <summary>
    /// Key metrics for CI/CD dashboard
    /// </summary>
    public Dictionary<string, object> Metrics { get; set; } = new();

    /// <summary>
    /// Artifacts generated during validation
    /// </summary>
    public List<string> Artifacts { get; set; } = new();
}