using FhirIntegrationService.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace FhirIntegrationService.Services;

/// <summary>
/// Implementation of SMART on FHIR v2 authentication service
/// </summary>
public class SmartAuthenticationService : ISmartAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly SmartAuthConfiguration _configuration;
    private readonly ILogger<SmartAuthenticationService> _logger;
    private readonly JsonWebTokenHandler _tokenHandler;
    private TokenValidationParameters? _validationParameters;

    /// <summary>
    /// Initializes a new instance of SmartAuthenticationService
    /// </summary>
    /// <param name="httpClient">HTTP client for token operations</param>
    /// <param name="configuration">SMART authentication configuration</param>
    /// <param name="logger">Logger instance</param>
    public SmartAuthenticationService(
        HttpClient httpClient,
        IOptions<SmartAuthConfiguration> configuration,
        ILogger<SmartAuthenticationService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tokenHandler = new JsonWebTokenHandler();

        // Configure HTTP client
        _httpClient.Timeout = TimeSpan.FromSeconds(_configuration.TokenRequestTimeoutSeconds);
    }

    /// <inheritdoc />
    public async Task<TokenValidationResult> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return new TokenValidationResult
            {
                IsValid = false,
                ErrorMessage = "Token is null or empty"
            };
        }

        try
        {
            // Ensure validation parameters are initialized
            if (_validationParameters == null)
            {
                await InitializeValidationParametersAsync(cancellationToken);
            }

            // Validate the token
            var validationResult = await _tokenHandler.ValidateTokenAsync(token, _validationParameters);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Token validation failed: {Exception}", validationResult.Exception?.Message);
                return new TokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = validationResult.Exception?.Message ?? "Token validation failed"
                };
            }

            // Extract claims
            var claims = new Dictionary<string, object>();
            var scopes = Array.Empty<string>();
            string? subject = null;
            string? clientId = null;
            string? issuer = null;
            var expiresAt = DateTime.UtcNow;

            if (validationResult.SecurityToken is JsonWebToken jwt)
            {
                foreach (var claim in jwt.Claims)
                {
                    claims[claim.Type] = claim.Value;

                    switch (claim.Type)
                    {
                        case "scope":
                            scopes = claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            break;
                        case "sub":
                            subject = claim.Value;
                            break;
                        case "client_id":
                        case "aud":
                            clientId = claim.Value;
                            break;
                        case "iss":
                            issuer = claim.Value;
                            break;
                        case "exp":
                            if (long.TryParse(claim.Value, out var exp))
                            {
                                expiresAt = DateTimeOffset.FromUnixTimeSeconds(exp).DateTime;
                            }
                            break;
                    }
                }
            }

            _logger.LogDebug("Token validated successfully for subject: {Subject}, scopes: {Scopes}",
                subject, string.Join(", ", scopes));

            return new TokenValidationResult
            {
                IsValid = true,
                Claims = claims,
                Scopes = scopes,
                Subject = subject,
                ClientId = clientId,
                Issuer = issuer,
                ExpiresAt = expiresAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating JWT token");
            return new TokenValidationResult
            {
                IsValid = false,
                ErrorMessage = ex.Message
            };
        }
    }

    /// <inheritdoc />
    public bool IsAuthorized(string[] requiredScopes, string[] tokenScopes, string resourceType, string operation)
    {
        if (requiredScopes == null || requiredScopes.Length == 0)
            return true;

        if (tokenScopes == null || tokenScopes.Length == 0)
            return false;

        // Check for exact scope matches
        foreach (var requiredScope in requiredScopes)
        {
            if (tokenScopes.Contains(requiredScope, StringComparer.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        // Check for wildcard scopes (e.g., patient/*.read, patient/*.*)
        var wildcardScopes = new[]
        {
            $"{resourceType.ToLowerInvariant()}/*.{operation}",
            $"{resourceType.ToLowerInvariant()}/*.*",
            "*.read", // Global read access
            "*.*"     // Global access
        };

        foreach (var wildcardScope in wildcardScopes)
        {
            if (tokenScopes.Contains(wildcardScope, StringComparer.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        _logger.LogWarning("Authorization failed. Required: {RequiredScopes}, Token: {TokenScopes}",
            string.Join(", ", requiredScopes), string.Join(", ", tokenScopes));

        return false;
    }

    /// <inheritdoc />
    public string GetAuthorizationUrl(string clientId, string redirectUri, string[] scopes, string state, string codeChallenge)
    {
        if (string.IsNullOrWhiteSpace(clientId))
            throw new ArgumentException("Client ID cannot be null or empty", nameof(clientId));

        if (string.IsNullOrWhiteSpace(redirectUri))
            throw new ArgumentException("Redirect URI cannot be null or empty", nameof(redirectUri));

        var queryParams = new Dictionary<string, string>
        {
            ["response_type"] = "code",
            ["client_id"] = clientId,
            ["redirect_uri"] = redirectUri,
            ["scope"] = string.Join(" ", scopes),
            ["state"] = state,
            ["code_challenge"] = codeChallenge,
            ["code_challenge_method"] = "S256",
            ["aud"] = _configuration.FhirServerUrl
        };

        var queryString = string.Join("&", queryParams.Select(kvp =>
            $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

        return $"{_configuration.AuthorizationEndpoint}?{queryString}";
    }

    /// <inheritdoc />
    public async Task<TokenResponse> ExchangeCodeForTokenAsync(string code, string redirectUri, string clientId, string codeVerifier, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Authorization code cannot be null or empty", nameof(code));

        try
        {
            var tokenRequest = new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["code"] = code,
                ["redirect_uri"] = redirectUri,
                ["client_id"] = clientId,
                ["code_verifier"] = codeVerifier
            };

            var content = new FormUrlEncodedContent(tokenRequest);
            var response = await _httpClient.PostAsync(_configuration.TokenEndpoint, content, cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Token exchange failed with status {StatusCode}: {Content}",
                    response.StatusCode, responseContent);

                var errorResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent, GetJsonOptions());
                return errorResponse ?? new TokenResponse
                {
                    Error = "token_exchange_failed",
                    ErrorDescription = $"HTTP {response.StatusCode}: {response.ReasonPhrase}"
                };
            }

            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent, GetJsonOptions());

            _logger.LogDebug("Successfully exchanged authorization code for access token");
            return tokenResponse ?? new TokenResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exchanging authorization code for token");
            return new TokenResponse
            {
                Error = "token_exchange_error",
                ErrorDescription = ex.Message
            };
        }
    }

    /// <inheritdoc />
    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Refresh token cannot be null or empty", nameof(refreshToken));

        try
        {
            var tokenRequest = new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken,
                ["client_id"] = _configuration.ClientId
            };

            // Add client secret if configured (confidential client)
            if (!string.IsNullOrEmpty(_configuration.ClientSecret))
            {
                tokenRequest["client_secret"] = _configuration.ClientSecret;
            }

            var content = new FormUrlEncodedContent(tokenRequest);
            var response = await _httpClient.PostAsync(_configuration.TokenEndpoint, content, cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Token refresh failed with status {StatusCode}: {Content}",
                    response.StatusCode, responseContent);

                var errorResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent, GetJsonOptions());
                return errorResponse ?? new TokenResponse
                {
                    Error = "token_refresh_failed",
                    ErrorDescription = $"HTTP {response.StatusCode}: {response.ReasonPhrase}"
                };
            }

            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent, GetJsonOptions());

            _logger.LogDebug("Successfully refreshed access token");
            return tokenResponse ?? new TokenResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return new TokenResponse
            {
                Error = "token_refresh_error",
                ErrorDescription = ex.Message
            };
        }
    }

    /// <summary>
    /// Initializes token validation parameters
    /// </summary>
    private async Task InitializeValidationParametersAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Fetch JWKS from the authorization server
            var jwksResponse = await _httpClient.GetAsync(_configuration.JwksEndpoint, cancellationToken);
            jwksResponse.EnsureSuccessStatusCode();

            var jwksContent = await jwksResponse.Content.ReadAsStringAsync(cancellationToken);
            var jwks = new JsonWebKeySet(jwksContent);

            _validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = jwks.Keys,
                ValidateIssuer = true,
                ValidIssuer = _configuration.Issuer,
                ValidateAudience = true,
                ValidAudience = _configuration.FhirServerUrl,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(_configuration.ClockSkewMinutes),
                RequireExpirationTime = true,
                RequireSignedTokens = true
            };

            _logger.LogDebug("Initialized token validation parameters");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize token validation parameters");
            throw;
        }
    }

    /// <summary>
    /// Gets JSON serialization options
    /// </summary>
    private static JsonSerializerOptions GetJsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Generates a code verifier for PKCE
    /// </summary>
    public static string GenerateCodeVerifier()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";
        var random = new Random();
        var codeVerifier = new char[128];

        for (int i = 0; i < codeVerifier.Length; i++)
        {
            codeVerifier[i] = chars[random.Next(chars.Length)];
        }

        return new string(codeVerifier);
    }

    /// <summary>
    /// Generates a code challenge from a code verifier
    /// </summary>
    public static string GenerateCodeChallenge(string codeVerifier)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
        return Convert.ToBase64String(hash)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}

/// <summary>
/// Configuration for SMART on FHIR authentication
/// </summary>
public class SmartAuthConfiguration
{
    /// <summary>
    /// Authorization server issuer URL
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Authorization endpoint URL
    /// </summary>
    public string AuthorizationEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Token endpoint URL
    /// </summary>
    public string TokenEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// JWKS endpoint URL for token validation
    /// </summary>
    public string JwksEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// FHIR server URL (audience)
    /// </summary>
    public string FhirServerUrl { get; set; } = string.Empty;

    /// <summary>
    /// Client ID for this application
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Client secret (for confidential clients)
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Clock skew tolerance in minutes
    /// </summary>
    public int ClockSkewMinutes { get; set; } = 5;

    /// <summary>
    /// Token request timeout in seconds
    /// </summary>
    public int TokenRequestTimeoutSeconds { get; set; } = 10;
}