# Epic 4.3: Create Production API Endpoints with Live FHIR Validation

**Story ID:** Epic 4.3
**Epic:** Development & Implementation Workflow
**Priority:** High - Critical Path
**Estimated Effort:** 8-10 hours
**Status:** Ready for Implementation
**Depends On:** Epic 4.1 (Service Scaffolding) + Epic 4.2 (Data Mapping)

---

## User Story

**As a healthcare integration developer,**
I want production-ready API endpoints that retrieve OpenEMR data, transform it using our mapping service, and return validated FHIR ChemistryPanelObservation resources,
So that the integration service is complete and guaranteed to be FHIR-compliant.

---

## Story Context

### Existing System Integration

- **Integrates with:** Epic 4.1 service foundation + Epic 4.2 mapping service
- **Technology:** ASP.NET Core Web API, Firely .NET SDK, OpenEMR API client
- **Follows pattern:** RESTful FHIR API endpoints with comprehensive validation
- **Touch points:** OpenEMR OAuth2 authentication, ChemistryPanelObservation validation, error handling

### Input Dependencies

**Required Artifacts:**
- Epic 4.1: Service scaffolding with health check endpoint ✅
- Epic 4.2: Data mapping service with quirks handling ✅
- `docs/demo/completed-integration-partner-profile-openemr.md` - OAuth2 authentication patterns
- `docs/demo/ChemistryPanelObservation.json` - FHIR profile for validation
- OpenEMR sample data from Epic 3 research

**Prerequisites:**
- Epic 4.1 and 4.2 completed and integrated
- Understanding of FHIR Bundle and OperationOutcome resources
- OpenEMR API authentication credentials for testing

---

## Acceptance Criteria

### Core API Requirements

1. **Patient Observation Endpoint:** `GET /api/patients/{id}/observations/chemistry` returns ChemistryPanelObservation resources
2. **Observation by ID Endpoint:** `GET /api/observations/{id}` returns individual ChemistryPanelObservation
3. **Batch Observation Endpoint:** `GET /api/observations?patient={id}&date={range}` returns multiple observations
4. **Authentication Integration:** OAuth2 token-based authentication for OpenEMR API calls

### FHIR Compliance Requirements

5. **Live Validation:** All responses validated against ChemistryPanelObservation profile before return
6. **FHIR Metadata:** Proper FHIR Bundle responses with pagination and metadata
7. **Error Handling:** FHIR OperationOutcome responses for validation failures and errors
8. **Content Negotiation:** Support application/fhir+json and application/json content types

### Production Readiness Requirements

9. **Performance Monitoring:** Response time tracking and performance metrics
10. **Comprehensive Logging:** Structured logging for troubleshooting and audit
11. **Swagger Documentation:** Complete OpenAPI documentation for all endpoints
12. **Integration Tests:** End-to-end tests with real OpenEMR mock data

---

## Technical Implementation Architecture

### Core API Controller

```csharp
[ApiController]
[Route("api/[controller]")]
[Produces("application/fhir+json", "application/json")]
public class ObservationsController : ControllerBase
{
    private readonly IFhirMappingService _mappingService;
    private readonly IOpenEmrApiClient _openEmrClient;
    private readonly IFhirValidator _validator;
    private readonly IFhirResponseBuilder _responseBuilder;
    private readonly ILogger<ObservationsController> _logger;

    public ObservationsController(
        IFhirMappingService mappingService,
        IOpenEmrApiClient openEmrClient,
        IFhirValidator validator,
        IFhirResponseBuilder responseBuilder,
        ILogger<ObservationsController> logger)
    {
        _mappingService = mappingService;
        _openEmrClient = openEmrClient;
        _validator = validator;
        _responseBuilder = responseBuilder;
        _logger = logger;
    }

    // Endpoint implementations...
}
```

### Supporting Services

**OpenEMR API Client Service:**
```csharp
public interface IOpenEmrApiClient
{
    Task<OpenEmrObservation> GetObservationAsync(string observationId);
    Task<List<OpenEmrObservation>> GetPatientObservationsAsync(string patientId, DateRange dateRange = null);
    Task<List<OpenEmrObservation>> GetChemistryObservationsAsync(string patientId, DateRange dateRange = null);
    Task<AuthenticationResult> AuthenticateAsync();
    Task<bool> ValidateConnectionAsync();
}

public class OpenEmrApiClient : IOpenEmrApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IOpenEmrAuthenticationService _authService;
    private readonly ILogger<OpenEmrApiClient> _logger;
    private readonly OpenEmrConfiguration _config;

    // Implementation with retry logic, circuit breaker pattern
}
```

**FHIR Response Builder Service:**
```csharp
public interface IFhirResponseBuilder
{
    Bundle CreateObservationBundle(List<ChemistryPanelObservation> observations, string requestUrl);
    Bundle CreateSearchBundle(List<ChemistryPanelObservation> observations, SearchParameters searchParams);
    OperationOutcome CreateValidationErrorResponse(List<ValidationIssue> issues);
    OperationOutcome CreateApiErrorResponse(string message, string diagnostics);
}

public class FhirResponseBuilder : IFhirResponseBuilder
{
    public Bundle CreateObservationBundle(List<ChemistryPanelObservation> observations, string requestUrl)
    {
        return new Bundle
        {
            Id = Guid.NewGuid().ToString(),
            Type = Bundle.BundleType.Searchset,
            Timestamp = DateTimeOffset.UtcNow,
            Total = observations.Count,
            Entry = observations.Select(obs => new Bundle.EntryComponent
            {
                Resource = obs,
                FullUrl = $"{requestUrl}/{obs.Id}"
            }).ToList()
        };
    }
}
```

**Authentication Service:**
```csharp
public interface IOpenEmrAuthenticationService
{
    Task<string> GetAccessTokenAsync();
    Task<bool> RefreshTokenAsync();
    Task<bool> ValidateTokenAsync(string token);
}

public class OpenEmrAuthenticationService : IOpenEmrAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _tokenCache;
    private readonly OpenEmrConfiguration _config;
    private readonly ILogger<OpenEmrAuthenticationService> _logger;

    public async Task<string> GetAccessTokenAsync()
    {
        // Check cache first
        if (_tokenCache.TryGetValue("openemr_token", out string cachedToken))
        {
            if (await ValidateTokenAsync(cachedToken))
                return cachedToken;
        }

        // OAuth2 client credentials flow
        var tokenRequest = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = _config.ClientId,
            ["client_secret"] = _config.ClientSecret,
            ["scope"] = "patient/read"
        };

        // Token acquisition and caching logic
    }
}
```

---

## API Endpoint Specifications

### 1. Patient Chemistry Observations Endpoint

```csharp
/// <summary>
/// Retrieves chemistry panel observations for a specific patient
/// </summary>
/// <param name="patientId">OpenEMR patient identifier</param>
/// <param name="from">Start date for observation range (optional)</param>
/// <param name="to">End date for observation range (optional)</param>
/// <param name="count">Maximum number of observations to return (default: 20, max: 100)</param>
/// <param name="offset">Number of observations to skip for pagination (default: 0)</param>
/// <returns>FHIR Bundle containing ChemistryPanelObservation resources</returns>
[HttpGet("/api/patients/{patientId}/observations/chemistry")]
[ProducesResponseType(typeof(Bundle), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status502BadGateway)]
public async Task<ActionResult<Bundle>> GetPatientChemistryObservations(
    string patientId,
    [FromQuery] DateTime? from = null,
    [FromQuery] DateTime? to = null,
    [FromQuery] int count = 20,
    [FromQuery] int offset = 0)
{
    using var activity = ActivitySource.StartActivity("GetPatientChemistryObservations");
    activity?.SetTag("patient.id", patientId);

    try
    {
        // Validate parameters
        if (count > 100) count = 100;
        if (count < 1) count = 20;

        _logger.LogInformation("Retrieving chemistry observations for patient {PatientId}", patientId);

        // Get observations from OpenEMR
        var dateRange = (from != null || to != null) ? new DateRange(from, to) : null;
        var openEmrObservations = await _openEmrClient.GetChemistryObservationsAsync(patientId, dateRange);

        if (!openEmrObservations.Any())
        {
            _logger.LogInformation("No chemistry observations found for patient {PatientId}", patientId);
            return Ok(_responseBuilder.CreateObservationBundle(new List<ChemistryPanelObservation>(), Request.GetDisplayUrl()));
        }

        // Apply pagination
        var paginatedObservations = openEmrObservations.Skip(offset).Take(count).ToList();

        // Transform to FHIR
        var fhirObservations = new List<ChemistryPanelObservation>();
        foreach (var openEmrObs in paginatedObservations)
        {
            var mapped = await _mappingService.MapObservationAsync(openEmrObs);

            // Validate against profile
            var validationResult = await _validator.ValidateChemistryPanelAsync(mapped);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for observation {ObservationId}: {Issues}",
                    openEmrObs.Id, string.Join(", ", validationResult.Issues.Select(i => i.Message)));

                return BadRequest(_responseBuilder.CreateValidationErrorResponse(validationResult.Issues));
            }

            fhirObservations.Add(mapped);
        }

        // Create FHIR Bundle response
        var bundle = _responseBuilder.CreateObservationBundle(fhirObservations, Request.GetDisplayUrl());

        _logger.LogInformation("Successfully retrieved {Count} chemistry observations for patient {PatientId}",
            fhirObservations.Count, patientId);

        return Ok(bundle);
    }
    catch (OpenEmrApiException ex)
    {
        _logger.LogError(ex, "OpenEMR API error for patient {PatientId}", patientId);
        return StatusCode(502, _responseBuilder.CreateApiErrorResponse("External API Error", ex.Message));
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error retrieving observations for patient {PatientId}", patientId);
        return StatusCode(500, _responseBuilder.CreateApiErrorResponse("Internal Server Error", "An unexpected error occurred"));
    }
}
```

**Sample Response:**
```json
{
  "resourceType": "Bundle",
  "id": "chemistry-observations-bundle-123",
  "type": "searchset",
  "timestamp": "2024-01-17T15:30:00Z",
  "total": 15,
  "entry": [
    {
      "fullUrl": "https://api.hospital.org/api/observations/glucose-123",
      "resource": {
        "resourceType": "Observation",
        "id": "glucose-123",
        "meta": {
          "profile": ["http://hospital.org/fhir/StructureDefinition/ChemistryPanelObservation"],
          "lastUpdated": "2024-01-17T15:30:00Z"
        },
        "status": "final",
        "code": {
          "coding": [{
            "system": "http://loinc.org",
            "code": "2345-7",
            "display": "Glucose [Mass/volume] in Serum or Plasma"
          }]
        },
        "subject": {
          "reference": "Patient/openemr-patient-456"
        },
        "effectiveDateTime": "2024-01-17T10:30:00Z",
        "performer": [{
          "reference": "Organization/hospital-lab"
        }],
        "valueQuantity": {
          "value": 95,
          "unit": "mg/dL",
          "system": "http://unitsofmeasure.org",
          "code": "mg/dL"
        }
      }
    }
  ]
}
```

### 2. Individual Observation Endpoint

```csharp
/// <summary>
/// Retrieves a specific chemistry panel observation by ID
/// </summary>
/// <param name="observationId">OpenEMR observation identifier</param>
/// <returns>ChemistryPanelObservation FHIR resource</returns>
[HttpGet("/api/observations/{observationId}")]
[ProducesResponseType(typeof(ChemistryPanelObservation), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status502BadGateway)]
public async Task<ActionResult<ChemistryPanelObservation>> GetObservation(string observationId)
{
    using var activity = ActivitySource.StartActivity("GetObservation");
    activity?.SetTag("observation.id", observationId);

    try
    {
        _logger.LogInformation("Retrieving observation {ObservationId}", observationId);

        // Get observation from OpenEMR
        var openEmrObservation = await _openEmrClient.GetObservationAsync(observationId);
        if (openEmrObservation == null)
        {
            return NotFound(_responseBuilder.CreateApiErrorResponse("Not Found", $"Observation {observationId} not found"));
        }

        // Transform to FHIR
        var fhirObservation = await _mappingService.MapObservationAsync(openEmrObservation);

        // Validate against profile
        var validationResult = await _validator.ValidateChemistryPanelAsync(fhirObservation);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for observation {ObservationId}: {Issues}",
                observationId, string.Join(", ", validationResult.Issues.Select(i => i.Message)));

            return BadRequest(_responseBuilder.CreateValidationErrorResponse(validationResult.Issues));
        }

        _logger.LogInformation("Successfully retrieved and validated observation {ObservationId}", observationId);
        return Ok(fhirObservation);
    }
    catch (OpenEmrApiException ex)
    {
        _logger.LogError(ex, "OpenEMR API error for observation {ObservationId}", observationId);
        return StatusCode(502, _responseBuilder.CreateApiErrorResponse("External API Error", ex.Message));
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error retrieving observation {ObservationId}", observationId);
        return StatusCode(500, _responseBuilder.CreateApiErrorResponse("Internal Server Error", "An unexpected error occurred"));
    }
}
```

### 3. Batch Observations Search Endpoint

```csharp
/// <summary>
/// Search for chemistry panel observations with FHIR search parameters
/// </summary>
/// <param name="patient">Patient identifier</param>
/// <param name="date">Observation date (supports ranges like ge2024-01-01)</param>
/// <param name="code">LOINC code for specific test</param>
/// <param name="_count">Number of results to return</param>
/// <param name="_offset">Pagination offset</param>
/// <returns>FHIR Bundle with search results</returns>
[HttpGet("/api/observations")]
[ProducesResponseType(typeof(Bundle), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status400BadRequest)]
public async Task<ActionResult<Bundle>> SearchObservations(
    [FromQuery] string patient,
    [FromQuery] string date,
    [FromQuery] string code,
    [FromQuery] int _count = 20,
    [FromQuery] int _offset = 0)
{
    using var activity = ActivitySource.StartActivity("SearchObservations");

    try
    {
        var searchParams = new SearchParameters
        {
            Patient = patient,
            Date = date,
            Code = code,
            Count = Math.Min(_count, 100),
            Offset = Math.Max(_offset, 0)
        };

        _logger.LogInformation("Searching observations with parameters: {SearchParams}",
            JsonSerializer.Serialize(searchParams));

        // Parse search parameters and query OpenEMR
        var dateRange = ParseDateParameter(date);
        var openEmrObservations = await _openEmrClient.GetPatientObservationsAsync(patient, dateRange);

        // Filter by LOINC code if specified
        if (!string.IsNullOrEmpty(code))
        {
            openEmrObservations = openEmrObservations
                .Where(obs => obs.Code?.Coding?.Any(c => c.Code == code) == true)
                .ToList();
        }

        // Apply pagination
        var paginatedObservations = openEmrObservations
            .Skip(searchParams.Offset)
            .Take(searchParams.Count)
            .ToList();

        // Transform and validate
        var fhirObservations = new List<ChemistryPanelObservation>();
        foreach (var openEmrObs in paginatedObservations)
        {
            var mapped = await _mappingService.MapObservationAsync(openEmrObs);
            var validationResult = await _validator.ValidateChemistryPanelAsync(mapped);

            if (validationResult.IsValid)
            {
                fhirObservations.Add(mapped);
            }
            else
            {
                _logger.LogWarning("Skipping invalid observation {ObservationId}: {Issues}",
                    openEmrObs.Id, string.Join(", ", validationResult.Issues.Select(i => i.Message)));
            }
        }

        var bundle = _responseBuilder.CreateSearchBundle(fhirObservations, searchParams);

        _logger.LogInformation("Search completed: {Count} valid observations returned", fhirObservations.Count);
        return Ok(bundle);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error during observation search");
        return StatusCode(500, _responseBuilder.CreateApiErrorResponse("Search Error", "An error occurred during search"));
    }
}
```

---

## Middleware Implementation

### Performance Monitoring Middleware

```csharp
public class PerformanceMonitoringMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMonitoringMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();

        context.Items["RequestId"] = requestId;
        context.Items["StartTime"] = DateTime.UtcNow;

        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["RequestId"] = requestId,
            ["RequestPath"] = context.Request.Path,
            ["RequestMethod"] = context.Request.Method
        });

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            _logger.LogInformation("Request completed in {ElapsedMs}ms with status {StatusCode}",
                stopwatch.ElapsedMilliseconds,
                context.Response.StatusCode);

            // Performance metrics for monitoring
            if (stopwatch.ElapsedMilliseconds > 1000)
            {
                _logger.LogWarning("Slow request detected: {ElapsedMs}ms for {RequestPath}",
                    stopwatch.ElapsedMilliseconds,
                    context.Request.Path);
            }
        }
    }
}
```

### FHIR Exception Middleware

```csharp
public class FhirExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<FhirExceptionMiddleware> _logger;
    private readonly IFhirResponseBuilder _responseBuilder;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (FhirValidationException ex)
        {
            _logger.LogWarning(ex, "FHIR validation error");
            await WriteOperationOutcomeResponse(context,
                _responseBuilder.CreateValidationErrorResponse(ex.ValidationIssues), 400);
        }
        catch (OpenEmrApiException ex)
        {
            _logger.LogError(ex, "OpenEMR API error");
            await WriteOperationOutcomeResponse(context,
                _responseBuilder.CreateApiErrorResponse("External API Error", ex.Message), 502);
        }
        catch (AuthenticationException ex)
        {
            _logger.LogError(ex, "Authentication error");
            await WriteOperationOutcomeResponse(context,
                _responseBuilder.CreateApiErrorResponse("Authentication Failed", "Unable to authenticate with OpenEMR"), 401);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteOperationOutcomeResponse(context,
                _responseBuilder.CreateApiErrorResponse("Internal Server Error", "An unexpected error occurred"), 500);
        }
    }

    private async Task WriteOperationOutcomeResponse(HttpContext context, OperationOutcome outcome, int statusCode)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/fhir+json";

        var json = JsonSerializer.Serialize(outcome, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }
}
```

---

## Project File Structure

```
src/FhirIntegrationService/
├── Controllers/
│   ├── HealthController.cs (from Epic 4.1)
│   └── ObservationsController.cs (NEW)
├── Services/
│   ├── Mapping/ (from Epic 4.2)
│   │   ├── IFhirMappingService.cs
│   │   └── OpenEmrToFhirMappingService.cs
│   ├── ApiClient/ (NEW)
│   │   ├── IOpenEmrApiClient.cs
│   │   ├── OpenEmrApiClient.cs
│   │   ├── IOpenEmrAuthenticationService.cs
│   │   └── OpenEmrAuthenticationService.cs
│   ├── Response/ (NEW)
│   │   ├── IFhirResponseBuilder.cs
│   │   └── FhirResponseBuilder.cs
│   └── Validation/ (from Epic 4.2)
│       ├── IFhirValidator.cs
│       └── FhirProfileValidator.cs
├── Middleware/ (NEW)
│   ├── PerformanceMonitoringMiddleware.cs
│   └── FhirExceptionMiddleware.cs
├── Models/
│   ├── Configuration/ (NEW)
│   │   ├── OpenEmrConfiguration.cs
│   │   └── SearchParameters.cs
│   ├── Responses/ (NEW)
│   │   ├── AuthenticationResult.cs
│   │   └── ValidationResult.cs
│   └── (existing from Epic 4.2)
├── Configuration/ (NEW)
│   ├── ServiceRegistration.cs
│   └── SwaggerConfiguration.cs
└── Program.cs (UPDATED)

tests/FhirIntegrationService.Tests/
├── Controllers/
│   └── ObservationsControllerTests.cs (NEW)
├── Services/
│   ├── ApiClient/
│   │   ├── OpenEmrApiClientTests.cs (NEW)
│   │   └── OpenEmrAuthenticationServiceTests.cs (NEW)
│   └── Response/
│       └── FhirResponseBuilderTests.cs (NEW)
├── Integration/ (NEW)
│   ├── EndToEndApiTests.cs
│   ├── PerformanceTests.cs
│   └── ErrorHandlingTests.cs
├── MockData/ (NEW)
│   ├── OpenEmrApiResponses/
│   │   ├── patient-observations.json
│   │   ├── single-observation.json
│   │   └── auth-response.json
│   └── ExpectedFhirBundles/
│       ├── chemistry-observations-bundle.json
│       └── search-results-bundle.json
└── TestUtilities/ (NEW)
    ├── MockOpenEmrApiClient.cs
    └── TestDataBuilder.cs
```

---

## Implementation Phases

### Phase 1: Core API Structure (Hours 1-2)
- [ ] Create ObservationsController with basic endpoint structure
- [ ] Implement IOpenEmrApiClient interface and basic implementation
- [ ] Set up dependency injection for all services
- [ ] Create basic unit tests for controller actions

### Phase 2: OpenEMR Integration (Hours 3-4)
- [ ] Implement OAuth2 authentication service with token caching
- [ ] Complete OpenEmrApiClient with retry logic and error handling
- [ ] Integrate with Epic 4.2 mapping service
- [ ] Test authentication and API calls with OpenEMR demo environment

### Phase 3: FHIR Response & Validation (Hours 5-6)
- [ ] Implement FhirResponseBuilder for Bundle creation
- [ ] Integrate live FHIR validation using Firely SDK
- [ ] Implement FHIR OperationOutcome error responses
- [ ] Create comprehensive validation tests

### Phase 4: Production Features (Hours 7-8)
- [ ] Implement performance monitoring middleware
- [ ] Add comprehensive structured logging
- [ ] Create Swagger/OpenAPI documentation
- [ ] Implement error handling middleware

### Phase 5: Testing & Performance (Hours 9-10)
- [ ] Create end-to-end integration tests
- [ ] Implement performance tests with <500ms targets
- [ ] Test error scenarios and FHIR error responses
- [ ] Optimize performance and finalize documentation

---

## Configuration

### appsettings.json Updates

```json
{
  "OpenEmr": {
    "BaseUrl": "https://demo.openemr.io/apis/default/fhir",
    "TokenUrl": "https://demo.openemr.io/oauth2/default/token",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "Scope": "patient/read",
    "TimeoutSeconds": 30,
    "RetryAttempts": 3
  },
  "Fhir": {
    "ChemistryPanelProfileUrl": "http://hospital.org/fhir/StructureDefinition/ChemistryPanelObservation",
    "ValidationEnabled": true,
    "BundlePageSize": 20,
    "MaxBundleSize": 100
  },
  "Performance": {
    "SlowRequestThresholdMs": 1000,
    "EnableMetrics": true,
    "LogPerformanceWarnings": true
  }
}
```

### Dependency Injection Setup

```csharp
// Program.cs updates
builder.Services.Configure<OpenEmrConfiguration>(
    builder.Configuration.GetSection("OpenEmr"));

builder.Services.Configure<FhirConfiguration>(
    builder.Configuration.GetSection("Fhir"));

// Register services
builder.Services.AddScoped<IOpenEmrApiClient, OpenEmrApiClient>();
builder.Services.AddScoped<IOpenEmrAuthenticationService, OpenEmrAuthenticationService>();
builder.Services.AddScoped<IFhirResponseBuilder, FhirResponseBuilder>();

// Add existing services from Epic 4.1 and 4.2
builder.Services.AddScoped<IFhirMappingService, OpenEmrToFhirMappingService>();
builder.Services.AddScoped<IFhirValidator, FhirProfileValidator>();

// Add middleware
app.UseMiddleware<PerformanceMonitoringMiddleware>();
app.UseMiddleware<FhirExceptionMiddleware>();
```

---

## Sample Integration Tests

```csharp
[TestClass]
public class ObservationsControllerIntegrationTests
{
    private readonly TestServer _server;
    private readonly HttpClient _client;

    [TestInitialize]
    public void Setup()
    {
        var hostBuilder = new WebHostBuilder()
            .UseStartup<TestStartup>()
            .ConfigureServices(services =>
            {
                // Replace with mock OpenEMR client
                services.AddScoped<IOpenEmrApiClient, MockOpenEmrApiClient>();
            });

        _server = new TestServer(hostBuilder);
        _client = _server.CreateClient();
    }

    [TestMethod]
    public async Task GetPatientChemistryObservations_ValidPatient_ReturnsValidFhirBundle()
    {
        // Arrange
        var patientId = "test-patient-123";

        // Act
        var response = await _client.GetAsync($"/api/patients/{patientId}/observations/chemistry");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType.MediaType.Should().Be("application/fhir+json");

        var content = await response.Content.ReadAsStringAsync();
        var bundle = JsonSerializer.Deserialize<Bundle>(content);

        bundle.Should().NotBeNull();
        bundle.Type.Should().Be(Bundle.BundleType.Searchset);
        bundle.Entry.Should().NotBeEmpty();

        // Validate each observation in bundle
        foreach (var entry in bundle.Entry)
        {
            var observation = entry.Resource as Observation;
            observation.Should().NotBeNull();
            observation.Meta.Profile.Should().Contain("ChemistryPanelObservation");
        }
    }

    [TestMethod]
    public async Task GetObservation_CriticalGlucoseValue_IncludesRequiredNote()
    {
        // Arrange
        var observationId = "critical-glucose-observation";

        // Act
        var response = await _client.GetAsync($"/api/observations/{observationId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var observation = JsonSerializer.Deserialize<Observation>(content);

        observation.Should().NotBeNull();
        observation.ValueQuantity.Value.Should().BeGreaterThan(400); // Critical glucose level
        observation.Note.Should().NotBeEmpty();
        observation.Note.First().Text.Should().Contain("CRITICAL VALUE");
    }

    [TestMethod]
    public async Task GetObservations_InvalidPatientId_ReturnsOperationOutcome()
    {
        // Arrange
        var invalidPatientId = "non-existent-patient";

        // Act
        var response = await _client.GetAsync($"/api/patients/{invalidPatientId}/observations/chemistry");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType.MediaType.Should().Be("application/fhir+json");

        var content = await response.Content.ReadAsStringAsync();
        var operationOutcome = JsonSerializer.Deserialize<OperationOutcome>(content);

        operationOutcome.Should().NotBeNull();
        operationOutcome.Issue.Should().NotBeEmpty();
        operationOutcome.Issue.First().Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
    }

    [TestMethod]
    public async Task GetPatientObservations_PerformanceTest_CompletesWithin500ms()
    {
        // Arrange
        var patientId = "performance-test-patient";
        var stopwatch = Stopwatch.StartNew();

        // Act
        var response = await _client.GetAsync($"/api/patients/{patientId}/observations/chemistry");

        // Assert
        stopwatch.Stop();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(500);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

---

## Definition of Done

### API Implementation
- [ ] Patient chemistry observations endpoint implemented with pagination and date filtering
- [ ] Individual observation endpoint with comprehensive error handling
- [ ] Batch observations search endpoint with FHIR search parameter support
- [ ] OpenEMR API client with OAuth2 authentication and retry logic
- [ ] FHIR response builder creating compliant Bundle and OperationOutcome resources

### Validation & Quality
- [ ] Live FHIR validation using Firely SDK against ChemistryPanelObservation profile
- [ ] FHIR OperationOutcome error responses for all failure scenarios
- [ ] Performance monitoring middleware with response time tracking
- [ ] Comprehensive structured logging for audit and troubleshooting
- [ ] Swagger/OpenAPI documentation for all endpoints with examples

### Testing & Production Readiness
- [ ] Unit tests for all controllers and services (≥90% coverage)
- [ ] Integration tests with mocked OpenEMR API responses
- [ ] Performance tests meeting <500ms response time for single observations
- [ ] End-to-end tests validating complete OpenEMR → FHIR transformation flow
- [ ] Error scenario tests ensuring proper FHIR error handling
- [ ] Load testing with concurrent requests and batch operations

### Documentation & Deployment
- [ ] API documentation updated with endpoint specifications and examples
- [ ] Configuration guide for OpenEMR integration setup
- [ ] Troubleshooting guide for common integration issues
- [ ] Performance tuning recommendations
- [ ] Security considerations and best practices documented

---

## Risk Assessment & Mitigation

### Primary Risks

**1. OpenEMR API Reliability**
- **Risk:** External API timeouts, rate limiting, or authentication failures
- **Mitigation:** Circuit breaker pattern, exponential backoff retry, comprehensive error handling
- **Monitoring:** API response time tracking, error rate alerts, authentication failure detection

**2. FHIR Validation Performance**
- **Risk:** Complex profile validation may impact response times
- **Mitigation:** Validation result caching, asynchronous validation for batch operations
- **Monitoring:** Validation time tracking, performance degradation alerts

**3. Concurrent Request Handling**
- **Risk:** Multiple simultaneous OpenEMR API calls may exceed rate limits
- **Mitigation:** Request queuing, connection pooling, rate limit monitoring
- **Monitoring:** Concurrent request tracking, rate limit violation alerts

**4. Data Quality Issues**
- **Risk:** OpenEMR data may not always conform to expected patterns
- **Mitigation:** Robust data validation, graceful error handling, data quality reporting
- **Monitoring:** Validation failure tracking, data quality metrics

### Rollback Strategy
- **API Failures:** Return appropriate FHIR OperationOutcome with error details
- **Validation Failures:** Log validation issues, return partial results with warnings
- **Authentication Failures:** Retry authentication, fallback to cached tokens
- **Complete Service Failure:** Health check endpoint indicates service degradation

---

## Success Criteria

**Epic 4.3 is complete when:**

1. ✅ **Complete API Functionality:** All three endpoints working with proper FHIR responses and error handling
2. ✅ **Live FHIR Validation:** 100% ChemistryPanelObservation profile compliance for all responses
3. ✅ **Error Handling:** FHIR OperationOutcome responses for all error scenarios with appropriate HTTP status codes
4. ✅ **Performance Targets:** <500ms response time for individual observations, <2s for batch requests
5. ✅ **Production Readiness:** Comprehensive logging, monitoring, authentication, and documentation
6. ✅ **Integration Complete:** Seamless integration of Epic 4.1 (scaffolding) and Epic 4.2 (mapping) components
7. ✅ **Quality Assurance:** ≥90% test coverage with comprehensive integration and performance tests

---

## Next Steps After Completion

**After Epic 4.3 completion:**
- **Epic 4 Development Workflow COMPLETE** - Full working FHIR integration service
- Hand off to Epic 5.1: Implement Automated FHIR Resource Validation
- Use API endpoints as foundation for security assessment and compliance validation
- Leverage established patterns for additional FHIR resource types and endpoints
- Begin production deployment planning with comprehensive monitoring and alerting

---

**Generated by BMad PM Agent | Product Manager: John 📋**
**Date:** 2024-09-17
**Change Log:** Initial story creation for Epic 4.3 - Production API Endpoints Implementation
**Dependencies:** Epic 4.1 (Service Scaffolding) + Epic 4.2 (Data Mapping Logic)
**Completes:** Epic 4 Development & Implementation Workflow