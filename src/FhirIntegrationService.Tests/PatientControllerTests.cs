using FhirIntegrationService.Controllers;
using FhirIntegrationService.Exceptions;
using FhirIntegrationService.Services.Interfaces;
using FluentAssertions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace FhirIntegrationService.Tests;

/// <summary>
/// Integration tests for PatientController
/// </summary>
public class PatientControllerTests
{
    private readonly Mock<IDataMappingService> _mockDataMappingService;
    private readonly Mock<IVendorApiClient> _mockVendorApiClient;
    private readonly Mock<IFhirValidationService> _mockFhirValidationService;
    private readonly Mock<ILogger<PatientController>> _mockLogger;
    private readonly FhirJsonSerializer _fhirSerializer;
    private readonly PatientController _controller;

    public PatientControllerTests()
    {
        _mockDataMappingService = new Mock<IDataMappingService>();
        _mockVendorApiClient = new Mock<IVendorApiClient>();
        _mockFhirValidationService = new Mock<IFhirValidationService>();
        _mockLogger = new Mock<ILogger<PatientController>>();
        _fhirSerializer = new FhirJsonSerializer();

        _controller = new PatientController(
            _mockDataMappingService.Object,
            _mockVendorApiClient.Object,
            _mockFhirValidationService.Object,
            _mockLogger.Object,
            _fhirSerializer);

        // Setup controller context for testing
        SetupControllerContext();
    }

    [Fact]
    public async Task GetPatient_WithValidId_ReturnsPatientResource()
    {
        // Arrange
        var patientId = "test-patient-123";
        var vendorData = CreateTestVendorPatientData(patientId);
        var fhirPatient = CreateTestFhirPatient(patientId);
        var mappingResult = new DataMappingResult<Patient>
        {
            IsSuccess = true,
            Resource = fhirPatient,
            Metadata = new TransformationMetadata
            {
                FieldsMapped = 8,
                QuirksHandled = 2,
                Duration = TimeSpan.FromMilliseconds(85)
            }
        };
        var validationResult = new FhirValidationResult
        {
            IsValid = true,
            ProfileUrl = "http://hl7.org/fhir/StructureDefinition/Patient"
        };

        _mockVendorApiClient
            .Setup(x => x.GetPatientAsync(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vendorData);

        _mockDataMappingService
            .Setup(x => x.TransformPatientAsync(vendorData))
            .ReturnsAsync(mappingResult);

        _mockFhirValidationService
            .Setup(x => x.GetDefaultProfileUrl("Patient"))
            .Returns("http://hl7.org/fhir/StructureDefinition/Patient");

        _mockFhirValidationService
            .Setup(x => x.ValidateResourceAsync(fhirPatient, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.GetPatient(patientId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().BeOfType<Patient>();

        var returnedPatient = (Patient)okResult.Value!;
        returnedPatient.Id.Should().Be(patientId);
        returnedPatient.Name.First().Family.Should().Be("Doe");
        returnedPatient.Name.First().Given.First().Should().Be("John");

        // Verify all services were called
        _mockVendorApiClient.Verify(x => x.GetPatientAsync(patientId, It.IsAny<CancellationToken>()), Times.Once);
        _mockDataMappingService.Verify(x => x.TransformPatientAsync(vendorData), Times.Once);
        _mockFhirValidationService.Verify(x => x.ValidateResourceAsync(fhirPatient, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPatient_WithNullOrEmptyId_ReturnsBadRequest()
    {
        // Arrange & Act
        var resultNull = await _controller.GetPatient(null!);
        var resultEmpty = await _controller.GetPatient("");
        var resultWhitespace = await _controller.GetPatient("   ");

        // Assert
        resultNull.Should().BeOfType<BadRequestObjectResult>();
        resultEmpty.Should().BeOfType<BadRequestObjectResult>();
        resultWhitespace.Should().BeOfType<BadRequestObjectResult>();

        var badRequestNull = (BadRequestObjectResult)resultNull;
        badRequestNull.Value.Should().BeOfType<OperationOutcome>();

        // Verify no vendor API calls were made
        _mockVendorApiClient.Verify(x => x.GetPatientAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetPatient_WhenPatientNotFound_ReturnsNotFound()
    {
        // Arrange
        var patientId = "non-existent-patient";

        _mockVendorApiClient
            .Setup(x => x.GetPatientAsync(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((VendorPatientData)null!);

        // Act
        var result = await _controller.GetPatient(patientId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result;
        notFoundResult.Value.Should().BeOfType<OperationOutcome>();

        var operationOutcome = (OperationOutcome)notFoundResult.Value!;
        operationOutcome.Issue.Should().HaveCount(1);
        operationOutcome.Issue.First().Diagnostics.Should().Contain("not found");
    }

    [Fact]
    public async Task GetPatient_WhenVendorApiThrowsException_ReturnsBadGateway()
    {
        // Arrange
        var patientId = "test-patient-123";

        _mockVendorApiClient
            .Setup(x => x.GetPatientAsync(patientId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new VendorApiException("Vendor API is down", 503, "Service Unavailable", "/api/patients/test-patient-123"));

        // Act
        var result = await _controller.GetPatient(patientId);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(502);
        objectResult.Value.Should().BeOfType<OperationOutcome>();
    }

    [Fact]
    public async Task GetPatient_WhenMappingFails_ReturnsUnprocessableEntity()
    {
        // Arrange
        var patientId = "test-patient-123";
        var vendorData = CreateTestVendorPatientData(patientId);
        var mappingResult = new DataMappingResult<Patient>
        {
            IsSuccess = false,
            Resource = null,
            Errors = new List<string> { "Failed to map gender field", "Invalid date format" }
        };

        _mockVendorApiClient
            .Setup(x => x.GetPatientAsync(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vendorData);

        _mockDataMappingService
            .Setup(x => x.TransformPatientAsync(vendorData))
            .ReturnsAsync(mappingResult);

        // Act
        var result = await _controller.GetPatient(patientId);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(422);
        objectResult.Value.Should().BeOfType<OperationOutcome>();

        var operationOutcome = (OperationOutcome)objectResult.Value!;
        operationOutcome.Issue.Should().HaveCount(1);
        operationOutcome.Issue.First().Diagnostics.Should().Contain("transformation failed");
    }

    [Fact]
    public async Task GetPatient_WhenFhirValidationFails_ReturnsUnprocessableEntity()
    {
        // Arrange
        var patientId = "test-patient-123";
        var vendorData = CreateTestVendorPatientData(patientId);
        var fhirPatient = CreateTestFhirPatient(patientId);
        var mappingResult = new DataMappingResult<Patient>
        {
            IsSuccess = true,
            Resource = fhirPatient,
            Metadata = new TransformationMetadata()
        };
        var validationResult = new FhirValidationResult
        {
            IsValid = false,
            Issues = new List<ValidationIssue>
            {
                new() { Severity = IssueSeverity.Error, Description = "Missing required identifier", Location = "Patient.identifier" }
            }
        };

        _mockVendorApiClient
            .Setup(x => x.GetPatientAsync(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vendorData);

        _mockDataMappingService
            .Setup(x => x.TransformPatientAsync(vendorData))
            .ReturnsAsync(mappingResult);

        _mockFhirValidationService
            .Setup(x => x.GetDefaultProfileUrl("Patient"))
            .Returns("http://hl7.org/fhir/StructureDefinition/Patient");

        _mockFhirValidationService
            .Setup(x => x.ValidateResourceAsync(fhirPatient, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _mockFhirValidationService
            .Setup(x => x.CreateOperationOutcome(validationResult))
            .Returns(new OperationOutcome());

        // Act
        var result = await _controller.GetPatient(patientId);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(422);
        objectResult.Value.Should().BeOfType<OperationOutcome>();
    }

    [Fact]
    public async Task GetPatientObservations_WithValidId_ReturnsObservationBundle()
    {
        // Arrange
        var patientId = "test-patient-123";
        var vendorObservations = new List<VendorObservationData>
        {
            CreateTestVendorObservationData("obs-1", patientId),
            CreateTestVendorObservationData("obs-2", patientId)
        };

        _mockVendorApiClient
            .Setup(x => x.GetPatientObservationsAsync(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vendorObservations);

        foreach (var vendorObs in vendorObservations)
        {
            var fhirObservation = CreateTestFhirObservation(vendorObs.ObservationId!, patientId);
            var mappingResult = new DataMappingResult<Observation>
            {
                IsSuccess = true,
                Resource = fhirObservation,
                Metadata = new TransformationMetadata()
            };

            _mockDataMappingService
                .Setup(x => x.TransformObservationAsync(vendorObs))
                .ReturnsAsync(mappingResult);

            _mockFhirValidationService
                .Setup(x => x.ValidateResourceAsync(fhirObservation, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FhirValidationResult { IsValid = true });
        }

        // Act
        var result = await _controller.GetPatientObservations(patientId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().BeOfType<Bundle>();

        var bundle = (Bundle)okResult.Value!;
        bundle.Type.Should().Be(Bundle.BundleType.Searchset);
        bundle.Entry.Should().HaveCount(2);
        bundle.Total.Should().Be(2);

        // Verify all observations are in the bundle
        var observationIds = bundle.Entry.Select(e => e.Resource.Id).ToArray();
        observationIds.Should().Contain("obs-1");
        observationIds.Should().Contain("obs-2");
    }

    [Fact]
    public async Task GetPatientObservations_WhenNoObservationsFound_ReturnsEmptyBundle()
    {
        // Arrange
        var patientId = "test-patient-123";

        _mockVendorApiClient
            .Setup(x => x.GetPatientObservationsAsync(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<VendorObservationData>());

        // Act
        var result = await _controller.GetPatientObservations(patientId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().BeOfType<Bundle>();

        var bundle = (Bundle)okResult.Value!;
        bundle.Entry.Should().BeEmpty();
        bundle.Total.Should().Be(0);
    }

    [Fact]
    public async Task GetPatient_WithCancellationToken_ReturnsTimeout()
    {
        // Arrange
        var patientId = "test-patient-123";
        using var cts = new CancellationTokenSource();
        cts.Cancel(); // Immediately cancel

        _mockVendorApiClient
            .Setup(x => x.GetPatientAsync(patientId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = await _controller.GetPatient(patientId, cts.Token);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(408);
        objectResult.Value.Should().BeOfType<OperationOutcome>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task GetPatientObservations_WithInvalidId_ReturnsBadRequest(string? patientId)
    {
        // Act
        var result = await _controller.GetPatientObservations(patientId!);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)result;
        badRequestResult.Value.Should().BeOfType<OperationOutcome>();
    }

    [Fact]
    public async Task GetPatient_ShouldIncludeCorrelationIdAndProcessingTime()
    {
        // Arrange
        var patientId = "test-patient-123";
        var vendorData = CreateTestVendorPatientData(patientId);
        var fhirPatient = CreateTestFhirPatient(patientId);
        var mappingResult = new DataMappingResult<Patient>
        {
            IsSuccess = true,
            Resource = fhirPatient,
            Metadata = new TransformationMetadata()
        };
        var validationResult = new FhirValidationResult { IsValid = true };

        _mockVendorApiClient
            .Setup(x => x.GetPatientAsync(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vendorData);

        _mockDataMappingService
            .Setup(x => x.TransformPatientAsync(vendorData))
            .ReturnsAsync(mappingResult);

        _mockFhirValidationService
            .Setup(x => x.GetDefaultProfileUrl("Patient"))
            .Returns("http://hl7.org/fhir/StructureDefinition/Patient");

        _mockFhirValidationService
            .Setup(x => x.ValidateResourceAsync(fhirPatient, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.GetPatient(patientId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        // Check response headers
        _controller.Response.Headers.Should().ContainKey("X-Correlation-ID");
        _controller.Response.Headers.Should().ContainKey("X-Processing-Time");
    }

    private VendorPatientData CreateTestVendorPatientData(string patientId)
    {
        return new VendorPatientData
        {
            PatientId = patientId,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = "1980-01-15",
            Gender = "Male",
            PhoneNumber = "+1-555-0123",
            Email = "john.doe@example.com",
            Address = new VendorAddress
            {
                Street = "123 Main St",
                City = "Anytown",
                State = "CA",
                ZipCode = "12345",
                Country = "USA"
            }
        };
    }

    private Patient CreateTestFhirPatient(string patientId)
    {
        return new Patient
        {
            Id = patientId,
            Name = new List<HumanName>
            {
                new HumanName
                {
                    Family = "Doe",
                    Given = new[] { "John" }
                }
            },
            Gender = AdministrativeGender.Male,
            BirthDate = "1980-01-15",
            Telecom = new List<ContactPoint>
            {
                new ContactPoint
                {
                    System = ContactPoint.ContactPointSystem.Phone,
                    Value = "+1-555-0123"
                },
                new ContactPoint
                {
                    System = ContactPoint.ContactPointSystem.Email,
                    Value = "john.doe@example.com"
                }
            },
            Address = new List<Address>
            {
                new Address
                {
                    Line = new[] { "123 Main St" },
                    City = "Anytown",
                    State = "CA",
                    PostalCode = "12345",
                    Country = "USA"
                }
            }
        };
    }

    private VendorObservationData CreateTestVendorObservationData(string observationId, string patientId)
    {
        return new VendorObservationData
        {
            ObservationId = observationId,
            PatientId = patientId,
            ObservationType = "vital-signs",
            Value = "120",
            Unit = "mmHg",
            DateTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            Status = "final"
        };
    }

    private Observation CreateTestFhirObservation(string observationId, string patientId)
    {
        return new Observation
        {
            Id = observationId,
            Status = ObservationStatus.Final,
            Category = new List<CodeableConcept>
            {
                new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://terminology.hl7.org/CodeSystem/observation-category",
                            Code = "vital-signs"
                        }
                    }
                }
            },
            Code = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding
                    {
                        System = "http://loinc.org",
                        Code = "85354-9",
                        Display = "Blood pressure panel"
                    }
                }
            },
            Subject = new ResourceReference($"Patient/{patientId}"),
            Value = new Quantity
            {
                Value = 120,
                Unit = "mmHg",
                System = "http://unitsofmeasure.org",
                Code = "mm[Hg]"
            },
            Effective = new FhirDateTime(DateTimeOffset.UtcNow)
        };
    }

    private void SetupControllerContext()
    {
        var httpContext = new DefaultHttpContext();

        // Setup user claims for authentication tests
        var claims = new List<Claim>
        {
            new("scope", "patient/*.read"),
            new("scope", "patient/*.write"),
            new(ClaimTypes.NameIdentifier, "test-user"),
            new("client_id", "test-client")
        };

        var identity = new ClaimsIdentity(claims, "Bearer");
        httpContext.User = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
        {
            HttpContext = httpContext
        };
    }
}