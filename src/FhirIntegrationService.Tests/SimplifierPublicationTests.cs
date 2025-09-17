using Xunit;
using FluentAssertions;
using System.IO;
using System.Text.Json;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;

namespace FhirIntegrationService.Tests;

/// <summary>
/// Tests for Simplifier.net publication process validation
/// </summary>
public class SimplifierPublicationTests
{
    private readonly Mock<ILogger<SimplifierPublicationTests>> _mockLogger;
    private readonly string _fhirPackagePath;
    private readonly string _publicationDocsPath;

    public SimplifierPublicationTests()
    {
        _mockLogger = new Mock<ILogger<SimplifierPublicationTests>>();
        _fhirPackagePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "fhir-package");
        _publicationDocsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "docs", "publication");
    }

    [Fact]
    public void FhirPackage_ShouldHaveValidPackageManifest()
    {
        // Arrange
        var packageJsonPath = Path.Combine(_fhirPackagePath, "package.json");

        // Act & Assert
        File.Exists(packageJsonPath).Should().BeTrue("Package manifest should exist");

        var packageContent = File.ReadAllText(packageJsonPath);
        var package = JsonSerializer.Deserialize<JsonElement>(packageContent);

        // Validate required manifest fields
        package.TryGetProperty("name", out var name).Should().BeTrue("Package should have name");
        name.GetString().Should().Be("hl7-fhir-expansion-pack", "Package name should match project");

        package.TryGetProperty("version", out var version).Should().BeTrue("Package should have version");
        version.GetString().Should().MatchRegex(@"^\d+\.\d+\.\d+$", "Version should follow semantic versioning");

        package.TryGetProperty("fhirVersion", out var fhirVersion).Should().BeTrue("Package should specify FHIR version");
        fhirVersion.GetString().Should().Be("4.0.1", "Package should use FHIR R4");

        package.TryGetProperty("canonical", out var canonical).Should().BeTrue("Package should have canonical URL");
        canonical.GetString().Should().StartWith("http://", "Canonical URL should be valid HTTP URL");

        package.TryGetProperty("type", out var type).Should().BeTrue("Package should have type");
        type.GetString().Should().Be("fhir.ig", "Package should be marked as implementation guide");
    }

    [Fact]
    public void FhirPackage_ShouldHaveCompleteMetadata()
    {
        // Arrange
        var packageJsonPath = Path.Combine(_fhirPackagePath, "package.json");
        var packageContent = File.ReadAllText(packageJsonPath);
        var package = JsonSerializer.Deserialize<JsonElement>(packageContent);

        // Act & Assert - Validate comprehensive metadata
        package.TryGetProperty("description", out var description).Should().BeTrue("Package should have description");
        description.GetString().Should().NotBeNullOrEmpty("Description should not be empty");
        description.GetString().Should().Contain("FHIR", "Description should mention FHIR");

        package.TryGetProperty("license", out var license).Should().BeTrue("Package should have license");
        license.GetString().Should().NotBeNullOrEmpty("License should be specified");

        package.TryGetProperty("author", out var author).Should().BeTrue("Package should have author");
        author.GetString().Should().NotBeNullOrEmpty("Author should be specified");

        package.TryGetProperty("keywords", out var keywords).Should().BeTrue("Package should have keywords");
        keywords.GetArrayLength().Should().BeGreaterThan(0, "Keywords should not be empty");

        package.TryGetProperty("homepage", out var homepage).Should().BeTrue("Package should have homepage");
        homepage.GetString().Should().StartWith("http", "Homepage should be valid URL");
    }

    [Fact]
    public void FhirPackage_ShouldHaveValidDependencies()
    {
        // Arrange
        var packageJsonPath = Path.Combine(_fhirPackagePath, "package.json");
        var packageContent = File.ReadAllText(packageJsonPath);
        var package = JsonSerializer.Deserialize<JsonElement>(packageContent);

        // Act & Assert
        package.TryGetProperty("dependencies", out var dependencies).Should().BeTrue("Package should have dependencies");

        dependencies.TryGetProperty("hl7.fhir.r4.core", out var fhirCore).Should().BeTrue("Package should depend on FHIR R4 core");
        fhirCore.GetString().Should().Be("4.0.1", "FHIR core dependency should be R4");

        dependencies.TryGetProperty("hl7.fhir.us.core", out var usCore).Should().BeTrue("Package should depend on US Core");
        usCore.GetString().Should().NotBeNullOrEmpty("US Core version should be specified");
    }

    [Fact]
    public void FhirPackage_ShouldHaveSimplifierMetadata()
    {
        // Arrange
        var packageJsonPath = Path.Combine(_fhirPackagePath, "package.json");
        var packageContent = File.ReadAllText(packageJsonPath);
        var package = JsonSerializer.Deserialize<JsonElement>(packageContent);

        // Act & Assert
        package.TryGetProperty("simplifierMetadata", out var simplifierMeta).Should().BeTrue("Package should have Simplifier metadata");

        simplifierMeta.TryGetProperty("projectId", out var projectId).Should().BeTrue("Simplifier metadata should have project ID");
        projectId.GetString().Should().NotBeNullOrEmpty("Project ID should not be empty");

        simplifierMeta.TryGetProperty("visibility", out var visibility).Should().BeTrue("Simplifier metadata should specify visibility");
        visibility.GetString().Should().Be("public", "Implementation guide should be public");

        simplifierMeta.TryGetProperty("allowedToPublish", out var allowedToPublish).Should().BeTrue("Simplifier metadata should specify publication permission");
        allowedToPublish.GetBoolean().Should().BeTrue("Package should be allowed to publish");

        simplifierMeta.TryGetProperty("tags", out var tags).Should().BeTrue("Simplifier metadata should have tags");
        tags.GetArrayLength().Should().BeGreaterThan(0, "Tags should not be empty");
    }

    [Fact]
    public void FhirPackage_ShouldHaveValidFileStructure()
    {
        // Arrange & Act & Assert
        Directory.Exists(_fhirPackagePath).Should().BeTrue("FHIR package directory should exist");

        // Verify required directories exist
        Directory.Exists(Path.Combine(_fhirPackagePath, "profiles")).Should().BeTrue("Profiles directory should exist");
        Directory.Exists(Path.Combine(_fhirPackagePath, "examples")).Should().BeTrue("Examples directory should exist");
        Directory.Exists(Path.Combine(_fhirPackagePath, "valuesets")).Should().BeTrue("ValueSets directory should exist");
        Directory.Exists(Path.Combine(_fhirPackagePath, "extensions")).Should().BeTrue("Extensions directory should exist");

        // Verify manifest exists
        File.Exists(Path.Combine(_fhirPackagePath, "package.json")).Should().BeTrue("Package manifest should exist");
    }

    [Fact]
    public void FhirPackage_ShouldContainValidJsonFiles()
    {
        // Arrange
        var profilesPath = Path.Combine(_fhirPackagePath, "profiles");
        var examplesPath = Path.Combine(_fhirPackagePath, "examples");
        var valuesetsPath = Path.Combine(_fhirPackagePath, "valuesets");

        // Act & Assert - Check profiles
        if (Directory.Exists(profilesPath))
        {
            var profileFiles = Directory.GetFiles(profilesPath, "*.json");
            foreach (var profileFile in profileFiles)
            {
                var profileContent = File.ReadAllText(profileFile);
                var isValidJson = IsValidJson(profileContent);
                isValidJson.Should().BeTrue($"Profile file {Path.GetFileName(profileFile)} should contain valid JSON");

                var profile = JsonSerializer.Deserialize<JsonElement>(profileContent);
                profile.TryGetProperty("resourceType", out var resourceType).Should().BeTrue("Profile should have resourceType");
                resourceType.GetString().Should().Be("StructureDefinition", "Profile should be StructureDefinition");
            }
        }

        // Check examples
        if (Directory.Exists(examplesPath))
        {
            var exampleFiles = Directory.GetFiles(examplesPath, "*.json");
            exampleFiles.Should().NotBeEmpty("Examples directory should contain example files");

            foreach (var exampleFile in exampleFiles)
            {
                var exampleContent = File.ReadAllText(exampleFile);
                var isValidJson = IsValidJson(exampleContent);
                isValidJson.Should().BeTrue($"Example file {Path.GetFileName(exampleFile)} should contain valid JSON");

                var example = JsonSerializer.Deserialize<JsonElement>(exampleContent);
                example.TryGetProperty("resourceType", out var resourceType).Should().BeTrue("Example should have resourceType");
                resourceType.GetString().Should().NotBeNullOrEmpty("Example should have valid resource type");
            }
        }

        // Check value sets
        if (Directory.Exists(valuesetsPath))
        {
            var valuesetFiles = Directory.GetFiles(valuesetsPath, "*.json");
            foreach (var valuesetFile in valuesetFiles)
            {
                var valuesetContent = File.ReadAllText(valuesetFile);
                var isValidJson = IsValidJson(valuesetContent);
                isValidJson.Should().BeTrue($"ValueSet file {Path.GetFileName(valuesetFile)} should contain valid JSON");

                var valueset = JsonSerializer.Deserialize<JsonElement>(valuesetContent);
                valueset.TryGetProperty("resourceType", out var resourceType).Should().BeTrue("ValueSet should have resourceType");
                resourceType.GetString().Should().Be("ValueSet", "ValueSet should be ValueSet resource type");
            }
        }
    }

    [Fact]
    public void PublicationGuide_ShouldExistAndBeComprehensive()
    {
        // Arrange
        var publicationGuidePath = Path.Combine(_publicationDocsPath, "simplifier-publication-guide.md");

        // Act & Assert
        File.Exists(publicationGuidePath).Should().BeTrue("Simplifier publication guide should exist");

        var content = File.ReadAllText(publicationGuidePath);
        content.Should().NotBeNullOrEmpty("Publication guide should have content");
        content.Length.Should().BeGreaterThan(5000, "Publication guide should be comprehensive");

        // Verify key sections
        content.Should().Contain("Step-by-Step Publication Process", "Guide should provide step-by-step process");
        content.Should().Contain("Simplifier.net", "Guide should reference Simplifier.net");
        content.Should().Contain("FHIR package", "Guide should address FHIR package upload");
        content.Should().Contain("canonical URL", "Guide should address canonical URLs");
        content.Should().Contain("external partners", "Guide should address external partner access");
    }

    [Fact]
    public void PublicationGuide_ShouldContainRequiredSections()
    {
        // Arrange
        var publicationGuidePath = Path.Combine(_publicationDocsPath, "simplifier-publication-guide.md");
        var content = File.ReadAllText(publicationGuidePath);

        // Act & Assert
        content.Should().Contain("## Prerequisites", "Guide should have prerequisites section");
        content.Should().Contain("## Step-by-Step Publication Process", "Guide should have publication process section");
        content.Should().Contain("Step 1:", "Guide should have numbered steps");
        content.Should().Contain("Step 2:", "Guide should have multiple steps");
        content.Should().Contain("Project Setup", "Guide should cover project setup");
        content.Should().Contain("Upload FHIR Package", "Guide should cover package upload");
        content.Should().Contain("Implementation Guide Configuration", "Guide should cover IG configuration");
        content.Should().Contain("Quality Assurance", "Guide should cover quality assurance");
        content.Should().Contain("Publication and Release", "Guide should cover publication process");
        content.Should().Contain("Post-Publication Activities", "Guide should cover post-publication activities");
    }

    [Fact]
    public void ExternalPartnerAccess_ShouldExistAndBeComprehensive()
    {
        // Arrange
        var partnerAccessPath = Path.Combine(_publicationDocsPath, "external-partner-access.md");

        // Act & Assert
        File.Exists(partnerAccessPath).Should().BeTrue("External partner access strategy should exist");

        var content = File.ReadAllText(partnerAccessPath);
        content.Should().NotBeNullOrEmpty("Partner access strategy should have content");
        content.Length.Should().BeGreaterThan(10000, "Partner access strategy should be comprehensive");

        // Verify key partner types are addressed
        content.Should().Contain("Healthcare Organizations", "Strategy should address healthcare organizations");
        content.Should().Contain("EHR", "Strategy should address EHR vendors");
        content.Should().Contain("Technology Vendors", "Strategy should address technology vendors");
        content.Should().Contain("Healthcare IT Consultants", "Strategy should address consultants");
        content.Should().Contain("Implementation Partners", "Strategy should address implementation partners");
    }

    [Fact]
    public void PublicationChecklist_ShouldExistAndBeActionable()
    {
        // Arrange
        var checklistPath = Path.Combine(_publicationDocsPath, "publication-checklist.md");

        // Act & Assert
        File.Exists(checklistPath).Should().BeTrue("Publication checklist should exist");

        var content = File.ReadAllText(checklistPath);
        content.Should().NotBeNullOrEmpty("Checklist should have content");

        // Verify checklist format
        var checkboxCount = content.Split("- [ ]").Length - 1;
        checkboxCount.Should().BeGreaterThan(20, "Checklist should have many actionable items");

        // Verify key sections
        content.Should().Contain("Pre-Publication Validation", "Checklist should have pre-publication validation");
        content.Should().Contain("Publication Requirements", "Checklist should have publication requirements");
        content.Should().Contain("External Access Verification", "Checklist should have access verification");
        content.Should().Contain("Quality Assurance", "Checklist should have quality assurance");
        content.Should().Contain("Post-Publication Activities", "Checklist should have post-publication activities");
        content.Should().Contain("Final Approval", "Checklist should have final approval section");
    }

    [Fact]
    public void PublicationDocuments_ShouldReferenceCanonicalUrls()
    {
        // Arrange
        var baseCanonicalUrl = "http://example.org/fhir/ig/hl7-fhir-expansion-pack";
        var publicationFiles = Directory.GetFiles(_publicationDocsPath, "*.md");

        // Act & Assert
        publicationFiles.Should().NotBeEmpty("Publication documents should exist");

        foreach (var file in publicationFiles)
        {
            var content = File.ReadAllText(file);
            if (content.Contains("canonical") || content.Contains("URL"))
            {
                content.Should().Contain("example.org", $"File {Path.GetFileName(file)} should reference consistent domain");
            }
        }
    }

    [Fact]
    public void PublicationDocuments_ShouldBeWellFormatted()
    {
        // Arrange
        var publicationFiles = Directory.GetFiles(_publicationDocsPath, "*.md");

        // Act & Assert
        foreach (var file in publicationFiles)
        {
            var content = File.ReadAllText(file);
            var fileName = Path.GetFileName(file);

            // Verify markdown structure
            content.Should().Contain("# ", $"File {fileName} should have H1 heading");
            content.Should().Contain("## ", $"File {fileName} should have H2 headings");

            // Verify no placeholder content
            content.Should().NotContain("TODO", $"File {fileName} should not contain TODO items");
            content.Should().NotContain("TBD", $"File {fileName} should not contain TBD items");
            content.Should().NotContain("FIXME", $"File {fileName} should not contain FIXME items");

            // Verify reasonable content length
            content.Length.Should().BeGreaterThan(1000, $"File {fileName} should have substantial content");
        }
    }

    [Theory]
    [InlineData("package.json")]
    [InlineData("profiles")]
    [InlineData("examples")]
    [InlineData("valuesets")]
    [InlineData("extensions")]
    public void FhirPackage_RequiredFilesAndDirectories_ShouldExist(string pathComponent)
    {
        // Arrange
        var fullPath = Path.Combine(_fhirPackagePath, pathComponent);

        // Act & Assert
        if (pathComponent.EndsWith(".json"))
        {
            File.Exists(fullPath).Should().BeTrue($"Required file {pathComponent} should exist");
        }
        else
        {
            Directory.Exists(fullPath).Should().BeTrue($"Required directory {pathComponent} should exist");
        }
    }

    [Fact]
    public void FhirPackage_ShouldBeReadyForSimplifierUpload()
    {
        // Arrange
        var packageJsonPath = Path.Combine(_fhirPackagePath, "package.json");

        // Act & Assert
        // Verify package can be serialized/deserialized
        var packageContent = File.ReadAllText(packageJsonPath);
        var package = JsonSerializer.Deserialize<JsonElement>(packageContent);

        // Verify essential Simplifier.net requirements
        package.TryGetProperty("canonical", out var canonical).Should().BeTrue();
        canonical.GetString().Should().StartWith("http", "Canonical URL should be valid HTTP URL");

        package.TryGetProperty("version", out var version).Should().BeTrue();
        version.GetString().Should().MatchRegex(@"^\d+\.\d+\.\d+$", "Version should be semantic version");

        package.TryGetProperty("fhirVersion", out var fhirVersion).Should().BeTrue();
        fhirVersion.GetString().Should().Be("4.0.1", "Should use FHIR R4");

        // Verify file structure is complete
        var requiredDirs = new[] { "profiles", "examples", "valuesets", "extensions" };
        foreach (var dir in requiredDirs)
        {
            Directory.Exists(Path.Combine(_fhirPackagePath, dir)).Should().BeTrue($"Directory {dir} should exist");
        }
    }

    private bool IsValidJson(string jsonString)
    {
        try
        {
            JsonSerializer.Deserialize<JsonElement>(jsonString);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}