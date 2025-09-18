using System.Text.Json.Serialization;

namespace FhirIntegrationService.ValidationPipeline.Models;

/// <summary>
/// Configuration for FHIR validation pipeline execution
/// </summary>
public class ValidationPipelineConfiguration
{
    /// <summary>
    /// Directory containing FHIR profiles to validate
    /// </summary>
    [JsonPropertyName("profileDirectory")]
    public string ProfileDirectory { get; set; } = "src/";

    /// <summary>
    /// Directory containing example FHIR resources
    /// </summary>
    [JsonPropertyName("exampleResourceDirectory")]
    public string ExampleResourceDirectory { get; set; } = "docs/examples/";

    /// <summary>
    /// Directory containing Implementation Guide documentation
    /// </summary>
    [JsonPropertyName("implementationGuideDirectory")]
    public string ImplementationGuideDirectory { get; set; } = "docs/implementation-guide/";

    /// <summary>
    /// Path to quality gate configuration file
    /// </summary>
    [JsonPropertyName("qualityGateConfigPath")]
    public string QualityGateConfigPath { get; set; } = "docs/qa/gates/5.3-implementation-guide-publication.yml";

    /// <summary>
    /// Output directory for validation reports
    /// </summary>
    [JsonPropertyName("outputDirectory")]
    public string OutputDirectory { get; set; } = "validation-output/";

    /// <summary>
    /// FHIR version for validation
    /// </summary>
    [JsonPropertyName("fhirVersion")]
    public string FhirVersion { get; set; } = "4.0.1";

    /// <summary>
    /// Enable parallel validation execution
    /// </summary>
    [JsonPropertyName("enableParallelValidation")]
    public bool EnableParallelValidation { get; set; } = true;

    /// <summary>
    /// Maximum number of parallel validation tasks
    /// </summary>
    [JsonPropertyName("maxParallelTasks")]
    public int MaxParallelTasks { get; set; } = Environment.ProcessorCount;

    /// <summary>
    /// Validation timeout in seconds
    /// </summary>
    [JsonPropertyName("validationTimeoutSeconds")]
    public int ValidationTimeoutSeconds { get; set; } = 300;

    /// <summary>
    /// Enable verbose logging
    /// </summary>
    [JsonPropertyName("verboseLogging")]
    public bool VerboseLogging { get; set; } = false;

    /// <summary>
    /// Include recommendations in validation report
    /// </summary>
    [JsonPropertyName("includeRecommendations")]
    public bool IncludeRecommendations { get; set; } = true;

    /// <summary>
    /// Fail on warnings
    /// </summary>
    [JsonPropertyName("failOnWarnings")]
    public bool FailOnWarnings { get; set; } = false;

    /// <summary>
    /// Custom validation rules directory
    /// </summary>
    [JsonPropertyName("customValidationRulesDirectory")]
    public string? CustomValidationRulesDirectory { get; set; }

    /// <summary>
    /// Canonical URL base for validation
    /// </summary>
    [JsonPropertyName("canonicalUrlBase")]
    public string CanonicalUrlBase { get; set; } = "http://example.org/fhir/ig/hl7-fhir-expansion-pack";

    /// <summary>
    /// FHIR package configuration
    /// </summary>
    [JsonPropertyName("packageConfiguration")]
    public FhirPackageConfiguration PackageConfiguration { get; set; } = new();

    /// <summary>
    /// Security validation configuration
    /// </summary>
    [JsonPropertyName("securityConfiguration")]
    public SecurityValidationConfiguration SecurityConfiguration { get; set; } = new();

    /// <summary>
    /// Publication validation configuration
    /// </summary>
    [JsonPropertyName("publicationConfiguration")]
    public PublicationValidationConfiguration PublicationConfiguration { get; set; } = new();
}

/// <summary>
/// FHIR package configuration
/// </summary>
public class FhirPackageConfiguration
{
    /// <summary>
    /// Package name
    /// </summary>
    [JsonPropertyName("packageName")]
    public string PackageName { get; set; } = "hl7.fhir.expansion.pack";

    /// <summary>
    /// Package version
    /// </summary>
    [JsonPropertyName("packageVersion")]
    public string PackageVersion { get; set; } = "1.0.0";

    /// <summary>
    /// Package description
    /// </summary>
    [JsonPropertyName("packageDescription")]
    public string PackageDescription { get; set; } = "HL7 FHIR Integration Expansion Pack";

    /// <summary>
    /// Package dependencies
    /// </summary>
    [JsonPropertyName("dependencies")]
    public Dictionary<string, string> Dependencies { get; set; } = new()
    {
        { "hl7.fhir.r4.core", "4.0.1" }
    };

    /// <summary>
    /// Package output directory
    /// </summary>
    [JsonPropertyName("outputDirectory")]
    public string OutputDirectory { get; set; } = "fhir-package/";
}

/// <summary>
/// Security validation configuration
/// </summary>
public class SecurityValidationConfiguration
{
    /// <summary>
    /// Enable PHI exposure scanning
    /// </summary>
    [JsonPropertyName("enablePhiScanning")]
    public bool EnablePhiScanning { get; set; } = true;

    /// <summary>
    /// PHI detection patterns
    /// </summary>
    [JsonPropertyName("phiPatterns")]
    public List<string> PhiPatterns { get; set; } = new()
    {
        @"\b\d{3}-\d{2}-\d{4}\b", // SSN pattern
        @"\b[A-Z]{2}\d{7}\b", // Medical record number pattern
        @"\b\d{3}-\d{3}-\d{4}\b", // Phone number pattern
        @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b" // Email pattern
    };

    /// <summary>
    /// Enable access control validation
    /// </summary>
    [JsonPropertyName("enableAccessControlValidation")]
    public bool EnableAccessControlValidation { get; set; } = true;

    /// <summary>
    /// Enable publication security assessment
    /// </summary>
    [JsonPropertyName("enablePublicationSecurityAssessment")]
    public bool EnablePublicationSecurityAssessment { get; set; } = true;

    /// <summary>
    /// Security requirement checklist
    /// </summary>
    [JsonPropertyName("securityRequirements")]
    public List<string> SecurityRequirements { get; set; } = new()
    {
        "No PHI in documentation",
        "Appropriate access controls configured",
        "Encryption requirements documented",
        "Audit logging specified"
    };
}

/// <summary>
/// Publication validation configuration
/// </summary>
public class PublicationValidationConfiguration
{
    /// <summary>
    /// Target publication platform
    /// </summary>
    [JsonPropertyName("targetPlatform")]
    public string TargetPlatform { get; set; } = "Simplifier.net";

    /// <summary>
    /// Enable package structure validation
    /// </summary>
    [JsonPropertyName("enablePackageStructureValidation")]
    public bool EnablePackageStructureValidation { get; set; } = true;

    /// <summary>
    /// Enable Simplifier.net readiness check
    /// </summary>
    [JsonPropertyName("enableSimplifierReadinessCheck")]
    public bool EnableSimplifierReadinessCheck { get; set; } = true;

    /// <summary>
    /// Enable versioning validation
    /// </summary>
    [JsonPropertyName("enableVersioningValidation")]
    public bool EnableVersioningValidation { get; set; } = true;

    /// <summary>
    /// Required package components
    /// </summary>
    [JsonPropertyName("requiredPackageComponents")]
    public List<string> RequiredPackageComponents { get; set; } = new()
    {
        "package.json",
        "profiles/",
        "examples/",
        "valuesets/"
    };

    /// <summary>
    /// Publication checklist items
    /// </summary>
    [JsonPropertyName("publicationChecklist")]
    public List<string> PublicationChecklist { get; set; } = new()
    {
        "All profiles validate against FHIR R4",
        "Canonical URLs are unique and accessible",
        "Example resources conform to profiles",
        "Implementation Guide is complete",
        "Security requirements are met"
    };
}

/// <summary>
/// Command line options for validation pipeline
/// </summary>
public class ValidationCommandLineOptions
{
    [CommandLine.Option('p', "profiles", Required = false, HelpText = "Directory containing FHIR profiles")]
    public string? ProfileDirectory { get; set; }

    [CommandLine.Option('e', "examples", Required = false, HelpText = "Directory containing example resources")]
    public string? ExampleResourceDirectory { get; set; }

    [CommandLine.Option('i', "implementation-guide", Required = false, HelpText = "Directory containing Implementation Guide")]
    public string? ImplementationGuideDirectory { get; set; }

    [CommandLine.Option('o', "output", Required = false, HelpText = "Output directory for validation reports")]
    public string? OutputDirectory { get; set; }

    [CommandLine.Option('c', "config", Required = false, HelpText = "Path to configuration file")]
    public string? ConfigurationFile { get; set; }

    [CommandLine.Option("gate-config", Required = false, HelpText = "Path to quality gate configuration")]
    public string? QualityGateConfigPath { get; set; }

    [CommandLine.Option("check-gates", Required = false, HelpText = "Path to validation report for quality gate checking")]
    public string? CheckGatesReportPath { get; set; }

    [CommandLine.Option('v', "verbose", Required = false, HelpText = "Enable verbose logging")]
    public bool VerboseLogging { get; set; } = false;

    [CommandLine.Option("parallel", Required = false, HelpText = "Enable parallel validation")]
    public bool EnableParallelValidation { get; set; } = true;

    [CommandLine.Option("fail-on-warnings", Required = false, HelpText = "Fail validation on warnings")]
    public bool FailOnWarnings { get; set; } = false;

    [CommandLine.Option("timeout", Required = false, HelpText = "Validation timeout in seconds")]
    public int ValidationTimeoutSeconds { get; set; } = 300;
}