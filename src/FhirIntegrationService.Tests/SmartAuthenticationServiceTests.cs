using FhirIntegrationService.Services;
using FhirIntegrationService.Services.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Moq.Protected;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Xunit;

namespace FhirIntegrationService.Tests;

/// <summary>
/// Unit tests for SmartAuthenticationService
/// </summary>
public class SmartAuthenticationServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly Mock<ILogger<SmartAuthenticationService>> _mockLogger;
    private readonly SmartAuthConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly SmartAuthenticationService _authService;
    private readonly string _testPrivateKey;
    private readonly SecurityKey _testSecurityKey;

    public SmartAuthenticationServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _mockLogger = new Mock<ILogger<SmartAuthenticationService>>();

        _configuration = new SmartAuthConfiguration
        {
            Issuer = "https://auth.example.com",
            AuthorizationEndpoint = "https://auth.example.com/oauth/authorize",
            TokenEndpoint = "https://auth.example.com/oauth/token",
            JwksEndpoint = "https://auth.example.com/.well-known/jwks.json",
            FhirServerUrl = "https://fhir.example.com",
            ClientId = "test-client",
            ClientSecret = "test-secret",
            ClockSkewMinutes = 5,
            TokenRequestTimeoutSeconds = 10
        };

        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);

        var options = Options.Create(_configuration);
        _authService = new SmartAuthenticationService(_httpClient, options, _mockLogger.Object);

        // Setup test key for JWT validation
        _testPrivateKey = "test-key-that-is-at-least-256-bits-long-for-hmac-sha256-algorithm";
        _testSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_testPrivateKey));
    }

    [Fact]
    public async Task ValidateTokenAsync_WithValidToken_ReturnsSuccessResult()
    {
        // Arrange
        var token = CreateTestJwtToken(new[]
        {
            new Claim("sub", "test-user"),
            new Claim("client_id", "test-client"),
            new Claim("iss", _configuration.Issuer),
            new Claim("aud", _configuration.FhirServerUrl),
            new Claim("scope", "patient/*.read patient/*.write"),
            new Claim("exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds().ToString())
        });

        SetupJwksEndpoint();

        // Act
        var result = await _authService.ValidateTokenAsync(token);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Subject.Should().Be("test-user");
        result.ClientId.Should().Be("test-client");
        result.Scopes.Should().Contain("patient/*.read");
        result.Scopes.Should().Contain("patient/*.write");
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task ValidateTokenAsync_WithNullToken_ReturnsFailureResult()
    {
        // Act
        var result = await _authService.ValidateTokenAsync(null!);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Token is null or empty");
    }

    [Fact]
    public async Task ValidateTokenAsync_WithEmptyToken_ReturnsFailureResult()
    {
        // Act
        var result = await _authService.ValidateTokenAsync("");

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Token is null or empty");
    }

    [Fact]
    public async Task ValidateTokenAsync_WithExpiredToken_ReturnsFailureResult()
    {
        // Arrange
        var token = CreateTestJwtToken(new[]
        {
            new Claim("sub", "test-user"),
            new Claim("iss", _configuration.Issuer),
            new Claim("aud", _configuration.FhirServerUrl),
            new Claim("exp", DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeSeconds().ToString()) // Expired
        });

        SetupJwksEndpoint();

        // Act
        var result = await _authService.ValidateTokenAsync(token);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNull();
    }

    [Theory]
    [InlineData(new[] { "patient/*.read" }, new[] { "patient/*.read" }, "Patient", "read", true)]
    [InlineData(new[] { "patient/*.read" }, new[] { "patient/*.write" }, "Patient", "read", false)]
    [InlineData(new[] { "patient/*.read" }, new[] { "patient/*.*" }, "Patient", "read", true)]
    [InlineData(new[] { "patient/*.read" }, new[] { "*.*" }, "Patient", "read", true)]
    [InlineData(new[] { "observation/*.read" }, new[] { "patient/*.read" }, "Observation", "read", false)]
    [InlineData(new[] { "patient/*.write" }, new[] { "patient/*.read", "patient/*.write" }, "Patient", "write", true)]
    public void IsAuthorized_WithVariousScopes_ReturnsExpectedResult(
        string[] requiredScopes,
        string[] tokenScopes,
        string resourceType,
        string operation,
        bool expectedResult)
    {
        // Act
        var result = _authService.IsAuthorized(requiredScopes, tokenScopes, resourceType, operation);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void IsAuthorized_WithEmptyRequiredScopes_ReturnsTrue()
    {
        // Act
        var result = _authService.IsAuthorized(
            Array.Empty<string>(),
            new[] { "patient/*.read" },
            "Patient",
            "read");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsAuthorized_WithEmptyTokenScopes_ReturnsFalse()
    {
        // Act
        var result = _authService.IsAuthorized(
            new[] { "patient/*.read" },
            Array.Empty<string>(),
            "Patient",
            "read");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetAuthorizationUrl_WithValidParameters_ReturnsFormattedUrl()
    {
        // Arrange
        var clientId = "test-client";
        var redirectUri = "https://app.example.com/callback";
        var scopes = new[] { "patient/*.read", "launch/patient" };
        var state = "test-state";
        var codeChallenge = "test-challenge";

        // Act
        var result = _authService.GetAuthorizationUrl(clientId, redirectUri, scopes, state, codeChallenge);

        // Assert
        result.Should().StartWith(_configuration.AuthorizationEndpoint);
        result.Should().Contain($"client_id={Uri.EscapeDataString(clientId)}");
        result.Should().Contain($"redirect_uri={Uri.EscapeDataString(redirectUri)}");
        result.Should().Contain($"scope={Uri.EscapeDataString(string.Join(" ", scopes))}");
        result.Should().Contain($"state={state}");
        result.Should().Contain($"code_challenge={codeChallenge}");
        result.Should().Contain("response_type=code");
        result.Should().Contain("code_challenge_method=S256");
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_WithValidCode_ReturnsTokenResponse()
    {
        // Arrange
        var code = "test-auth-code";
        var redirectUri = "https://app.example.com/callback";
        var clientId = "test-client";
        var codeVerifier = "test-verifier";

        var expectedTokenResponse = new TokenResponse
        {
            AccessToken = "test-access-token",
            TokenType = "Bearer",
            ExpiresIn = 3600,
            RefreshToken = "test-refresh-token",
            Scope = "patient/*.read"
        };

        SetupHttpResponse(
            HttpMethod.Post,
            _configuration.TokenEndpoint,
            HttpStatusCode.OK,
            JsonSerializer.Serialize(expectedTokenResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            }));

        // Act
        var result = await _authService.ExchangeCodeForTokenAsync(code, redirectUri, clientId, codeVerifier);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("test-access-token");
        result.TokenType.Should().Be("Bearer");
        result.ExpiresIn.Should().Be(3600);
        result.RefreshToken.Should().Be("test-refresh-token");
        result.Scope.Should().Be("patient/*.read");
        result.Error.Should().BeNull();
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_WithInvalidCode_ReturnsErrorResponse()
    {
        // Arrange
        var code = "invalid-code";
        var redirectUri = "https://app.example.com/callback";
        var clientId = "test-client";
        var codeVerifier = "test-verifier";

        var errorResponse = new TokenResponse
        {
            Error = "invalid_grant",
            ErrorDescription = "The authorization code is invalid"
        };

        SetupHttpResponse(
            HttpMethod.Post,
            _configuration.TokenEndpoint,
            HttpStatusCode.BadRequest,
            JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            }));

        // Act
        var result = await _authService.ExchangeCodeForTokenAsync(code, redirectUri, clientId, codeVerifier);

        // Assert
        result.Should().NotBeNull();
        result.Error.Should().Be("invalid_grant");
        result.ErrorDescription.Should().Be("The authorization code is invalid");
        result.AccessToken.Should().BeNull();
    }

    [Fact]
    public async Task RefreshTokenAsync_WithValidRefreshToken_ReturnsNewTokenResponse()
    {
        // Arrange
        var refreshToken = "test-refresh-token";

        var expectedTokenResponse = new TokenResponse
        {
            AccessToken = "new-access-token",
            TokenType = "Bearer",
            ExpiresIn = 3600,
            RefreshToken = "new-refresh-token",
            Scope = "patient/*.read"
        };

        SetupHttpResponse(
            HttpMethod.Post,
            _configuration.TokenEndpoint,
            HttpStatusCode.OK,
            JsonSerializer.Serialize(expectedTokenResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            }));

        // Act
        var result = await _authService.RefreshTokenAsync(refreshToken);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("new-access-token");
        result.TokenType.Should().Be("Bearer");
        result.RefreshToken.Should().Be("new-refresh-token");
        result.Error.Should().BeNull();
    }

    [Fact]
    public void GenerateCodeVerifier_ShouldReturnValidVerifier()
    {
        // Act
        var verifier = SmartAuthenticationService.GenerateCodeVerifier();

        // Assert
        verifier.Should().NotBeNullOrEmpty();
        verifier.Length.Should().Be(128);
        verifier.Should().MatchRegex("^[A-Za-z0-9\\-\\._~]+$"); // Valid PKCE characters
    }

    [Fact]
    public void GenerateCodeChallenge_ShouldReturnValidChallenge()
    {
        // Arrange
        var verifier = "test-verifier-123";

        // Act
        var challenge = SmartAuthenticationService.GenerateCodeChallenge(verifier);

        // Assert
        challenge.Should().NotBeNullOrEmpty();
        challenge.Should().MatchRegex("^[A-Za-z0-9\\-_]+$"); // Valid base64url characters
        challenge.Should().NotContain("="); // No padding
        challenge.Should().NotContain("+"); // No + characters
        challenge.Should().NotContain("/"); // No / characters
    }

    private string CreateTestJwtToken(Claim[] claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(_testSecurityKey, SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private void SetupJwksEndpoint()
    {
        var jwks = new
        {
            keys = new[]
            {
                new
                {
                    kty = "oct",
                    k = Convert.ToBase64String(Encoding.UTF8.GetBytes(_testPrivateKey)),
                    alg = "HS256",
                    use = "sig"
                }
            }
        };

        SetupHttpResponse(
            HttpMethod.Get,
            _configuration.JwksEndpoint,
            HttpStatusCode.OK,
            JsonSerializer.Serialize(jwks));
    }

    private void SetupHttpResponse(HttpMethod method, string requestUri, HttpStatusCode statusCode, string content)
    {
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == method &&
                    req.RequestUri!.ToString().Contains(requestUri)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            });
    }
}