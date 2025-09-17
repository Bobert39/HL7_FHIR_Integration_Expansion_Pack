using FhirIntegrationService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FhirIntegrationService.Controllers;

/// <summary>
/// Controller for health check endpoints
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    private readonly IHealthCheckService _healthCheckService;
    private readonly ILogger<HealthController> _logger;

    /// <summary>
    /// Initializes a new instance of the HealthController
    /// </summary>
    /// <param name="healthCheckService">Health check service</param>
    /// <param name="logger">Logger instance</param>
    public HealthController(IHealthCheckService healthCheckService, ILogger<HealthController> logger)
    {
        _healthCheckService = healthCheckService ?? throw new ArgumentNullException(nameof(healthCheckService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Basic health check endpoint
    /// </summary>
    /// <returns>Health status of the service</returns>
    [HttpGet]
    public async Task<IActionResult> GetHealth()
    {
        try
        {
            var healthResult = await _healthCheckService.CheckHealthAsync();

            var statusCode = healthResult.Status switch
            {
                HealthStatus.Healthy => StatusCodes.Status200OK,
                HealthStatus.Degraded => StatusCodes.Status200OK,
                HealthStatus.Unhealthy => StatusCodes.Status503ServiceUnavailable,
                _ => StatusCodes.Status500InternalServerError
            };

            _logger.LogInformation("Health check requested, status: {Status}", healthResult.Status);

            return StatusCode(statusCode, new
            {
                status = healthResult.Status.ToString().ToLowerInvariant(),
                description = healthResult.Description,
                details = healthResult.Details,
                duration = healthResult.Duration.TotalMilliseconds
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check endpoint failed");

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                status = "unhealthy",
                description = "Health check service is not available",
                details = new { error = "Internal server error" },
                duration = 0
            });
        }
    }

    /// <summary>
    /// Detailed health check endpoint with component status
    /// </summary>
    /// <returns>Detailed health status of all components</returns>
    [HttpGet("detailed")]
    public async Task<IActionResult> GetDetailedHealth()
    {
        try
        {
            var healthResult = await _healthCheckService.CheckHealthAsync();
            var fhirHealthy = await _healthCheckService.CheckFhirServerHealthAsync();
            var vendorHealthy = await _healthCheckService.CheckVendorApiHealthAsync();

            var detailedResult = new
            {
                status = healthResult.Status.ToString().ToLowerInvariant(),
                description = healthResult.Description,
                duration = healthResult.Duration.TotalMilliseconds,
                timestamp = DateTime.UtcNow,
                components = new
                {
                    fhir_server = new
                    {
                        status = fhirHealthy ? "healthy" : "unhealthy",
                        description = fhirHealthy ? "FHIR server is accessible" : "FHIR server is not accessible"
                    },
                    vendor_api = new
                    {
                        status = vendorHealthy ? "healthy" : "degraded",
                        description = vendorHealthy ? "Vendor API is accessible" : "Vendor API connectivity issues"
                    }
                },
                details = healthResult.Details
            };

            var statusCode = healthResult.Status switch
            {
                HealthStatus.Healthy => StatusCodes.Status200OK,
                HealthStatus.Degraded => StatusCodes.Status200OK,
                HealthStatus.Unhealthy => StatusCodes.Status503ServiceUnavailable,
                _ => StatusCodes.Status500InternalServerError
            };

            return StatusCode(statusCode, detailedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Detailed health check endpoint failed");

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                status = "unhealthy",
                description = "Detailed health check service is not available",
                components = new { },
                details = new { error = "Internal server error" },
                duration = 0
            });
        }
    }
}