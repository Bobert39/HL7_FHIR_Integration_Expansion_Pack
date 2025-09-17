namespace FhirIntegrationService.Exceptions;

/// <summary>
/// Exception thrown when vendor API operations fail
/// </summary>
public class VendorApiException : Exception
{
    /// <summary>
    /// HTTP status code from the vendor API response
    /// </summary>
    public int? StatusCode { get; }

    /// <summary>
    /// Response content from the vendor API
    /// </summary>
    public string? ResponseContent { get; }

    /// <summary>
    /// Vendor API endpoint that failed
    /// </summary>
    public string? Endpoint { get; }

    /// <summary>
    /// Initializes a new instance of VendorApiException
    /// </summary>
    /// <param name="message">Exception message</param>
    public VendorApiException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of VendorApiException
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exception</param>
    public VendorApiException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of VendorApiException with detailed information
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <param name="responseContent">Response content</param>
    /// <param name="endpoint">Failed endpoint</param>
    public VendorApiException(string message, int? statusCode, string? responseContent, string? endpoint) : base(message)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
        Endpoint = endpoint;
    }

    /// <summary>
    /// Initializes a new instance of VendorApiException with detailed information and inner exception
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <param name="responseContent">Response content</param>
    /// <param name="endpoint">Failed endpoint</param>
    /// <param name="innerException">Inner exception</param>
    public VendorApiException(string message, int? statusCode, string? responseContent, string? endpoint, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
        Endpoint = endpoint;
    }
}