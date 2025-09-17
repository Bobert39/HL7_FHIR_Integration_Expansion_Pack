namespace FhirIntegrationService.Services.Interfaces;

/// <summary>
/// Interface for SMART on FHIR v2 authentication operations
/// </summary>
public interface ISmartAuthenticationService
{
    /// <summary>
    /// Validates a JWT token and extracts claims
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Token validation result with claims</returns>
    Task<TokenValidationResult> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the provided scopes are authorized for the resource
    /// </summary>
    /// <param name="requiredScopes">Required scopes for the operation</param>
    /// <param name="tokenScopes">Scopes present in the token</param>
    /// <param name="resourceType">FHIR resource type</param>
    /// <param name="operation">Operation type (read, write, etc.)</param>
    /// <returns>True if authorized, false otherwise</returns>
    bool IsAuthorized(string[] requiredScopes, string[] tokenScopes, string resourceType, string operation);

    /// <summary>
    /// Refreshes an access token using a refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New token response</returns>
    Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the authorization URL for SMART on FHIR flow
    /// </summary>
    /// <param name="clientId">Client ID</param>
    /// <param name="redirectUri">Redirect URI</param>
    /// <param name="scopes">Requested scopes</param>
    /// <param name="state">State parameter for security</param>
    /// <param name="codeChallenge">PKCE code challenge</param>
    /// <returns>Authorization URL</returns>
    string GetAuthorizationUrl(string clientId, string redirectUri, string[] scopes, string state, string codeChallenge);

    /// <summary>
    /// Exchanges authorization code for access token
    /// </summary>
    /// <param name="code">Authorization code</param>
    /// <param name="redirectUri">Redirect URI</param>
    /// <param name="clientId">Client ID</param>
    /// <param name="codeVerifier">PKCE code verifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Token response</returns>
    Task<TokenResponse> ExchangeCodeForTokenAsync(string code, string redirectUri, string clientId, string codeVerifier, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of token validation
/// </summary>
public class TokenValidationResult
{
    /// <summary>
    /// Whether the token is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Claims extracted from the token
    /// </summary>
    public Dictionary<string, object> Claims { get; set; } = new();

    /// <summary>
    /// Scopes granted to the token
    /// </summary>
    public string[] Scopes { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Token expiration time
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Subject identifier
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Client identifier
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Issuer of the token
    /// </summary>
    public string? Issuer { get; set; }

    /// <summary>
    /// Validation error message if token is invalid
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// OAuth 2.0 token response
/// </summary>
public class TokenResponse
{
    /// <summary>
    /// Access token
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Token type (usually Bearer)
    /// </summary>
    public string? TokenType { get; set; }

    /// <summary>
    /// Token lifetime in seconds
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Refresh token for obtaining new access tokens
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Granted scopes
    /// </summary>
    public string? Scope { get; set; }

    /// <summary>
    /// Error code if token request failed
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Error description if token request failed
    /// </summary>
    public string? ErrorDescription { get; set; }
}