namespace FhirIntegrationService.Services.Interfaces;

/// <summary>
/// Interface for health check operations
/// </summary>
public interface IHealthCheckService
{
    /// <summary>
    /// Performs a comprehensive health check of all system dependencies
    /// </summary>
    /// <returns>Health check result with status and details</returns>
    Task<HealthCheckResult> CheckHealthAsync();

    /// <summary>
    /// Checks the connectivity to the FHIR server
    /// </summary>
    /// <returns>True if FHIR server is accessible, false otherwise</returns>
    Task<bool> CheckFhirServerHealthAsync();

    /// <summary>
    /// Checks the connectivity to vendor APIs
    /// </summary>
    /// <returns>True if vendor APIs are accessible, false otherwise</returns>
    Task<bool> CheckVendorApiHealthAsync();
}

/// <summary>
/// Represents the result of a health check operation
/// </summary>
public class HealthCheckResult
{
    /// <summary>
    /// Overall health status
    /// </summary>
    public HealthStatus Status { get; set; }

    /// <summary>
    /// Detailed health check information
    /// </summary>
    public Dictionary<string, object> Details { get; set; } = new();

    /// <summary>
    /// Description of the health check result
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Duration of the health check
    /// </summary>
    public TimeSpan Duration { get; set; }
}

/// <summary>
/// Health status enumeration
/// </summary>
public enum HealthStatus
{
    Healthy,
    Degraded,
    Unhealthy
}