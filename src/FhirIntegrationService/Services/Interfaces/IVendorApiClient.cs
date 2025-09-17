using FhirIntegrationService.Services.Interfaces;

namespace FhirIntegrationService.Services.Interfaces;

/// <summary>
/// Interface for vendor API client operations
/// </summary>
public interface IVendorApiClient
{
    /// <summary>
    /// Retrieves patient data from vendor API
    /// </summary>
    /// <param name="patientId">Patient identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Vendor patient data</returns>
    Task<VendorPatientData> GetPatientAsync(string patientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves observation data for a patient from vendor API
    /// </summary>
    /// <param name="patientId">Patient identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of vendor observation data</returns>
    Task<List<VendorObservationData>> GetPatientObservationsAsync(string patientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if vendor API is healthy and responsive
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if API is healthy, false otherwise</returns>
    Task<bool> CheckHealthAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates with vendor API using configured credentials
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if authentication successful, false otherwise</returns>
    Task<bool> AuthenticateAsync(CancellationToken cancellationToken = default);
}