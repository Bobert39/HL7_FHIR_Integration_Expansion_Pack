using FhirIntegrationService.Exceptions;
using FhirIntegrationService.Services;
using FhirIntegrationService.Services.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using Xunit;

namespace FhirIntegrationService.Tests;

/// <summary>
/// Unit tests for VendorApiClient
/// </summary>
public class VendorApiClientTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly Mock<ILogger<VendorApiClient>> _mockLogger;
    private readonly VendorApiConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly VendorApiClient _vendorApiClient;

    public VendorApiClientTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _mockLogger = new Mock<ILogger<VendorApiClient>>();

        _configuration = new VendorApiConfiguration
        {
            BaseUrl = "https://api.vendor.com",
            TokenEndpoint = "/oauth/token",
            ClientId = "test-client",
            ClientSecret = "test-secret",
            Scope = "patient.read",
            TimeoutSeconds = 30,
            RetryCount = 3,
            CircuitBreakerFailureThreshold = 5,
            CircuitBreakerOpenDurationSeconds = 60
        };

        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri(_configuration.BaseUrl)
        };

        var options = Options.Create(_configuration);
        _vendorApiClient = new VendorApiClient(_httpClient, options, _mockLogger.Object);
    }

    [Fact]
    public async Task GetPatientAsync_WithValidId_ReturnsPatientData()
    {
        // Arrange
        var patientId = "test-patient-123";
        var expectedPatientData = new VendorPatientData
        {
            PatientId = patientId,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = "1980-01-15",
            Gender = "Male"
        };

        var tokenResponse = new
        {
            access_token = "test-access-token",
            token_type = "Bearer",
            expires_in = 3600,
            scope = "patient.read"
        };

        var patientResponse = JsonSerializer.Serialize(expectedPatientData, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Setup token endpoint response
        SetupHttpResponse(
            HttpMethod.Post,
            "/oauth/token",
            HttpStatusCode.OK,
            JsonSerializer.Serialize(tokenResponse));

        // Setup patient endpoint response
        SetupHttpResponse(
            HttpMethod.Get,
            $"/api/patients/{Uri.EscapeDataString(patientId)}",
            HttpStatusCode.OK,
            patientResponse);

        // Act
        var result = await _vendorApiClient.GetPatientAsync(patientId);

        // Assert
        result.Should().NotBeNull();
        result.PatientId.Should().Be(patientId);
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.Gender.Should().Be("Male");

        VerifyHttpCall(HttpMethod.Post, "/oauth/token", Times.Once());
        VerifyHttpCall(HttpMethod.Get, $"/api/patients/{Uri.EscapeDataString(patientId)}", Times.Once());
    }

    [Fact]
    public async Task GetPatientAsync_WithNullId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _vendorApiClient.GetPatientAsync(null!));
    }

    [Fact]
    public async Task GetPatientAsync_WhenPatientNotFound_ReturnsNull()
    {
        // Arrange
        var patientId = "non-existent-patient";

        var tokenResponse = new
        {
            access_token = "test-access-token",
            token_type = "Bearer",
            expires_in = 3600
        };

        // Setup token endpoint response
        SetupHttpResponse(
            HttpMethod.Post,
            "/oauth/token",
            HttpStatusCode.OK,
            JsonSerializer.Serialize(tokenResponse));

        // Setup patient endpoint response (404)
        SetupHttpResponse(
            HttpMethod.Get,
            $"/api/patients/{Uri.EscapeDataString(patientId)}",
            HttpStatusCode.NotFound,
            "");

        // Act
        var result = await _vendorApiClient.GetPatientAsync(patientId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPatientAsync_WhenVendorApiReturnsError_ThrowsVendorApiException()
    {
        // Arrange
        var patientId = "test-patient-123";

        var tokenResponse = new
        {
            access_token = "test-access-token",
            token_type = "Bearer",
            expires_in = 3600
        };

        // Setup token endpoint response
        SetupHttpResponse(
            HttpMethod.Post,
            "/oauth/token",
            HttpStatusCode.OK,
            JsonSerializer.Serialize(tokenResponse));

        // Setup patient endpoint response (500)
        SetupHttpResponse(
            HttpMethod.Get,
            $"/api/patients/{Uri.EscapeDataString(patientId)}",
            HttpStatusCode.InternalServerError,
            "Internal server error");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<VendorApiException>(() => _vendorApiClient.GetPatientAsync(patientId));
        exception.StatusCode.Should().Be(500);
        exception.ResponseContent.Should().Be("Internal server error");
    }

    [Fact]
    public async Task GetPatientObservationsAsync_WithValidId_ReturnsObservations()
    {
        // Arrange
        var patientId = "test-patient-123";
        var expectedObservations = new List<VendorObservationData>
        {
            new VendorObservationData
            {
                ObservationId = "obs-1",
                PatientId = patientId,
                ObservationType = "vital-signs",
                Value = "120",
                Unit = "mmHg"
            },
            new VendorObservationData
            {
                ObservationId = "obs-2",
                PatientId = patientId,
                ObservationType = "lab-result",
                Value = "7.2",
                Unit = "mg/dL"
            }
        };

        var tokenResponse = new
        {
            access_token = "test-access-token",
            token_type = "Bearer",
            expires_in = 3600
        };

        var observationsResponse = JsonSerializer.Serialize(expectedObservations, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Setup token endpoint response
        SetupHttpResponse(
            HttpMethod.Post,
            "/oauth/token",
            HttpStatusCode.OK,
            JsonSerializer.Serialize(tokenResponse));

        // Setup observations endpoint response
        SetupHttpResponse(
            HttpMethod.Get,
            $"/api/patients/{Uri.EscapeDataString(patientId)}/observations",
            HttpStatusCode.OK,
            observationsResponse);

        // Act
        var result = await _vendorApiClient.GetPatientObservationsAsync(patientId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].ObservationId.Should().Be("obs-1");
        result[1].ObservationId.Should().Be("obs-2");
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ReturnsTrue()
    {
        // Arrange
        var tokenResponse = new
        {
            access_token = "test-access-token",
            token_type = "Bearer",
            expires_in = 3600,
            scope = "patient.read"
        };

        SetupHttpResponse(
            HttpMethod.Post,
            "/oauth/token",
            HttpStatusCode.OK,
            JsonSerializer.Serialize(tokenResponse));

        // Act
        var result = await _vendorApiClient.AuthenticateAsync();

        // Assert
        result.Should().BeTrue();
        VerifyHttpCall(HttpMethod.Post, "/oauth/token", Times.Once());
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidCredentials_ReturnsFalse()
    {
        // Arrange
        var errorResponse = new
        {
            error = "invalid_client",
            error_description = "Invalid client credentials"
        };

        SetupHttpResponse(
            HttpMethod.Post,
            "/oauth/token",
            HttpStatusCode.Unauthorized,
            JsonSerializer.Serialize(errorResponse));

        // Act
        var result = await _vendorApiClient.AuthenticateAsync();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CheckHealthAsync_WhenApiIsHealthy_ReturnsTrue()
    {
        // Arrange
        SetupHttpResponse(
            HttpMethod.Get,
            "/api/health",
            HttpStatusCode.OK,
            "OK");

        // Act
        var result = await _vendorApiClient.CheckHealthAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CheckHealthAsync_WhenApiIsUnhealthy_ReturnsFalse()
    {
        // Arrange
        SetupHttpResponse(
            HttpMethod.Get,
            "/api/health",
            HttpStatusCode.ServiceUnavailable,
            "Service Unavailable");

        // Act
        var result = await _vendorApiClient.CheckHealthAsync();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetPatientAsync_WhenHttpRequestExceptionThrown_ThrowsVendorApiException()
    {
        // Arrange
        var patientId = "test-patient-123";

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<VendorApiException>(() => _vendorApiClient.GetPatientAsync(patientId));
        exception.Message.Should().Contain("Network error");
    }

    private void SetupHttpResponse(HttpMethod method, string requestUri, HttpStatusCode statusCode, string content)
    {
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == method &&
                    req.RequestUri!.PathAndQuery.Contains(requestUri)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            });
    }

    private void VerifyHttpCall(HttpMethod method, string requestUri, Times times)
    {
        _mockHttpMessageHandler
            .Protected()
            .Verify(
                "SendAsync",
                times,
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == method &&
                    req.RequestUri!.PathAndQuery.Contains(requestUri)),
                ItExpr.IsAny<CancellationToken>());
    }
}