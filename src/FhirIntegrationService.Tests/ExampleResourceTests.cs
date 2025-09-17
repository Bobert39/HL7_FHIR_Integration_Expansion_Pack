using Xunit;
using FluentAssertions;
using System.IO;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.Logging;
using Moq;

namespace FhirIntegrationService.Tests;

/// <summary>
/// Tests for FHIR example resource validation and quality
/// </summary>
public class ExampleResourceTests
{
    private readonly Mock<ILogger<ExampleResourceTests>> _mockLogger;
    private readonly FhirJsonParser _fhirParser;
    private readonly string _testResourcesPath;
    private readonly string _fhirPackageExamplesPath;

    public ExampleResourceTests()
    {
        _mockLogger = new Mock<ILogger<ExampleResourceTests>>();
        _fhirParser = new FhirJsonParser();
        _testResourcesPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "tests", "resources");
        _fhirPackageExamplesPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "fhir-package", "examples");
    }

    [Fact]
    public void PatientValidExample_ShouldExistAndBeValidFhir()
    {
        // Arrange
        var patientExamplePath = Path.Combine(_testResourcesPath, "patient-valid-001.json");

        // Act & Assert
        File.Exists(patientExamplePath).Should().BeTrue("Valid patient example should exist");

        var patientJson = File.ReadAllText(patientExamplePath);
        patientJson.Should().NotBeNullOrEmpty("Patient example should have content");

        var patient = _fhirParser.Parse<Patient>(patientJson);
        patient.Should().NotBeNull("Patient example should be valid FHIR");
        patient.ResourceType.Should().Be(ResourceType.Patient);
    }

    [Fact]
    public void PatientValidExample_ShouldHaveRequiredElements()
    {
        // Arrange
        var patientExamplePath = Path.Combine(_testResourcesPath, "patient-valid-001.json");
        var patientJson = File.ReadAllText(patientExamplePath);
        var patient = _fhirParser.Parse<Patient>(patientJson);

        // Act & Assert
        patient.Id.Should().NotBeNullOrEmpty("Patient should have ID");
        patient.Id.Should().Be("patient-001", "Patient should have expected ID");

        patient.Meta.Should().NotBeNull("Patient should have meta");
        patient.Meta.VersionId.Should().NotBeNullOrEmpty("Patient should have version");
        patient.Meta.LastUpdated.Should().NotBeNull("Patient should have last updated");

        patient.Active.Should().BeTrue("Patient should be active");

        patient.Identifier.Should().NotBeEmpty("Patient should have identifiers");
        patient.Name.Should().NotBeEmpty("Patient should have names");
        patient.Telecom.Should().NotBeEmpty("Patient should have telecom");
        patient.Gender.Should().NotBeNull("Patient should have gender");
        patient.BirthDate.Should().NotBeNullOrEmpty("Patient should have birth date");
        patient.Address.Should().NotBeEmpty("Patient should have addresses");
    }

    [Fact]
    public void PatientValidExample_ShouldHaveValidIdentifiers()
    {
        // Arrange
        var patientExamplePath = Path.Combine(_testResourcesPath, "patient-valid-001.json");
        var patientJson = File.ReadAllText(patientExamplePath);
        var patient = _fhirParser.Parse<Patient>(patientJson);

        // Act & Assert
        var mrnIdentifier = patient.Identifier.FirstOrDefault(i =>
            i.Type?.Coding?.Any(c => c.Code == "MR" && c.System == "http://terminology.hl7.org/CodeSystem/v2-0203") == true);

        mrnIdentifier.Should().NotBeNull("Patient should have MRN identifier");
        mrnIdentifier.Use.Should().Be(Identifier.IdentifierUse.Usual, "MRN should have 'usual' use");
        mrnIdentifier.System.Should().NotBeNullOrEmpty("MRN should have system");
        mrnIdentifier.System.Should().Be("http://hospital.example.org/patient-ids", "MRN should have expected system");
        mrnIdentifier.Value.Should().NotBeNullOrEmpty("MRN should have value");
        mrnIdentifier.Value.Should().Be("MRN-001234", "MRN should have expected value");
    }

    [Fact]
    public void PatientValidExample_ShouldHaveValidName()
    {
        // Arrange
        var patientExamplePath = Path.Combine(_testResourcesPath, "patient-valid-001.json");
        var patientJson = File.ReadAllText(patientExamplePath);
        var patient = _fhirParser.Parse<Patient>(patientJson);

        // Act & Assert
        var officialName = patient.Name.FirstOrDefault(n => n.Use == HumanName.NameUse.Official);
        officialName.Should().NotBeNull("Patient should have official name");
        officialName.Family.Should().NotBeNullOrEmpty("Official name should have family name");
        officialName.Family.Should().Be("TestPatient", "Official name should have expected family name");
        officialName.Given.Should().NotBeEmpty("Official name should have given names");
        officialName.Given.Should().Contain("John", "Official name should contain expected given name");
        officialName.Given.Should().Contain("Michael", "Official name should contain expected middle name");
    }

    [Fact]
    public void PatientValidExample_ShouldHaveValidTelecom()
    {
        // Arrange
        var patientExamplePath = Path.Combine(_testResourcesPath, "patient-valid-001.json");
        var patientJson = File.ReadAllText(patientExamplePath);
        var patient = _fhirParser.Parse<Patient>(patientJson);

        // Act & Assert
        var phoneContact = patient.Telecom.FirstOrDefault(t => t.System == ContactPoint.ContactPointSystem.Phone);
        phoneContact.Should().NotBeNull("Patient should have phone contact");
        phoneContact.Value.Should().NotBeNullOrEmpty("Phone contact should have value");
        phoneContact.Value.Should().Be("+1-555-123-4567", "Phone contact should have expected value");
        phoneContact.Use.Should().Be(ContactPoint.ContactPointUse.Home, "Phone contact should have home use");

        var emailContact = patient.Telecom.FirstOrDefault(t => t.System == ContactPoint.ContactPointSystem.Email);
        emailContact.Should().NotBeNull("Patient should have email contact");
        emailContact.Value.Should().NotBeNullOrEmpty("Email contact should have value");
        emailContact.Value.Should().Be("john.testpatient@example.com", "Email contact should have expected value");
        emailContact.Use.Should().Be(ContactPoint.ContactPointUse.Home, "Email contact should have home use");
    }

    [Fact]
    public void PatientValidExample_ShouldHaveValidAddress()
    {
        // Arrange
        var patientExamplePath = Path.Combine(_testResourcesPath, "patient-valid-001.json");
        var patientJson = File.ReadAllText(patientExamplePath);
        var patient = _fhirParser.Parse<Patient>(patientJson);

        // Act & Assert
        var homeAddress = patient.Address.FirstOrDefault(a => a.Use == Address.AddressUse.Home);
        homeAddress.Should().NotBeNull("Patient should have home address");
        homeAddress.Type.Should().Be(Address.AddressType.Both, "Home address should have 'both' type");
        homeAddress.Line.Should().NotBeEmpty("Home address should have address lines");
        homeAddress.Line.Should().Contain("123 Main Street", "Home address should contain street");
        homeAddress.Line.Should().Contain("Apt 4B", "Home address should contain apartment");
        homeAddress.City.Should().Be("Anytown", "Home address should have expected city");
        homeAddress.State.Should().Be("NY", "Home address should have expected state");
        homeAddress.PostalCode.Should().Be("12345", "Home address should have expected postal code");
        homeAddress.Country.Should().Be("US", "Home address should have expected country");
    }

    [Fact]
    public void PatientValidExample_ShouldHaveValidEmergencyContact()
    {
        // Arrange
        var patientExamplePath = Path.Combine(_testResourcesPath, "patient-valid-001.json");
        var patientJson = File.ReadAllText(patientExamplePath);
        var patient = _fhirParser.Parse<Patient>(patientJson);

        // Act & Assert
        patient.Contact.Should().NotBeEmpty("Patient should have contacts");

        var emergencyContact = patient.Contact.FirstOrDefault(c =>
            c.Relationship?.Any(r => r.Coding?.Any(coding => coding.Code == "C") == true) == true);

        emergencyContact.Should().NotBeNull("Patient should have emergency contact");
        emergencyContact.Name.Should().NotBeNull("Emergency contact should have name");
        emergencyContact.Name.Family.Should().Be("TestPatient", "Emergency contact should have expected family name");
        emergencyContact.Name.Given.Should().Contain("Jane", "Emergency contact should have expected given name");
        emergencyContact.Telecom.Should().NotBeEmpty("Emergency contact should have telecom");

        var emergencyPhone = emergencyContact.Telecom.FirstOrDefault(t => t.System == ContactPoint.ContactPointSystem.Phone);
        emergencyPhone.Should().NotBeNull("Emergency contact should have phone");
        emergencyPhone.Value.Should().Be("+1-555-987-6543", "Emergency contact phone should have expected value");
        emergencyPhone.Use.Should().Be(ContactPoint.ContactPointUse.Mobile, "Emergency contact phone should be mobile");
    }

    [Fact]
    public void ObservationValidExample_ShouldExistAndBeValidFhir()
    {
        // Arrange
        var observationExamplePath = Path.Combine(_testResourcesPath, "observation-valid-001.json");

        // Act & Assert
        File.Exists(observationExamplePath).Should().BeTrue("Valid observation example should exist");

        var observationJson = File.ReadAllText(observationExamplePath);
        observationJson.Should().NotBeNullOrEmpty("Observation example should have content");

        var observation = _fhirParser.Parse<Observation>(observationJson);
        observation.Should().NotBeNull("Observation example should be valid FHIR");
        observation.ResourceType.Should().Be(ResourceType.Observation);
    }

    [Fact]
    public void ObservationValidExample_ShouldHaveRequiredElements()
    {
        // Arrange
        var observationExamplePath = Path.Combine(_testResourcesPath, "observation-valid-001.json");
        var observationJson = File.ReadAllText(observationExamplePath);
        var observation = _fhirParser.Parse<Observation>(observationJson);

        // Act & Assert
        observation.Id.Should().NotBeNullOrEmpty("Observation should have ID");
        observation.Id.Should().Be("observation-001", "Observation should have expected ID");

        observation.Meta.Should().NotBeNull("Observation should have meta");
        observation.Meta.VersionId.Should().NotBeNullOrEmpty("Observation should have version");
        observation.Meta.LastUpdated.Should().NotBeNull("Observation should have last updated");

        observation.Status.Should().Be(ObservationStatus.Final, "Observation should have final status");
        observation.Category.Should().NotBeEmpty("Observation should have categories");
        observation.Code.Should().NotBeNull("Observation should have code");
        observation.Subject.Should().NotBeNull("Observation should have subject");
        observation.Effective.Should().NotBeNull("Observation should have effective time");
        observation.Value.Should().NotBeNull("Observation should have value");
        observation.Performer.Should().NotBeEmpty("Observation should have performers");
    }

    [Fact]
    public void ObservationValidExample_ShouldHaveValidCategory()
    {
        // Arrange
        var observationExamplePath = Path.Combine(_testResourcesPath, "observation-valid-001.json");
        var observationJson = File.ReadAllText(observationExamplePath);
        var observation = _fhirParser.Parse<Observation>(observationJson);

        // Act & Assert
        var vitalSignsCategory = observation.Category.FirstOrDefault(c =>
            c.Coding?.Any(coding =>
                coding.System == "http://terminology.hl7.org/CodeSystem/observation-category" &&
                coding.Code == "vital-signs") == true);

        vitalSignsCategory.Should().NotBeNull("Observation should have vital-signs category");
        var vitalSignsCoding = vitalSignsCategory.Coding.First(c => c.Code == "vital-signs");
        vitalSignsCoding.Display.Should().Be("Vital Signs", "Vital signs category should have expected display");
    }

    [Fact]
    public void ObservationValidExample_ShouldHaveValidLoincCode()
    {
        // Arrange
        var observationExamplePath = Path.Combine(_testResourcesPath, "observation-valid-001.json");
        var observationJson = File.ReadAllText(observationExamplePath);
        var observation = _fhirParser.Parse<Observation>(observationJson);

        // Act & Assert
        var loincCoding = observation.Code.Coding.FirstOrDefault(c => c.System == "http://loinc.org");
        loincCoding.Should().NotBeNull("Observation should have LOINC coding");
        loincCoding.Code.Should().Be("8480-6", "Observation should have expected LOINC code");
        loincCoding.Display.Should().Be("Systolic blood pressure", "LOINC coding should have expected display");
    }

    [Fact]
    public void ObservationValidExample_ShouldHaveValidSubjectReference()
    {
        // Arrange
        var observationExamplePath = Path.Combine(_testResourcesPath, "observation-valid-001.json");
        var observationJson = File.ReadAllText(observationExamplePath);
        var observation = _fhirParser.Parse<Observation>(observationJson);

        // Act & Assert
        observation.Subject.Reference.Should().NotBeNullOrEmpty("Observation should have subject reference");
        observation.Subject.Reference.Should().Be("Patient/patient-001", "Observation should reference expected patient");
        observation.Subject.Display.Should().NotBeNullOrEmpty("Subject reference should have display");
        observation.Subject.Display.Should().Be("John Michael TestPatient", "Subject display should match patient name");
    }

    [Fact]
    public void ObservationValidExample_ShouldHaveValidQuantityValue()
    {
        // Arrange
        var observationExamplePath = Path.Combine(_testResourcesPath, "observation-valid-001.json");
        var observationJson = File.ReadAllText(observationExamplePath);
        var observation = _fhirParser.Parse<Observation>(observationJson);

        // Act & Assert
        var quantityValue = observation.Value as Quantity;
        quantityValue.Should().NotBeNull("Observation should have quantity value");
        quantityValue.Value.Should().Be(120, "Systolic BP value should be 120");
        quantityValue.Unit.Should().Be("mmHg", "Systolic BP unit should be mmHg");
        quantityValue.System.Should().Be("http://unitsofmeasure.org", "Unit should use UCUM system");
        quantityValue.Code.Should().Be("mm[Hg]", "Unit code should be UCUM code");
    }

    [Fact]
    public void ObservationValidExample_ShouldHaveValidComponents()
    {
        // Arrange
        var observationExamplePath = Path.Combine(_testResourcesPath, "observation-valid-001.json");
        var observationJson = File.ReadAllText(observationExamplePath);
        var observation = _fhirParser.Parse<Observation>(observationJson);

        // Act & Assert
        observation.Component.Should().NotBeEmpty("Blood pressure observation should have components");

        var diastolicComponent = observation.Component.FirstOrDefault(c =>
            c.Code?.Coding?.Any(coding => coding.Code == "8462-4") == true);

        diastolicComponent.Should().NotBeNull("Observation should have diastolic component");

        var diastolicCoding = diastolicComponent.Code.Coding.First(c => c.Code == "8462-4");
        diastolicCoding.System.Should().Be("http://loinc.org", "Diastolic component should use LOINC");
        diastolicCoding.Display.Should().Be("Diastolic blood pressure", "Diastolic component should have expected display");

        var diastolicValue = diastolicComponent.Value as Quantity;
        diastolicValue.Should().NotBeNull("Diastolic component should have quantity value");
        diastolicValue.Value.Should().Be(80, "Diastolic BP value should be 80");
        diastolicValue.Unit.Should().Be("mmHg", "Diastolic BP unit should be mmHg");
    }

    [Fact]
    public void PatientInvalidExample_ShouldExistForNegativeTesting()
    {
        // Arrange
        var patientInvalidPath = Path.Combine(_testResourcesPath, "patient-invalid-001.json");

        // Act & Assert
        File.Exists(patientInvalidPath).Should().BeTrue("Invalid patient example should exist for testing");

        var patientJson = File.ReadAllText(patientInvalidPath);
        patientJson.Should().NotBeNullOrEmpty("Invalid patient example should have content");

        // Verify it can be parsed as JSON but may have validation issues
        var isValidJson = IsValidJson(patientJson);
        isValidJson.Should().BeTrue("Invalid patient example should be valid JSON");
    }

    [Fact]
    public void ObservationInvalidExample_ShouldExistForNegativeTesting()
    {
        // Arrange
        var observationInvalidPath = Path.Combine(_testResourcesPath, "observation-invalid-001.json");

        // Act & Assert
        File.Exists(observationInvalidPath).Should().BeTrue("Invalid observation example should exist for testing");

        var observationJson = File.ReadAllText(observationInvalidPath);
        observationJson.Should().NotBeNullOrEmpty("Invalid observation example should have content");

        // Verify it can be parsed as JSON but may have validation issues
        var isValidJson = IsValidJson(observationJson);
        isValidJson.Should().BeTrue("Invalid observation example should be valid JSON");
    }

    [Fact]
    public void FhirPackageExamples_ShouldExistAndBeValid()
    {
        // Arrange & Act & Assert
        Directory.Exists(_fhirPackageExamplesPath).Should().BeTrue("FHIR package examples directory should exist");

        var exampleFiles = Directory.GetFiles(_fhirPackageExamplesPath, "*.json");
        exampleFiles.Should().NotBeEmpty("FHIR package should contain example files");

        foreach (var exampleFile in exampleFiles)
        {
            var fileName = Path.GetFileName(exampleFile);
            var exampleJson = File.ReadAllText(exampleFile);

            exampleJson.Should().NotBeNullOrEmpty($"Example {fileName} should have content");

            // Verify it can be parsed as FHIR resource
            var resource = _fhirParser.Parse<Resource>(exampleJson);
            resource.Should().NotBeNull($"Example {fileName} should be valid FHIR resource");
            resource.Id.Should().NotBeNullOrEmpty($"Example {fileName} should have resource ID");
        }
    }

    [Fact]
    public void ExampleResources_ShouldDemonstrateProfileUsage()
    {
        // Arrange
        var testExampleFiles = Directory.GetFiles(_testResourcesPath, "*-valid-*.json");

        // Act & Assert
        testExampleFiles.Should().NotBeEmpty("Test resources should contain valid examples");

        foreach (var exampleFile in testExampleFiles)
        {
            var fileName = Path.GetFileName(exampleFile);
            var exampleJson = File.ReadAllText(exampleFile);
            var resource = _fhirParser.Parse<Resource>(exampleJson);

            // Verify examples demonstrate proper FHIR usage
            resource.Meta.Should().NotBeNull($"Example {fileName} should have meta");

            if (resource.Meta.Profile.Any())
            {
                foreach (var profileUrl in resource.Meta.Profile)
                {
                    profileUrl.Should().StartWith("http://", $"Profile URL in {fileName} should be valid HTTP URL");
                }
            }

            // Verify resource type-specific requirements
            if (resource is Patient patient)
            {
                patient.Identifier.Should().NotBeEmpty($"Patient example {fileName} should have identifiers");
                patient.Name.Should().NotBeEmpty($"Patient example {fileName} should have names");
            }

            if (resource is Observation observation)
            {
                observation.Status.Should().NotBeNull($"Observation example {fileName} should have status");
                observation.Code.Should().NotBeNull($"Observation example {fileName} should have code");
                observation.Subject.Should().NotBeNull($"Observation example {fileName} should have subject");
            }
        }
    }

    [Theory]
    [InlineData("patient-valid-001.json", "Patient", "patient-001")]
    [InlineData("observation-valid-001.json", "Observation", "observation-001")]
    public void ExampleResource_ShouldHaveExpectedPropertiesAndIds(string fileName, string expectedType, string expectedId)
    {
        // Arrange
        var examplePath = Path.Combine(_testResourcesPath, fileName);
        var exampleJson = File.ReadAllText(examplePath);
        var resource = _fhirParser.Parse<Resource>(exampleJson);

        // Act & Assert
        resource.TypeName.Should().Be(expectedType, $"Example {fileName} should be {expectedType} resource");
        resource.Id.Should().Be(expectedId, $"Example {fileName} should have expected ID");
    }

    [Fact]
    public void ExampleResources_ShouldFollowNamingConventions()
    {
        // Arrange
        var testResourceFiles = Directory.GetFiles(_testResourcesPath, "*.json");

        // Act & Assert
        foreach (var resourceFile in testResourceFiles)
        {
            var fileName = Path.GetFileName(resourceFile);

            // Verify naming convention: {resourcetype}-{valid|invalid}-{number}.json
            fileName.Should().MatchRegex(@"^[a-z]+-(?:valid|invalid)-\d{3}\.json$",
                $"File {fileName} should follow naming convention");

            if (fileName.Contains("valid"))
            {
                // Valid examples should parse without exceptions
                var resourceJson = File.ReadAllText(resourceFile);
                var parseAction = () => _fhirParser.Parse<Resource>(resourceJson);
                parseAction.Should().NotThrow($"Valid example {fileName} should parse without errors");
            }
        }
    }

    [Fact]
    public void ExampleResources_ShouldHaveRealisticData()
    {
        // Arrange
        var patientExamplePath = Path.Combine(_testResourcesPath, "patient-valid-001.json");
        var patientJson = File.ReadAllText(patientExamplePath);
        var patient = _fhirParser.Parse<Patient>(patientJson);

        // Act & Assert - Verify realistic data patterns
        patient.BirthDate.Should().NotBeNullOrEmpty("Patient should have birth date");

        // Birth date should be reasonable (not in future, not too old)
        var birthDate = DateTime.Parse(patient.BirthDate);
        birthDate.Should().BeBefore(DateTime.Now, "Birth date should be in the past");
        birthDate.Should().BeAfter(DateTime.Now.AddYears(-150), "Birth date should not be too old");

        // Phone numbers should follow reasonable patterns
        var phoneContact = patient.Telecom.FirstOrDefault(t => t.System == ContactPoint.ContactPointSystem.Phone);
        if (phoneContact != null)
        {
            phoneContact.Value.Should().MatchRegex(@"^\+?\d[\d\s\-\(\)]+$", "Phone number should have realistic format");
        }

        // Email should follow realistic patterns
        var emailContact = patient.Telecom.FirstOrDefault(t => t.System == ContactPoint.ContactPointSystem.Email);
        if (emailContact != null)
        {
            emailContact.Value.Should().MatchRegex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", "Email should have realistic format");
        }
    }

    private bool IsValidJson(string jsonString)
    {
        try
        {
            System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(jsonString);
            return true;
        }
        catch (System.Text.Json.JsonException)
        {
            return false;
        }
    }
}