using Xunit;
using FluentAssertions;
using System.IO;
using System.Text.Json;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;

namespace FhirIntegrationService.Tests;

/// <summary>
/// Tests for FHIR profile compliance and validation
/// </summary>
public class ProfileValidationTests
{
    private readonly Mock<ILogger<ProfileValidationTests>> _mockLogger;
    private readonly FhirJsonParser _fhirParser;
    private readonly FhirJsonSerializer _fhirSerializer;
    private readonly string _fhirPackagePath;
    private readonly string _testResourcesPath;

    public ProfileValidationTests()
    {
        _mockLogger = new Mock<ILogger<ProfileValidationTests>>();
        _fhirParser = new FhirJsonParser();
        _fhirSerializer = new FhirJsonSerializer();
        _fhirPackagePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "fhir-package");
        _testResourcesPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "tests", "resources");
    }

    [Fact]
    public void PatientProfile_ShouldBeValidStructureDefinition()
    {
        // Arrange
        var patientProfilePath = Path.Combine(_fhirPackagePath, "profiles", "Patient.json");

        // Act & Assert
        File.Exists(patientProfilePath).Should().BeTrue("Patient profile should exist");

        var profileJson = File.ReadAllText(patientProfilePath);
        var profile = _fhirParser.Parse<StructureDefinition>(profileJson);

        profile.Should().NotBeNull("Patient profile should be valid StructureDefinition");
        profile.ResourceType.Should().Be(ResourceType.StructureDefinition);
        profile.Id.Should().NotBeNullOrEmpty("Profile should have ID");
        profile.Url.Should().NotBeNullOrEmpty("Profile should have canonical URL");
        profile.Name.Should().NotBeNullOrEmpty("Profile should have name");
        profile.Status.Should().Be(PublicationStatus.Active, "Profile should be active");
        profile.FhirVersion.Should().Be("4.0.1", "Profile should be FHIR R4");
        profile.Kind.Should().Be(StructureDefinition.StructureDefinitionKind.Resource, "Profile should be resource profile");
        profile.Type.Should().Be("Patient", "Profile should be Patient type");
        profile.BaseDefinition.Should().Be("http://hl7.org/fhir/StructureDefinition/Patient", "Profile should derive from base Patient");
    }

    [Fact]
    public void PatientProfile_ShouldHaveRequiredElements()
    {
        // Arrange
        var patientProfilePath = Path.Combine(_fhirPackagePath, "profiles", "Patient.json");
        var profileJson = File.ReadAllText(patientProfilePath);
        var profile = _fhirParser.Parse<StructureDefinition>(profileJson);

        // Act & Assert
        profile.Differential.Should().NotBeNull("Profile should have differential");
        profile.Differential.Element.Should().NotBeEmpty("Profile should have differential elements");

        // Verify key constraints
        var identifierElement = profile.Differential.Element.FirstOrDefault(e => e.Path == "Patient.identifier");
        identifierElement.Should().NotBeNull("Profile should constrain identifier");
        identifierElement.Min.Should().Be(1, "Identifier should be required");
        identifierElement.MustSupport.Should().BeTrue("Identifier should be must support");

        var activeElement = profile.Differential.Element.FirstOrDefault(e => e.Path == "Patient.active");
        activeElement.Should().NotBeNull("Profile should constrain active");
        activeElement.Min.Should().Be(1, "Active should be required");

        var nameElement = profile.Differential.Element.FirstOrDefault(e => e.Path == "Patient.name");
        nameElement.Should().NotBeNull("Profile should constrain name");
        nameElement.Min.Should().Be(1, "Name should be required");
    }

    [Fact]
    public void PatientProfile_ShouldHaveMrnSlice()
    {
        // Arrange
        var patientProfilePath = Path.Combine(_fhirPackagePath, "profiles", "Patient.json");
        var profileJson = File.ReadAllText(patientProfilePath);
        var profile = _fhirParser.Parse<StructureDefinition>(profileJson);

        // Act & Assert
        var mrnSlice = profile.Differential.Element.FirstOrDefault(e => e.Path == "Patient.identifier" && e.SliceName == "mrn");
        mrnSlice.Should().NotBeNull("Profile should have MRN identifier slice");

        var mrnUseElement = profile.Differential.Element.FirstOrDefault(e => e.Path == "Patient.identifier.use" && e.SliceName == null);
        if (mrnUseElement?.Fixed != null)
        {
            var fixedUse = mrnUseElement.Fixed as Code;
            fixedUse?.Value.Should().Be("usual", "MRN use should be fixed to 'usual'");
        }

        var mrnTypeElement = profile.Differential.Element.FirstOrDefault(e => e.Path == "Patient.identifier.type");
        mrnTypeElement.Should().NotBeNull("MRN slice should constrain type");
    }

    [Fact]
    public void PatientProfile_ShouldHaveValidCanonicalUrl()
    {
        // Arrange
        var patientProfilePath = Path.Combine(_fhirPackagePath, "profiles", "Patient.json");
        var profileJson = File.ReadAllText(patientProfilePath);
        var profile = _fhirParser.Parse<StructureDefinition>(profileJson);

        // Act & Assert
        profile.Url.Should().StartWith("http://example.org/fhir/StructureDefinition/", "Profile URL should use consistent base");
        profile.Url.Should().Contain("patient", "Profile URL should identify resource type");

        // Verify URL is well-formed
        var isValidUrl = Uri.TryCreate(profile.Url, UriKind.Absolute, out var uri);
        isValidUrl.Should().BeTrue("Profile URL should be valid URI");
        uri.Scheme.Should().Be("http", "Profile URL should use HTTP scheme");
    }

    [Fact]
    public void IntegrationSystemTypesValueSet_ShouldBeValidValueSet()
    {
        // Arrange
        var valueSetPath = Path.Combine(_fhirPackagePath, "valuesets", "IntegrationSystemTypes.json");

        // Act & Assert
        File.Exists(valueSetPath).Should().BeTrue("Integration system types value set should exist");

        var valueSetJson = File.ReadAllText(valueSetPath);
        var valueSet = _fhirParser.Parse<ValueSet>(valueSetJson);

        valueSet.Should().NotBeNull("Value set should be valid ValueSet resource");
        valueSet.ResourceType.Should().Be(ResourceType.ValueSet);
        valueSet.Id.Should().NotBeNullOrEmpty("Value set should have ID");
        valueSet.Url.Should().NotBeNullOrEmpty("Value set should have canonical URL");
        valueSet.Name.Should().NotBeNullOrEmpty("Value set should have name");
        valueSet.Status.Should().Be(PublicationStatus.Active, "Value set should be active");
        valueSet.Compose.Should().NotBeNull("Value set should have compose");
        valueSet.Compose.Include.Should().NotBeEmpty("Value set should include concepts");
    }

    [Fact]
    public void IntegrationSystemTypesValueSet_ShouldHaveValidConcepts()
    {
        // Arrange
        var valueSetPath = Path.Combine(_fhirPackagePath, "valuesets", "IntegrationSystemTypes.json");
        var valueSetJson = File.ReadAllText(valueSetPath);
        var valueSet = _fhirParser.Parse<ValueSet>(valueSetJson);

        // Act & Assert
        var include = valueSet.Compose.Include.First();
        include.Concept.Should().NotBeEmpty("Value set should have concepts");

        var expectedConcepts = new[] { "EHR", "LIS", "RIS", "PACS", "HIE", "CDSS", "PM", "PHARMACY" };
        var actualCodes = include.Concept.Select(c => c.Code).ToList();

        foreach (var expectedCode in expectedConcepts)
        {
            actualCodes.Should().Contain(expectedCode, $"Value set should contain {expectedCode} concept");
        }

        // Verify each concept has display and definition
        foreach (var concept in include.Concept)
        {
            concept.Code.Should().NotBeNullOrEmpty("Concept should have code");
            concept.Display.Should().NotBeNullOrEmpty("Concept should have display");
            concept.Definition.Should().NotBeNullOrEmpty("Concept should have definition");
        }
    }

    [Fact]
    public void PatientExample_ShouldValidateAgainstBaseProfile()
    {
        // Arrange
        var patientExamplePath = Path.Combine(_testResourcesPath, "patient-valid-001.json");
        File.Exists(patientExamplePath).Should().BeTrue("Patient example should exist");

        var patientJson = File.ReadAllText(patientExamplePath);
        var patient = _fhirParser.Parse<Patient>(patientJson);

        // Act & Assert
        patient.Should().NotBeNull("Patient example should be valid FHIR");
        patient.ResourceType.Should().Be(ResourceType.Patient);
        patient.Id.Should().NotBeNullOrEmpty("Patient should have ID");
        patient.Active.Should().BeTrue("Patient should be active");
        patient.Name.Should().NotBeEmpty("Patient should have name");
        patient.Identifier.Should().NotBeEmpty("Patient should have identifier");

        // Verify required elements are present
        var mrn = patient.Identifier.FirstOrDefault(i =>
            i.Type?.Coding?.Any(c => c.Code == "MR") == true);
        mrn.Should().NotBeNull("Patient should have MRN identifier");
        mrn.Value.Should().NotBeNullOrEmpty("MRN should have value");
        mrn.System.Should().NotBeNullOrEmpty("MRN should have system");
    }

    [Fact]
    public void ObservationExample_ShouldValidateAgainstBaseProfile()
    {
        // Arrange
        var observationExamplePath = Path.Combine(_testResourcesPath, "observation-valid-001.json");
        File.Exists(observationExamplePath).Should().BeTrue("Observation example should exist");

        var observationJson = File.ReadAllText(observationExamplePath);
        var observation = _fhirParser.Parse<Observation>(observationJson);

        // Act & Assert
        observation.Should().NotBeNull("Observation example should be valid FHIR");
        observation.ResourceType.Should().Be(ResourceType.Observation);
        observation.Id.Should().NotBeNullOrEmpty("Observation should have ID");
        observation.Status.Should().Be(ObservationStatus.Final, "Observation should have final status");
        observation.Code.Should().NotBeNull("Observation should have code");
        observation.Subject.Should().NotBeNull("Observation should have subject");

        // Verify LOINC coding
        var loincCoding = observation.Code.Coding.FirstOrDefault(c => c.System == "http://loinc.org");
        loincCoding.Should().NotBeNull("Observation should have LOINC coding");
        loincCoding.Code.Should().NotBeNullOrEmpty("LOINC coding should have code");
        loincCoding.Display.Should().NotBeNullOrEmpty("LOINC coding should have display");

        // Verify vital signs category
        var vitalSignsCategory = observation.Category.FirstOrDefault(c =>
            c.Coding.Any(coding => coding.Code == "vital-signs"));
        vitalSignsCategory.Should().NotBeNull("Observation should have vital-signs category");
    }

    [Fact]
    public void FhirProfiles_ShouldHaveConsistentMetadata()
    {
        // Arrange
        var profilesPath = Path.Combine(_fhirPackagePath, "profiles");
        if (!Directory.Exists(profilesPath))
        {
            // Skip test if profiles directory doesn't exist
            return;
        }

        var profileFiles = Directory.GetFiles(profilesPath, "*.json");

        // Act & Assert
        foreach (var profileFile in profileFiles)
        {
            var profileJson = File.ReadAllText(profileFile);
            var profile = _fhirParser.Parse<StructureDefinition>(profileJson);

            var fileName = Path.GetFileName(profileFile);

            // Verify consistent metadata
            profile.Version.Should().NotBeNullOrEmpty($"Profile {fileName} should have version");
            profile.Date.Should().NotBeNullOrEmpty($"Profile {fileName} should have date");
            profile.Publisher.Should().NotBeNullOrEmpty($"Profile {fileName} should have publisher");
            profile.Contact.Should().NotBeEmpty($"Profile {fileName} should have contact information");
            profile.Description.Should().NotBeNullOrEmpty($"Profile {fileName} should have description");

            // Verify FHIR version consistency
            profile.FhirVersion.Should().Be("4.0.1", $"Profile {fileName} should use FHIR R4");

            // Verify experimental flag
            profile.Experimental.Should().BeFalse($"Profile {fileName} should not be experimental");

            // Verify jurisdiction
            profile.Jurisdiction.Should().NotBeEmpty($"Profile {fileName} should have jurisdiction");
        }
    }

    [Fact]
    public void FhirValueSets_ShouldHaveConsistentMetadata()
    {
        // Arrange
        var valuesetsPath = Path.Combine(_fhirPackagePath, "valuesets");
        if (!Directory.Exists(valuesetsPath))
        {
            // Skip test if valuesets directory doesn't exist
            return;
        }

        var valuesetFiles = Directory.GetFiles(valuesetsPath, "*.json");

        // Act & Assert
        foreach (var valuesetFile in valuesetFiles)
        {
            var valuesetJson = File.ReadAllText(valuesetFile);
            var valueset = _fhirParser.Parse<ValueSet>(valuesetJson);

            var fileName = Path.GetFileName(valuesetFile);

            // Verify consistent metadata
            valueset.Version.Should().NotBeNullOrEmpty($"ValueSet {fileName} should have version");
            valueset.Date.Should().NotBeNullOrEmpty($"ValueSet {fileName} should have date");
            valueset.Publisher.Should().NotBeNullOrEmpty($"ValueSet {fileName} should have publisher");
            valueset.Contact.Should().NotBeEmpty($"ValueSet {fileName} should have contact information");
            valueset.Description.Should().NotBeNullOrEmpty($"ValueSet {fileName} should have description");

            // Verify experimental flag
            valueset.Experimental.Should().BeFalse($"ValueSet {fileName} should not be experimental");

            // Verify jurisdiction
            valueset.Jurisdiction.Should().NotBeEmpty($"ValueSet {fileName} should have jurisdiction");

            // Verify immutable flag is set
            valueset.Immutable.Should().BeFalse($"ValueSet {fileName} should be mutable for updates");
        }
    }

    [Fact]
    public void ExampleResources_ShouldHaveValidMetadata()
    {
        // Arrange
        var examplesPath = Path.Combine(_fhirPackagePath, "examples");
        var exampleFiles = Directory.GetFiles(examplesPath, "*.json");

        // Act & Assert
        exampleFiles.Should().NotBeEmpty("Examples directory should contain example files");

        foreach (var exampleFile in exampleFiles)
        {
            var exampleJson = File.ReadAllText(exampleFile);
            var resource = _fhirParser.Parse<Resource>(exampleJson);

            var fileName = Path.GetFileName(exampleFile);

            // Verify basic resource properties
            resource.Should().NotBeNull($"Example {fileName} should be valid FHIR resource");
            resource.Id.Should().NotBeNullOrEmpty($"Example {fileName} should have ID");
            resource.Meta.Should().NotBeNull($"Example {fileName} should have meta");
            resource.Meta.LastUpdated.Should().NotBeNull($"Example {fileName} should have lastUpdated");

            // Verify profile references if present
            if (resource.Meta.Profile.Any())
            {
                foreach (var profileUrl in resource.Meta.Profile)
                {
                    profileUrl.Should().StartWith("http://", $"Profile URL in {fileName} should be valid HTTP URL");
                }
            }
        }
    }

    [Theory]
    [InlineData("patient-valid-001.json", "Patient")]
    [InlineData("observation-valid-001.json", "Observation")]
    public void ExampleResource_ShouldMatchExpectedResourceType(string fileName, string expectedResourceType)
    {
        // Arrange
        var examplePath = Path.Combine(_testResourcesPath, fileName);
        if (!File.Exists(examplePath))
        {
            // Try FHIR package examples directory
            examplePath = Path.Combine(_fhirPackagePath, "examples", fileName);
        }

        File.Exists(examplePath).Should().BeTrue($"Example {fileName} should exist");

        // Act
        var exampleJson = File.ReadAllText(examplePath);
        var resource = _fhirParser.Parse<Resource>(exampleJson);

        // Assert
        resource.TypeName.Should().Be(expectedResourceType, $"Example {fileName} should be {expectedResourceType} resource");
    }

    [Fact]
    public void ProfileCanonicalUrls_ShouldBeConsistent()
    {
        // Arrange
        var packageJsonPath = Path.Combine(_fhirPackagePath, "package.json");
        var packageContent = File.ReadAllText(packageJsonPath);
        var package = JsonSerializer.Deserialize<JsonElement>(packageContent);

        var baseCanonicalUrl = package.GetProperty("canonical").GetString();

        // Act & Assert
        var profilesPath = Path.Combine(_fhirPackagePath, "profiles");
        if (Directory.Exists(profilesPath))
        {
            var profileFiles = Directory.GetFiles(profilesPath, "*.json");
            foreach (var profileFile in profileFiles)
            {
                var profileJson = File.ReadAllText(profileFile);
                var profile = _fhirParser.Parse<StructureDefinition>(profileJson);

                profile.Url.Should().StartWith("http://example.org/fhir/StructureDefinition/",
                    $"Profile {Path.GetFileName(profileFile)} should use consistent canonical URL base");
            }
        }

        var valuesetsPath = Path.Combine(_fhirPackagePath, "valuesets");
        if (Directory.Exists(valuesetsPath))
        {
            var valuesetFiles = Directory.GetFiles(valuesetsPath, "*.json");
            foreach (var valuesetFile in valuesetFiles)
            {
                var valuesetJson = File.ReadAllText(valuesetFile);
                var valueset = _fhirParser.Parse<ValueSet>(valuesetJson);

                valueset.Url.Should().StartWith("http://example.org/fhir/ValueSet/",
                    $"ValueSet {Path.GetFileName(valuesetFile)} should use consistent canonical URL base");
            }
        }
    }

    [Fact]
    public void FhirResources_ShouldSerializeAndDeserializeCorrectly()
    {
        // Arrange
        var allResourcePaths = new List<string>();

        // Add profiles
        var profilesPath = Path.Combine(_fhirPackagePath, "profiles");
        if (Directory.Exists(profilesPath))
        {
            allResourcePaths.AddRange(Directory.GetFiles(profilesPath, "*.json"));
        }

        // Add valuesets
        var valuesetsPath = Path.Combine(_fhirPackagePath, "valuesets");
        if (Directory.Exists(valuesetsPath))
        {
            allResourcePaths.AddRange(Directory.GetFiles(valuesetsPath, "*.json"));
        }

        // Add examples
        var examplesPath = Path.Combine(_fhirPackagePath, "examples");
        if (Directory.Exists(examplesPath))
        {
            allResourcePaths.AddRange(Directory.GetFiles(examplesPath, "*.json"));
        }

        // Act & Assert
        foreach (var resourcePath in allResourcePaths)
        {
            var fileName = Path.GetFileName(resourcePath);
            var originalJson = File.ReadAllText(resourcePath);

            // Parse resource
            var resource = _fhirParser.Parse<Resource>(originalJson);
            resource.Should().NotBeNull($"Resource {fileName} should parse successfully");

            // Serialize back to JSON
            var serializedJson = _fhirSerializer.SerializeToString(resource);
            serializedJson.Should().NotBeNullOrEmpty($"Resource {fileName} should serialize successfully");

            // Parse serialized JSON to verify round-trip
            var reparsedResource = _fhirParser.Parse<Resource>(serializedJson);
            reparsedResource.Should().NotBeNull($"Resource {fileName} should reparse successfully");
            reparsedResource.TypeName.Should().Be(resource.TypeName, $"Resource type should be preserved for {fileName}");
        }
    }
}