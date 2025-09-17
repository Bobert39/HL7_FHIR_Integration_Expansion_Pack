using FhirIntegrationService.ValidationSuite.Exceptions;
using FhirIntegrationService.ValidationSuite.Interfaces;
using FhirIntegrationService.ValidationSuite.Models;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace FhirIntegrationService.ValidationSuite.Services;

/// <summary>
/// Implementation of validation report generator for multiple formats
/// </summary>
public class ValidationReportGenerator : IValidationReportGenerator
{
    private readonly ILogger<ValidationReportGenerator> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ValidationReportGenerator(ILogger<ValidationReportGenerator> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <inheritdoc />
    public async Task<string> GenerateHtmlReportAsync(BatchValidationReport report, string outputPath, CancellationToken cancellationToken = default)
    {
        if (report == null)
            throw new ArgumentNullException(nameof(report));

        if (string.IsNullOrWhiteSpace(outputPath))
            throw new ArgumentException("Output path cannot be null or empty", nameof(outputPath));

        try
        {
            _logger.LogDebug("Generating HTML validation report to: {OutputPath}", outputPath);

            var html = GenerateHtmlContent(report);
            await File.WriteAllTextAsync(outputPath, html, cancellationToken);

            _logger.LogInformation("HTML validation report generated successfully: {OutputPath}", outputPath);
            return outputPath;
        }
        catch (Exception ex) when (!(ex is OperationCanceledException))
        {
            _logger.LogError(ex, "Error generating HTML validation report");
            throw new ReportGenerationException("HTML", $"Failed to generate HTML report: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<string> GenerateJsonReportAsync(BatchValidationReport report, string outputPath, CancellationToken cancellationToken = default)
    {
        if (report == null)
            throw new ArgumentNullException(nameof(report));

        if (string.IsNullOrWhiteSpace(outputPath))
            throw new ArgumentException("Output path cannot be null or empty", nameof(outputPath));

        try
        {
            _logger.LogDebug("Generating JSON validation report to: {OutputPath}", outputPath);

            var json = JsonSerializer.Serialize(report, _jsonOptions);
            await File.WriteAllTextAsync(outputPath, json, cancellationToken);

            _logger.LogInformation("JSON validation report generated successfully: {OutputPath}", outputPath);
            return outputPath;
        }
        catch (Exception ex) when (!(ex is OperationCanceledException))
        {
            _logger.LogError(ex, "Error generating JSON validation report");
            throw new ReportGenerationException("JSON", $"Failed to generate JSON report: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<string> GenerateCsvReportAsync(BatchValidationReport report, string outputPath, CancellationToken cancellationToken = default)
    {
        if (report == null)
            throw new ArgumentNullException(nameof(report));

        if (string.IsNullOrWhiteSpace(outputPath))
            throw new ArgumentException("Output path cannot be null or empty", nameof(outputPath));

        try
        {
            _logger.LogDebug("Generating CSV validation report to: {OutputPath}", outputPath);

            var csv = GenerateCsvContent(report);
            await File.WriteAllTextAsync(outputPath, csv, cancellationToken);

            _logger.LogInformation("CSV validation report generated successfully: {OutputPath}", outputPath);
            return outputPath;
        }
        catch (Exception ex) when (!(ex is OperationCanceledException))
        {
            _logger.LogError(ex, "Error generating CSV validation report");
            throw new ReportGenerationException("CSV", $"Failed to generate CSV report: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<string> GenerateConsoleReportAsync(BatchValidationReport report, bool includeDetails = false)
    {
        if (report == null)
            throw new ArgumentNullException(nameof(report));

        return await Task.Run(() => GenerateConsoleContent(report, includeDetails));
    }

    /// <inheritdoc />
    public async Task<ValidationCiSummary> GenerateCiSummaryAsync(BatchValidationReport report)
    {
        if (report == null)
            throw new ArgumentNullException(nameof(report));

        return await Task.Run(() =>
        {
            var summary = new ValidationCiSummary
            {
                Success = report.Summary.OverallSuccess,
                ExitCode = report.Summary.OverallSuccess ? 0 : 1,
                Summary = $"Validation completed: {report.Summary.PassedResources}/{report.Summary.TotalResources} resources passed ({report.Summary.PassRate:F1}%)",
                Details = GenerateDetailedCiSummary(report),
                Metrics = new Dictionary<string, object>
                {
                    ["total_resources"] = report.Summary.TotalResources,
                    ["passed_resources"] = report.Summary.PassedResources,
                    ["failed_resources"] = report.Summary.FailedResources,
                    ["warning_resources"] = report.Summary.WarningResources,
                    ["pass_rate"] = report.Summary.PassRate,
                    ["total_issues"] = report.Summary.TotalIssues,
                    ["validation_duration_seconds"] = report.TotalValidationDuration.TotalSeconds,
                    ["average_validation_time_ms"] = report.PerformanceMetrics.AverageValidationTime.TotalMilliseconds,
                    ["resources_per_second"] = report.PerformanceMetrics.ResourcesPerSecond
                }
            };

            return summary;
        });
    }

    /// <summary>
    /// Generates HTML content for the validation report
    /// </summary>
    private string GenerateHtmlContent(BatchValidationReport report)
    {
        var html = new StringBuilder();

        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang=\"en\">");
        html.AppendLine("<head>");
        html.AppendLine("    <meta charset=\"UTF-8\">");
        html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        html.AppendLine("    <title>FHIR Validation Report</title>");
        html.AppendLine("    <style>");
        html.AppendLine(GetHtmlStyles());
        html.AppendLine("    </style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");

        // Header
        html.AppendLine("    <div class=\"header\">");
        html.AppendLine("        <h1>FHIR Validation Report</h1>");
        html.AppendLine($"        <p class=\"batch-name\">{report.BatchName}</p>");
        html.AppendLine($"        <p class=\"timestamp\">Generated: {report.ValidationEndTime:yyyy-MM-dd HH:mm:ss} UTC</p>");
        html.AppendLine("    </div>");

        // Summary
        html.AppendLine("    <div class=\"summary\">");
        html.AppendLine("        <h2>Summary</h2>");
        html.AppendLine("        <div class=\"summary-grid\">");
        html.AppendLine($"            <div class=\"summary-item\"><span class=\"label\">Total Resources:</span> <span class=\"value\">{report.Summary.TotalResources}</span></div>");
        html.AppendLine($"            <div class=\"summary-item\"><span class=\"label\">Passed:</span> <span class=\"value success\">{report.Summary.PassedResources}</span></div>");
        html.AppendLine($"            <div class=\"summary-item\"><span class=\"label\">Failed:</span> <span class=\"value error\">{report.Summary.FailedResources}</span></div>");
        html.AppendLine($"            <div class=\"summary-item\"><span class=\"label\">Warnings:</span> <span class=\"value warning\">{report.Summary.WarningResources}</span></div>");
        html.AppendLine($"            <div class=\"summary-item\"><span class=\"label\">Pass Rate:</span> <span class=\"value\">{report.Summary.PassRate:F1}%</span></div>");
        html.AppendLine($"            <div class=\"summary-item\"><span class=\"label\">Duration:</span> <span class=\"value\">{report.TotalValidationDuration:mm\\:ss}</span></div>");
        html.AppendLine("        </div>");

        var statusClass = report.Summary.OverallSuccess ? "success" : "error";
        var statusText = report.Summary.OverallSuccess ? "PASSED" : "FAILED";
        html.AppendLine($"        <div class=\"overall-status {statusClass}\">Overall Status: {statusText}</div>");
        html.AppendLine("    </div>");

        // Performance Metrics
        html.AppendLine("    <div class=\"performance\">");
        html.AppendLine("        <h2>Performance Metrics</h2>");
        html.AppendLine("        <div class=\"metrics-grid\">");
        html.AppendLine($"            <div class=\"metric\"><span class=\"label\">Average Time:</span> <span class=\"value\">{report.PerformanceMetrics.AverageValidationTime.TotalMilliseconds:F0}ms</span></div>");
        html.AppendLine($"            <div class=\"metric\"><span class=\"label\">Min Time:</span> <span class=\"value\">{report.PerformanceMetrics.MinimumValidationTime.TotalMilliseconds:F0}ms</span></div>");
        html.AppendLine($"            <div class=\"metric\"><span class=\"label\">Max Time:</span> <span class=\"value\">{report.PerformanceMetrics.MaximumValidationTime.TotalMilliseconds:F0}ms</span></div>");
        html.AppendLine($"            <div class=\"metric\"><span class=\"label\">Resources/Second:</span> <span class=\"value\">{report.PerformanceMetrics.ResourcesPerSecond:F2}</span></div>");
        html.AppendLine("        </div>");
        html.AppendLine("    </div>");

        // Issues by Severity
        if (report.Summary.IssuesBySeverity.Any())
        {
            html.AppendLine("    <div class=\"issues-summary\">");
            html.AppendLine("        <h2>Issues by Severity</h2>");
            html.AppendLine("        <div class=\"issues-grid\">");
            foreach (var issue in report.Summary.IssuesBySeverity.OrderBy(i => i.Key))
            {
                html.AppendLine($"            <div class=\"issue-item\"><span class=\"label\">{issue.Key}:</span> <span class=\"value\">{issue.Value}</span></div>");
            }
            html.AppendLine("        </div>");
            html.AppendLine("    </div>");
        }

        // Detailed Results
        html.AppendLine("    <div class=\"detailed-results\">");
        html.AppendLine("        <h2>Detailed Results</h2>");
        html.AppendLine("        <table class=\"results-table\">");
        html.AppendLine("            <thead>");
        html.AppendLine("                <tr>");
        html.AppendLine("                    <th>Resource</th>");
        html.AppendLine("                    <th>Type</th>");
        html.AppendLine("                    <th>Status</th>");
        html.AppendLine("                    <th>Issues</th>");
        html.AppendLine("                    <th>Duration</th>");
        html.AppendLine("                </tr>");
        html.AppendLine("            </thead>");
        html.AppendLine("            <tbody>");

        foreach (var result in report.Results.OrderBy(r => r.ResourceName))
        {
            var statusClass = result.IsValid ? "success" : "error";
            var statusText = result.IsValid ? "VALID" : "INVALID";

            html.AppendLine("                <tr>");
            html.AppendLine($"                    <td>{result.ResourceName}</td>");
            html.AppendLine($"                    <td>{result.ResourceType}</td>");
            html.AppendLine($"                    <td class=\"{statusClass}\">{statusText}</td>");
            html.AppendLine($"                    <td>{result.Issues.Count}</td>");
            html.AppendLine($"                    <td>{result.ValidationDuration.TotalMilliseconds:F0}ms</td>");
            html.AppendLine("                </tr>");

            // Add issues if any
            if (result.Issues.Any())
            {
                html.AppendLine("                <tr class=\"issues-row\">");
                html.AppendLine("                    <td colspan=\"5\">");
                html.AppendLine("                        <div class=\"issues-detail\">");
                foreach (var issue in result.Issues)
                {
                    var issueClass = issue.Severity.ToString().ToLower();
                    html.AppendLine($"                            <div class=\"issue {issueClass}\">");
                    html.AppendLine($"                                <span class=\"severity\">{issue.Severity}</span>");
                    html.AppendLine($"                                <span class=\"code\">{issue.Code}</span>");
                    html.AppendLine($"                                <span class=\"description\">{issue.Description}</span>");
                    if (!string.IsNullOrEmpty(issue.ElementPath))
                    {
                        html.AppendLine($"                                <span class=\"path\">Path: {issue.ElementPath}</span>");
                    }
                    html.AppendLine("                            </div>");
                }
                html.AppendLine("                        </div>");
                html.AppendLine("                    </td>");
                html.AppendLine("                </tr>");
            }
        }

        html.AppendLine("            </tbody>");
        html.AppendLine("        </table>");
        html.AppendLine("    </div>");

        html.AppendLine("</body>");
        html.AppendLine("</html>");

        return html.ToString();
    }

    /// <summary>
    /// Gets CSS styles for HTML report
    /// </summary>
    private string GetHtmlStyles()
    {
        return @"
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }
        .header { background: #2c3e50; color: white; padding: 20px; margin: -20px -20px 20px -20px; }
        .header h1 { margin: 0; }
        .batch-name { font-size: 1.2em; margin: 10px 0 5px 0; }
        .timestamp { margin: 0; opacity: 0.8; }
        .summary, .performance, .issues-summary, .detailed-results { background: white; padding: 20px; margin-bottom: 20px; border-radius: 5px; box-shadow: 0 2px 5px rgba(0,0,0,0.1); }
        .summary-grid, .metrics-grid, .issues-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 15px; margin-bottom: 20px; }
        .summary-item, .metric, .issue-item { padding: 10px; background: #f8f9fa; border-radius: 3px; }
        .label { font-weight: bold; }
        .value { float: right; }
        .success { color: #28a745; font-weight: bold; }
        .error { color: #dc3545; font-weight: bold; }
        .warning { color: #ffc107; font-weight: bold; }
        .overall-status { text-align: center; padding: 15px; border-radius: 5px; font-size: 1.2em; font-weight: bold; }
        .overall-status.success { background: #d4edda; color: #155724; border: 1px solid #c3e6cb; }
        .overall-status.error { background: #f8d7da; color: #721c24; border: 1px solid #f5c6cb; }
        .results-table { width: 100%; border-collapse: collapse; }
        .results-table th, .results-table td { padding: 10px; text-align: left; border-bottom: 1px solid #ddd; }
        .results-table th { background: #f8f9fa; font-weight: bold; }
        .issues-row { background: #f8f9fa; }
        .issues-detail { padding: 10px; }
        .issue { margin-bottom: 5px; padding: 5px; border-left: 3px solid #ddd; }
        .issue.error { border-left-color: #dc3545; }
        .issue.warning { border-left-color: #ffc107; }
        .issue.information { border-left-color: #17a2b8; }
        .issue.fatal { border-left-color: #6f42c1; }
        .severity { font-weight: bold; margin-right: 10px; }
        .code { background: #e9ecef; padding: 2px 6px; border-radius: 3px; margin-right: 10px; }
        .path { font-style: italic; color: #6c757d; margin-left: 10px; }
        ";
    }

    /// <summary>
    /// Generates CSV content for the validation report
    /// </summary>
    private string GenerateCsvContent(BatchValidationReport report)
    {
        var csv = new StringBuilder();

        // Header
        csv.AppendLine("ResourceName,ResourceType,IsValid,IssueCount,ValidationDurationMs,Issues");

        // Data rows
        foreach (var result in report.Results)
        {
            var issues = string.Join("; ", result.Issues.Select(i => $"{i.Severity}:{i.Code}:{i.Description}"));
            csv.AppendLine($"\"{result.ResourceName}\",\"{result.ResourceType}\",{result.IsValid},{result.Issues.Count},{result.ValidationDuration.TotalMilliseconds:F0},\"{issues}\"");
        }

        return csv.ToString();
    }

    /// <summary>
    /// Generates console content for the validation report
    /// </summary>
    private string GenerateConsoleContent(BatchValidationReport report, bool includeDetails)
    {
        var console = new StringBuilder();

        console.AppendLine("=".PadRight(80, '='));
        console.AppendLine($"FHIR VALIDATION REPORT");
        console.AppendLine($"Batch: {report.BatchName}");
        console.AppendLine($"Generated: {report.ValidationEndTime:yyyy-MM-dd HH:mm:ss} UTC");
        console.AppendLine("=".PadRight(80, '='));
        console.AppendLine();

        // Summary
        console.AppendLine("SUMMARY:");
        console.AppendLine($"  Total Resources: {report.Summary.TotalResources}");
        console.AppendLine($"  Passed:          {report.Summary.PassedResources}");
        console.AppendLine($"  Failed:          {report.Summary.FailedResources}");
        console.AppendLine($"  Warnings:        {report.Summary.WarningResources}");
        console.AppendLine($"  Pass Rate:       {report.Summary.PassRate:F1}%");
        console.AppendLine($"  Duration:        {report.TotalValidationDuration:mm\\:ss}");
        console.AppendLine();

        var status = report.Summary.OverallSuccess ? "PASSED" : "FAILED";
        console.AppendLine($"OVERALL STATUS: {status}");
        console.AppendLine();

        // Performance
        console.AppendLine("PERFORMANCE:");
        console.AppendLine($"  Average Time:    {report.PerformanceMetrics.AverageValidationTime.TotalMilliseconds:F0}ms");
        console.AppendLine($"  Resources/Sec:   {report.PerformanceMetrics.ResourcesPerSecond:F2}");
        console.AppendLine();

        // Issues summary
        if (report.Summary.IssuesBySeverity.Any())
        {
            console.AppendLine("ISSUES BY SEVERITY:");
            foreach (var issue in report.Summary.IssuesBySeverity.OrderBy(i => i.Key))
            {
                console.AppendLine($"  {issue.Key}: {issue.Value}");
            }
            console.AppendLine();
        }

        // Detailed results if requested
        if (includeDetails)
        {
            console.AppendLine("DETAILED RESULTS:");
            console.AppendLine("-".PadRight(80, '-'));

            foreach (var result in report.Results.OrderBy(r => r.ResourceName))
            {
                var status_detail = result.IsValid ? "VALID" : "INVALID";
                console.AppendLine($"{result.ResourceName} ({result.ResourceType}): {status_detail} - {result.Issues.Count} issues - {result.ValidationDuration.TotalMilliseconds:F0}ms");

                if (result.Issues.Any())
                {
                    foreach (var issue in result.Issues)
                    {
                        console.AppendLine($"  [{issue.Severity}] {issue.Code}: {issue.Description}");
                        if (!string.IsNullOrEmpty(issue.ElementPath))
                        {
                            console.AppendLine($"    Path: {issue.ElementPath}");
                        }
                    }
                }
                console.AppendLine();
            }
        }

        return console.ToString();
    }

    /// <summary>
    /// Generates detailed CI summary
    /// </summary>
    private string GenerateDetailedCiSummary(BatchValidationReport report)
    {
        var details = new StringBuilder();

        details.AppendLine($"Batch: {report.BatchName}");
        details.AppendLine($"Duration: {report.TotalValidationDuration:mm\\:ss}");
        details.AppendLine($"Resources: {report.Summary.TotalResources} total, {report.Summary.PassedResources} passed, {report.Summary.FailedResources} failed");

        if (report.Summary.IssuesBySeverity.Any())
        {
            details.AppendLine("Issues:");
            foreach (var issue in report.Summary.IssuesBySeverity.OrderBy(i => i.Key))
            {
                details.AppendLine($"  {issue.Key}: {issue.Value}");
            }
        }

        if (!report.Summary.OverallSuccess)
        {
            details.AppendLine("Failed Resources:");
            foreach (var result in report.Results.Where(r => !r.IsValid).Take(10))
            {
                details.AppendLine($"  - {result.ResourceName} ({result.ResourceType}): {result.Issues.Count} issues");
            }
        }

        return details.ToString();
    }
}