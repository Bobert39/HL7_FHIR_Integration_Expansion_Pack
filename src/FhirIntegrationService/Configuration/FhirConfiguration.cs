namespace FhirIntegrationService.Configuration;

/// <summary>
/// Configuration settings for FHIR server connection
/// </summary>
public class FhirConfiguration
{
    /// <summary>
    /// The base URL of the FHIR server
    /// </summary>
    public string ServerUrl { get; set; } = string.Empty;

    /// <summary>
    /// Timeout for FHIR server requests in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Whether to use compression for FHIR requests
    /// </summary>
    public bool UseCompression { get; set; } = true;

    /// <summary>
    /// Whether to verify SSL certificates
    /// </summary>
    public bool VerifySSL { get; set; } = true;

    /// <summary>
    /// FHIR version to use (R4, R5, etc.)
    /// </summary>
    public string FhirVersion { get; set; } = "R4";

    /// <summary>
    /// System identifier for patient identifiers
    /// </summary>
    public string PatientIdentifierSystem { get; set; } = "http://example.org/patient-ids";

    /// <summary>
    /// System identifier for observation identifiers
    /// </summary>
    public string ObservationIdentifierSystem { get; set; } = "http://example.org/observation-ids";

    /// <summary>
    /// Code system for observation codes
    /// </summary>
    public string ObservationCodeSystem { get; set; } = "http://loinc.org";

    /// <summary>
    /// Default organization reference for resources
    /// </summary>
    public string OrganizationReference { get; set; } = "Organization/default";

    /// <summary>
    /// Whether to enable strict FHIR validation
    /// </summary>
    public bool EnableStrictValidation { get; set; } = true;

    /// <summary>
    /// Maximum number of retry attempts for failed operations
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Delay between retry attempts in milliseconds
    /// </summary>
    public int RetryDelayMs { get; set; } = 1000;
}