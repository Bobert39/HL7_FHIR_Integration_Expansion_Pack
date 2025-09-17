namespace FhirIntegrationService.Exceptions;

/// <summary>
/// Exception thrown when FHIR validation operations fail
/// </summary>
public class FhirValidationException : Exception
{
    /// <summary>
    /// FHIR resource type that failed validation
    /// </summary>
    public string? ResourceType { get; }

    /// <summary>
    /// Profile URL used for validation
    /// </summary>
    public string? ProfileUrl { get; }

    /// <summary>
    /// List of validation errors
    /// </summary>
    public List<string> ValidationErrors { get; }

    /// <summary>
    /// Initializes a new instance of FhirValidationException
    /// </summary>
    /// <param name="message">Exception message</param>
    public FhirValidationException(string message) : base(message)
    {
        ValidationErrors = new List<string>();
    }

    /// <summary>
    /// Initializes a new instance of FhirValidationException
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exception</param>
    public FhirValidationException(string message, Exception innerException) : base(message, innerException)
    {
        ValidationErrors = new List<string>();
    }

    /// <summary>
    /// Initializes a new instance of FhirValidationException with detailed information
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="resourceType">FHIR resource type</param>
    /// <param name="profileUrl">Profile URL</param>
    /// <param name="validationErrors">List of validation errors</param>
    public FhirValidationException(string message, string? resourceType, string? profileUrl, List<string>? validationErrors = null)
        : base(message)
    {
        ResourceType = resourceType;
        ProfileUrl = profileUrl;
        ValidationErrors = validationErrors ?? new List<string>();
    }

    /// <summary>
    /// Initializes a new instance of FhirValidationException with detailed information and inner exception
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="resourceType">FHIR resource type</param>
    /// <param name="profileUrl">Profile URL</param>
    /// <param name="validationErrors">List of validation errors</param>
    /// <param name="innerException">Inner exception</param>
    public FhirValidationException(string message, string? resourceType, string? profileUrl, List<string>? validationErrors, Exception innerException)
        : base(message, innerException)
    {
        ResourceType = resourceType;
        ProfileUrl = profileUrl;
        ValidationErrors = validationErrors ?? new List<string>();
    }
}