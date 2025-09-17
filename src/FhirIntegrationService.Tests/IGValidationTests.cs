using Xunit;
using FluentAssertions;
using System.IO;
using System.Text.Json;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using FhirIntegrationService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace FhirIntegrationService.Tests;

/// <summary>
/// Comprehensive tests for Implementation Guide validation and completeness
/// </summary>
public class IGValidationTests
{
    private readonly Mock<ILogger<IGValidationTests>> _mockLogger;
    private readonly FhirJsonParser _fhirParser;
    private readonly string _testResourcesPath;
    private readonly string _fhirPackagePath;
    private readonly string _implementationGuidePath;

    public IGValidationTests()
    {
        _mockLogger = new Mock<ILogger<IGValidationTests>>();
        _fhirParser = new FhirJsonParser();
        _testResourcesPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "tests", "resources");
        _fhirPackagePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "fhir-package");
        _implementationGuidePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "docs", "implementation-guide");
    }

    [Fact]
    public void ImplementationGuide_ShouldExistAndBeAccessible()
    {
        // Arrange
        var igFilePath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");

        // Act & Assert
        File.Exists(igFilePath).Should().BeTrue("Implementation Guide document should exist");

        var content = File.ReadAllText(igFilePath);
        content.Should().NotBeNullOrEmpty("Implementation Guide should have content");
        content.Should().Contain("# HL7 FHIR Integration Expansion Pack - Implementation Guide", "Implementation Guide should have proper title");
        content.Should().Contain("## Version 1.0.0", "Implementation Guide should specify version");
    }

    [Fact]
    public void ImplementationGuide_ShouldContainAllRequiredSections()
    {
        // Arrange
        var igFilePath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
        var content = File.ReadAllText(igFilePath);

        // Act & Assert - Verify all required sections exist
        content.Should().Contain("## Overview", "IG should contain Overview section");
        content.Should().Contain("## Clinical Context", "IG should contain Clinical Context section");
        content.Should().Contain("## Technical Specifications", "IG should contain Technical Specifications section");
        content.Should().Contain("## Security Considerations", "IG should contain Security Considerations section");
        content.Should().Contain("## Implementation Examples", "IG should contain Implementation Examples section");
        content.Should().Contain("## Testing and Validation", "IG should contain Testing and Validation section");
        content.Should().Contain("## Support and Resources", "IG should contain Support and Resources section");
    }

    [Fact]
    public void ImplementationGuide_ShouldContainClinicalContent()
    {
        // Arrange
        var igFilePath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
        var content = File.ReadAllText(igFilePath);

        // Act & Assert - Verify clinical content is included
        content.Should().Contain("Clinical Teams", "IG should address clinical teams as stakeholders");
        content.Should().Contain("Patient resource", "IG should reference Patient resource");
        content.Should().Contain("Observation resource", "IG should reference Observation resource");
        content.Should().Contain("healthcare organizations", "IG should address healthcare organizations");
        content.Should().Contain("clinical workflow", "IG should describe clinical workflows");
    }

    [Fact]
    public void ImplementationGuide_ShouldContainTechnicalSpecifications()
    {
        // Arrange
        var igFilePath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
        var content = File.ReadAllText(igFilePath);

        // Act & Assert - Verify technical content is included
        content.Should().Contain("FHIR R4", "IG should specify FHIR version");
        content.Should().Contain("API Endpoints", "IG should document API endpoints");
        content.Should().Contain("/fhir/Patient", "IG should specify Patient endpoint");
        content.Should().Contain("GET", "IG should document GET operations");
        content.Should().Contain("POST", "IG should document POST operations");
        content.Should().Contain("application/fhir+json", "IG should specify FHIR content type");
    }

    [Fact]
    public void ImplementationGuide_ShouldContainSecurityGuidance()
    {
        // Arrange
        var igFilePath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
        var content = File.ReadAllText(igFilePath);

        // Act & Assert - Verify security content is included
        content.Should().Contain("SMART on FHIR", "IG should reference SMART on FHIR");
        content.Should().Contain("OAuth 2.0", "IG should reference OAuth 2.0");
        content.Should().Contain("TLS", "IG should reference TLS encryption");
        content.Should().Contain("PHI", "IG should address PHI handling");
        content.Should().Contain("audit", "IG should address audit logging");
        content.Should().Contain("HIPAA", "IG should reference HIPAA compliance");
    }

    [Fact]
    public void ImplementationGuide_ShouldContainCodeExamples()
    {
        // Arrange
        var igFilePath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
        var content = File.ReadAllText(igFilePath);

        // Act & Assert - Verify code examples are included
        content.Should().Contain("```csharp", "IG should contain C# code examples");
        content.Should().Contain("```http", "IG should contain HTTP request examples");
        content.Should().Contain("```json", "IG should contain JSON examples");
        content.Should().Contain("DataMappingService", "IG should include service implementation examples");
        content.Should().Contain("FhirValidationService", "IG should include validation service examples");
    }

    [Fact]
    public void ProjectArtifacts_ShouldExistAndBeComplete()
    {
        // Arrange & Act & Assert
        var clinicalRequirementsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "docs", "artifacts", "clinical-requirements-summary.md");
        File.Exists(clinicalRequirementsPath).Should().BeTrue("Clinical requirements summary should exist");

        var profilesInventoryPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "docs", "artifacts", "fhir-profiles-inventory.md");
        File.Exists(profilesInventoryPath).Should().BeTrue("FHIR profiles inventory should exist");

        var implementationEvidencePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "docs", "artifacts", "implementation-evidence.md");
        File.Exists(implementationEvidencePath).Should().BeTrue("Implementation evidence should exist");
    }

    [Fact]
    public void FhirPackage_ShouldHaveValidManifest()
    {
        // Arrange
        var packageJsonPath = Path.Combine(_fhirPackagePath, "package.json");

        // Act & Assert
        File.Exists(packageJsonPath).Should().BeTrue("FHIR package manifest should exist");

        var packageJson = File.ReadAllText(packageJsonPath);
        var package = JsonSerializer.Deserialize<JsonElement>(packageJson);

        package.GetProperty("name").GetString().Should().Be("hl7-fhir-expansion-pack");
        package.GetProperty("version").GetString().Should().Be("1.0.0");
        package.GetProperty("fhirVersion").GetString().Should().Be("4.0.1");
        package.GetProperty("canonical").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void FhirPackage_ShouldContainRequiredDirectories()
    {
        // Arrange & Act & Assert
        Directory.Exists(Path.Combine(_fhirPackagePath, "profiles")).Should().BeTrue("Profiles directory should exist");
        Directory.Exists(Path.Combine(_fhirPackagePath, "examples")).Should().BeTrue("Examples directory should exist");
        Directory.Exists(Path.Combine(_fhirPackagePath, "valuesets")).Should().BeTrue("ValueSets directory should exist");
        Directory.Exists(Path.Combine(_fhirPackagePath, "extensions")).Should().BeTrue("Extensions directory should exist");
    }

    [Fact]
    public void FhirPackage_ShouldContainValidExampleResources()
    {
        // Arrange
        var examplesPath = Path.Combine(_fhirPackagePath, "examples");
        var patientExamplePath = Path.Combine(examplesPath, "patient-valid-001.json");
        var observationExamplePath = Path.Combine(examplesPath, "observation-valid-001.json");

        // Act & Assert
        File.Exists(patientExamplePath).Should().BeTrue("Patient example should exist in FHIR package");
        File.Exists(observationExamplePath).Should().BeTrue("Observation example should exist in FHIR package");

        // Validate Patient example can be parsed
        var patientJson = File.ReadAllText(patientExamplePath);
        var patientResource = _fhirParser.Parse<Patient>(patientJson);
        patientResource.Should().NotBeNull("Patient example should be valid FHIR");
        patientResource.ResourceType.Should().Be(ResourceType.Patient);

        // Validate Observation example can be parsed
        var observationJson = File.ReadAllText(observationExamplePath);
        var observationResource = _fhirParser.Parse<Observation>(observationJson);
        observationResource.Should().NotBeNull("Observation example should be valid FHIR");
        observationResource.ResourceType.Should().Be(ResourceType.Observation);
    }

    [Fact]
    public void PublicationDocuments_ShouldExistAndBeComplete()
    {
        // Arrange
        var publicationPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "docs", "publication");

        // Act & Assert
        var simplifierGuidePath = Path.Combine(publicationPath, "simplifier-publication-guide.md");
        File.Exists(simplifierGuidePath).Should().BeTrue("Simplifier publication guide should exist");

        var partnerAccessPath = Path.Combine(publicationPath, "external-partner-access.md");
        File.Exists(partnerAccessPath).Should().BeTrue("External partner access strategy should exist");

        var checklistPath = Path.Combine(publicationPath, "publication-checklist.md");
        File.Exists(checklistPath).Should().BeTrue("Publication checklist should exist");

        // Verify content quality
        var simplifierContent = File.ReadAllText(simplifierGuidePath);
        simplifierContent.Should().Contain("Simplifier.net", "Publication guide should reference Simplifier.net");
        simplifierContent.Should().Contain("step-by-step", "Publication guide should provide detailed steps");

        var partnerContent = File.ReadAllText(partnerAccessPath);
        partnerContent.Should().Contain("Healthcare Organizations", "Partner access should address healthcare organizations");
        partnerContent.Should().Contain("EHR vendors", "Partner access should address EHR vendors");

        var checklistContent = File.ReadAllText(checklistPath);
        checklistContent.Should().Contain("- [ ]", "Checklist should contain actionable items");
        checklistContent.Should().Contain("validation", "Checklist should include validation steps");
    }

    [Fact]
    public void ImplementationGuide_ShouldMeetQualityStandards()
    {
        // Arrange
        var igFilePath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
        var content = File.ReadAllText(igFilePath);

        // Act & Assert - Quality standards verification
        content.Length.Should().BeGreaterThan(10000, "IG should be comprehensive (>10K characters)");
        content.Should().NotContain("TODO", "IG should not contain TODO items");
        content.Should().NotContain("FIXME", "IG should not contain FIXME items");
        content.Should().NotContain("TBD", "IG should not contain TBD items");

        // Verify proper markdown structure
        content.Should().Contain("# HL7 FHIR Integration Expansion Pack", "IG should have proper H1 title");
        content.Should().Contain("## Table of Contents", "IG should have table of contents");

        // Count sections to ensure comprehensive coverage
        var sectionCount = content.Split("## ").Length - 1;
        sectionCount.Should().BeGreaterOrEqualTo(7, "IG should have at least 7 major sections");
    }

    [Fact]
    public void AuthorImplementationGuideTask_ShouldExistAndBeComplete()
    {
        // Arrange
        var taskPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", ".bmad-core", "tasks", "author-implementation-guide.md");

        // Act & Assert
        File.Exists(taskPath).Should().BeTrue("author-implementation-guide task should exist");

        var taskContent = File.ReadAllText(taskPath);
        taskContent.Should().Contain("Author Implementation Guide", "Task should have proper title");
        taskContent.Should().Contain("EXECUTABLE WORKFLOW", "Task should be marked as executable workflow");
        taskContent.Should().Contain("Simplifier.net", "Task should reference Simplifier.net publication");
        taskContent.Should().Contain("semantic versioning", "Task should reference semantic versioning");
        taskContent.Should().Contain("external partners", "Task should address external partners");
    }

    [Theory]
    [InlineData("clinical-requirements-summary.md")]
    [InlineData("fhir-profiles-inventory.md")]
    [InlineData("implementation-evidence.md")]
    public void ArtifactDocuments_ShouldContainRequiredContent(string fileName)
    {
        // Arrange
        var artifactPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "docs", "artifacts", fileName);

        // Act & Assert
        File.Exists(artifactPath).Should().BeTrue($"Artifact {fileName} should exist");

        var content = File.ReadAllText(artifactPath);
        content.Should().NotBeNullOrEmpty($"Artifact {fileName} should have content");
        content.Should().Contain("FHIR", $"Artifact {fileName} should reference FHIR");
        content.Should().Contain("Implementation Guide", $"Artifact {fileName} should reference Implementation Guide");
        content.Length.Should().BeGreaterThan(1000, $"Artifact {fileName} should be comprehensive");
    }

    [Fact]
    public void CanonicalUrls_ShouldBeConsistentAcrossResources()
    {
        // Arrange
        var baseCanonicalUrl = "http://example.org/fhir/ig/hl7-fhir-expansion-pack";

        // Act & Assert - Check package.json
        var packageJsonPath = Path.Combine(_fhirPackagePath, "package.json");
        var packageJson = File.ReadAllText(packageJsonPath);
        packageJson.Should().Contain(baseCanonicalUrl, "Package manifest should use consistent canonical URL");

        // Check Implementation Guide
        var igFilePath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
        var igContent = File.ReadAllText(igFilePath);
        igContent.Should().Contain("example.org", "IG should reference consistent domain");

        // Check if any profile files exist and validate their canonical URLs
        var profilesPath = Path.Combine(_fhirPackagePath, "profiles");
        if (Directory.Exists(profilesPath))
        {
            var profileFiles = Directory.GetFiles(profilesPath, "*.json");
            foreach (var profileFile in profileFiles)
            {
                var profileContent = File.ReadAllText(profileFile);
                profileContent.Should().Contain("http://example.org/fhir", "Profile should use consistent canonical URL base");
            }
        }
    }

    [Fact]
    public void VersionNumbers_ShouldBeConsistentAcrossDocuments()
    {
        // Arrange
        var expectedVersion = "1.0.0";

        // Act & Assert
        var packageJsonPath = Path.Combine(_fhirPackagePath, "package.json");
        var packageJson = File.ReadAllText(packageJsonPath);
        packageJson.Should().Contain($"\"version\": \"{expectedVersion}\"", "Package should use consistent version");

        var igFilePath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
        var igContent = File.ReadAllText(igFilePath);
        igContent.Should().Contain($"## Version {expectedVersion}", "IG should use consistent version");
    }
}