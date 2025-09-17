using Hl7.Fhir.Model;

namespace FhirIntegrationService.Services.Interfaces;

/// <summary>
/// Interface for FHIR resource validation operations using Firely SDK
/// </summary>
public interface IFhirValidationService
{
    /// <summary>
    /// Validates a FHIR resource against specified profile
    /// </summary>
    /// <param name="resource">FHIR resource to validate</param>
    /// <param name="profileUrl">Profile URL for validation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with detailed information</returns>
    Task<FhirValidationResult> ValidateResourceAsync(Resource resource, string profileUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a FHIR resource against default profile for the resource type
    /// </summary>
    /// <param name="resource">FHIR resource to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with detailed information</returns>
    Task<FhirValidationResult> ValidateResourceAsync(Resource resource, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an OperationOutcome for validation errors
    /// </summary>
    /// <param name="validationResult">Validation result containing errors</param>
    /// <returns>FHIR OperationOutcome resource</returns>
    OperationOutcome CreateOperationOutcome(FhirValidationResult validationResult);

    /// <summary>
    /// Gets the default profile URL for a given resource type
    /// </summary>
    /// <param name="resourceType">FHIR resource type</param>
    /// <returns>Default profile URL</returns>
    string GetDefaultProfileUrl(string resourceType);
}

/// <summary>
/// Enhanced FHIR validation result with additional details
/// </summary>
public class FhirValidationResult
{
    /// <summary>
    /// Indicates whether the FHIR resource is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// List of validation issues with severity levels
    /// </summary>
    public List<ValidationIssue> Issues { get; set; } = new();

    /// <summary>
    /// The profile URL used for validation
    /// </summary>
    public string? ProfileUrl { get; set; }

    /// <summary>
    /// Validation timestamp
    /// </summary>
    public DateTime ValidationTimestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Time taken for validation
    /// </summary>
    public TimeSpan ValidationDuration { get; set; }
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
    /// Issue type/code
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Human-readable description of the issue
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Location in the resource where the issue occurred
    /// </summary>
    public string? Location { get; set; }
}

/// <summary>
/// Issue severity levels matching FHIR specification
/// </summary>
public enum IssueSeverity
{
    Fatal,
    Error,
    Warning,
    Information
}