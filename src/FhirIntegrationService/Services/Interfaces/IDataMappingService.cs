using Hl7.Fhir.Model;

namespace FhirIntegrationService.Services.Interfaces;

/// <summary>
/// Interface for data mapping and transformation operations between vendor formats and FHIR resources
/// </summary>
public interface IDataMappingService
{
    /// <summary>
    /// Transforms vendor patient data to FHIR Patient resource
    /// </summary>
    /// <param name="vendorPatientData">Vendor-specific patient data</param>
    /// <returns>FHIR Patient resource and transformation result</returns>
    Task<DataMappingResult<Patient>> TransformPatientAsync(VendorPatientData vendorPatientData);

    /// <summary>
    /// Transforms vendor observation data to FHIR Observation resource
    /// </summary>
    /// <param name="vendorObservationData">Vendor-specific observation data</param>
    /// <returns>FHIR Observation resource and transformation result</returns>
    Task<DataMappingResult<Observation>> TransformObservationAsync(VendorObservationData vendorObservationData);

    /// <summary>
    /// Validates vendor data format and structure before transformation
    /// </summary>
    /// <param name="vendorData">Raw vendor data to validate</param>
    /// <returns>Validation result with success status and error details</returns>
    Task<ValidationResult> ValidateVendorDataAsync(object vendorData);

    /// <summary>
    /// Handles vendor-specific data quirks and anomalies
    /// </summary>
    /// <param name="fieldName">Name of the field with quirks</param>
    /// <param name="rawValue">Raw value from vendor system</param>
    /// <returns>Normalized value ready for FHIR mapping</returns>
    Task<QuirkHandlingResult> HandleDataQuirkAsync(string fieldName, object rawValue);

    /// <summary>
    /// Validates FHIR resource against specified profiles
    /// </summary>
    /// <param name="resource">FHIR resource to validate</param>
    /// <param name="profileUrl">URL of the FHIR profile to validate against</param>
    /// <returns>Validation result with compliance status</returns>
    Task<FhirValidationResult> ValidateFhirResourceAsync(Resource resource, string profileUrl);
}

/// <summary>
/// Represents the result of a data mapping operation
/// </summary>
/// <typeparam name="T">Type of the FHIR resource</typeparam>
public class DataMappingResult<T> where T : Resource
{
    /// <summary>
    /// The transformed FHIR resource (null if transformation failed)
    /// </summary>
    public T? Resource { get; set; }

    /// <summary>
    /// Indicates whether the transformation was successful
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// List of errors encountered during transformation
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// List of warnings generated during transformation
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Metadata about the transformation process
    /// </summary>
    public TransformationMetadata Metadata { get; set; } = new();
}

/// <summary>
/// Represents vendor patient data structure
/// </summary>
public class VendorPatientData
{
    public string? PatientId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public VendorAddress? Address { get; set; }
    public List<VendorInsurance>? Insurance { get; set; }
    public Dictionary<string, object>? CustomFields { get; set; }
}

/// <summary>
/// Represents vendor observation data structure
/// </summary>
public class VendorObservationData
{
    public string? ObservationId { get; set; }
    public string? PatientId { get; set; }
    public string? ObservationType { get; set; }
    public string? Value { get; set; }
    public string? Unit { get; set; }
    public string? DateTime { get; set; }
    public string? Status { get; set; }
    public Dictionary<string, object>? CustomFields { get; set; }
}

/// <summary>
/// Represents vendor address data structure
/// </summary>
public class VendorAddress
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }
}

/// <summary>
/// Represents vendor insurance data structure
/// </summary>
public class VendorInsurance
{
    public string? InsuranceId { get; set; }
    public string? PlanName { get; set; }
    public string? PolicyNumber { get; set; }
    public string? GroupNumber { get; set; }
}

/// <summary>
/// Represents the result of vendor data validation
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Indicates whether the validation passed
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// List of validation errors
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// List of validation warnings
    /// </summary>
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// Represents the result of data quirk handling
/// </summary>
public class QuirkHandlingResult
{
    /// <summary>
    /// The normalized value after quirk handling
    /// </summary>
    public object? NormalizedValue { get; set; }

    /// <summary>
    /// Indicates whether quirk handling was successful
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Description of the quirk handling applied
    /// </summary>
    public string? QuirkDescription { get; set; }

    /// <summary>
    /// Any warnings or notes about the transformation
    /// </summary>
    public List<string> Notes { get; set; } = new();
}

/// <summary>
/// Represents the result of FHIR resource validation
/// </summary>
public class FhirValidationResult
{
    /// <summary>
    /// Indicates whether the FHIR resource is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// List of validation issues
    /// </summary>
    public List<string> Issues { get; set; } = new();

    /// <summary>
    /// The profile URL used for validation
    /// </summary>
    public string? ProfileUrl { get; set; }
}

/// <summary>
/// Metadata about the transformation process
/// </summary>
public class TransformationMetadata
{
    /// <summary>
    /// Time taken for the transformation
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Number of fields successfully mapped
    /// </summary>
    public int FieldsMapped { get; set; }

    /// <summary>
    /// Number of quirks handled
    /// </summary>
    public int QuirksHandled { get; set; }

    /// <summary>
    /// Transformation timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}