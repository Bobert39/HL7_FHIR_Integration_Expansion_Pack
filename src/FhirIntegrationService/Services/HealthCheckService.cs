using FhirIntegrationService.Configuration;
using FhirIntegrationService.Services.Interfaces;
using Hl7.Fhir.Rest;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace FhirIntegrationService.Services;

/// <summary>
/// Implementation of health check service for monitoring system dependencies
/// </summary>
public class HealthCheckService : IHealthCheckService
{
    private readonly FhirClient _fhirClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly FhirConfiguration _fhirConfig;
    private readonly ILogger<HealthCheckService> _logger;

    /// <summary>
    /// Initializes a new instance of the HealthCheckService
    /// </summary>
    public HealthCheckService(
        FhirClient fhirClient,
        IHttpClientFactory httpClientFactory,
        IOptions<FhirConfiguration> fhirConfig,
        ILogger<HealthCheckService> logger)
    {
        _fhirClient = fhirClient ?? throw new ArgumentNullException(nameof(fhirClient));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _fhirConfig = fhirConfig?.Value ?? throw new ArgumentNullException(nameof(fhirConfig));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Performs a comprehensive health check of all system dependencies
    /// </summary>
    public async Task<HealthCheckResult> CheckHealthAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new HealthCheckResult
        {
            Status = HealthStatus.Healthy,
            Description = "All systems operational"
        };

        try
        {
            // Check FHIR server
            var fhirHealthy = await CheckFhirServerHealthAsync();
            result.Details["fhir_server"] = fhirHealthy ? "Healthy" : "Unhealthy";

            // Check vendor API
            var vendorHealthy = await CheckVendorApiHealthAsync();
            result.Details["vendor_api"] = vendorHealthy ? "Healthy" : "Degraded";

            // Add system information
            result.Details["timestamp"] = DateTime.UtcNow;
            result.Details["version"] = GetType().Assembly.GetName().Version?.ToString() ?? "Unknown";
            result.Details["environment"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";

            // Determine overall status
            if (!fhirHealthy)
            {
                result.Status = HealthStatus.Unhealthy;
                result.Description = "FHIR server is not accessible";
            }
            else if (!vendorHealthy)
            {
                result.Status = HealthStatus.Degraded;
                result.Description = "Vendor API connectivity issues detected";
            }

            _logger.LogInformation("Health check completed with status: {Status}", result.Status);
        }
        catch (Exception ex)
        {
            result.Status = HealthStatus.Unhealthy;
            result.Description = "Health check failed due to system error";
            result.Details["error"] = "System error during health check";

            _logger.LogError(ex, "Health check failed with exception");
        }
        finally
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
            result.Details["duration_ms"] = stopwatch.ElapsedMilliseconds;
        }

        return result;
    }

    /// <summary>
    /// Checks the connectivity to the FHIR server
    /// </summary>
    public async Task<bool> CheckFhirServerHealthAsync()
    {
        try
        {
            // Attempt to read the capability statement
            var capabilityStatement = await _fhirClient.CapabilityStatementAsync();

            _logger.LogDebug("FHIR server health check successful");
            return capabilityStatement != null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "FHIR server health check failed");
            return false;
        }
    }

    /// <summary>
    /// Checks the connectivity to vendor APIs
    /// </summary>
    public async Task<bool> CheckVendorApiHealthAsync()
    {
        try
        {
            using var httpClient = _httpClientFactory.CreateClient("VendorApi");

            // This is a placeholder check - in a real implementation,
            // this would check the actual vendor API endpoints
            var response = await httpClient.GetAsync("https://httpbin.org/status/200");

            var isHealthy = response.IsSuccessStatusCode;
            _logger.LogDebug("Vendor API health check result: {IsHealthy}", isHealthy);

            return isHealthy;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Vendor API health check failed");
            return false;
        }
    }
}