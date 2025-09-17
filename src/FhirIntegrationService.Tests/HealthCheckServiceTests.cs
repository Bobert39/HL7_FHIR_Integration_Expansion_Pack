using FhirIntegrationService.Configuration;
using FhirIntegrationService.Services;
using FhirIntegrationService.Services.Interfaces;
using FluentAssertions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using Xunit;

namespace FhirIntegrationService.Tests;

/// <summary>
/// Unit tests for HealthCheckService
/// </summary>
public class HealthCheckServiceTests
{
    private readonly Mock<FhirClient> _mockFhirClient;
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<IOptions<FhirConfiguration>> _mockFhirConfig;
    private readonly Mock<ILogger<HealthCheckService>> _mockLogger;
    private readonly FhirConfiguration _fhirConfig;
    private readonly HealthCheckService _service;

    public HealthCheckServiceTests()
    {
        _mockFhirClient = new Mock<FhirClient>("https://test.fhir.org");
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockFhirConfig = new Mock<IOptions<FhirConfiguration>>();
        _mockLogger = new Mock<ILogger<HealthCheckService>>();

        _fhirConfig = new FhirConfiguration
        {
            ServerUrl = "https://test.fhir.org",
            TimeoutSeconds = 30,
            UseCompression = true,
            VerifySSL = true,
            FhirVersion = "R4"
        };

        _mockFhirConfig.Setup(x => x.Value).Returns(_fhirConfig);

        _service = new HealthCheckService(
            _mockFhirClient.Object,
            _mockHttpClientFactory.Object,
            _mockFhirConfig.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenAllSystemsHealthy_ShouldReturnHealthyStatus()
    {
        // Arrange
        var capabilityStatement = new CapabilityStatement
        {
            Status = PublicationStatus.Active
        };

        _mockFhirClient
            .Setup(x => x.CapabilityStatementAsync())
            .ReturnsAsync(capabilityStatement);

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        _mockHttpClientFactory
            .Setup(x => x.CreateClient("VendorApi"))
            .Returns(httpClient);

        // Act
        var result = await _service.CheckHealthAsync();

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(HealthStatus.Healthy);
        result.Description.Should().Be("All systems operational");
        result.Details.Should().ContainKey("fhir_server");
        result.Details.Should().ContainKey("vendor_api");
        result.Details.Should().ContainKey("timestamp");
        result.Details.Should().ContainKey("version");
        result.Details.Should().ContainKey("environment");
        result.Details.Should().ContainKey("duration_ms");
        result.Duration.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenFhirServerUnhealthy_ShouldReturnUnhealthyStatus()
    {
        // Arrange
        _mockFhirClient
            .Setup(x => x.CapabilityStatementAsync())
            .ThrowsAsync(new FhirOperationException("Server unavailable"));

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        _mockHttpClientFactory
            .Setup(x => x.CreateClient("VendorApi"))
            .Returns(httpClient);

        // Act
        var result = await _service.CheckHealthAsync();

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Description.Should().Be("FHIR server is not accessible");
        result.Details["fhir_server"].Should().Be("Unhealthy");
        result.Details["vendor_api"].Should().Be("Healthy");
    }

    [Fact]
    public async Task CheckHealthAsync_WhenVendorApiUnhealthy_ShouldReturnDegradedStatus()
    {
        // Arrange
        var capabilityStatement = new CapabilityStatement
        {
            Status = PublicationStatus.Active
        };

        _mockFhirClient
            .Setup(x => x.CapabilityStatementAsync())
            .ReturnsAsync(capabilityStatement);

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        _mockHttpClientFactory
            .Setup(x => x.CreateClient("VendorApi"))
            .Returns(httpClient);

        // Act
        var result = await _service.CheckHealthAsync();

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(HealthStatus.Degraded);
        result.Description.Should().Be("Vendor API connectivity issues detected");
        result.Details["fhir_server"].Should().Be("Healthy");
        result.Details["vendor_api"].Should().Be("Degraded");
    }

    [Fact]
    public async Task CheckHealthAsync_WhenExceptionThrown_ShouldReturnUnhealthyStatus()
    {
        // Arrange
        _mockFhirClient
            .Setup(x => x.CapabilityStatementAsync())
            .ThrowsAsync(new InvalidOperationException("Unexpected error"));

        _mockHttpClientFactory
            .Setup(x => x.CreateClient("VendorApi"))
            .Throws(new InvalidOperationException("HTTP client error"));

        // Act
        var result = await _service.CheckHealthAsync();

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Description.Should().Be("Health check failed due to system error");
        result.Details.Should().ContainKey("error");
        result.Details["error"].Should().Be("System error during health check");
    }

    [Fact]
    public async Task CheckFhirServerHealthAsync_WhenServerResponds_ShouldReturnTrue()
    {
        // Arrange
        var capabilityStatement = new CapabilityStatement
        {
            Status = PublicationStatus.Active
        };

        _mockFhirClient
            .Setup(x => x.CapabilityStatementAsync())
            .ReturnsAsync(capabilityStatement);

        // Act
        var result = await _service.CheckFhirServerHealthAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CheckFhirServerHealthAsync_WhenServerThrowsException_ShouldReturnFalse()
    {
        // Arrange
        _mockFhirClient
            .Setup(x => x.CapabilityStatementAsync())
            .ThrowsAsync(new FhirOperationException("Server error"));

        // Act
        var result = await _service.CheckFhirServerHealthAsync();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CheckVendorApiHealthAsync_WhenApiResponds_ShouldReturnTrue()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        _mockHttpClientFactory
            .Setup(x => x.CreateClient("VendorApi"))
            .Returns(httpClient);

        // Act
        var result = await _service.CheckVendorApiHealthAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CheckVendorApiHealthAsync_WhenApiThrowsException_ShouldReturnFalse()
    {
        // Arrange
        _mockHttpClientFactory
            .Setup(x => x.CreateClient("VendorApi"))
            .Throws(new HttpRequestException("Network error"));

        // Act
        var result = await _service.CheckVendorApiHealthAsync();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WhenFhirClientIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new HealthCheckService(null!, _mockHttpClientFactory.Object, _mockFhirConfig.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WhenHttpClientFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new HealthCheckService(_mockFhirClient.Object, null!, _mockFhirConfig.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WhenFhirConfigIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new HealthCheckService(_mockFhirClient.Object, _mockHttpClientFactory.Object, null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WhenLoggerIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new HealthCheckService(_mockFhirClient.Object, _mockHttpClientFactory.Object, _mockFhirConfig.Object, null!));
    }
}