using Xunit;
using FluentAssertions;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FhirIntegrationService.Tests;

/// <summary>
/// Tests for Implementation Guide navigation, accessibility, and usability
/// </summary>
public class NavigationTests
{
    private readonly Mock<ILogger<NavigationTests>> _mockLogger;
    private readonly string _implementationGuidePath;
    private readonly string _publicationDocsPath;
    private readonly string _artifactsPath;

    public NavigationTests()
    {
        _mockLogger = new Mock<ILogger<NavigationTests>>();
        _implementationGuidePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "docs", "implementation-guide");
        _publicationDocsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "docs", "publication");
        _artifactsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "docs", "artifacts");
    }

    [Fact]
    public void ImplementationGuide_ShouldHaveTableOfContents()
    {
        // Arrange
        var igFilePath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
        var content = File.ReadAllText(igFilePath);

        // Act & Assert
        content.Should().Contain("## Table of Contents", "IG should have table of contents");

        // Verify TOC contains major sections
        content.Should().Contain("[Overview](#overview)", "TOC should link to Overview");
        content.Should().Contain("[Clinical Context](#clinical-context)", "TOC should link to Clinical Context");
        content.Should().Contain("[Technical Specifications](#technical-specifications)", "TOC should link to Technical Specifications");
        content.Should().Contain("[Security Considerations](#security-considerations)", "TOC should link to Security Considerations");
        content.Should().Contain("[Implementation Examples](#implementation-examples)", "TOC should link to Implementation Examples");
        content.Should().Contain("[Testing and Validation](#testing-and-validation)", "TOC should link to Testing and Validation");
        content.Should().Contain("[Support and Resources](#support-and-resources)", "TOC should link to Support and Resources");
    }

    [Fact]
    public void ImplementationGuide_ShouldHaveValidMarkdownStructure()
    {
        // Arrange
        var igFilePath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
        var content = File.ReadAllText(igFilePath);

        // Act & Assert
        // Verify proper heading hierarchy
        content.Should().Contain("# HL7 FHIR Integration Expansion Pack - Implementation Guide", "IG should have H1 title");

        var h1Count = Regex.Matches(content, @"^# ", RegexOptions.Multiline).Count;
        h1Count.Should().Be(1, "IG should have exactly one H1 heading");

        var h2Count = Regex.Matches(content, @"^## ", RegexOptions.Multiline).Count;
        h2Count.Should().BeGreaterOrEqualTo(7, "IG should have at least 7 H2 headings for major sections");

        var h3Count = Regex.Matches(content, @"^### ", RegexOptions.Multiline).Count;
        h3Count.Should().BeGreaterThan(0, "IG should have H3 headings for subsections");
    }

    [Fact]
    public void ImplementationGuide_ShouldHaveValidInternalLinks()
    {
        // Arrange
        var igFilePath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
        var content = File.ReadAllText(igFilePath);

        // Act & Assert
        // Find all markdown anchor links [text](#anchor)
        var anchorLinkPattern = @"\[([^\]]+)\]\(#([^)]+)\)";
        var anchorLinks = Regex.Matches(content, anchorLinkPattern);

        anchorLinks.Should().NotBeEmpty("IG should contain internal anchor links");

        // Find all heading anchors
        var headingPattern = @"^#{1,6}\s+(.+)$";
        var headings = Regex.Matches(content, headingPattern, RegexOptions.Multiline);

        var headingAnchors = headings
            .Select(h => h.Groups[1].Value.Trim())
            .Select(h => h.ToLowerInvariant().Replace(" ", "-").Replace(".", "").Replace(",", ""))
            .ToHashSet();

        // Verify each anchor link has a corresponding heading
        foreach (Match link in anchorLinks)
        {
            var anchorName = link.Groups[2].Value.ToLowerInvariant();
            headingAnchors.Should().Contain(anchorName, $"Anchor link #{anchorName} should have corresponding heading");
        }
    }

    [Fact]
    public void ImplementationGuide_ShouldHaveValidExternalLinks()
    {
        // Arrange
        var igFilePath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
        var content = File.ReadAllText(igFilePath);

        // Act & Assert
        // Find all external links [text](http://...)
        var externalLinkPattern = @"\[([^\]]+)\]\((https?://[^)]+)\)";
        var externalLinks = Regex.Matches(content, externalLinkPattern);

        externalLinks.Should().NotBeEmpty("IG should contain external links");

        foreach (Match link in externalLinks)
        {
            var url = link.Groups[2].Value;
            var isValidUrl = Uri.TryCreate(url, UriKind.Absolute, out var uri);
            isValidUrl.Should().BeTrue($"External link {url} should be valid URL");

            if (isValidUrl)
            {
                uri.Scheme.Should().Match(scheme => scheme == "http" || scheme == "https",
                    $"External link {url} should use HTTP or HTTPS");
            }
        }
    }

    [Fact]
    public void ImplementationGuide_ShouldHaveProperCodeBlocks()
    {
        // Arrange
        var igFilePath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
        var content = File.ReadAllText(igFilePath);

        // Act & Assert
        // Verify code blocks are properly formatted
        content.Should().Contain("```csharp", "IG should contain C# code examples");
        content.Should().Contain("```json", "IG should contain JSON examples");
        content.Should().Contain("```http", "IG should contain HTTP examples");

        // Count opening and closing code block markers
        var openingCodeBlocks = Regex.Matches(content, @"^```", RegexOptions.Multiline).Count;
        var closingCodeBlocks = Regex.Matches(content, @"^```\s*$", RegexOptions.Multiline).Count;

        // Note: This is a simplified check - in real markdown, opening blocks include language
        // so we count all ``` markers and verify they're even (each opening has a closing)
        var totalCodeBlockMarkers = Regex.Matches(content, @"^```", RegexOptions.Multiline).Count;
        (totalCodeBlockMarkers % 2).Should().Be(0, "All code blocks should be properly closed");
    }

    [Fact]
    public void ImplementationGuide_ShouldHaveReadableContent()
    {
        // Arrange
        var igFilePath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
        var content = File.ReadAllText(igFilePath);

        // Act & Assert
        // Basic readability checks
        content.Should().NotContain("Lorem ipsum", "IG should not contain placeholder text");
        content.Should().NotContain("TODO", "IG should not contain TODO items");
        content.Should().NotContain("FIXME", "IG should not contain FIXME items");
        content.Should().NotContain("TBD", "IG should not contain TBD items");

        // Verify proper sentence structure (basic check)
        var sentences = content.Split('.', StringSplitOptions.RemoveEmptyEntries);
        sentences.Length.Should().BeGreaterThan(50, "IG should contain substantial content with many sentences");

        // Verify paragraphs are not too long (basic readability)
        var paragraphs = content.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
        var longParagraphs = paragraphs.Where(p => p.Length > 2000 && !p.Contains("```")).Count();
        longParagraphs.Should().BeLessThan(paragraphs.Length / 4, "Most paragraphs should be reasonably sized for readability");
    }

    [Fact]
    public void PublicationDocuments_ShouldHaveValidNavigation()
    {
        // Arrange
        var publicationFiles = Directory.GetFiles(_publicationDocsPath, "*.md");

        // Act & Assert
        publicationFiles.Should().NotBeEmpty("Publication documents should exist");

        foreach (var file in publicationFiles)
        {
            var content = File.ReadAllText(file);
            var fileName = Path.GetFileName(file);

            // Verify basic structure
            content.Should().Contain("# ", $"Publication document {fileName} should have H1 heading");
            content.Should().Contain("## ", $"Publication document {fileName} should have H2 headings");

            // Verify no broken internal references
            if (content.Contains("](#"))
            {
                var anchorLinks = Regex.Matches(content, @"\[([^\]]+)\]\(#([^)]+)\)");
                var headings = Regex.Matches(content, @"^#{1,6}\s+(.+)$", RegexOptions.Multiline);

                var headingAnchors = headings
                    .Select(h => h.Groups[1].Value.Trim())
                    .Select(h => h.ToLowerInvariant().Replace(" ", "-").Replace(".", "").Replace(",", ""))
                    .ToHashSet();

                foreach (Match link in anchorLinks)
                {
                    var anchorName = link.Groups[2].Value.ToLowerInvariant();
                    headingAnchors.Should().Contain(anchorName,
                        $"Anchor link #{anchorName} in {fileName} should have corresponding heading");
                }
            }
        }
    }

    [Fact]
    public void DocumentationHierarchy_ShouldBeLogicalAndComplete()
    {
        // Arrange & Act & Assert
        // Verify main IG document exists
        var mainIgPath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
        File.Exists(mainIgPath).Should().BeTrue("Main Implementation Guide should exist");

        // Verify artifact documents exist
        var requiredArtifacts = new[]
        {
            "clinical-requirements-summary.md",
            "fhir-profiles-inventory.md",
            "implementation-evidence.md"
        };

        foreach (var artifact in requiredArtifacts)
        {
            var artifactPath = Path.Combine(_artifactsPath, artifact);
            File.Exists(artifactPath).Should().BeTrue($"Artifact document {artifact} should exist");
        }

        // Verify publication documents exist
        var requiredPublicationDocs = new[]
        {
            "simplifier-publication-guide.md",
            "external-partner-access.md",
            "publication-checklist.md"
        };

        foreach (var pubDoc in requiredPublicationDocs)
        {
            var pubDocPath = Path.Combine(_publicationDocsPath, pubDoc);
            File.Exists(pubDocPath).Should().BeTrue($"Publication document {pubDoc} should exist");
        }
    }

    [Fact]
    public void DocumentCrossReferences_ShouldBeConsistent()
    {
        // Arrange
        var allDocuments = new List<(string path, string content)>();

        // Read main IG
        var mainIgPath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
        allDocuments.Add((mainIgPath, File.ReadAllText(mainIgPath)));

        // Read artifacts
        var artifactFiles = Directory.GetFiles(_artifactsPath, "*.md");
        foreach (var file in artifactFiles)
        {
            allDocuments.Add((file, File.ReadAllText(file)));
        }

        // Read publication docs
        var publicationFiles = Directory.GetFiles(_publicationDocsPath, "*.md");
        foreach (var file in publicationFiles)
        {
            allDocuments.Add((file, File.ReadAllText(file)));
        }

        // Act & Assert
        foreach (var (docPath, content) in allDocuments)
        {
            var fileName = Path.GetFileName(docPath);

            // Verify consistent terminology
            if (content.Contains("FHIR"))
            {
                content.Should().Contain("R4", $"Document {fileName} referencing FHIR should specify R4 version");
            }

            // Verify consistent version references
            if (content.Contains("version") || content.Contains("Version"))
            {
                content.Should().MatchRegex(@"1\.0\.0|v1\.0\.0|Version 1\.0\.0",
                    $"Document {fileName} should use consistent version numbering");
            }

            // Verify consistent canonical URL patterns
            if (content.Contains("canonical") || content.Contains("URL"))
            {
                if (content.Contains("example.org"))
                {
                    content.Should().Contain("http://example.org/fhir",
                        $"Document {fileName} should use consistent canonical URL pattern");
                }
            }
        }
    }

    [Fact]
    public void ImplementationGuide_ShouldBeAccessibleToVariousAudiences()
    {
        // Arrange
        var igFilePath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
        var content = File.ReadAllText(igFilePath);

        // Act & Assert
        // Verify content addresses different audiences
        content.Should().Contain("Clinical Teams", "IG should address clinical teams");
        content.Should().Contain("Technical Implementers", "IG should address technical implementers");
        content.Should().Contain("System Administrators", "IG should address system administrators");
        content.Should().Contain("Compliance Officers", "IG should address compliance officers");

        // Verify progressive disclosure of complexity
        var overviewSection = ExtractSection(content, "## Overview");
        var technicalSection = ExtractSection(content, "## Technical Specifications");
        var implementationSection = ExtractSection(content, "## Implementation Examples");

        overviewSection.Should().NotBeNullOrEmpty("Overview section should exist");
        technicalSection.Should().NotBeNullOrEmpty("Technical section should exist");
        implementationSection.Should().NotBeNullOrEmpty("Implementation section should exist");

        // Overview should be less technical than implementation examples
        var overviewCodeBlocks = Regex.Matches(overviewSection, @"```").Count;
        var implementationCodeBlocks = Regex.Matches(implementationSection, @"```").Count;

        implementationCodeBlocks.Should().BeGreaterThan(overviewCodeBlocks,
            "Implementation section should have more code examples than overview");
    }

    [Fact]
    public void DocumentationStructure_ShouldSupportDifferentAccessPatterns()
    {
        // Arrange
        var igFilePath = Path.Combine(_implementationGuidePath, "hl7-fhir-expansion-pack-implementation-guide.md");
        var content = File.ReadAllText(igFilePath);

        // Act & Assert
        // Verify sequential reading support
        var sections = ExtractAllSections(content);
        sections.Should().HaveCountGreaterOrEqualTo(7, "IG should have sufficient sections for sequential reading");

        // Verify each section is substantial
        foreach (var section in sections)
        {
            if (!string.IsNullOrEmpty(section.Value))
            {
                section.Value.Length.Should().BeGreaterThan(500,
                    $"Section {section.Key} should have substantial content");
            }
        }

        // Verify reference access support (clear headings, good navigation)
        var headings = Regex.Matches(content, @"^#{1,6}\s+(.+)$", RegexOptions.Multiline);
        headings.Should().HaveCountGreaterThan(15, "IG should have sufficient headings for reference access");

        // Verify tutorial access support (examples and step-by-step guidance)
        content.Should().Contain("example", "IG should contain examples for tutorial access");
        content.Should().Contain("Example", "IG should contain explicit examples");
        content.Should().MatchRegex(@"\d+\.\s", "IG should contain numbered steps or lists");
    }

    [Theory]
    [InlineData("simplifier-publication-guide.md")]
    [InlineData("external-partner-access.md")]
    [InlineData("publication-checklist.md")]
    public void PublicationDocument_ShouldHaveUsableStructure(string fileName)
    {
        // Arrange
        var docPath = Path.Combine(_publicationDocsPath, fileName);
        var content = File.ReadAllText(docPath);

        // Act & Assert
        content.Should().NotBeNullOrEmpty($"Publication document {fileName} should have content");
        content.Should().Contain("# ", $"Document {fileName} should have main title");
        content.Should().Contain("## ", $"Document {fileName} should have section headers");

        // Verify actionable content structure
        if (fileName.Contains("guide"))
        {
            content.Should().MatchRegex(@"Step \d+|^\d+\.",
                $"Guide document {fileName} should contain numbered steps");
        }

        if (fileName.Contains("checklist"))
        {
            content.Should().Contain("- [ ]",
                $"Checklist document {fileName} should contain checkboxes");
        }

        // Basic length check for usefulness
        content.Length.Should().BeGreaterThan(2000,
            $"Document {fileName} should be comprehensive");
    }

    private string ExtractSection(string content, string sectionHeading)
    {
        var startIndex = content.IndexOf(sectionHeading);
        if (startIndex == -1) return string.Empty;

        var nextSectionIndex = content.IndexOf("\n## ", startIndex + sectionHeading.Length);
        if (nextSectionIndex == -1) nextSectionIndex = content.Length;

        return content.Substring(startIndex, nextSectionIndex - startIndex);
    }

    private Dictionary<string, string> ExtractAllSections(string content)
    {
        var sections = new Dictionary<string, string>();
        var sectionMatches = Regex.Matches(content, @"^## (.+)$", RegexOptions.Multiline);

        for (int i = 0; i < sectionMatches.Count; i++)
        {
            var sectionName = sectionMatches[i].Groups[1].Value.Trim();
            var startIndex = sectionMatches[i].Index;
            var endIndex = i + 1 < sectionMatches.Count ?
                sectionMatches[i + 1].Index : content.Length;

            var sectionContent = content.Substring(startIndex, endIndex - startIndex);
            sections[sectionName] = sectionContent;
        }

        return sections;
    }
}