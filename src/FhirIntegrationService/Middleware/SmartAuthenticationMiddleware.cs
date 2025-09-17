using FhirIntegrationService.Services.Interfaces;
using System.Security.Claims;

namespace FhirIntegrationService.Middleware;

/// <summary>
/// Middleware for SMART on FHIR authentication
/// </summary>
public class SmartAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SmartAuthenticationMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of SmartAuthenticationMiddleware
    /// </summary>
    /// <param name="next">Next middleware in pipeline</param>
    /// <param name="logger">Logger instance</param>
    public SmartAuthenticationMiddleware(RequestDelegate next, ILogger<SmartAuthenticationMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Invokes the middleware
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="authService">SMART authentication service</param>
    /// <returns>Task</returns>
    public async Task InvokeAsync(HttpContext context, ISmartAuthenticationService authService)
    {
        // Skip authentication for certain paths
        if (ShouldSkipAuthentication(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var token = ExtractBearerToken(context.Request);

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("No bearer token found in request to {Path}", context.Request.Path);
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Bearer token required");
            return;
        }

        try
        {
            var validationResult = await authService.ValidateTokenAsync(token, context.RequestAborted);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Invalid token for request to {Path}: {Error}",
                    context.Request.Path, validationResult.ErrorMessage);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid token");
                return;
            }

            // Create claims principal
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, validationResult.Subject ?? "unknown"),
                new("client_id", validationResult.ClientId ?? "unknown"),
                new("iss", validationResult.Issuer ?? "unknown")
            };

            // Add scope claims
            foreach (var scope in validationResult.Scopes)
            {
                claims.Add(new Claim("scope", scope));
            }

            // Add other claims from token
            foreach (var claim in validationResult.Claims)
            {
                if (claim.Value is string stringValue)
                {
                    claims.Add(new Claim(claim.Key, stringValue));
                }
            }

            var identity = new ClaimsIdentity(claims, "Bearer");
            context.User = new ClaimsPrincipal(identity);

            _logger.LogDebug("Successfully authenticated user {Subject} with scopes: {Scopes}",
                validationResult.Subject, string.Join(", ", validationResult.Scopes));

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during authentication for request to {Path}", context.Request.Path);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Authentication service error");
        }
    }

    /// <summary>
    /// Extracts bearer token from Authorization header
    /// </summary>
    /// <param name="request">HTTP request</param>
    /// <returns>Bearer token or null</returns>
    private static string? ExtractBearerToken(HttpRequest request)
    {
        var authorization = request.Headers.Authorization.FirstOrDefault();

        if (string.IsNullOrEmpty(authorization))
            return null;

        const string bearerPrefix = "Bearer ";
        if (!authorization.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
            return null;

        return authorization[bearerPrefix.Length..].Trim();
    }

    /// <summary>
    /// Determines if authentication should be skipped for the given path
    /// </summary>
    /// <param name="path">Request path</param>
    /// <returns>True if authentication should be skipped</returns>
    private static bool ShouldSkipAuthentication(PathString path)
    {
        var skipPaths = new[]
        {
            "/health",
            "/swagger",
            "/api-docs",
            "/.well-known",
            "/metadata"
        };

        return skipPaths.Any(skipPath => path.StartsWithSegments(skipPath, StringComparison.OrdinalIgnoreCase));
    }
}