using FhirIntegrationService.Exceptions;
using FhirIntegrationService.Services.Interfaces;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FhirIntegrationService.Services;

/// <summary>
/// Implementation of vendor API client with resilience patterns
/// </summary>
public class VendorApiClient : IVendorApiClient
{
    private readonly HttpClient _httpClient;
    private readonly VendorApiConfiguration _configuration;
    private readonly ILogger<VendorApiClient> _logger;
    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;
    private string? _accessToken;
    private DateTime _tokenExpiration = DateTime.MinValue;

    /// <summary>
    /// Initializes a new instance of VendorApiClient
    /// </summary>
    /// <param name="httpClient">HTTP client instance</param>
    /// <param name="configuration">Vendor API configuration</param>
    /// <param name="logger">Logger instance</param>
    public VendorApiClient(
        HttpClient httpClient,
        IOptions<VendorApiConfiguration> configuration,
        ILogger<VendorApiClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Configure HTTP client
        _httpClient.BaseAddress = new Uri(_configuration.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_configuration.TimeoutSeconds);
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // Configure Polly retry policy with circuit breaker
        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode && (int)r.StatusCode >= 500)
            .WaitAndRetryAsync(
                retryCount: _configuration.RetryCount,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning("Retry attempt {RetryCount} for vendor API call after {Delay}ms delay",
                        retryCount, timespan.TotalMilliseconds);
                })
            .WrapAsync(Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: _configuration.CircuitBreakerFailureThreshold,
                    durationOfBreak: TimeSpan.FromSeconds(_configuration.CircuitBreakerOpenDurationSeconds),
                    onBreak: (exception, duration) =>
                    {
                        _logger.LogError("Circuit breaker opened for {Duration}s due to: {Exception}",
                            duration.TotalSeconds, exception.Exception?.Message ?? exception.Result?.ReasonPhrase);
                    },
                    onReset: () => _logger.LogInformation("Circuit breaker reset - vendor API calls resumed")));
    }

    /// <inheritdoc />
    public async Task<VendorPatientData> GetPatientAsync(string patientId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(patientId))
            throw new ArgumentException("Patient ID cannot be null or empty", nameof(patientId));

        await EnsureAuthenticatedAsync(cancellationToken);

        var endpoint = $"/api/patients/{Uri.EscapeDataString(patientId)}";

        try
        {
            _logger.LogDebug("Retrieving patient data from vendor API");

            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                AddAuthenticationHeader(request);
                return await _httpClient.SendAsync(request, cancellationToken);
            });

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null!; // Patient not found
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new VendorApiException(
                    $"Vendor API returned error: {response.ReasonPhrase}",
                    (int)response.StatusCode,
                    errorContent,
                    endpoint);
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var patientData = JsonSerializer.Deserialize<VendorPatientData>(content, GetJsonOptions());

            if (patientData == null)
            {
                throw new VendorApiException("Failed to deserialize patient data from vendor API response", null, content, endpoint);
            }

            _logger.LogDebug("Successfully retrieved patient data from vendor API");
            return patientData;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Patient retrieval was cancelled");
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error occurred while retrieving patient data");
            throw new VendorApiException("Network error occurred while calling vendor API", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Vendor API call timed out");
            throw new VendorApiException("Vendor API call timed out", ex);
        }
    }

    /// <inheritdoc />
    public async Task<List<VendorObservationData>> GetPatientObservationsAsync(string patientId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(patientId))
            throw new ArgumentException("Patient ID cannot be null or empty", nameof(patientId));

        await EnsureAuthenticatedAsync(cancellationToken);

        var endpoint = $"/api/patients/{Uri.EscapeDataString(patientId)}/observations";

        try
        {
            _logger.LogDebug("Retrieving patient observations from vendor API");

            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                AddAuthenticationHeader(request);
                return await _httpClient.SendAsync(request, cancellationToken);
            });

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<VendorObservationData>(); // No observations found
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new VendorApiException(
                    $"Vendor API returned error: {response.ReasonPhrase}",
                    (int)response.StatusCode,
                    errorContent,
                    endpoint);
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var observations = JsonSerializer.Deserialize<List<VendorObservationData>>(content, GetJsonOptions()) ?? new List<VendorObservationData>();

            _logger.LogDebug("Successfully retrieved {Count} observations from vendor API", observations.Count);
            return observations;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Patient observations retrieval was cancelled");
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error occurred while retrieving patient observations");
            throw new VendorApiException("Network error occurred while calling vendor API", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Vendor API call timed out");
            throw new VendorApiException("Vendor API call timed out", ex);
        }
    }

    /// <inheritdoc />
    public async Task<bool> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/health", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Vendor API health check failed");
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> AuthenticateAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var authRequest = new
            {
                client_id = _configuration.ClientId,
                client_secret = _configuration.ClientSecret,
                grant_type = "client_credentials",
                scope = _configuration.Scope
            };

            var authContent = new StringContent(
                JsonSerializer.Serialize(authRequest, GetJsonOptions()),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(_configuration.TokenEndpoint, authContent, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Authentication failed with status {StatusCode}: {ErrorContent}",
                    response.StatusCode, errorContent);
                return false;
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent, GetJsonOptions());

            if (tokenResponse?.AccessToken == null)
            {
                _logger.LogError("Authentication response did not contain access token");
                return false;
            }

            _accessToken = tokenResponse.AccessToken;
            _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60); // Refresh 1 minute early

            _logger.LogDebug("Successfully authenticated with vendor API");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during authentication");
            return false;
        }
    }

    /// <summary>
    /// Ensures the client is authenticated and token is valid
    /// </summary>
    private async Task EnsureAuthenticatedAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_accessToken) || DateTime.UtcNow >= _tokenExpiration)
        {
            var authenticated = await AuthenticateAsync(cancellationToken);
            if (!authenticated)
            {
                throw new VendorApiException("Failed to authenticate with vendor API");
            }
        }
    }

    /// <summary>
    /// Adds authentication header to the request
    /// </summary>
    private void AddAuthenticationHeader(HttpRequestMessage request)
    {
        if (!string.IsNullOrEmpty(_accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }
    }

    /// <summary>
    /// Gets JSON serialization options
    /// </summary>
    private static JsonSerializerOptions GetJsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }
}

/// <summary>
/// Configuration for vendor API client
/// </summary>
public class VendorApiConfiguration
{
    /// <summary>
    /// Base URL of the vendor API
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Token endpoint for authentication
    /// </summary>
    public string TokenEndpoint { get; set; } = "/oauth/token";

    /// <summary>
    /// Client ID for API authentication
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Client secret for API authentication
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// OAuth scope for API access
    /// </summary>
    public string Scope { get; set; } = "patient.read";

    /// <summary>
    /// Request timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Number of retry attempts
    /// </summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Circuit breaker failure threshold
    /// </summary>
    public int CircuitBreakerFailureThreshold { get; set; } = 5;

    /// <summary>
    /// Circuit breaker open duration in seconds
    /// </summary>
    public int CircuitBreakerOpenDurationSeconds { get; set; } = 60;
}

/// <summary>
/// Token response from vendor API
/// </summary>
internal class TokenResponse
{
    public string? AccessToken { get; set; }
    public string? TokenType { get; set; }
    public int ExpiresIn { get; set; }
    public string? Scope { get; set; }
}