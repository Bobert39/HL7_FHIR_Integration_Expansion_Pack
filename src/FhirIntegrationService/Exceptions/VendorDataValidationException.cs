namespace FhirIntegrationService.Exceptions;

/// <summary>
/// Exception thrown when vendor data is invalid or malformed
/// </summary>
public class VendorDataValidationException : DataMappingException
{
    /// <summary>
    /// The validation rules that failed
    /// </summary>
    public List<string> FailedValidationRules { get; } = new();

    /// <summary>
    /// The data type expected for the field
    /// </summary>
    public string? ExpectedDataType { get; }

    /// <summary>
    /// The actual data type received
    /// </summary>
    public string? ActualDataType { get; }

    /// <summary>
    /// Initializes a new instance of the VendorDataValidationException
    /// </summary>
    public VendorDataValidationException() : base("Vendor data validation failed.")
    {
    }

    /// <summary>
    /// Initializes a new instance with a specific message
    /// </summary>
    /// <param name="message">Error message</param>
    public VendorDataValidationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance with a message and inner exception
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    public VendorDataValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance with detailed validation context
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="fieldName">Field that failed validation</param>
    /// <param name="expectedDataType">Expected data type</param>
    /// <param name="actualDataType">Actual data type received</param>
    public VendorDataValidationException(string message, string fieldName, string expectedDataType, string actualDataType)
        : base(message, fieldName)
    {
        ExpectedDataType = expectedDataType;
        ActualDataType = actualDataType;
        AddContext("ExpectedDataType", expectedDataType);
        AddContext("ActualDataType", actualDataType);
    }

    /// <summary>
    /// Initializes a new instance with failed validation rules
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="fieldName">Field that failed validation</param>
    /// <param name="failedValidationRules">List of validation rules that failed</param>
    public VendorDataValidationException(string message, string fieldName, List<string> failedValidationRules)
        : base(message, fieldName)
    {
        FailedValidationRules.AddRange(failedValidationRules);
        AddContext("FailedValidationRules", string.Join(", ", failedValidationRules));
    }

    /// <summary>
    /// Adds a failed validation rule to the exception
    /// </summary>
    /// <param name="rule">The validation rule that failed</param>
    public void AddFailedValidationRule(string rule)
    {
        FailedValidationRules.Add(rule);
        AddContext("FailedValidationRules", string.Join(", ", FailedValidationRules));
    }

    /// <summary>
    /// Gets a detailed validation error summary
    /// </summary>
    /// <returns>Detailed validation error information</returns>
    public string GetValidationSummary()
    {
        var summary = GetSanitizedMessage();

        if (!string.IsNullOrEmpty(ExpectedDataType) && !string.IsNullOrEmpty(ActualDataType))
        {
            summary += $" Expected: {ExpectedDataType}, Received: {ActualDataType}";
        }

        if (FailedValidationRules.Any())
        {
            summary += $" Failed rules: {string.Join(", ", FailedValidationRules)}";
        }

        return summary;
    }
}