using FhirIntegrationService.Controllers;
using FhirIntegrationService.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FhirIntegrationService.Tests;

/// <summary>
/// Unit tests for HealthController
/// </summary>
public class HealthControllerTests
{
    private readonly Mock<IHealthCheckService> _mockHealthCheckService;
    private readonly Mock<ILogger<HealthController>> _mockLogger;
    private readonly HealthController _controller;

    public HealthControllerTests()
    {
        _mockHealthCheckService = new Mock<IHealthCheckService>();
        _mockLogger = new Mock<ILogger<HealthController>>();
        _controller = new HealthController(_mockHealthCheckService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetHealth_WhenServiceReturnsHealthy_ShouldReturn200Ok()
    {
        // Arrange
        var healthResult = new HealthCheckResult
        {
            Status = HealthStatus.Healthy,
            Description = "All systems operational",
            Duration = TimeSpan.FromMilliseconds(100),
            Details = new Dictionary<string, object> { { "test", "value" } }
        };

        _mockHealthCheckService
            .Setup(x => x.CheckHealthAsync())
            .ReturnsAsync(healthResult);

        // Act
        var result = await _controller.GetHealth();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = objectResult.Value;
        response.Should().NotBeNull();

        // Verify the response structure using reflection
        var responseType = response!.GetType();
        var statusProperty = responseType.GetProperty("status");
        statusProperty!.GetValue(response).Should().Be("healthy");

        var descriptionProperty = responseType.GetProperty("description");
        descriptionProperty!.GetValue(response).Should().Be("All systems operational");

        var durationProperty = responseType.GetProperty("duration");
        durationProperty!.GetValue(response).Should().Be(100.0);
    }

    [Fact]
    public async Task GetHealth_WhenServiceReturnsDegraded_ShouldReturn200Ok()
    {
        // Arrange
        var healthResult = new HealthCheckResult
        {
            Status = HealthStatus.Degraded,
            Description = "Some issues detected",
            Duration = TimeSpan.FromMilliseconds(150),
            Details = new Dictionary<string, object> { { "vendor_api", "degraded" } }
        };

        _mockHealthCheckService
            .Setup(x => x.CheckHealthAsync())
            .ReturnsAsync(healthResult);

        // Act
        var result = await _controller.GetHealth();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = objectResult.Value;
        var responseType = response!.GetType();
        var statusProperty = responseType.GetProperty("status");
        statusProperty!.GetValue(response).Should().Be("degraded");
    }

    [Fact]
    public async Task GetHealth_WhenServiceReturnsUnhealthy_ShouldReturn503ServiceUnavailable()
    {
        // Arrange
        var healthResult = new HealthCheckResult
        {
            Status = HealthStatus.Unhealthy,
            Description = "Critical systems down",
            Duration = TimeSpan.FromMilliseconds(200),
            Details = new Dictionary<string, object> { { "fhir_server", "unhealthy" } }
        };

        _mockHealthCheckService
            .Setup(x => x.CheckHealthAsync())
            .ReturnsAsync(healthResult);

        // Act
        var result = await _controller.GetHealth();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status503ServiceUnavailable);

        var response = objectResult.Value;
        var responseType = response!.GetType();
        var statusProperty = responseType.GetProperty("status");
        statusProperty!.GetValue(response).Should().Be("unhealthy");
    }

    [Fact]
    public async Task GetHealth_WhenServiceThrowsException_ShouldReturn500InternalServerError()
    {
        // Arrange
        _mockHealthCheckService
            .Setup(x => x.CheckHealthAsync())
            .ThrowsAsync(new InvalidOperationException("Service unavailable"));

        // Act
        var result = await _controller.GetHealth();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        var response = objectResult.Value;
        var responseType = response!.GetType();
        var statusProperty = responseType.GetProperty("status");
        statusProperty!.GetValue(response).Should().Be("unhealthy");

        var descriptionProperty = responseType.GetProperty("description");
        descriptionProperty!.GetValue(response).Should().Be("Health check service is not available");
    }

    [Fact]
    public async Task GetDetailedHealth_WhenServiceReturnsHealthy_ShouldReturnDetailedInfo()
    {
        // Arrange
        var healthResult = new HealthCheckResult
        {
            Status = HealthStatus.Healthy,
            Description = "All systems operational",
            Duration = TimeSpan.FromMilliseconds(100),
            Details = new Dictionary<string, object> { { "test", "value" } }
        };

        _mockHealthCheckService
            .Setup(x => x.CheckHealthAsync())
            .ReturnsAsync(healthResult);

        _mockHealthCheckService
            .Setup(x => x.CheckFhirServerHealthAsync())
            .ReturnsAsync(true);

        _mockHealthCheckService
            .Setup(x => x.CheckVendorApiHealthAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _controller.GetDetailedHealth();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = objectResult.Value;
        response.Should().NotBeNull();

        // Verify the response contains components
        var responseType = response!.GetType();
        var componentsProperty = responseType.GetProperty("components");
        componentsProperty.Should().NotBeNull();
        componentsProperty!.GetValue(response).Should().NotBeNull();
    }

    [Fact]
    public async Task GetDetailedHealth_WhenServiceThrowsException_ShouldReturn500InternalServerError()
    {
        // Arrange
        _mockHealthCheckService
            .Setup(x => x.CheckHealthAsync())
            .ThrowsAsync(new InvalidOperationException("Service unavailable"));

        // Act
        var result = await _controller.GetDetailedHealth();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        var response = objectResult.Value;
        var responseType = response!.GetType();
        var statusProperty = responseType.GetProperty("status");
        statusProperty!.GetValue(response).Should().Be("unhealthy");
    }

    [Fact]
    public void Constructor_WhenHealthCheckServiceIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new HealthController(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WhenLoggerIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new HealthController(_mockHealthCheckService.Object, null!));
    }
}