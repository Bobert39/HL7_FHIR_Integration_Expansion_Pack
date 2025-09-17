using System.ComponentModel.DataAnnotations;

namespace FhirIntegrationService.Configuration;

/// <summary>
/// Configuration settings for data mapping and transformation operations
/// </summary>
public class DataMappingConfiguration
{
    /// <summary>
    /// Configuration section name in appsettings.json
    /// </summary>
    public const string SectionName = "DataMapping";

    /// <summary>
    /// Maximum number of concurrent transformation operations
    /// </summary>
    [Range(1, 100)]
    public int MaxConcurrentTransformations { get; set; } = 10;

    /// <summary>
    /// Timeout for individual transformation operations in milliseconds
    /// </summary>
    [Range(1000, 300000)] // 1 second to 5 minutes
    public int TransformationTimeoutMs { get; set; } = 30000;

    /// <summary>
    /// Maximum batch size for bulk transformations
    /// </summary>
    [Range(1, 1000)]
    public int MaxBatchSize { get; set; } = 100;

    /// <summary>
    /// Whether to enable performance metrics collection
    /// </summary>
    public bool EnablePerformanceMetrics { get; set; } = true;

    /// <summary>
    /// Whether to enable detailed transformation logging
    /// </summary>
    public bool EnableDetailedLogging { get; set; } = false;

    /// <summary>
    /// Whether to fail fast on the first mapping error in a batch
    /// </summary>
    public bool FailFastOnError { get; set; } = false;

    /// <summary>
    /// Default country code for address normalization
    /// </summary>
    [StringLength(2, MinimumLength = 2)]
    public string DefaultCountryCode { get; set; } = "US";

    /// <summary>
    /// Default phone country prefix for phone number normalization
    /// </summary>
    [StringLength(4, MinimumLength = 1)]
    public string DefaultPhoneCountryPrefix { get; set; } = "+1";

    /// <summary>
    /// Vendor-specific configuration settings
    /// </summary>
    public VendorMappingConfiguration Vendor { get; set; } = new();

    /// <summary>
    /// Data validation configuration
    /// </summary>
    public ValidationConfiguration Validation { get; set; } = new();

    /// <summary>
    /// Performance thresholds and limits
    /// </summary>
    public PerformanceConfiguration Performance { get; set; } = new();
}

/// <summary>
/// Vendor-specific mapping configuration
/// </summary>
public class VendorMappingConfiguration
{
    /// <summary>
    /// Vendor system identifier
    /// </summary>
    public string SystemId { get; set; } = "unknown-vendor";

    /// <summary>
    /// Vendor system version
    /// </summary>
    public string SystemVersion { get; set; } = "1.0";

    /// <summary>
    /// Whether to apply vendor-specific quirks handling
    /// </summary>
    public bool EnableQuirksHandling { get; set; } = true;

    /// <summary>
    /// Whether to use strict field validation for vendor data
    /// </summary>
    public bool UseStrictValidation { get; set; } = true;

    /// <summary>
    /// Custom field mappings for vendor-specific fields
    /// </summary>
    public Dictionary<string, string> CustomFieldMappings { get; set; } = new();

    /// <summary>
    /// Vendor-specific date formats to try during parsing
    /// </summary>
    public List<string> DateFormats { get; set; } = new()
    {
        "yyyy-MM-dd",
        "MM/dd/yyyy",
        "dd/MM/yyyy",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-ddTHH:mm:ssZ",
        "yyyyMMdd"
    };

    /// <summary>
    /// Vendor-specific gender code mappings
    /// </summary>
    public Dictionary<string, string> GenderMappings { get; set; } = new()
    {
        { "M", "male" },
        { "F", "female" },
        { "O", "other" },
        { "U", "unknown" }
    };

    /// <summary>
    /// Vendor-specific observation status mappings
    /// </summary>
    public Dictionary<string, string> ObservationStatusMappings { get; set; } = new()
    {
        { "complete", "final" },
        { "partial", "preliminary" },
        { "reviewed", "final" }
    };
}

/// <summary>
/// Data validation configuration
/// </summary>
public class ValidationConfiguration
{
    /// <summary>
    /// Whether to validate email addresses using strict regex
    /// </summary>
    public bool ValidateEmailFormat { get; set; } = true;

    /// <summary>
    /// Whether to validate phone numbers using international formats
    /// </summary>
    public bool ValidatePhoneFormat { get; set; } = true;

    /// <summary>
    /// Whether to validate postal codes based on country
    /// </summary>
    public bool ValidatePostalCodes { get; set; } = false;

    /// <summary>
    /// Minimum required fields for patient records
    /// </summary>
    public List<string> RequiredPatientFields { get; set; } = new()
    {
        "PatientId"
    };

    /// <summary>
    /// Minimum required fields for observation records
    /// </summary>
    public List<string> RequiredObservationFields { get; set; } = new()
    {
        "PatientId",
        "ObservationType"
    };

    /// <summary>
    /// Maximum length for text fields to prevent data overflow
    /// </summary>
    public Dictionary<string, int> FieldMaxLengths { get; set; } = new()
    {
        { "FirstName", 100 },
        { "LastName", 100 },
        { "Email", 254 },
        { "PhoneNumber", 20 },
        { "Street", 200 },
        { "City", 100 },
        { "State", 50 },
        { "PostalCode", 20 }
    };
}

/// <summary>
/// Performance configuration and thresholds
/// </summary>
public class PerformanceConfiguration
{
    /// <summary>
    /// Maximum allowed transformation time in milliseconds before warning
    /// </summary>
    [Range(100, 10000)]
    public int WarningThresholdMs { get; set; } = 1000;

    /// <summary>
    /// Maximum allowed transformation time in milliseconds before error
    /// </summary>
    [Range(1000, 60000)]
    public int ErrorThresholdMs { get; set; } = 5000;

    /// <summary>
    /// Maximum memory usage per transformation in MB
    /// </summary>
    [Range(1, 1000)]
    public int MaxMemoryUsageMB { get; set; } = 100;

    /// <summary>
    /// Whether to enable memory pressure monitoring
    /// </summary>
    public bool EnableMemoryMonitoring { get; set; } = true;

    /// <summary>
    /// Sample rate for performance metrics (0.0 to 1.0)
    /// </summary>
    [Range(0.0, 1.0)]
    public double MetricsSampleRate { get; set; } = 0.1;

    /// <summary>
    /// Whether to enable caching of transformation results
    /// </summary>
    public bool EnableCaching { get; set; } = false;

    /// <summary>
    /// Cache TTL in minutes for transformation results
    /// </summary>
    [Range(1, 1440)] // 1 minute to 24 hours
    public int CacheTtlMinutes { get; set; } = 60;
}

/// <summary>
/// Strongly-typed options for data mapping configuration
/// </summary>
public class DataMappingOptions
{
    /// <summary>
    /// Data mapping configuration
    /// </summary>
    public DataMappingConfiguration DataMapping { get; set; } = new();

    /// <summary>
    /// Validates the configuration settings
    /// </summary>
    /// <returns>List of validation errors, empty if valid</returns>
    public List<string> Validate()
    {
        var errors = new List<string>();

        if (DataMapping.MaxConcurrentTransformations <= 0)
        {
            errors.Add("MaxConcurrentTransformations must be greater than 0");
        }

        if (DataMapping.TransformationTimeoutMs < 1000)
        {
            errors.Add("TransformationTimeoutMs must be at least 1000ms");
        }

        if (DataMapping.MaxBatchSize <= 0)
        {
            errors.Add("MaxBatchSize must be greater than 0");
        }

        if (string.IsNullOrWhiteSpace(DataMapping.DefaultCountryCode) ||
            DataMapping.DefaultCountryCode.Length != 2)
        {
            errors.Add("DefaultCountryCode must be a valid 2-character country code");
        }

        if (string.IsNullOrWhiteSpace(DataMapping.DefaultPhoneCountryPrefix))
        {
            errors.Add("DefaultPhoneCountryPrefix cannot be empty");
        }

        if (DataMapping.Performance.WarningThresholdMs >= DataMapping.Performance.ErrorThresholdMs)
        {
            errors.Add("WarningThresholdMs must be less than ErrorThresholdMs");
        }

        if (DataMapping.Performance.MetricsSampleRate < 0.0 || DataMapping.Performance.MetricsSampleRate > 1.0)
        {
            errors.Add("MetricsSampleRate must be between 0.0 and 1.0");
        }

        return errors;
    }
}