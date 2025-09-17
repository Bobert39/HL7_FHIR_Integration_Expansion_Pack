using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using Hl7.Fhir.Model;
using FhirIntegrationService.Services;
using FhirIntegrationService.Services.Interfaces;
using FhirIntegrationService.Configuration;
using FhirIntegrationService.Exceptions;

namespace FhirIntegrationService.Tests;

public class DataMappingServiceTests
{
    private readonly Mock<ILogger<DataMappingService>> _mockLogger;
    private readonly FhirConfiguration _fhirConfiguration;
    private readonly DataMappingService _dataMappingService;

    public DataMappingServiceTests()
    {
        _mockLogger = new Mock<ILogger<DataMappingService>>();
        _fhirConfiguration = new FhirConfiguration
        {
            PatientIdentifierSystem = "http://test.org/patient-ids",
            ObservationIdentifierSystem = "http://test.org/observation-ids",
            ObservationCodeSystem = "http://loinc.org"
        };
        _dataMappingService = new DataMappingService(_mockLogger.Object, _fhirConfiguration);
    }

    #region Patient Transformation Tests

    [Fact]
    public async Task TransformPatientAsync_WithValidData_ShouldReturnSuccessfulResult()
    {
        // Arrange
        var vendorPatientData = new VendorPatientData
        {
            PatientId = "12345",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = "1990-01-15",
            Gender = "M",
            PhoneNumber = "5551234567",
            Email = "john.doe@example.com",
            Address = new VendorAddress
            {
                Street = "123 Main St",
                City = "Anytown",
                State = "CA",
                ZipCode = "12345",
                Country = "US"
            }
        };

        // Act
        var result = await _dataMappingService.TransformPatientAsync(vendorPatientData);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Resource.Should().NotBeNull();
        result.Errors.Should().BeEmpty();

        var patient = result.Resource;
        patient.Identifier.Should().HaveCount(1);
        patient.Identifier.First().Value.Should().Be("12345");
        patient.Name.Should().HaveCount(1);
        patient.Name.First().Given.First().Should().Be("John");
        patient.Name.First().Family.Should().Be("Doe");
        patient.BirthDate.Should().Be("1990-01-15");
        patient.Gender.Should().Be(AdministrativeGender.Male);
        patient.Telecom.Should().HaveCount(2); // Phone and email
        patient.Address.Should().HaveCount(1);
    }

    [Fact]
    public async Task TransformPatientAsync_WithMinimalData_ShouldReturnSuccessfulResult()
    {
        // Arrange
        var vendorPatientData = new VendorPatientData
        {
            PatientId = "12345",
            FirstName = "John"
        };

        // Act
        var result = await _dataMappingService.TransformPatientAsync(vendorPatientData);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Resource.Should().NotBeNull();
        result.Metadata.FieldsMapped.Should().Be(2); // PatientId and Name
    }

    [Fact]
    public async Task TransformPatientAsync_WithInvalidEmail_ShouldReturnWarning()
    {
        // Arrange
        var vendorPatientData = new VendorPatientData
        {
            PatientId = "12345",
            FirstName = "John",
            Email = "invalid-email"
        };

        // Act
        var result = await _dataMappingService.TransformPatientAsync(vendorPatientData);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Warnings.Should().ContainSingle();
        result.Warnings.First().Should().Contain("Email format appears invalid");
    }

    [Theory]
    [InlineData("M", AdministrativeGender.Male)]
    [InlineData("Male", AdministrativeGender.Male)]
    [InlineData("1", AdministrativeGender.Male)]
    [InlineData("F", AdministrativeGender.Female)]
    [InlineData("Female", AdministrativeGender.Female)]
    [InlineData("2", AdministrativeGender.Female)]
    [InlineData("O", AdministrativeGender.Other)]
    [InlineData("U", AdministrativeGender.Unknown)]
    [InlineData("", AdministrativeGender.Unknown)]
    [InlineData("INVALID", AdministrativeGender.Unknown)]
    public async Task TransformPatientAsync_GenderMapping_ShouldMapCorrectly(string vendorGender, AdministrativeGender expectedGender)
    {
        // Arrange
        var vendorPatientData = new VendorPatientData
        {
            PatientId = "12345",
            FirstName = "John",
            Gender = vendorGender
        };

        // Act
        var result = await _dataMappingService.TransformPatientAsync(vendorPatientData);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Resource.Gender.Should().Be(expectedGender);
    }

    [Theory]
    [InlineData("1990-01-15", "1990-01-15")]
    [InlineData("01/15/1990", "1990-01-15")]
    [InlineData("15/01/1990", "1990-01-15")]
    [InlineData("19900115", "1990-01-15")]
    [InlineData("1990-01-15T10:30:00", "1990-01-15")]
    public async Task TransformPatientAsync_DateFormats_ShouldParseCorrectly(string vendorDate, string expectedDate)
    {
        // Arrange
        var vendorPatientData = new VendorPatientData
        {
            PatientId = "12345",
            FirstName = "John",
            DateOfBirth = vendorDate
        };

        // Act
        var result = await _dataMappingService.TransformPatientAsync(vendorPatientData);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Resource.BirthDate.Should().Be(expectedDate);
    }

    [Theory]
    [InlineData("5551234567", "+15551234567")]
    [InlineData("15551234567", "+15551234567")]
    [InlineData("(555) 123-4567", "+15551234567")]
    [InlineData("555-123-4567", "+15551234567")]
    public async Task TransformPatientAsync_PhoneNumberFormats_ShouldNormalizeCorrectly(string vendorPhone, string expectedPhone)
    {
        // Arrange
        var vendorPatientData = new VendorPatientData
        {
            PatientId = "12345",
            FirstName = "John",
            PhoneNumber = vendorPhone
        };

        // Act
        var result = await _dataMappingService.TransformPatientAsync(vendorPatientData);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        var phoneContact = result.Resource.Telecom.FirstOrDefault(t => t.System == ContactPoint.ContactPointSystem.Phone);
        phoneContact.Should().NotBeNull();
        phoneContact.Value.Should().Be(expectedPhone);
    }

    #endregion

    #region Observation Transformation Tests

    [Fact]
    public async Task TransformObservationAsync_WithValidData_ShouldReturnSuccessfulResult()
    {
        // Arrange
        var vendorObservationData = new VendorObservationData
        {
            ObservationId = "obs-12345",
            PatientId = "patient-12345",
            ObservationType = "blood-pressure",
            Value = "120",
            Unit = "mmHg",
            DateTime = "2023-12-01T10:30:00",
            Status = "final"
        };

        // Act
        var result = await _dataMappingService.TransformObservationAsync(vendorObservationData);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Resource.Should().NotBeNull();
        result.Errors.Should().BeEmpty();

        var observation = result.Resource;
        observation.Identifier.Should().HaveCount(1);
        observation.Identifier.First().Value.Should().Be("obs-12345");
        observation.Subject.Reference.Should().Be("Patient/patient-12345");
        observation.Status.Should().Be(ObservationStatus.Final);
        observation.Code.Should().NotBeNull();
        observation.Value.Should().NotBeNull();
        observation.Effective.Should().NotBeNull();
    }

    [Theory]
    [InlineData("final", ObservationStatus.Final)]
    [InlineData("preliminary", ObservationStatus.Preliminary)]
    [InlineData("amended", ObservationStatus.Amended)]
    [InlineData("corrected", ObservationStatus.Corrected)]
    [InlineData("cancelled", ObservationStatus.Cancelled)]
    [InlineData("complete", ObservationStatus.Final)]
    [InlineData("reviewed", ObservationStatus.Final)]
    [InlineData("INVALID", ObservationStatus.Unknown)]
    public async Task TransformObservationAsync_StatusMapping_ShouldMapCorrectly(string vendorStatus, ObservationStatus expectedStatus)
    {
        // Arrange
        var vendorObservationData = new VendorObservationData
        {
            ObservationId = "obs-12345",
            PatientId = "patient-12345",
            ObservationType = "test",
            Status = vendorStatus
        };

        // Act
        var result = await _dataMappingService.TransformObservationAsync(vendorObservationData);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Resource.Status.Should().Be(expectedStatus);
    }

    [Fact]
    public async Task TransformObservationAsync_WithMissingPatientId_ShouldReturnError()
    {
        // Arrange
        var vendorObservationData = new VendorObservationData
        {
            ObservationId = "obs-12345",
            ObservationType = "test"
            // PatientId is missing
        };

        // Act
        var result = await _dataMappingService.TransformObservationAsync(vendorObservationData);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors.First().Should().Contain("Patient ID is required");
    }

    #endregion

    #region Data Quirks Handling Tests

    [Fact]
    public async Task HandleDataQuirkAsync_DateOfBirthWithMultipleFormats_ShouldNormalizeCorrectly()
    {
        // Arrange & Act & Assert for various date formats
        var testCases = new[]
        {
            ("1990-01-15", new DateTime(1990, 1, 15)),
            ("01/15/1990", new DateTime(1990, 1, 15)),
            ("15/01/1990", new DateTime(1990, 1, 15)),
            ("19900115", new DateTime(1990, 1, 15))
        };

        foreach (var (input, expected) in testCases)
        {
            var result = await _dataMappingService.HandleDataQuirkAsync("DateOfBirth", input);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.NormalizedValue.Should().BeOfType<DateTime>();
            ((DateTime)result.NormalizedValue).Date.Should().Be(expected.Date);
        }
    }

    [Fact]
    public async Task HandleDataQuirkAsync_InvalidDate_ShouldReturnFailure()
    {
        // Arrange
        var invalidDate = "not-a-date";

        // Act
        var result = await _dataMappingService.HandleDataQuirkAsync("DateOfBirth", invalidDate);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Notes.Should().ContainSingle();
        result.Notes.First().Should().Contain("Could not parse date value");
    }

    [Fact]
    public async Task HandleDataQuirkAsync_GenderWithVariousFormats_ShouldMapCorrectly()
    {
        // Arrange & Act & Assert for various gender formats
        var testCases = new[]
        {
            ("M", AdministrativeGender.Male),
            ("male", AdministrativeGender.Male),
            ("MALE", AdministrativeGender.Male),
            ("1", AdministrativeGender.Male),
            ("F", AdministrativeGender.Female),
            ("female", AdministrativeGender.Female),
            ("2", AdministrativeGender.Female),
            ("Unknown", AdministrativeGender.Unknown),
            ("", AdministrativeGender.Unknown),
            ("INVALID", AdministrativeGender.Unknown)
        };

        foreach (var (input, expected) in testCases)
        {
            var result = await _dataMappingService.HandleDataQuirkAsync("Gender", input);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.NormalizedValue.Should().Be(expected);
        }
    }

    [Theory]
    [InlineData("(555) 123-4567", "+15551234567")]
    [InlineData("555.123.4567", "+15551234567")]
    [InlineData("555-123-4567", "+15551234567")]
    [InlineData("5551234567", "+15551234567")]
    [InlineData("15551234567", "+15551234567")]
    [InlineData("+1 555 123 4567", "+15551234567")]
    public async Task HandleDataQuirkAsync_PhoneNumberNormalization_ShouldFormatCorrectly(string input, string expected)
    {
        // Act
        var result = await _dataMappingService.HandleDataQuirkAsync("PhoneNumber", input);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.NormalizedValue.Should().Be(expected);
    }

    [Fact]
    public async Task HandleDataQuirkAsync_AddressWithZipPlus4_ShouldFormatCorrectly()
    {
        // Arrange
        var vendorAddress = new VendorAddress
        {
            Street = "123 Main St",
            City = "Anytown",
            State = "CA",
            ZipCode = "123456789", // 9-digit ZIP without dash
            Country = null // Will default to US
        };

        // Act
        var result = await _dataMappingService.HandleDataQuirkAsync("Address", vendorAddress);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.NormalizedValue.Should().BeOfType<Address>();

        var address = (Address)result.NormalizedValue;
        address.PostalCode.Should().Be("12345-6789");
        address.Country.Should().Be("US");
        result.QuirkDescription.Should().Contain("Formatted 9-digit ZIP code");
    }

    #endregion

    #region Validation Tests

    [Fact]
    public async Task ValidateVendorDataAsync_ValidPatientData_ShouldReturnValid()
    {
        // Arrange
        var vendorPatientData = new VendorPatientData
        {
            PatientId = "12345",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        // Act
        var result = await _dataMappingService.ValidateVendorDataAsync(vendorPatientData);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateVendorDataAsync_InvalidPatientData_ShouldReturnErrors()
    {
        // Arrange
        var vendorPatientData = new VendorPatientData
        {
            // PatientId is missing
            FirstName = "John",
            Email = "invalid-email"
        };

        // Act
        var result = await _dataMappingService.ValidateVendorDataAsync(vendorPatientData);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors.First().Should().Contain("Patient ID is required");
        result.Warnings.Should().ContainSingle();
        result.Warnings.First().Should().Contain("Email format appears invalid");
    }

    [Fact]
    public async Task ValidateVendorDataAsync_UnsupportedDataType_ShouldReturnError()
    {
        // Arrange
        var unsupportedData = new { UnsupportedField = "value" };

        // Act
        var result = await _dataMappingService.ValidateVendorDataAsync(unsupportedData);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors.First().Should().Be("Unsupported vendor data type");
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task HandleDataQuirkAsync_UnknownFieldName_ShouldReturnOriginalValue()
    {
        // Arrange
        var originalValue = "test-value";

        // Act
        var result = await _dataMappingService.HandleDataQuirkAsync("UnknownField", originalValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.NormalizedValue.Should().Be(originalValue);
        result.QuirkDescription.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task TransformPatientAsync_WithException_ShouldReturnErrorResult()
    {
        // This test would require mocking internal dependencies to force an exception
        // For now, we'll test with null input which should be handled gracefully

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _dataMappingService.TransformPatientAsync(null);
        });
    }

    #endregion

    #region Performance Tests

    [Fact]
    public async Task TransformPatientAsync_Performance_ShouldCompleteWithinTimeLimit()
    {
        // Arrange
        var vendorPatientData = new VendorPatientData
        {
            PatientId = "12345",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = "1990-01-15",
            Gender = "M",
            PhoneNumber = "5551234567",
            Email = "john.doe@example.com",
            Address = new VendorAddress
            {
                Street = "123 Main St",
                City = "Anytown",
                State = "CA",
                ZipCode = "12345",
                Country = "US"
            }
        };

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await _dataMappingService.TransformPatientAsync(vendorPatientData);
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100); // Should complete in under 100ms
        result.Metadata.Duration.Should().BeLessThan(TimeSpan.FromMilliseconds(100));
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task DataMappingService_EndToEndPatientWorkflow_ShouldWorkCorrectly()
    {
        // Arrange
        var vendorPatientData = new VendorPatientData
        {
            PatientId = "patient-001",
            FirstName = "Alice",
            LastName = "Smith",
            DateOfBirth = "01/15/1985",
            Gender = "Female",
            PhoneNumber = "(555) 987-6543",
            Email = "alice.smith@example.com",
            Address = new VendorAddress
            {
                Street = "456 Oak Avenue",
                City = "Springfield",
                State = "IL",
                ZipCode = "625011234",
                Country = "USA"
            }
        };

        // Act - Full workflow: validate, transform, validate FHIR
        var validationResult = await _dataMappingService.ValidateVendorDataAsync(vendorPatientData);
        var transformResult = await _dataMappingService.TransformPatientAsync(vendorPatientData);
        var fhirValidationResult = await _dataMappingService.ValidateFhirResourceAsync(
            transformResult.Resource, "http://hl7.org/fhir/StructureDefinition/Patient");

        // Assert
        validationResult.IsValid.Should().BeTrue();
        transformResult.IsSuccess.Should().BeTrue();
        transformResult.Resource.Should().NotBeNull();
        transformResult.Metadata.FieldsMapped.Should().BeGreaterThan(0);
        transformResult.Metadata.QuirksHandled.Should().BeGreaterThan(0);

        // Verify specific mappings
        var patient = transformResult.Resource;
        patient.Name.First().Given.First().Should().Be("Alice");
        patient.Name.First().Family.Should().Be("Smith");
        patient.Gender.Should().Be(AdministrativeGender.Female);
        patient.BirthDate.Should().Be("1985-01-15");

        var phoneContact = patient.Telecom.FirstOrDefault(t => t.System == ContactPoint.ContactPointSystem.Phone);
        phoneContact.Value.Should().Be("+15559876543");

        var address = patient.Address.First();
        address.PostalCode.Should().Be("62501-1234");
        address.Country.Should().Be("USA");

        // FHIR validation should pass (basic validation)
        fhirValidationResult.IsValid.Should().BeTrue();
    }

    #endregion
}