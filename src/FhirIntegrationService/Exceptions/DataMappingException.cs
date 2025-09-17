namespace FhirIntegrationService.Exceptions;

/// <summary>
/// Base exception for all data mapping and transformation failures
/// </summary>
public class DataMappingException : Exception
{
    /// <summary>
    /// The field or property that caused the mapping failure
    /// </summary>
    public string? FieldName { get; }

    /// <summary>
    /// The vendor data value that caused the failure (sanitized for logging)
    /// </summary>
    public string? SanitizedValue { get; }

    /// <summary>
    /// Additional context about the mapping failure
    /// </summary>
    public Dictionary<string, object> Context { get; } = new();

    /// <summary>
    /// Initializes a new instance of the DataMappingException
    /// </summary>
    public DataMappingException() : base("A data mapping error occurred.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the DataMappingException with a specific message
    /// </summary>
    /// <param name="message">Error message</param>
    public DataMappingException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the DataMappingException with a message and inner exception
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    public DataMappingException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the DataMappingException with detailed context
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="fieldName">Field that caused the error</param>
    /// <param name="sanitizedValue">Sanitized value (no PHI)</param>
    public DataMappingException(string message, string fieldName, string? sanitizedValue = null) : base(message)
    {
        FieldName = fieldName;
        SanitizedValue = sanitizedValue;
    }

    /// <summary>
    /// Initializes a new instance with detailed context and inner exception
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="fieldName">Field that caused the error</param>
    /// <param name="sanitizedValue">Sanitized value (no PHI)</param>
    /// <param name="innerException">Inner exception</param>
    public DataMappingException(string message, string fieldName, string? sanitizedValue, Exception innerException)
        : base(message, innerException)
    {
        FieldName = fieldName;
        SanitizedValue = sanitizedValue;
    }

    /// <summary>
    /// Adds context information to the exception
    /// </summary>
    /// <param name="key">Context key</param>
    /// <param name="value">Context value (ensure no PHI)</param>
    public void AddContext(string key, object value)
    {
        Context[key] = value;
    }

    /// <summary>
    /// Creates a sanitized error message safe for logging (no PHI)
    /// </summary>
    /// <returns>Sanitized error message</returns>
    public string GetSanitizedMessage()
    {
        var sanitized = Message;

        if (!string.IsNullOrEmpty(FieldName))
        {
            sanitized += $" Field: {FieldName}";
        }

        if (!string.IsNullOrEmpty(SanitizedValue))
        {
            sanitized += $" Value type: {SanitizedValue}";
        }

        if (Context.Any())
        {
            var contextInfo = string.Join(", ", Context.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
            sanitized += $" Context: {contextInfo}";
        }

        return sanitized;
    }
}