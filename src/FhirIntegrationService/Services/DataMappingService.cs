using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Validation;
using Microsoft.Extensions.Logging;
using FhirIntegrationService.Services.Interfaces;
using FhirIntegrationService.Exceptions;
using FhirIntegrationService.Configuration;

namespace FhirIntegrationService.Services;

/// <summary>
/// Service for mapping and transforming vendor data to FHIR resources
/// </summary>
public class DataMappingService : IDataMappingService
{
    private readonly ILogger<DataMappingService> _logger;
    private readonly FhirConfiguration _fhirConfiguration;
    private readonly Validator _fhirValidator;

    // Date format patterns commonly used by healthcare vendors
    private static readonly string[] DateFormats = {
        "yyyy-MM-dd",
        "MM/dd/yyyy",
        "dd/MM/yyyy",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-ddTHH:mm:ssZ",
        "yyyy-MM-ddTHH:mm:ss.fffZ",
        "yyyyMMdd",
        "MMddyyyy"
    };

    // Gender code mappings from common vendor formats to FHIR
    private static readonly Dictionary<string, AdministrativeGender> GenderMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        { "M", AdministrativeGender.Male },
        { "Male", AdministrativeGender.Male },
        { "1", AdministrativeGender.Male },
        { "F", AdministrativeGender.Female },
        { "Female", AdministrativeGender.Female },
        { "2", AdministrativeGender.Female },
        { "O", AdministrativeGender.Other },
        { "Other", AdministrativeGender.Other },
        { "3", AdministrativeGender.Other },
        { "U", AdministrativeGender.Unknown },
        { "Unknown", AdministrativeGender.Unknown },
        { "4", AdministrativeGender.Unknown },
        { "", AdministrativeGender.Unknown },
        { "NULL", AdministrativeGender.Unknown }
    };

    public DataMappingService(
        ILogger<DataMappingService> logger,
        FhirConfiguration fhirConfiguration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fhirConfiguration = fhirConfiguration ?? throw new ArgumentNullException(nameof(fhirConfiguration));
        _fhirValidator = new Validator();
    }

    /// <summary>
    /// Transforms vendor patient data to FHIR Patient resource
    /// </summary>
    public async Task<DataMappingResult<Patient>> TransformPatientAsync(VendorPatientData vendorPatientData)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new DataMappingResult<Patient>();

        try
        {
            _logger.LogInformation("Starting patient transformation for vendor patient ID: {PatientId}",
                SanitizeForLogging(vendorPatientData.PatientId));

            // Validate input data
            var validationResult = await ValidateVendorDataAsync(vendorPatientData);
            if (!validationResult.IsValid)
            {
                result.Errors.AddRange(validationResult.Errors);
                result.Warnings.AddRange(validationResult.Warnings);
                if (validationResult.Errors.Any())
                {
                    return result;
                }
            }

            var patient = new Patient();
            var fieldsMapped = 0;
            var quirksHandled = 0;

            // Map patient identifier
            if (!string.IsNullOrWhiteSpace(vendorPatientData.PatientId))
            {
                patient.Identifier.Add(new Identifier
                {
                    System = _fhirConfiguration.PatientIdentifierSystem,
                    Value = vendorPatientData.PatientId
                });
                fieldsMapped++;
            }

            // Map patient name
            if (!string.IsNullOrWhiteSpace(vendorPatientData.FirstName) ||
                !string.IsNullOrWhiteSpace(vendorPatientData.LastName))
            {
                patient.Name.Add(new HumanName
                {
                    Use = HumanName.NameUse.Official,
                    Given = !string.IsNullOrWhiteSpace(vendorPatientData.FirstName)
                        ? new[] { vendorPatientData.FirstName.Trim() }
                        : null,
                    Family = !string.IsNullOrWhiteSpace(vendorPatientData.LastName)
                        ? vendorPatientData.LastName.Trim()
                        : null
                });
                fieldsMapped++;
            }

            // Map date of birth with quirk handling
            if (!string.IsNullOrWhiteSpace(vendorPatientData.DateOfBirth))
            {
                var dateQuirkResult = await HandleDataQuirkAsync("DateOfBirth", vendorPatientData.DateOfBirth);
                if (dateQuirkResult.IsSuccess && dateQuirkResult.NormalizedValue is DateTime birthDate)
                {
                    patient.BirthDate = birthDate.ToString("yyyy-MM-dd");
                    fieldsMapped++;
                    if (!string.IsNullOrEmpty(dateQuirkResult.QuirkDescription))
                    {
                        quirksHandled++;
                    }
                }
                else
                {
                    result.Warnings.Add($"Could not parse date of birth: {dateQuirkResult.Notes.FirstOrDefault() ?? "Invalid format"}");
                }
            }

            // Map gender with quirk handling
            if (!string.IsNullOrWhiteSpace(vendorPatientData.Gender))
            {
                var genderQuirkResult = await HandleDataQuirkAsync("Gender", vendorPatientData.Gender);
                if (genderQuirkResult.IsSuccess && genderQuirkResult.NormalizedValue is AdministrativeGender gender)
                {
                    patient.Gender = gender;
                    fieldsMapped++;
                    if (!string.IsNullOrEmpty(genderQuirkResult.QuirkDescription))
                    {
                        quirksHandled++;
                    }
                }
                else
                {
                    result.Warnings.Add($"Could not map gender value: {genderQuirkResult.Notes.FirstOrDefault() ?? "Unknown format"}");
                }
            }

            // Map contact information
            var contactPoints = new List<ContactPoint>();

            // Phone number with quirk handling
            if (!string.IsNullOrWhiteSpace(vendorPatientData.PhoneNumber))
            {
                var phoneQuirkResult = await HandleDataQuirkAsync("PhoneNumber", vendorPatientData.PhoneNumber);
                if (phoneQuirkResult.IsSuccess && phoneQuirkResult.NormalizedValue is string normalizedPhone)
                {
                    contactPoints.Add(new ContactPoint
                    {
                        System = ContactPoint.ContactPointSystem.Phone,
                        Use = ContactPoint.ContactPointUse.Mobile,
                        Value = normalizedPhone
                    });
                    fieldsMapped++;
                    if (!string.IsNullOrEmpty(phoneQuirkResult.QuirkDescription))
                    {
                        quirksHandled++;
                    }
                }
                else
                {
                    result.Warnings.Add($"Could not normalize phone number: {phoneQuirkResult.Notes.FirstOrDefault() ?? "Invalid format"}");
                }
            }

            // Email
            if (!string.IsNullOrWhiteSpace(vendorPatientData.Email) && IsValidEmail(vendorPatientData.Email))
            {
                contactPoints.Add(new ContactPoint
                {
                    System = ContactPoint.ContactPointSystem.Email,
                    Value = vendorPatientData.Email.Trim().ToLowerInvariant()
                });
                fieldsMapped++;
            }

            if (contactPoints.Any())
            {
                patient.Telecom = contactPoints;
            }

            // Map address with quirk handling
            if (vendorPatientData.Address != null)
            {
                var addressQuirkResult = await HandleDataQuirkAsync("Address", vendorPatientData.Address);
                if (addressQuirkResult.IsSuccess && addressQuirkResult.NormalizedValue is Address normalizedAddress)
                {
                    patient.Address.Add(normalizedAddress);
                    fieldsMapped++;
                    if (!string.IsNullOrEmpty(addressQuirkResult.QuirkDescription))
                    {
                        quirksHandled++;
                    }
                }
                else
                {
                    result.Warnings.Add($"Could not normalize address: {addressQuirkResult.Notes.FirstOrDefault() ?? "Invalid format"}");
                }
            }

            result.Resource = patient;
            result.IsSuccess = true;
            result.Metadata.Duration = stopwatch.Elapsed;
            result.Metadata.FieldsMapped = fieldsMapped;
            result.Metadata.QuirksHandled = quirksHandled;

            _logger.LogInformation("Patient transformation completed successfully. Fields mapped: {FieldsMapped}, Quirks handled: {QuirksHandled}, Duration: {Duration}ms",
                fieldsMapped, quirksHandled, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error transforming patient data: {ErrorMessage}", ex.Message);
            result.Errors.Add($"Transformation failed: {ex.Message}");
            return result;
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    /// <summary>
    /// Transforms vendor observation data to FHIR Observation resource
    /// </summary>
    public async Task<DataMappingResult<Observation>> TransformObservationAsync(VendorObservationData vendorObservationData)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new DataMappingResult<Observation>();

        try
        {
            _logger.LogInformation("Starting observation transformation for vendor observation ID: {ObservationId}",
                SanitizeForLogging(vendorObservationData.ObservationId));

            // Validate input data
            var validationResult = await ValidateVendorDataAsync(vendorObservationData);
            if (!validationResult.IsValid)
            {
                result.Errors.AddRange(validationResult.Errors);
                result.Warnings.AddRange(validationResult.Warnings);
                if (validationResult.Errors.Any())
                {
                    return result;
                }
            }

            var observation = new Observation();
            var fieldsMapped = 0;
            var quirksHandled = 0;

            // Map observation identifier
            if (!string.IsNullOrWhiteSpace(vendorObservationData.ObservationId))
            {
                observation.Identifier.Add(new Identifier
                {
                    System = _fhirConfiguration.ObservationIdentifierSystem,
                    Value = vendorObservationData.ObservationId
                });
                fieldsMapped++;
            }

            // Map patient reference
            if (!string.IsNullOrWhiteSpace(vendorObservationData.PatientId))
            {
                observation.Subject = new ResourceReference($"Patient/{vendorObservationData.PatientId}");
                fieldsMapped++;
            }

            // Map observation status with quirk handling
            if (!string.IsNullOrWhiteSpace(vendorObservationData.Status))
            {
                var statusQuirkResult = await HandleDataQuirkAsync("ObservationStatus", vendorObservationData.Status);
                if (statusQuirkResult.IsSuccess && statusQuirkResult.NormalizedValue is ObservationStatus status)
                {
                    observation.Status = status;
                    fieldsMapped++;
                    if (!string.IsNullOrEmpty(statusQuirkResult.QuirkDescription))
                    {
                        quirksHandled++;
                    }
                }
                else
                {
                    result.Warnings.Add($"Could not map observation status: {statusQuirkResult.Notes.FirstOrDefault() ?? "Unknown format"}");
                    observation.Status = ObservationStatus.Unknown; // Default fallback
                }
            }
            else
            {
                observation.Status = ObservationStatus.Final; // Default status
            }

            // Map observation code/type
            if (!string.IsNullOrWhiteSpace(vendorObservationData.ObservationType))
            {
                observation.Code = new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = _fhirConfiguration.ObservationCodeSystem,
                            Code = vendorObservationData.ObservationType,
                            Display = vendorObservationData.ObservationType
                        }
                    }
                };
                fieldsMapped++;
            }

            // Map observation value and unit
            if (!string.IsNullOrWhiteSpace(vendorObservationData.Value))
            {
                var valueQuirkResult = await HandleDataQuirkAsync("ObservationValue", vendorObservationData.Value);
                if (valueQuirkResult.IsSuccess)
                {
                    if (decimal.TryParse(valueQuirkResult.NormalizedValue?.ToString(), out var numericValue))
                    {
                        var quantity = new Quantity
                        {
                            Value = numericValue,
                            Unit = vendorObservationData.Unit ?? "1",
                            System = "http://unitsofmeasure.org"
                        };
                        observation.Value = quantity;
                    }
                    else
                    {
                        observation.Value = new FhirString(valueQuirkResult.NormalizedValue?.ToString() ?? vendorObservationData.Value);
                    }
                    fieldsMapped++;
                    if (!string.IsNullOrEmpty(valueQuirkResult.QuirkDescription))
                    {
                        quirksHandled++;
                    }
                }
                else
                {
                    result.Warnings.Add($"Could not normalize observation value: {valueQuirkResult.Notes.FirstOrDefault() ?? "Invalid format"}");
                }
            }

            // Map observation date/time
            if (!string.IsNullOrWhiteSpace(vendorObservationData.DateTime))
            {
                var dateQuirkResult = await HandleDataQuirkAsync("DateTime", vendorObservationData.DateTime);
                if (dateQuirkResult.IsSuccess && dateQuirkResult.NormalizedValue is DateTime observationDateTime)
                {
                    observation.Effective = new FhirDateTime(observationDateTime);
                    fieldsMapped++;
                    if (!string.IsNullOrEmpty(dateQuirkResult.QuirkDescription))
                    {
                        quirksHandled++;
                    }
                }
                else
                {
                    result.Warnings.Add($"Could not parse observation date/time: {dateQuirkResult.Notes.FirstOrDefault() ?? "Invalid format"}");
                }
            }

            result.Resource = observation;
            result.IsSuccess = true;
            result.Metadata.Duration = stopwatch.Elapsed;
            result.Metadata.FieldsMapped = fieldsMapped;
            result.Metadata.QuirksHandled = quirksHandled;

            _logger.LogInformation("Observation transformation completed successfully. Fields mapped: {FieldsMapped}, Quirks handled: {QuirksHandled}, Duration: {Duration}ms",
                fieldsMapped, quirksHandled, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error transforming observation data: {ErrorMessage}", ex.Message);
            result.Errors.Add($"Transformation failed: {ex.Message}");
            return result;
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    /// <summary>
    /// Validates vendor data format and structure before transformation
    /// </summary>
    public async Task<ValidationResult> ValidateVendorDataAsync(object vendorData)
    {
        var result = new ValidationResult { IsValid = true };

        try
        {
            switch (vendorData)
            {
                case VendorPatientData patientData:
                    await ValidatePatientDataAsync(patientData, result);
                    break;
                case VendorObservationData observationData:
                    await ValidateObservationDataAsync(observationData, result);
                    break;
                default:
                    result.Errors.Add("Unsupported vendor data type");
                    result.IsValid = false;
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating vendor data: {ErrorMessage}", ex.Message);
            result.Errors.Add($"Validation failed: {ex.Message}");
            result.IsValid = false;
        }

        result.IsValid = !result.Errors.Any();
        return result;
    }

    /// <summary>
    /// Handles vendor-specific data quirks and anomalies
    /// </summary>
    public async Task<QuirkHandlingResult> HandleDataQuirkAsync(string fieldName, object rawValue)
    {
        var result = new QuirkHandlingResult { IsSuccess = true };

        try
        {
            switch (fieldName?.ToLowerInvariant())
            {
                case "dateofbirth":
                case "datetime":
                    result = await HandleDateQuirkAsync(rawValue);
                    break;
                case "gender":
                    result = await HandleGenderQuirkAsync(rawValue);
                    break;
                case "phonenumber":
                    result = await HandlePhoneNumberQuirkAsync(rawValue);
                    break;
                case "address":
                    result = await HandleAddressQuirkAsync(rawValue);
                    break;
                case "observationstatus":
                    result = await HandleObservationStatusQuirkAsync(rawValue);
                    break;
                case "observationvalue":
                    result = await HandleObservationValueQuirkAsync(rawValue);
                    break;
                default:
                    // No specific quirk handling needed, return original value
                    result.NormalizedValue = rawValue;
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling data quirk for field {FieldName}: {ErrorMessage}", fieldName, ex.Message);
            result.IsSuccess = false;
            result.Notes.Add($"Quirk handling failed: {ex.Message}");
            throw new DataQuirkHandlingException($"Failed to handle data quirk for field '{fieldName}'",
                fieldName ?? "unknown", fieldName ?? "unknown", ex.Message, ex);
        }

        return result;
    }

    /// <summary>
    /// Validates FHIR resource against specified profiles
    /// </summary>
    public async Task<FhirValidationResult> ValidateFhirResourceAsync(Resource resource, string profileUrl)
    {
        var result = new FhirValidationResult
        {
            IsValid = true,
            ProfileUrl = profileUrl
        };

        try
        {
            var validationResult = await Task.Run(() => _fhirValidator.Validate(resource));

            if (validationResult.Issue?.Any() == true)
            {
                foreach (var issue in validationResult.Issue)
                {
                    var issueMessage = $"{issue.Severity}: {issue.Details?.Text ?? issue.Diagnostics}";
                    result.Issues.Add(issueMessage);

                    if (issue.Severity == OperationOutcome.IssueSeverity.Error ||
                        issue.Severity == OperationOutcome.IssueSeverity.Fatal)
                    {
                        result.IsValid = false;
                    }
                }
            }

            _logger.LogInformation("FHIR resource validation completed. Valid: {IsValid}, Issues: {IssueCount}",
                result.IsValid, result.Issues.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating FHIR resource: {ErrorMessage}", ex.Message);
            result.Issues.Add($"Validation failed: {ex.Message}");
            result.IsValid = false;
        }

        return result;
    }

    #region Private Helper Methods

    private async Task ValidatePatientDataAsync(VendorPatientData patientData, ValidationResult result)
    {
        // Required field validation
        if (string.IsNullOrWhiteSpace(patientData.PatientId))
        {
            result.Errors.Add("Patient ID is required");
        }

        // Data format validation
        if (!string.IsNullOrWhiteSpace(patientData.Email) && !IsValidEmail(patientData.Email))
        {
            result.Warnings.Add("Email format appears invalid");
        }

        await Task.CompletedTask; // For async consistency
    }

    private async Task ValidateObservationDataAsync(VendorObservationData observationData, ValidationResult result)
    {
        // Required field validation
        if (string.IsNullOrWhiteSpace(observationData.PatientId))
        {
            result.Errors.Add("Patient ID is required for observation");
        }

        if (string.IsNullOrWhiteSpace(observationData.ObservationType))
        {
            result.Errors.Add("Observation type is required");
        }

        await Task.CompletedTask; // For async consistency
    }

    private async Task<QuirkHandlingResult> HandleDateQuirkAsync(object rawValue)
    {
        var result = new QuirkHandlingResult();
        var dateString = rawValue?.ToString()?.Trim();

        if (string.IsNullOrEmpty(dateString))
        {
            result.IsSuccess = false;
            result.Notes.Add("Date value is null or empty");
            return result;
        }

        // Handle common vendor-specific date quirks
        foreach (var format in DateFormats)
        {
            if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                result.NormalizedValue = parsedDate;
                result.IsSuccess = true;
                result.QuirkDescription = $"Parsed date using format: {format}";
                return result;
            }
        }

        // Fallback to general parsing
        if (DateTime.TryParse(dateString, out var generalDate))
        {
            result.NormalizedValue = generalDate;
            result.IsSuccess = true;
            result.QuirkDescription = "Parsed date using general parsing";
            return result;
        }

        result.IsSuccess = false;
        result.Notes.Add($"Could not parse date value in any supported format");
        return result;
    }

    private async Task<QuirkHandlingResult> HandleGenderQuirkAsync(object rawValue)
    {
        var result = new QuirkHandlingResult();
        var genderString = rawValue?.ToString()?.Trim();

        if (string.IsNullOrEmpty(genderString))
        {
            result.NormalizedValue = AdministrativeGender.Unknown;
            result.IsSuccess = true;
            result.QuirkDescription = "Empty gender mapped to Unknown";
            return result;
        }

        if (GenderMappings.TryGetValue(genderString, out var mappedGender))
        {
            result.NormalizedValue = mappedGender;
            result.IsSuccess = true;
            if (!genderString.Equals(mappedGender.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                result.QuirkDescription = $"Mapped vendor gender '{genderString}' to FHIR '{mappedGender}'";
            }
            return result;
        }

        // Fallback for unknown gender codes
        result.NormalizedValue = AdministrativeGender.Unknown;
        result.IsSuccess = true;
        result.QuirkDescription = $"Unknown gender code '{genderString}' mapped to Unknown";
        result.Notes.Add($"Vendor gender code '{genderString}' not recognized");

        await Task.CompletedTask; // For async consistency
        return result;
    }

    private async Task<QuirkHandlingResult> HandlePhoneNumberQuirkAsync(object rawValue)
    {
        var result = new QuirkHandlingResult();
        var phoneString = rawValue?.ToString()?.Trim();

        if (string.IsNullOrEmpty(phoneString))
        {
            result.IsSuccess = false;
            result.Notes.Add("Phone number is null or empty");
            return result;
        }

        // Remove common formatting characters
        var normalized = Regex.Replace(phoneString, @"[^\d+]", "");

        // Handle US phone numbers
        if (normalized.Length == 10 && !normalized.StartsWith("+"))
        {
            normalized = "+1" + normalized;
            result.QuirkDescription = "Added US country code to 10-digit number";
        }
        else if (normalized.Length == 11 && normalized.StartsWith("1") && !normalized.StartsWith("+"))
        {
            normalized = "+" + normalized;
            result.QuirkDescription = "Added + prefix to US number";
        }

        result.NormalizedValue = normalized;
        result.IsSuccess = true;

        await Task.CompletedTask; // For async consistency
        return result;
    }

    private async Task<QuirkHandlingResult> HandleAddressQuirkAsync(object rawValue)
    {
        var result = new QuirkHandlingResult();

        if (rawValue is not VendorAddress vendorAddress)
        {
            result.IsSuccess = false;
            result.Notes.Add("Address value is not in expected format");
            return result;
        }

        var fhirAddress = new Address
        {
            Use = Address.AddressUse.Home,
            Type = Address.AddressType.Physical
        };

        var addressParts = new List<string>();

        if (!string.IsNullOrWhiteSpace(vendorAddress.Street))
        {
            addressParts.Add(vendorAddress.Street.Trim());
        }

        if (addressParts.Any())
        {
            fhirAddress.Line = addressParts;
        }

        if (!string.IsNullOrWhiteSpace(vendorAddress.City))
        {
            fhirAddress.City = vendorAddress.City.Trim();
        }

        if (!string.IsNullOrWhiteSpace(vendorAddress.State))
        {
            fhirAddress.State = vendorAddress.State.Trim();
        }

        if (!string.IsNullOrWhiteSpace(vendorAddress.ZipCode))
        {
            // Handle ZIP+4 format quirk
            var zipCode = vendorAddress.ZipCode.Trim();
            if (zipCode.Length == 9 && !zipCode.Contains("-"))
            {
                zipCode = zipCode.Insert(5, "-");
                result.QuirkDescription = "Formatted 9-digit ZIP code with dash";
            }
            fhirAddress.PostalCode = zipCode;
        }

        if (!string.IsNullOrWhiteSpace(vendorAddress.Country))
        {
            fhirAddress.Country = vendorAddress.Country.Trim();
        }
        else
        {
            fhirAddress.Country = "US"; // Default for US-based systems
            result.QuirkDescription = (result.QuirkDescription ?? "") + " Added default country US";
        }

        result.NormalizedValue = fhirAddress;
        result.IsSuccess = true;

        await Task.CompletedTask; // For async consistency
        return result;
    }

    private async Task<QuirkHandlingResult> HandleObservationStatusQuirkAsync(object rawValue)
    {
        var result = new QuirkHandlingResult();
        var statusString = rawValue?.ToString()?.Trim();

        if (string.IsNullOrEmpty(statusString))
        {
            result.NormalizedValue = ObservationStatus.Unknown;
            result.IsSuccess = true;
            result.QuirkDescription = "Empty status mapped to Unknown";
            return result;
        }

        // Common vendor status mappings
        var statusMappings = new Dictionary<string, ObservationStatus>(StringComparer.OrdinalIgnoreCase)
        {
            { "final", ObservationStatus.Final },
            { "preliminary", ObservationStatus.Preliminary },
            { "amended", ObservationStatus.Amended },
            { "corrected", ObservationStatus.Corrected },
            { "cancelled", ObservationStatus.Cancelled },
            { "entered-in-error", ObservationStatus.EnteredInError },
            { "unknown", ObservationStatus.Unknown },
            { "complete", ObservationStatus.Final },
            { "partial", ObservationStatus.Preliminary },
            { "reviewed", ObservationStatus.Final }
        };

        if (statusMappings.TryGetValue(statusString, out var mappedStatus))
        {
            result.NormalizedValue = mappedStatus;
            result.IsSuccess = true;
            if (!statusString.Equals(mappedStatus.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                result.QuirkDescription = $"Mapped vendor status '{statusString}' to FHIR '{mappedStatus}'";
            }
        }
        else
        {
            result.NormalizedValue = ObservationStatus.Unknown;
            result.IsSuccess = true;
            result.QuirkDescription = $"Unknown status '{statusString}' mapped to Unknown";
            result.Notes.Add($"Vendor status '{statusString}' not recognized");
        }

        await Task.CompletedTask; // For async consistency
        return result;
    }

    private async Task<QuirkHandlingResult> HandleObservationValueQuirkAsync(object rawValue)
    {
        var result = new QuirkHandlingResult();
        var valueString = rawValue?.ToString()?.Trim();

        if (string.IsNullOrEmpty(valueString))
        {
            result.IsSuccess = false;
            result.Notes.Add("Observation value is null or empty");
            return result;
        }

        // Handle common vendor value quirks
        if (valueString.Equals("NULL", StringComparison.OrdinalIgnoreCase) ||
            valueString.Equals("N/A", StringComparison.OrdinalIgnoreCase))
        {
            result.IsSuccess = false;
            result.Notes.Add("Observation value indicates null/missing data");
            return result;
        }

        // Remove common non-numeric characters for numeric values
        var cleanValue = valueString.Replace(",", "").Replace("$", "").Trim();

        // Try to parse as numeric
        if (decimal.TryParse(cleanValue, out var numericValue))
        {
            result.NormalizedValue = numericValue;
            if (cleanValue != valueString)
            {
                result.QuirkDescription = $"Cleaned numeric value from '{valueString}' to '{cleanValue}'";
            }
        }
        else
        {
            result.NormalizedValue = valueString;
        }

        result.IsSuccess = true;

        await Task.CompletedTask; // For async consistency
        return result;
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private static string SanitizeForLogging(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return "null";

        // Return type/length info only to avoid PHI exposure
        return $"[{value.GetType().Name}:{value.Length}chars]";
    }

    #endregion
}