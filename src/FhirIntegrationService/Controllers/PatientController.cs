using FhirIntegrationService.Exceptions;
using FhirIntegrationService.Services.Interfaces;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FhirIntegrationService.Controllers;

/// <summary>
/// Controller for FHIR Patient resource operations
/// </summary>
[ApiController]
[Route("fhir/[controller]")]
[Authorize]
[Produces("application/fhir+json", "application/json")]
public class PatientController : ControllerBase
{
    private readonly IDataMappingService _dataMappingService;
    private readonly IVendorApiClient _vendorApiClient;
    private readonly IFhirValidationService _fhirValidationService;
    private readonly ILogger<PatientController> _logger;
    private readonly FhirJsonSerializer _fhirSerializer;

    /// <summary>
    /// Initializes a new instance of the PatientController
    /// </summary>
    /// <param name="dataMappingService">Data mapping service</param>
    /// <param name="vendorApiClient">Vendor API client</param>
    /// <param name="fhirValidationService">FHIR validation service</param>
    /// <param name="logger">Logger instance</param>
    /// <param name="fhirSerializer">FHIR JSON serializer</param>
    public PatientController(
        IDataMappingService dataMappingService,
        IVendorApiClient vendorApiClient,
        IFhirValidationService fhirValidationService,
        ILogger<PatientController> logger,
        FhirJsonSerializer fhirSerializer)
    {
        _dataMappingService = dataMappingService ?? throw new ArgumentNullException(nameof(dataMappingService));
        _vendorApiClient = vendorApiClient ?? throw new ArgumentNullException(nameof(vendorApiClient));
        _fhirValidationService = fhirValidationService ?? throw new ArgumentNullException(nameof(fhirValidationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fhirSerializer = fhirSerializer ?? throw new ArgumentNullException(nameof(fhirSerializer));
    }

    /// <summary>
    /// Retrieves a FHIR Patient resource by ID
    /// </summary>
    /// <param name="id">Patient identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>FHIR Patient resource or error response</returns>
    [HttpGet("{id}")]
    [RequiredScope("patient/*.read")]
    public async Task<IActionResult> GetPatient(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("Patient retrieval attempted with null or empty ID");
            return BadRequest(CreateOperationOutcome("invalid", "Patient ID is required"));
        }

        var stopwatch = Stopwatch.StartNew();
        var correlationId = Guid.NewGuid().ToString();

        try
        {
            _logger.LogInformation("Starting patient retrieval process for correlation ID: {CorrelationId}", correlationId);

            // Step 1: Retrieve data from vendor API
            VendorPatientData vendorData;
            try
            {
                vendorData = await _vendorApiClient.GetPatientAsync(id, cancellationToken);
            }
            catch (VendorApiException ex)
            {
                _logger.LogError(ex, "Vendor API call failed for correlation ID: {CorrelationId}", correlationId);
                return StatusCode(StatusCodes.Status502BadGateway,
                    CreateOperationOutcome("transient", "External system temporarily unavailable"));
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error during vendor API call for correlation ID: {CorrelationId}", correlationId);
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    CreateOperationOutcome("transient", "Service temporarily unavailable"));
            }

            if (vendorData == null)
            {
                _logger.LogWarning("Patient not found in vendor system for correlation ID: {CorrelationId}",
                    correlationId);
                return NotFound(CreateOperationOutcome("not-found", "Patient not found"));
            }

            // Step 2: Transform vendor data to FHIR resource
            DataMappingResult<Patient> mappingResult;
            try
            {
                mappingResult = await _dataMappingService.TransformPatientAsync(vendorData);
            }
            catch (DataMappingException ex)
            {
                _logger.LogError(ex, "Data mapping failed for correlation ID: {CorrelationId}", correlationId);
                return StatusCode(StatusCodes.Status422UnprocessableEntity,
                    CreateOperationOutcome("processing", "Failed to transform vendor data to FHIR format"));
            }

            if (!mappingResult.IsSuccess || mappingResult.Resource == null)
            {
                _logger.LogError("Data mapping unsuccessful for correlation ID: {CorrelationId}. Errors: {Errors}",
                    correlationId, string.Join(", ", mappingResult.Errors));
                return StatusCode(StatusCodes.Status422UnprocessableEntity,
                    CreateOperationOutcome("processing", "Data transformation failed", mappingResult.Errors));
            }

            // Step 3: Validate FHIR resource
            FhirValidationResult validationResult;
            try
            {
                var profileUrl = _fhirValidationService.GetDefaultProfileUrl("Patient");
                validationResult = await _fhirValidationService.ValidateResourceAsync(mappingResult.Resource, profileUrl, cancellationToken);
            }
            catch (FhirValidationException ex)
            {
                _logger.LogError(ex, "FHIR validation failed for correlation ID: {CorrelationId}", correlationId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateOperationOutcome("exception", "FHIR validation service error"));
            }

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("FHIR validation failed for correlation ID: {CorrelationId}. Issues: {Issues}",
                    correlationId, string.Join(", ", validationResult.Issues.Select(i => i.Description)));
                return StatusCode(StatusCodes.Status422UnprocessableEntity,
                    _fhirValidationService.CreateOperationOutcome(validationResult));
            }

            // Step 4: Return validated FHIR resource
            stopwatch.Stop();
            _logger.LogInformation("Patient retrieval completed successfully for correlation ID: {CorrelationId}. Duration: {Duration}ms, " +
                "Fields mapped: {FieldsMapped}, Quirks handled: {QuirksHandled}",
                correlationId, stopwatch.ElapsedMilliseconds, mappingResult.Metadata.FieldsMapped, mappingResult.Metadata.QuirksHandled);

            Response.Headers.Add("X-Correlation-ID", correlationId);
            Response.Headers.Add("X-Processing-Time", stopwatch.ElapsedMilliseconds.ToString());

            return Ok(mappingResult.Resource);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Patient retrieval was cancelled for correlation ID: {CorrelationId}", correlationId);
            return StatusCode(StatusCodes.Status408RequestTimeout,
                CreateOperationOutcome("timeout", "Request was cancelled"));
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Unexpected error during patient retrieval for correlation ID: {CorrelationId}. Duration: {Duration}ms",
                correlationId, stopwatch.ElapsedMilliseconds);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateOperationOutcome("exception", "An unexpected error occurred"));
        }
    }

    /// <summary>
    /// Retrieves observations for a specific patient
    /// </summary>
    /// <param name="id">Patient identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Bundle of FHIR Observation resources</returns>
    [HttpGet("{id}/observations")]
    [RequiredScope("patient/*.read")]
    public async Task<IActionResult> GetPatientObservations(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("Patient observations retrieval attempted with null or empty ID");
            return BadRequest(CreateOperationOutcome("invalid", "Patient ID is required"));
        }

        var stopwatch = Stopwatch.StartNew();
        var correlationId = Guid.NewGuid().ToString();

        try
        {
            _logger.LogInformation("Starting patient observations retrieval for patient ID and correlation ID: {CorrelationId}", correlationId);

            // Retrieve observations from vendor API
            List<VendorObservationData> vendorObservations;
            try
            {
                vendorObservations = await _vendorApiClient.GetPatientObservationsAsync(id, cancellationToken);
            }
            catch (VendorApiException ex)
            {
                _logger.LogError(ex, "Vendor API call failed for observations, correlation ID: {CorrelationId}", correlationId);
                return StatusCode(StatusCodes.Status502BadGateway,
                    CreateOperationOutcome("transient", "External system temporarily unavailable"));
            }

            var bundle = new Bundle
            {
                Id = Guid.NewGuid().ToString(),
                Type = Bundle.BundleType.Searchset,
                Timestamp = DateTimeOffset.UtcNow,
                Total = vendorObservations?.Count ?? 0
            };

            if (vendorObservations?.Any() == true)
            {
                foreach (var vendorObservation in vendorObservations)
                {
                    try
                    {
                        var mappingResult = await _dataMappingService.TransformObservationAsync(vendorObservation);

                        if (mappingResult.IsSuccess && mappingResult.Resource != null)
                        {
                            var validationResult = await _fhirValidationService.ValidateResourceAsync(mappingResult.Resource, cancellationToken);

                            if (validationResult.IsValid)
                            {
                                bundle.Entry.Add(new Bundle.EntryComponent
                                {
                                    Resource = mappingResult.Resource,
                                    FullUrl = $"{Request.Scheme}://{Request.Host}/fhir/Observation/{mappingResult.Resource.Id}"
                                });
                            }
                            else
                            {
                                _logger.LogWarning("Observation validation failed for correlation ID: {CorrelationId}", correlationId);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process observation for correlation ID: {CorrelationId}", correlationId);
                        // Continue processing other observations
                    }
                }
            }

            stopwatch.Stop();
            _logger.LogInformation("Patient observations retrieval completed for correlation ID: {CorrelationId}. " +
                "Duration: {Duration}ms, Observations returned: {Count}",
                correlationId, stopwatch.ElapsedMilliseconds, bundle.Entry.Count);

            Response.Headers.Add("X-Correlation-ID", correlationId);
            Response.Headers.Add("X-Processing-Time", stopwatch.ElapsedMilliseconds.ToString());

            return Ok(bundle);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Patient observations retrieval was cancelled for correlation ID: {CorrelationId}", correlationId);
            return StatusCode(StatusCodes.Status408RequestTimeout,
                CreateOperationOutcome("timeout", "Request was cancelled"));
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Unexpected error during patient observations retrieval for correlation ID: {CorrelationId}. Duration: {Duration}ms",
                correlationId, stopwatch.ElapsedMilliseconds);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateOperationOutcome("exception", "An unexpected error occurred"));
        }
    }

    /// <summary>
    /// Creates a structured FHIR OperationOutcome for error responses
    /// </summary>
    /// <param name="code">Error code</param>
    /// <param name="description">Error description</param>
    /// <param name="details">Additional error details</param>
    /// <param name="severity">Issue severity (defaults to Error)</param>
    /// <returns>FHIR OperationOutcome resource</returns>
    private OperationOutcome CreateOperationOutcome(string code, string description,
        List<string>? details = null, OperationOutcome.IssueSeverity severity = OperationOutcome.IssueSeverity.Error)
    {
        var operationOutcome = new OperationOutcome
        {
            Id = Guid.NewGuid().ToString(),
            Meta = new Meta
            {
                LastUpdated = DateTimeOffset.UtcNow,
                Profile = new[] { "http://hl7.org/fhir/StructureDefinition/OperationOutcome" }
            }
        };

        var issueType = code.ToLowerInvariant() switch
        {
            "invalid" => OperationOutcome.IssueType.Invalid,
            "not-found" => OperationOutcome.IssueType.NotFound,
            "timeout" => OperationOutcome.IssueType.Timeout,
            "transient" => OperationOutcome.IssueType.Transient,
            "processing" => OperationOutcome.IssueType.Processing,
            "exception" => OperationOutcome.IssueType.Exception,
            _ => OperationOutcome.IssueType.Processing
        };

        var issue = new OperationOutcome.IssueComponent
        {
            Severity = severity,
            Code = issueType,
            Diagnostics = description
        };

        if (!string.IsNullOrEmpty(code))
        {
            issue.Details = new CodeableConcept
            {
                Text = code,
                Coding = new List<Coding>
                {
                    new() { System = "http://terminology.hl7.org/CodeSystem/operation-outcome", Code = code }
                }
            };
        }

        if (details?.Any() == true)
        {
            issue.Diagnostics += $" Additional details: {string.Join("; ", details.Where(d => !string.IsNullOrWhiteSpace(d)))}";
        }

        operationOutcome.Issue.Add(issue);
        return operationOutcome;
    }
}

/// <summary>
/// Custom authorization attribute for SMART on FHIR scope requirements
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class RequiredScopeAttribute : AuthorizeAttribute
{
    public string Scope { get; }

    public RequiredScopeAttribute(string scope) : base(scope)
    {
        Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        Policy = scope; // Set the policy to the scope name for authorization
    }
}