using System.Text.Json.Serialization;

namespace FhirIntegrationService.ValidationPipeline.Models;

/// <summary>
/// Comprehensive result from FHIR validation pipeline execution
/// </summary>
public class ValidationPipelineResult
{
    /// <summary>
    /// Overall pipeline execution status
    /// </summary>
    [JsonPropertyName("overallStatus")]
    public ValidationStatus OverallStatus { get; set; } = ValidationStatus.Unknown;

    /// <summary>
    /// Pipeline execution start time
    /// </summary>
    [JsonPropertyName("executionStartTime")]
    public DateTime ExecutionStartTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Pipeline execution end time
    /// </summary>
    [JsonPropertyName("executionEndTime")]
    public DateTime? ExecutionEndTime { get; set; }

    /// <summary>
    /// Total pipeline execution duration
    /// </summary>
    [JsonPropertyName("executionDuration")]
    public TimeSpan? ExecutionDuration => ExecutionEndTime?.Subtract(ExecutionStartTime);

    /// <summary>
    /// FHIR technical validation results
    /// </summary>
    [JsonPropertyName("technicalValidation")]
    public TechnicalValidationResult TechnicalValidation { get; set; } = new();

    /// <summary>
    /// Resource validation results
    /// </summary>
    [JsonPropertyName("resourceValidation")]
    public ResourceValidationResult ResourceValidation { get; set; } = new();

    /// <summary>
    /// Content validation results
    /// </summary>
    [JsonPropertyName("contentValidation")]
    public ContentValidationResult ContentValidation { get; set; } = new();

    /// <summary>
    /// Security validation results
    /// </summary>
    [JsonPropertyName("securityValidation")]
    public SecurityValidationResult SecurityValidation { get; set; } = new();

    /// <summary>
    /// Publication readiness validation results
    /// </summary>
    [JsonPropertyName("publicationValidation")]
    public PublicationValidationResult PublicationValidation { get; set; } = new();

    /// <summary>
    /// Quality gate compliance assessment
    /// </summary>
    [JsonPropertyName("qualityGateCompliance")]
    public QualityGateComplianceResult QualityGateCompliance { get; set; } = new();

    /// <summary>
    /// Validation recommendations and suggestions
    /// </summary>
    [JsonPropertyName("recommendations")]
    public List<ValidationRecommendation> Recommendations { get; set; } = new();

    /// <summary>
    /// Validation metrics and performance data
    /// </summary>
    [JsonPropertyName("metrics")]
    public ValidationMetrics Metrics { get; set; } = new();
}

/// <summary>
/// Validation execution status
/// </summary>
public enum ValidationStatus
{
    Unknown,
    Success,
    Warning,
    Error,
    Failed
}

/// <summary>
/// FHIR technical validation results
/// </summary>
public class TechnicalValidationResult
{
    [JsonPropertyName("status")]
    public ValidationStatus Status { get; set; } = ValidationStatus.Unknown;

    [JsonPropertyName("profileValidationResults")]
    public List<ProfileValidationResult> ProfileValidationResults { get; set; } = new();

    [JsonPropertyName("canonicalUrlValidation")]
    public CanonicalUrlValidationResult CanonicalUrlValidation { get; set; } = new();

    [JsonPropertyName("metadataValidation")]
    public MetadataValidationResult MetadataValidation { get; set; } = new();

    [JsonPropertyName("constraintValidation")]
    public ConstraintValidationResult ConstraintValidation { get; set; } = new();
}

/// <summary>
/// Individual FHIR profile validation result
/// </summary>
public class ProfileValidationResult
{
    [JsonPropertyName("profilePath")]
    public string ProfilePath { get; set; } = string.Empty;

    [JsonPropertyName("profileName")]
    public string ProfileName { get; set; } = string.Empty;

    [JsonPropertyName("canonicalUrl")]
    public string CanonicalUrl { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public ValidationStatus Status { get; set; }

    [JsonPropertyName("validationIssues")]
    public List<ValidationIssue> ValidationIssues { get; set; } = new();

    [JsonPropertyName("validationDuration")]
    public TimeSpan ValidationDuration { get; set; }
}

/// <summary>
/// Resource validation results
/// </summary>
public class ResourceValidationResult
{
    [JsonPropertyName("status")]
    public ValidationStatus Status { get; set; } = ValidationStatus.Unknown;

    [JsonPropertyName("resourceValidationResults")]
    public List<ResourceInstanceValidationResult> ResourceValidationResults { get; set; } = new();

    [JsonPropertyName("profileConformanceResults")]
    public List<ProfileConformanceResult> ProfileConformanceResults { get; set; } = new();
}

/// <summary>
/// Individual resource instance validation result
/// </summary>
public class ResourceInstanceValidationResult
{
    [JsonPropertyName("resourcePath")]
    public string ResourcePath { get; set; } = string.Empty;

    [JsonPropertyName("resourceType")]
    public string ResourceType { get; set; } = string.Empty;

    [JsonPropertyName("profileUrl")]
    public string ProfileUrl { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public ValidationStatus Status { get; set; }

    [JsonPropertyName("validationIssues")]
    public List<ValidationIssue> ValidationIssues { get; set; } = new();
}

/// <summary>
/// Content validation results
/// </summary>
public class ContentValidationResult
{
    [JsonPropertyName("status")]
    public ValidationStatus Status { get; set; } = ValidationStatus.Unknown;

    [JsonPropertyName("implementationGuideValidation")]
    public ImplementationGuideValidationResult ImplementationGuideValidation { get; set; } = new();

    [JsonPropertyName("narrativeValidation")]
    public NarrativeValidationResult NarrativeValidation { get; set; } = new();

    [JsonPropertyName("linkValidation")]
    public LinkValidationResult LinkValidation { get; set; } = new();
}

/// <summary>
/// Security validation results
/// </summary>
public class SecurityValidationResult
{
    [JsonPropertyName("status")]
    public ValidationStatus Status { get; set; } = ValidationStatus.Unknown;

    [JsonPropertyName("phiExposureCheck")]
    public PhiExposureCheckResult PhiExposureCheck { get; set; } = new();

    [JsonPropertyName("accessControlValidation")]
    public AccessControlValidationResult AccessControlValidation { get; set; } = new();

    [JsonPropertyName("publicationSecurityAssessment")]
    public PublicationSecurityAssessmentResult PublicationSecurityAssessment { get; set; } = new();
}

/// <summary>
/// Publication readiness validation results
/// </summary>
public class PublicationValidationResult
{
    [JsonPropertyName("status")]
    public ValidationStatus Status { get; set; } = ValidationStatus.Unknown;

    [JsonPropertyName("packageStructureValidation")]
    public PackageStructureValidationResult PackageStructureValidation { get; set; } = new();

    [JsonPropertyName("simplifierReadiness")]
    public SimplifierReadinessResult SimplifierReadiness { get; set; } = new();

    [JsonPropertyName("versioningValidation")]
    public VersioningValidationResult VersioningValidation { get; set; } = new();
}

/// <summary>
/// Quality gate compliance assessment
/// </summary>
public class QualityGateComplianceResult
{
    [JsonPropertyName("overallCompliance")]
    public bool OverallCompliance { get; set; }

    [JsonPropertyName("gateResults")]
    public List<QualityGateResult> GateResults { get; set; } = new();

    [JsonPropertyName("blockingFailures")]
    public List<QualityGateResult> BlockingFailures { get; set; } = new();
}

/// <summary>
/// Individual quality gate result
/// </summary>
public class QualityGateResult
{
    [JsonPropertyName("gateName")]
    public string GateName { get; set; } = string.Empty;

    [JsonPropertyName("gateId")]
    public string GateId { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public ValidationStatus Status { get; set; }

    [JsonPropertyName("score")]
    public double Score { get; set; }

    [JsonPropertyName("passThreshold")]
    public double PassThreshold { get; set; }

    [JsonPropertyName("blocking")]
    public bool Blocking { get; set; }

    [JsonPropertyName("criteria")]
    public List<QualityGateCriterion> Criteria { get; set; } = new();
}

/// <summary>
/// Quality gate criterion assessment
/// </summary>
public class QualityGateCriterion
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public ValidationStatus Status { get; set; }

    [JsonPropertyName("actualValue")]
    public double ActualValue { get; set; }

    [JsonPropertyName("requiredValue")]
    public double RequiredValue { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Validation issue details
/// </summary>
public class ValidationIssue
{
    [JsonPropertyName("severity")]
    public string Severity { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("location")]
    public string Location { get; set; } = string.Empty;

    [JsonPropertyName("recommendation")]
    public string Recommendation { get; set; } = string.Empty;
}

/// <summary>
/// Validation recommendation
/// </summary>
public class ValidationRecommendation
{
    [JsonPropertyName("type")]
    public RecommendationType Type { get; set; }

    [JsonPropertyName("priority")]
    public RecommendationPriority Priority { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("actionItems")]
    public List<string> ActionItems { get; set; } = new();

    [JsonPropertyName("references")]
    public List<string> References { get; set; } = new();
}

/// <summary>
/// Validation metrics
/// </summary>
public class ValidationMetrics
{
    [JsonPropertyName("totalProfilesValidated")]
    public int TotalProfilesValidated { get; set; }

    [JsonPropertyName("totalResourcesValidated")]
    public int TotalResourcesValidated { get; set; }

    [JsonPropertyName("totalValidationIssues")]
    public int TotalValidationIssues { get; set; }

    [JsonPropertyName("criticalIssues")]
    public int CriticalIssues { get; set; }

    [JsonPropertyName("warningIssues")]
    public int WarningIssues { get; set; }

    [JsonPropertyName("averageValidationTime")]
    public TimeSpan AverageValidationTime { get; set; }

    [JsonPropertyName("memoryUsage")]
    public long MemoryUsage { get; set; }
}

/// <summary>
/// Recommendation types
/// </summary>
public enum RecommendationType
{
    Technical,
    Clinical,
    Security,
    Content,
    Publication
}

/// <summary>
/// Recommendation priorities
/// </summary>
public enum RecommendationPriority
{
    Low,
    Medium,
    High,
    Critical
}

// Supporting result classes with minimal implementations
public class CanonicalUrlValidationResult
{
    [JsonPropertyName("status")]
    public ValidationStatus Status { get; set; } = ValidationStatus.Unknown;

    [JsonPropertyName("issues")]
    public List<ValidationIssue> Issues { get; set; } = new();
}

public class MetadataValidationResult
{
    [JsonPropertyName("status")]
    public ValidationStatus Status { get; set; } = ValidationStatus.Unknown;

    [JsonPropertyName("issues")]
    public List<ValidationIssue> Issues { get; set; } = new();
}

public class ConstraintValidationResult
{
    [JsonPropertyName("status")]
    public ValidationStatus Status { get; set; } = ValidationStatus.Unknown;

    [JsonPropertyName("issues")]
    public List<ValidationIssue> Issues { get; set; } = new();
}

public class ProfileConformanceResult
{
    [JsonPropertyName("profileUrl")]
    public string ProfileUrl { get; set; } = string.Empty;

    [JsonPropertyName("conformantResources")]
    public int ConformantResources { get; set; }

    [JsonPropertyName("totalResources")]
    public int TotalResources { get; set; }

    [JsonPropertyName("conformancePercentage")]
    public double ConformancePercentage => TotalResources > 0 ? (double)ConformantResources / TotalResources * 100 : 0;
}

public class ImplementationGuideValidationResult
{
    [JsonPropertyName("status")]
    public ValidationStatus Status { get; set; } = ValidationStatus.Unknown;

    [JsonPropertyName("issues")]
    public List<ValidationIssue> Issues { get; set; } = new();
}

public class NarrativeValidationResult
{
    [JsonPropertyName("status")]
    public ValidationStatus Status { get; set; } = ValidationStatus.Unknown;

    [JsonPropertyName("issues")]
    public List<ValidationIssue> Issues { get; set; } = new();
}

public class LinkValidationResult
{
    [JsonPropertyName("status")]
    public ValidationStatus Status { get; set; } = ValidationStatus.Unknown;

    [JsonPropertyName("issues")]
    public List<ValidationIssue> Issues { get; set; } = new();
}

public class PhiExposureCheckResult
{
    [JsonPropertyName("status")]
    public ValidationStatus Status { get; set; } = ValidationStatus.Unknown;

    [JsonPropertyName("issues")]
    public List<ValidationIssue> Issues { get; set; } = new();
}

public class AccessControlValidationResult
{
    [JsonPropertyName("status")]
    public ValidationStatus Status { get; set; } = ValidationStatus.Unknown;

    [JsonPropertyName("issues")]
    public List<ValidationIssue> Issues { get; set; } = new();
}

public class PublicationSecurityAssessmentResult
{
    [JsonPropertyName("status")]
    public ValidationStatus Status { get; set; } = ValidationStatus.Unknown;

    [JsonPropertyName("issues")]
    public List<ValidationIssue> Issues { get; set; } = new();
}

public class PackageStructureValidationResult
{
    [JsonPropertyName("status")]
    public ValidationStatus Status { get; set; } = ValidationStatus.Unknown;

    [JsonPropertyName("issues")]
    public List<ValidationIssue> Issues { get; set; } = new();
}

public class SimplifierReadinessResult
{
    [JsonPropertyName("status")]
    public ValidationStatus Status { get; set; } = ValidationStatus.Unknown;

    [JsonPropertyName("issues")]
    public List<ValidationIssue> Issues { get; set; } = new();
}

public class VersioningValidationResult
{
    [JsonPropertyName("status")]
    public ValidationStatus Status { get; set; } = ValidationStatus.Unknown;

    [JsonPropertyName("issues")]
    public List<ValidationIssue> Issues { get; set; } = new();
}