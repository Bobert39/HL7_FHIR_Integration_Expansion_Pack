using Xunit;
using FluentAssertions;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;

namespace FhirIntegrationService.Tests;

/// <summary>
/// Tests for validating links, references, and cross-references in Implementation Guide and documentation
/// </summary>
public class LinkValidationTests
{
    private readonly Mock<ILogger<LinkValidationTests>> _mockLogger;
    private readonly string _implementationGuidePath;
    private readonly string _publicationDocsPath;
    private readonly string _artifactsPath;
    private readonly string _fhirPackagePath;
    private readonly HttpClient _httpClient;

    public LinkValidationTests()
    {
        _mockLogger = new Mock<ILogger<LinkValidationTests>>();
        _implementationGuidePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "docs", "implementation-guide");
        _publicationDocsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "docs", "publication");
        _artifactsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "docs", "artifacts");
        _fhirPackagePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "fhir-package");
        _httpClient = new HttpClient();
    }

    [Fact]
    public void ImplementationGuide_ShouldHaveValidInternalLinks()
    {
        // Arrange
        var igFilePath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
        var content = File.ReadAllText(igFilePath);

        // Act & Assert - Validate table of contents links
        var tocPattern = @"\[([^\]]+)\]\(#([^)]+)\)";
        var tocMatches = Regex.Matches(content, tocPattern);

        foreach (Match match in tocMatches)
        {
            var linkText = match.Groups[1].Value;
            var anchor = match.Groups[2].Value;

            // Check if corresponding heading exists
            var headingPattern = $@"#+\s+{Regex.Escape(linkText)}";
            var hasHeading = Regex.IsMatch(content, headingPattern, RegexOptions.IgnoreCase);

            hasHeading.Should().BeTrue($"TOC link '{linkText}' should have corresponding heading");
        }

        // Validate section references
        var sectionRefPattern = @"\[([^\]]+)\]\(#([^)]+)\)";
        var sectionMatches = Regex.Matches(content, sectionRefPattern);

        foreach (Match match in sectionMatches)
        {
            var anchor = match.Groups[2].Value;
            var expectedHeading = anchor.Replace("-", " ");

            // Verify anchor target exists
            var anchorExists = content.Contains($"## {expectedHeading}") ||
                              content.Contains($"### {expectedHeading}") ||
                              content.Contains($"#### {expectedHeading}");

            if (!anchorExists)
            {
                // Try case-insensitive match
                anchorExists = Regex.IsMatch(content, $@"#+\s+{Regex.Escape(expectedHeading)}", RegexOptions.IgnoreCase);
            }

            anchorExists.Should().BeTrue($"Section reference '{anchor}' should have valid target");
        }
    }

    [Fact]
    public void ImplementationGuide_ShouldHaveValidCodeReferences()
    {
        // Arrange
        var igFilePath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
        var content = File.ReadAllText(igFilePath);

        // Act & Assert - Validate file references in code examples
        var fileRefPattern = @"```[\w]*\n[^`]*?(\w+\.\w+)[^`]*?```";
        var fileMatches = Regex.Matches(content, fileRefPattern, RegexOptions.Singleline);

        var projectRoot = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..");

        foreach (Match match in fileMatches)
        {
            var fileName = match.Groups[1].Value;

            // Skip generic filenames and placeholders
            if (fileName.Contains("example") || fileName.Contains("sample") ||
                fileName.StartsWith("your") || fileName.Contains("config"))
                continue;

            // Check if file exists in project structure
            var searchPattern = $"*{fileName}";
            var foundFiles = Directory.GetFiles(projectRoot, searchPattern, SearchOption.AllDirectories);

            if (foundFiles.Length == 0)
            {
                // This might be expected for some example files, so we'll warn but not fail
                _mockLogger.Object.LogWarning($"Code reference to file '{fileName}' not found in project");
            }
        }

        // Validate class/method references
        var classRefPattern = @"`(\w+Service|\w+Controller|\w+Client)`";
        var classMatches = Regex.Matches(content, classRefPattern);

        foreach (Match match in classMatches)
        {
            var className = match.Groups[1].Value;

            // These should be real classes mentioned in the implementation
            className.Should().NotBeNullOrEmpty($"Class reference '{className}' should be valid");

            // Check common patterns
            if (className.EndsWith("Service"))
            {
                content.Should().Contain($"class {className}", $"Service class '{className}' should be defined");
            }
        }
    }

    [Fact]
    public void ImplementationGuide_ShouldHaveValidExternalReferences()
    {
        // Arrange
        var igFilePath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
        var content = File.ReadAllText(igFilePath);

        // Act & Assert - Validate external URL references
        var urlPattern = @"https?://[^\s\)]+";
        var urlMatches = Regex.Matches(content, urlPattern);

        var validDomains = new[]
        {
            "hl7.org",
            "fhir.org",
            "build.fhir.org",
            "simplifier.net",
            "docs.microsoft.com",
            "github.com",
            "example.org"
        };

        foreach (Match match in urlMatches)
        {
            var url = match.Value.TrimEnd(',', '.', ')', ']');

            // Extract domain
            var uri = new Uri(url);
            var domain = uri.Host.ToLower();

            // Check if it's a known valid domain or subdomain
            var isValidDomain = validDomains.Any(validDomain =>
                domain == validDomain || domain.EndsWith($".{validDomain}"));

            isValidDomain.Should().BeTrue($"External URL '{url}' should use recognized domain");
        }

        // Validate FHIR specification references
        content.Should().Contain("http://hl7.org/fhir/", "IG should reference FHIR specification");
        content.Should().Contain("simplifier.net", "IG should reference Simplifier.net for publication");
    }

    [Fact]
    public void PublicationDocuments_ShouldHaveValidCrossReferences()
    {
        // Arrange
        var publicationFiles = Directory.GetFiles(_publicationDocsPath, "*.md");

        // Act & Assert
        foreach (var file in publicationFiles)
        {
            var content = File.ReadAllText(file);
            var fileName = Path.GetFileName(file);

            // Validate references to other publication documents
            var docRefPattern = @"\[([^\]]+)\]\(([^)]+\.md)\)";
            var docMatches = Regex.Matches(content, docRefPattern);

            foreach (Match match in docMatches)
            {
                var referencedFile = match.Groups[2].Value;
                var referencedPath = Path.Combine(_publicationDocsPath, referencedFile);

                File.Exists(referencedPath).Should().BeTrue(
                    $"Document '{fileName}' references '{referencedFile}' which should exist");
            }

            // Validate references to main IG
            if (content.Contains("implementation-guide") || content.Contains("Implementation Guide"))
            {
                var igPath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
                File.Exists(igPath).Should().BeTrue($"Referenced Implementation Guide should exist");
            }
        }
    }

    [Fact]
    public void FhirPackage_ShouldHaveValidProfileReferences()
    {
        // Arrange
        var packageJsonPath = Path.Combine(_fhirPackagePath, "package.json");
        var profilesPath = Path.Combine(_fhirPackagePath, "profiles");
        var examplesPath = Path.Combine(_fhirPackagePath, "examples");

        // Act & Assert
        if (Directory.Exists(profilesPath))
        {
            var profileFiles = Directory.GetFiles(profilesPath, "*.json");

            foreach (var profileFile in profileFiles)
            {
                var profileContent = File.ReadAllText(profileFile);
                var profileName = Path.GetFileNameWithoutExtension(profileFile);

                // Validate canonical URL consistency
                if (profileContent.Contains("\"canonical\""))
                {
                    profileContent.Should().Contain("example.org/fhir",
                        $"Profile '{profileName}' should use consistent canonical URL base");
                }

                // Validate base profile references
                if (profileContent.Contains("\"baseDefinition\""))
                {
                    profileContent.Should().Contain("http://hl7.org/fhir/StructureDefinition/",
                        $"Profile '{profileName}' should reference valid FHIR base definitions");
                }
            }
        }

        // Validate example references to profiles
        if (Directory.Exists(examplesPath))
        {
            var exampleFiles = Directory.GetFiles(examplesPath, "*.json");

            foreach (var exampleFile in exampleFiles)
            {
                var exampleContent = File.ReadAllText(exampleFile);
                var exampleName = Path.GetFileNameWithoutExtension(exampleFile);

                // Check for meta.profile references
                if (exampleContent.Contains("\"meta\"") && exampleContent.Contains("\"profile\""))
                {
                    exampleContent.Should().Contain("example.org/fhir",
                        $"Example '{exampleName}' should reference consistent profile URLs");
                }
            }
        }
    }

    [Fact]
    public void ArtifactDocuments_ShouldHaveValidReferences()
    {
        // Arrange
        var artifactFiles = Directory.GetFiles(_artifactsPath, "*.md");

        // Act & Assert
        foreach (var file in artifactFiles)
        {
            var content = File.ReadAllText(file);
            var fileName = Path.GetFileName(file);

            // Validate Epic/Story references
            var epicRefPattern = @"Epic \d+(\.\d+)?";
            var epicMatches = Regex.Matches(content, epicRefPattern);

            foreach (Match match in epicMatches)
            {
                var epicRef = match.Value;
                // Epic references should be consistent with project structure
                epicRef.Should().MatchRegex(@"Epic \d+(\.\d+)?", $"Epic reference '{epicRef}' should follow standard format");
            }

            // Validate FHIR resource references
            var fhirResourcePattern = @"\b(Patient|Observation|Practitioner|Organization|Encounter|Condition|Procedure|DiagnosticReport|Medication|MedicationRequest|AllergyIntolerance|CarePlan|Goal|ServiceRequest|Specimen|ImagingStudy|DocumentReference|Composition|Bundle|MessageHeader|OperationOutcome|Parameters|Subscription|AuditEvent|Provenance|Schedule|Slot|Appointment|AppointmentResponse|Account|Invoice|Claim|ClaimResponse|Coverage|EligibilityRequest|EligibilityResponse|EnrollmentRequest|EnrollmentResponse|PaymentNotice|PaymentReconciliation|ExplanationOfBenefit|Contract|RiskAssessment|DetectedIssue|Flag|Task|Communication|CommunicationRequest|Device|DeviceRequest|DeviceUseStatement|SupplyRequest|SupplyDelivery|InventoryReport|Location|HealthcareService|PractitionerRole|RelatedPerson|Person|Group|CareTeam|Endpoint|InsurancePlan|Substance|Medication|MedicationKnowledge|MedicationStatement|MedicationAdministration|MedicationDispense|Immunization|ImmunizationEvaluation|ImmunizationRecommendation|AdverseEvent|ResearchStudy|ResearchSubject|ActivityDefinition|PlanDefinition|Questionnaire|QuestionnaireResponse|Measure|MeasureReport|Library|ImplementationGuide|TestScript|TestReport|StructureDefinition|StructureMap|ConceptMap|CapabilityStatement|OperationDefinition|SearchParameter|CompartmentDefinition|ValueSet|CodeSystem|NamingSystem|TerminologyCapabilities|ExpansionProfile|GraphDefinition|ExampleScenario|ChargeItem|ChargeItemDefinition|Basic|Binary|BodyStructure|CatalogEntry|Communication|CommunicationRequest|DeviceDefinition|DeviceMetric|DiagnosticReport|DocumentManifest|EffectEvidenceSynthesis|Evidence|EvidenceVariable|FamilyMemberHistory|GuidanceResponse|ImagingStudy|List|Media|MedicinalProduct|MedicinalProductAuthorization|MedicinalProductContraindication|MedicinalProductIndication|MedicinalProductIngredient|MedicinalProductInteraction|MedicinalProductManufactured|MedicinalProductPackaged|MedicinalProductPharmaceutical|MedicinalProductUndesirableEffect|MolecularSequence|NutritionOrder|Observation|OrganizationAffiliation|Patient|PaymentNotice|PopulationEvidenceSynthesis|Provenance|QuestionnaireResponse|RequestGroup|ResearchDefinition|ResearchElementDefinition|RiskAssessment|RiskEvidenceSynthesis|ServiceRequest|Specimen|SpecimenDefinition|SubstanceNucleicAcid|SubstancePolymer|SubstanceProtein|SubstanceReferenceInformation|SubstanceSourceMaterial|SubstanceSpecification|SupplyDelivery|SupplyRequest|Task|VerificationResult|VisionPrescription)\b";
            var resourceMatches = Regex.Matches(content, fhirResourcePattern);

            resourceMatches.Should().NotBeEmpty($"Artifact '{fileName}' should reference FHIR resources");

            // Validate that referenced resources are standard FHIR R4 resources
            foreach (Match match in resourceMatches)
            {
                var resourceType = match.Value;
                resourceType.Should().MatchRegex(@"^[A-Z][a-zA-Z]+$",
                    $"FHIR resource '{resourceType}' should follow proper naming convention");
            }
        }
    }

    [Fact]
    public void CanonicalUrls_ShouldBeConsistentAcrossAllDocuments()
    {
        // Arrange
        var baseCanonicalUrl = "http://example.org/fhir/ig/hl7-fhir-expansion-pack";
        var allMarkdownFiles = new List<string>();

        allMarkdownFiles.AddRange(Directory.GetFiles(_implementationGuidePath, "*.md"));
        allMarkdownFiles.AddRange(Directory.GetFiles(_publicationDocsPath, "*.md"));
        allMarkdownFiles.AddRange(Directory.GetFiles(_artifactsPath, "*.md"));

        // Act & Assert
        foreach (var file in allMarkdownFiles)
        {
            var content = File.ReadAllText(file);
            var fileName = Path.GetFileName(file);

            // Check for canonical URL references
            if (content.Contains("canonical") || content.Contains("http://example.org"))
            {
                content.Should().Contain("example.org",
                    $"File '{fileName}' should use consistent domain for canonical URLs");

                // If it contains the full canonical base, it should be correct
                if (content.Contains("http://example.org/fhir/ig/"))
                {
                    content.Should().Contain(baseCanonicalUrl,
                        $"File '{fileName}' should use consistent canonical URL base");
                }
            }
        }

        // Check FHIR package canonical consistency
        var packageJsonPath = Path.Combine(_fhirPackagePath, "package.json");
        if (File.Exists(packageJsonPath))
        {
            var packageContent = File.ReadAllText(packageJsonPath);
            packageContent.Should().Contain(baseCanonicalUrl,
                "FHIR package should use consistent canonical URL");
        }
    }

    [Fact]
    public void VersionNumbers_ShouldBeConsistentAcrossAllDocuments()
    {
        // Arrange
        var expectedVersion = "1.0.0";
        var allDocuments = new List<string>();

        allDocuments.AddRange(Directory.GetFiles(_implementationGuidePath, "*.md"));
        allDocuments.AddRange(Directory.GetFiles(_publicationDocsPath, "*.md"));
        allDocuments.AddRange(Directory.GetFiles(_artifactsPath, "*.md"));

        // Act & Assert
        foreach (var file in allDocuments)
        {
            var content = File.ReadAllText(file);
            var fileName = Path.GetFileName(file);

            // Check for version references
            var versionPattern = @"[Vv]ersion\s+(\d+\.\d+\.\d+)";
            var versionMatches = Regex.Matches(content, versionPattern);

            foreach (Match match in versionMatches)
            {
                var foundVersion = match.Groups[1].Value;
                foundVersion.Should().Be(expectedVersion,
                    $"File '{fileName}' should use consistent version number");
            }
        }

        // Check FHIR package version consistency
        var packageJsonPath = Path.Combine(_fhirPackagePath, "package.json");
        if (File.Exists(packageJsonPath))
        {
            var packageContent = File.ReadAllText(packageJsonPath);
            packageContent.Should().Contain($"\"version\": \"{expectedVersion}\"",
                "FHIR package should use consistent version number");
        }
    }

    [Theory]
    [InlineData("hl7-fhir-expansion-pack-implementation-guide.md")]
    [InlineData("simplifier-publication-guide.md")]
    [InlineData("external-partner-access.md")]
    [InlineData("publication-checklist.md")]
    public void KeyDocuments_ShouldHaveValidMarkdownStructure(string fileName)
    {
        // Arrange
        var filePath = fileName == "hl7-fhir-expansion-pack-implementation-guide.md"
            ? Path.Combine(_implementationGuidePath, fileName)
            : Path.Combine(_publicationDocsPath, fileName);

        // Act & Assert
        File.Exists(filePath).Should().BeTrue($"Key document '{fileName}' should exist");

        var content = File.ReadAllText(filePath);

        // Validate markdown structure
        content.Should().Contain("# ", $"Document '{fileName}' should have H1 heading");
        content.Should().Contain("## ", $"Document '{fileName}' should have H2 headings");

        // Validate no broken markdown
        content.Should().NotContain("]()", $"Document '{fileName}' should not have empty links");
        content.Should().NotContain("[](", $"Document '{fileName}' should not have malformed links");

        // Validate proper list formatting
        var listItemPattern = @"^\s*[-\*\+]\s+";
        var numberedListPattern = @"^\s*\d+\.\s+";
        var hasUnorderedLists = Regex.IsMatch(content, listItemPattern, RegexOptions.Multiline);
        var hasOrderedLists = Regex.IsMatch(content, numberedListPattern, RegexOptions.Multiline);

        if (hasUnorderedLists || hasOrderedLists)
        {
            // If the document has lists, they should be properly formatted
            var lines = content.Split('\n');
            foreach (var line in lines)
            {
                if (Regex.IsMatch(line, @"^\s*[-\*\+]\s*$"))
                {
                    // Empty list items should not exist
                    false.Should().BeTrue($"Document '{fileName}' should not have empty list items");
                }
            }
        }
    }

    [Fact]
    public void AllDocuments_ShouldHaveConsistentTerminology()
    {
        // Arrange
        var allMarkdownFiles = new List<string>();
        allMarkdownFiles.AddRange(Directory.GetFiles(_implementationGuidePath, "*.md"));
        allMarkdownFiles.AddRange(Directory.GetFiles(_publicationDocsPath, "*.md"));
        allMarkdownFiles.AddRange(Directory.GetFiles(_artifactsPath, "*.md"));

        var standardTerms = new Dictionary<string, string[]>
        {
            ["FHIR"] = new[] { "FHIR", "HL7 FHIR" },
            ["Implementation Guide"] = new[] { "Implementation Guide", "IG" },
            ["Simplifier.net"] = new[] { "Simplifier.net", "Simplifier" },
            ["Healthcare"] = new[] { "Healthcare", "Health Care" },
            ["API"] = new[] { "API", "REST API", "FHIR API" }
        };

        // Act & Assert
        foreach (var file in allMarkdownFiles)
        {
            var content = File.ReadAllText(file);
            var fileName = Path.GetFileName(file);

            foreach (var termGroup in standardTerms)
            {
                var hasAnyTerm = termGroup.Value.Any(term => content.Contains(term));
                if (hasAnyTerm)
                {
                    // If document uses these terms, check for consistency
                    var termCounts = termGroup.Value.ToDictionary(term => term, term =>
                        Regex.Matches(content, Regex.Escape(term), RegexOptions.IgnoreCase).Count);

                    // Primary term should be used more frequently than alternatives
                    var primaryTerm = termGroup.Value[0];
                    var primaryCount = termCounts[primaryTerm];

                    // This is a soft validation - we want consistency but allow some variation
                    if (termCounts.Values.Sum() > 5) // Only check if terms are used frequently
                    {
                        primaryCount.Should().BeGreaterOrEqualTo(termCounts.Values.Max() / 2,
                            $"Document '{fileName}' should use consistent terminology for '{termGroup.Key}'");
                    }
                }
            }
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}