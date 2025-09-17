namespace FhirIntegrationService.ValidationSuite.Models;

/// <summary>
/// Represents the result of validating multiple FHIR resources
/// </summary>
public class BatchValidationReport
{
    /// <summary>
    /// Unique identifier for this batch validation report
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Name or description of the validation batch
    /// </summary>
    public string BatchName { get; set; } = string.Empty;

    /// <summary>
    /// Individual validation results for each resource
    /// </summary>
    public List<ValidationResult> Results { get; set; } = new();

    /// <summary>
    /// Overall summary statistics
    /// </summary>
    public ValidationSummary Summary { get; set; } = new();

    /// <summary>
    /// Time taken to perform the entire batch validation
    /// </summary>
    public TimeSpan TotalValidationDuration { get; set; }

    /// <summary>
    /// Timestamp when batch validation was started
    /// </summary>
    public DateTime ValidationStartTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when batch validation was completed
    /// </summary>
    public DateTime ValidationEndTime { get; set; }

    /// <summary>
    /// Configuration used for this validation batch
    /// </summary>
    public ValidationConfiguration Configuration { get; set; } = new();

    /// <summary>
    /// Performance metrics for the batch validation
    /// </summary>
    public ValidationPerformanceMetrics PerformanceMetrics { get; set; } = new();
}

/// <summary>
/// Summary statistics for batch validation
/// </summary>
public class ValidationSummary
{
    /// <summary>
    /// Total number of resources validated
    /// </summary>
    public int TotalResources { get; set; }

    /// <summary>
    /// Number of resources that passed validation
    /// </summary>
    public int PassedResources { get; set; }

    /// <summary>
    /// Number of resources that failed validation
    /// </summary>
    public int FailedResources { get; set; }

    /// <summary>
    /// Number of resources with warnings
    /// </summary>
    public int WarningResources { get; set; }

    /// <summary>
    /// Total number of issues found across all resources
    /// </summary>
    public int TotalIssues { get; set; }

    /// <summary>
    /// Issue count by severity level
    /// </summary>
    public Dictionary<string, int> IssuesBySeverity { get; set; } = new();

    /// <summary>
    /// Issue count by resource type
    /// </summary>
    public Dictionary<string, int> IssuesByResourceType { get; set; } = new();

    /// <summary>
    /// Pass rate as a percentage
    /// </summary>
    public double PassRate => TotalResources > 0 ? (double)PassedResources / TotalResources * 100 : 0;

    /// <summary>
    /// Whether the overall batch validation passed based on configured thresholds
    /// </summary>
    public bool OverallSuccess { get; set; }
}

/// <summary>
/// Configuration for validation operations
/// </summary>
public class ValidationConfiguration
{
    /// <summary>
    /// Profile URLs to validate against
    /// </summary>
    public List<string> ProfileUrls { get; set; } = new();

    /// <summary>
    /// Whether to include warnings in validation results
    /// </summary>
    public bool IncludeWarnings { get; set; } = true;

    /// <summary>
    /// Whether to include informational messages
    /// </summary>
    public bool IncludeInformation { get; set; } = false;

    /// <summary>
    /// Maximum number of issues to report per resource
    /// </summary>
    public int MaxIssuesPerResource { get; set; } = 100;

    /// <summary>
    /// Minimum pass rate threshold for overall success
    /// </summary>
    public double MinimumPassRateThreshold { get; set; } = 95.0;

    /// <summary>
    /// Maximum allowed fatal errors for overall success
    /// </summary>
    public int MaximumFatalErrors { get; set; } = 0;

    /// <summary>
    /// Timeout for individual resource validation
    /// </summary>
    public TimeSpan ValidationTimeout { get; set; } = TimeSpan.FromSeconds(30);
}

/// <summary>
/// Performance metrics for validation operations
/// </summary>
public class ValidationPerformanceMetrics
{
    /// <summary>
    /// Average validation time per resource
    /// </summary>
    public TimeSpan AverageValidationTime { get; set; }

    /// <summary>
    /// Minimum validation time recorded
    /// </summary>
    public TimeSpan MinimumValidationTime { get; set; }

    /// <summary>
    /// Maximum validation time recorded
    /// </summary>
    public TimeSpan MaximumValidationTime { get; set; }

    /// <summary>
    /// Resources validated per second
    /// </summary>
    public double ResourcesPerSecond { get; set; }

    /// <summary>
    /// Memory usage during validation
    /// </summary>
    public long MemoryUsageBytes { get; set; }

    /// <summary>
    /// Number of concurrent validation operations
    /// </summary>
    public int ConcurrentOperations { get; set; }
}

/// <summary>
/// Progress information for batch validation operations
/// </summary>
public class BatchValidationProgress
{
    /// <summary>
    /// Current resource being processed
    /// </summary>
    public int CurrentResource { get; set; }

    /// <summary>
    /// Total number of resources to process
    /// </summary>
    public int TotalResources { get; set; }

    /// <summary>
    /// Name of the current resource being validated
    /// </summary>
    public string CurrentResourceName { get; set; } = string.Empty;

    /// <summary>
    /// Progress percentage (0-100)
    /// </summary>
    public double ProgressPercentage => TotalResources > 0 ? (double)CurrentResource / TotalResources * 100 : 0;

    /// <summary>
    /// Estimated time remaining
    /// </summary>
    public TimeSpan EstimatedTimeRemaining { get; set; }

    /// <summary>
    /// Current validation stage
    /// </summary>
    public string CurrentStage { get; set; } = string.Empty;
}