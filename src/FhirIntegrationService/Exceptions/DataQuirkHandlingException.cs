namespace FhirIntegrationService.Exceptions;

/// <summary>
/// Exception thrown when vendor-specific data quirk handling fails
/// </summary>
public class DataQuirkHandlingException : DataMappingException
{
    /// <summary>
    /// The type of data quirk that failed processing
    /// </summary>
    public string? QuirkType { get; }

    /// <summary>
    /// The vendor-specific rule or pattern that was being applied
    /// </summary>
    public string? VendorRule { get; }

    /// <summary>
    /// Initializes a new instance of the DataQuirkHandlingException
    /// </summary>
    public DataQuirkHandlingException() : base("A data quirk handling error occurred.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the DataQuirkHandlingException with a specific message
    /// </summary>
    /// <param name="message">Error message</param>
    public DataQuirkHandlingException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the DataQuirkHandlingException with a message and inner exception
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    public DataQuirkHandlingException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance with detailed quirk handling context
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="quirkType">Type of data quirk being handled</param>
    /// <param name="fieldName">Field that caused the error</param>
    /// <param name="vendorRule">Vendor-specific rule being applied</param>
    public DataQuirkHandlingException(string message, string quirkType, string fieldName, string? vendorRule = null)
        : base(message, fieldName)
    {
        QuirkType = quirkType;
        VendorRule = vendorRule;
    }

    /// <summary>
    /// Initializes a new instance with detailed context and inner exception
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="quirkType">Type of data quirk being handled</param>
    /// <param name="fieldName">Field that caused the error</param>
    /// <param name="vendorRule">Vendor-specific rule being applied</param>
    /// <param name="innerException">Inner exception</param>
    public DataQuirkHandlingException(string message, string quirkType, string fieldName, string? vendorRule, Exception innerException)
        : base(message, fieldName, vendorRule, innerException)
    {
        QuirkType = quirkType;
        VendorRule = vendorRule;
    }

    /// <summary>
    /// Creates a sanitized error message safe for logging (no PHI)
    /// </summary>
    /// <returns>Sanitized error message with quirk context</returns>
    public override string GetSanitizedMessage()
    {
        var sanitized = base.GetSanitizedMessage();

        if (!string.IsNullOrEmpty(QuirkType))
        {
            sanitized += $" QuirkType: {QuirkType}";
        }

        if (!string.IsNullOrEmpty(VendorRule))
        {
            sanitized += $" VendorRule: {VendorRule}";
        }

        return sanitized;
    }
}