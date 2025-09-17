namespace FhirIntegrationService.Exceptions;

/// <summary>
/// Exception thrown when FHIR resource creation or assembly fails
/// </summary>
public class FhirResourceCreationException : DataMappingException
{
    /// <summary>
    /// The FHIR resource type that failed to be created
    /// </summary>
    public string? FhirResourceType { get; }

    /// <summary>
    /// The profile URL that was being targeted
    /// </summary>
    public string? ProfileUrl { get; }

    /// <summary>
    /// List of FHIR validation issues
    /// </summary>
    public List<string> FhirValidationIssues { get; } = new();

    /// <summary>
    /// The step in the resource creation process where failure occurred
    /// </summary>
    public string? CreationStep { get; }

    /// <summary>
    /// Initializes a new instance of the FhirResourceCreationException
    /// </summary>
    public FhirResourceCreationException() : base("FHIR resource creation failed.")
    {
    }

    /// <summary>
    /// Initializes a new instance with a specific message
    /// </summary>
    /// <param name="message">Error message</param>
    public FhirResourceCreationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance with a message and inner exception
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    public FhirResourceCreationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance with detailed FHIR context
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="fhirResourceType">FHIR resource type</param>
    /// <param name="profileUrl">Profile URL being targeted</param>
    /// <param name="creationStep">Step where creation failed</param>
    public FhirResourceCreationException(string message, string fhirResourceType, string? profileUrl = null, string? creationStep = null)
        : base(message)
    {
        FhirResourceType = fhirResourceType;
        ProfileUrl = profileUrl;
        CreationStep = creationStep;

        AddContext("FhirResourceType", fhirResourceType);
        if (!string.IsNullOrEmpty(profileUrl))
            AddContext("ProfileUrl", profileUrl);
        if (!string.IsNullOrEmpty(creationStep))
            AddContext("CreationStep", creationStep);
    }

    /// <summary>
    /// Initializes a new instance with FHIR validation issues
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="fhirResourceType">FHIR resource type</param>
    /// <param name="validationIssues">List of FHIR validation issues</param>
    /// <param name="profileUrl">Profile URL being validated against</param>
    public FhirResourceCreationException(string message, string fhirResourceType, List<string> validationIssues, string? profileUrl = null)
        : base(message)
    {
        FhirResourceType = fhirResourceType;
        ProfileUrl = profileUrl;
        FhirValidationIssues.AddRange(validationIssues);

        AddContext("FhirResourceType", fhirResourceType);
        AddContext("ValidationIssuesCount", validationIssues.Count);
        if (!string.IsNullOrEmpty(profileUrl))
            AddContext("ProfileUrl", profileUrl);
    }

    /// <summary>
    /// Adds a FHIR validation issue to the exception
    /// </summary>
    /// <param name="issue">The validation issue</param>
    public void AddValidationIssue(string issue)
    {
        FhirValidationIssues.Add(issue);
        AddContext("ValidationIssuesCount", FhirValidationIssues.Count);
    }

    /// <summary>
    /// Gets a detailed FHIR creation error summary
    /// </summary>
    /// <returns>Detailed FHIR creation error information</returns>
    public string GetFhirCreationSummary()
    {
        var summary = GetSanitizedMessage();

        if (!string.IsNullOrEmpty(FhirResourceType))
        {
            summary += $" Resource Type: {FhirResourceType}";
        }

        if (!string.IsNullOrEmpty(ProfileUrl))
        {
            summary += $" Profile: {ProfileUrl}";
        }

        if (!string.IsNullOrEmpty(CreationStep))
        {
            summary += $" Failed at: {CreationStep}";
        }

        if (FhirValidationIssues.Any())
        {
            summary += $" Validation issues ({FhirValidationIssues.Count}): {string.Join("; ", FhirValidationIssues.Take(3))}";
            if (FhirValidationIssues.Count > 3)
            {
                summary += $" and {FhirValidationIssues.Count - 3} more...";
            }
        }

        return summary;
    }
}