namespace FhirIntegrationService.ValidationSuite.Exceptions;

/// <summary>
/// Base exception for FHIR validation suite operations
/// </summary>
public class FhirValidationSuiteException : Exception
{
    public FhirValidationSuiteException() : base()
    {
    }

    public FhirValidationSuiteException(string message) : base(message)
    {
    }

    public FhirValidationSuiteException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when resource loading fails
/// </summary>
public class ResourceLoadException : FhirValidationSuiteException
{
    public string? ResourcePath { get; }

    public ResourceLoadException(string resourcePath) : base($"Failed to load resource from: {resourcePath}")
    {
        ResourcePath = resourcePath;
    }

    public ResourceLoadException(string resourcePath, string message) : base(message)
    {
        ResourcePath = resourcePath;
    }

    public ResourceLoadException(string resourcePath, string message, Exception innerException) : base(message, innerException)
    {
        ResourcePath = resourcePath;
    }
}

/// <summary>
/// Exception thrown when validation configuration is invalid
/// </summary>
public class ValidationConfigurationException : FhirValidationSuiteException
{
    public ValidationConfigurationException(string message) : base(message)
    {
    }

    public ValidationConfigurationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when report generation fails
/// </summary>
public class ReportGenerationException : FhirValidationSuiteException
{
    public string? ReportType { get; }

    public ReportGenerationException(string reportType, string message) : base(message)
    {
        ReportType = reportType;
    }

    public ReportGenerationException(string reportType, string message, Exception innerException) : base(message, innerException)
    {
        ReportType = reportType;
    }
}