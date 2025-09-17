using FhirIntegrationService.Controllers;
using FhirIntegrationService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;

namespace FhirIntegrationService.Authorization;

/// <summary>
/// Authorization handler for SMART on FHIR scope requirements
/// </summary>
public class SmartScopeAuthorizationHandler : AuthorizationHandler<SmartScopeRequirement>
{
    private readonly ISmartAuthenticationService _authService;
    private readonly ILogger<SmartScopeAuthorizationHandler> _logger;

    /// <summary>
    /// Initializes a new instance of SmartScopeAuthorizationHandler
    /// </summary>
    /// <param name="authService">SMART authentication service</param>
    /// <param name="logger">Logger instance</param>
    public SmartScopeAuthorizationHandler(
        ISmartAuthenticationService authService,
        ILogger<SmartScopeAuthorizationHandler> logger)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the authorization requirement
    /// </summary>
    /// <param name="context">Authorization context</param>
    /// <param name="requirement">Scope requirement</param>
    /// <returns>Task</returns>
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SmartScopeRequirement requirement)
    {
        try
        {
            // Get user scopes from claims
            var tokenScopes = context.User.FindAll("scope")
                .Select(c => c.Value)
                .ToArray();

            if (tokenScopes.Length == 0)
            {
                _logger.LogWarning("No scopes found in user token");
                context.Fail();
                return Task.CompletedTask;
            }

            // Extract resource type and operation from the requirement or endpoint
            var (resourceType, operation) = ExtractResourceTypeAndOperation(context, requirement);

            // Check if the user is authorized
            var requiredScopes = string.IsNullOrEmpty(requirement.RequiredScope)
                ? new[] { $"{resourceType.ToLowerInvariant()}/{operation}" }
                : new[] { requirement.RequiredScope };

            var isAuthorized = _authService.IsAuthorized(requiredScopes, tokenScopes, resourceType, operation);

            if (isAuthorized)
            {
                _logger.LogDebug("Authorization succeeded for resource {ResourceType} operation {Operation} with scopes: {Scopes}",
                    resourceType, operation, string.Join(", ", tokenScopes));
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogWarning("Authorization failed for resource {ResourceType} operation {Operation}. " +
                    "Required: {RequiredScopes}, Token: {TokenScopes}",
                    resourceType, operation, string.Join(", ", requiredScopes), string.Join(", ", tokenScopes));
                context.Fail();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during SMART scope authorization");
            context.Fail();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Extracts resource type and operation from the authorization context
    /// </summary>
    /// <param name="context">Authorization context</param>
    /// <param name="requirement">Scope requirement</param>
    /// <returns>Resource type and operation</returns>
    private static (string ResourceType, string Operation) ExtractResourceTypeAndOperation(
        AuthorizationHandlerContext context,
        SmartScopeRequirement requirement)
    {
        // Try to get from explicit requirement first
        if (!string.IsNullOrEmpty(requirement.RequiredScope))
        {
            var parts = requirement.RequiredScope.Split('/', '.');
            if (parts.Length >= 2)
            {
                return (parts[0], parts[1]);
            }
        }

        // Extract from controller/action context
        if (context.Resource is Microsoft.AspNetCore.Http.DefaultHttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();
            var actionDescriptor = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();

            if (actionDescriptor != null)
            {
                // Extract resource type from controller name
                var controllerName = actionDescriptor.ControllerName;
                var resourceType = controllerName.Replace("Controller", "");

                // Extract operation from HTTP method and action name
                var httpMethod = httpContext.Request.Method;
                var operation = httpMethod.ToLowerInvariant() switch
                {
                    "get" => "read",
                    "post" => "write",
                    "put" => "write",
                    "patch" => "write",
                    "delete" => "write",
                    _ => "read"
                };

                return (resourceType, operation);
            }
        }

        // Extract from RequiredScope attribute if present
        if (context.Resource is Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext filterContext)
        {
            var requiredScopeAttribute = filterContext.ActionDescriptor.EndpointMetadata
                .OfType<RequiredScopeAttribute>()
                .FirstOrDefault();

            if (requiredScopeAttribute != null)
            {
                var parts = requiredScopeAttribute.Scope.Split('/', '.');
                if (parts.Length >= 2)
                {
                    return (parts[0], parts[1]);
                }
            }
        }

        // Default fallback
        return ("Patient", "read");
    }
}

/// <summary>
/// Authorization requirement for SMART on FHIR scopes
/// </summary>
public class SmartScopeRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Required scope for the operation
    /// </summary>
    public string RequiredScope { get; }

    /// <summary>
    /// Initializes a new instance of SmartScopeRequirement
    /// </summary>
    /// <param name="requiredScope">Required scope</param>
    public SmartScopeRequirement(string requiredScope)
    {
        RequiredScope = requiredScope ?? throw new ArgumentNullException(nameof(requiredScope));
    }
}

/// <summary>
/// Authorization policy names for SMART on FHIR
/// </summary>
public static class SmartAuthorizationPolicies
{
    public const string PatientRead = "patient.read";
    public const string PatientWrite = "patient.write";
    public const string PatientAll = "patient.*";
    public const string ObservationRead = "observation.read";
    public const string ObservationWrite = "observation.write";
    public const string UserRead = "user.read";
    public const string LaunchPatient = "launch/patient";
}