using FhirIntegrationService.ValidationSuite.Models;
using Hl7.Fhir.Model;

namespace FhirIntegrationService.ValidationSuite.Interfaces;

/// <summary>
/// Interface for FHIR resource validation using Firely .NET SDK
/// </summary>
public interface IFhirResourceValidator
{
    /// <summary>
    /// Validates a single FHIR resource against specified profiles
    /// </summary>
    /// <param name="resource">The FHIR resource to validate</param>
    /// <param name="profileUrls">Optional profile URLs to validate against</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Validation result with detailed findings</returns>
    Task<ValidationResult> ValidateResourceAsync(Resource resource, IEnumerable<string>? profileUrls = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a FHIR resource from JSON string
    /// </summary>
    /// <param name="resourceJson">JSON representation of the FHIR resource</param>
    /// <param name="profileUrls">Optional profile URLs to validate against</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Validation result with detailed findings</returns>
    Task<ValidationResult> ValidateResourceFromJsonAsync(string resourceJson, IEnumerable<string>? profileUrls = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a FHIR resource from XML string
    /// </summary>
    /// <param name="resourceXml">XML representation of the FHIR resource</param>
    /// <param name="profileUrls">Optional profile URLs to validate against</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Validation result with detailed findings</returns>
    Task<ValidationResult> ValidateResourceFromXmlAsync(string resourceXml, IEnumerable<string>? profileUrls = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a batch of FHIR resources from a directory
    /// </summary>
    /// <param name="directoryPath">Path to directory containing FHIR resources</param>
    /// <param name="filePattern">File pattern to match (default: *.json, *.xml)</param>
    /// <param name="profileUrls">Optional profile URLs to validate against</param>
    /// <param name="progressCallback">Optional progress callback for batch operations</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Batch validation report with aggregated results</returns>
    Task<BatchValidationReport> ValidateDirectoryAsync(string directoryPath, string filePattern = "*.*", IEnumerable<string>? profileUrls = null, IProgress<BatchValidationProgress>? progressCallback = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a collection of FHIR resources
    /// </summary>
    /// <param name="resources">Collection of FHIR resources to validate</param>
    /// <param name="profileUrls">Optional profile URLs to validate against</param>
    /// <param name="progressCallback">Optional progress callback for batch operations</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Batch validation report with aggregated results</returns>
    Task<BatchValidationReport> ValidateBatchAsync(IEnumerable<Resource> resources, IEnumerable<string>? profileUrls = null, IProgress<BatchValidationProgress>? progressCallback = null, CancellationToken cancellationToken = default);
}