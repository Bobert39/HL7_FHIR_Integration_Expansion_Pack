using Hl7.Fhir.Validation;

namespace FhirIntegrationService.ValidationSuite.Models;

/// <summary>
/// Represents the result of validating a single FHIR resource
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Unique identifier for this validation result
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Name or identifier of the resource being validated
    /// </summary>
    public string ResourceName { get; set; } = string.Empty;

    /// <summary>
    /// Type of the FHIR resource
    /// </summary>
    public string ResourceType { get; set; } = string.Empty;

    /// <summary>
    /// Whether the validation passed without errors
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// List of validation issues found
    /// </summary>
    public List<ValidationIssue> Issues { get; set; } = new();

    /// <summary>
    /// Profiles that were validated against
    /// </summary>
    public List<string> ValidatedProfiles { get; set; } = new();

    /// <summary>
    /// Time taken to perform validation
    /// </summary>
    public TimeSpan ValidationDuration { get; set; }

    /// <summary>
    /// Timestamp when validation was performed
    /// </summary>
    public DateTime ValidationTimestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Error count by severity
    /// </summary>
    public Dictionary<string, int> ErrorCountBySeverity => Issues
        .GroupBy(i => i.Severity.ToString())
        .ToDictionary(g => g.Key, g => g.Count());

    /// <summary>
    /// Whether this validation result has any errors (not warnings or info)
    /// </summary>
    public bool HasErrors => Issues.Any(i => i.Severity == IssueSeverity.Error || i.Severity == IssueSeverity.Fatal);

    /// <summary>
    /// Whether this validation result has any warnings
    /// </summary>
    public bool HasWarnings => Issues.Any(i => i.Severity == IssueSeverity.Warning);
}

/// <summary>
/// Represents a single validation issue
/// </summary>
public class ValidationIssue
{
    /// <summary>
    /// Severity level of the issue
    /// </summary>
    public IssueSeverity Severity { get; set; }

    /// <summary>
    /// Issue type or code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable description of the issue
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// FHIRPath or element path where the issue was found
    /// </summary>
    public string ElementPath { get; set; } = string.Empty;

    /// <summary>
    /// Location information for the issue
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Additional details about the issue
    /// </summary>
    public Dictionary<string, object> Details { get; set; } = new();
}

/// <summary>
/// Severity levels for validation issues
/// </summary>
public enum IssueSeverity
{
    Information,
    Warning,
    Error,
    Fatal
}