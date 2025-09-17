using System.Globalization;
using System.Text.RegularExpressions;
using Hl7.Fhir.Model;
using FhirIntegrationService.Configuration;
using FhirIntegrationService.Services.Interfaces;

namespace FhirIntegrationService.Services.Helpers;

/// <summary>
/// Helper class for safe data type conversions with vendor-specific handling
/// </summary>
public static class DataTypeConverters
{
    /// <summary>
    /// Safely converts a string to DateTime using multiple format attempts
    /// </summary>
    /// <param name="dateString">The date string to convert</param>
    /// <param name="supportedFormats">List of supported date formats to try</param>
    /// <param name="result">The parsed DateTime if successful</param>
    /// <returns>True if conversion was successful</returns>
    public static bool TryParseDate(string? dateString, IEnumerable<string> supportedFormats, out DateTime result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(dateString))
            return false;

        var cleanDateString = dateString.Trim();

        // Try each supported format
        foreach (var format in supportedFormats)
        {
            if (DateTime.TryParseExact(cleanDateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                return true;
        }

        // Fallback to general parsing
        if (DateTime.TryParse(cleanDateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            return true;

        return false;
    }

    /// <summary>
    /// Converts vendor gender codes to FHIR AdministrativeGender
    /// </summary>
    /// <param name="genderValue">The vendor gender value</param>
    /// <param name="genderMappings">Custom gender mappings from configuration</param>
    /// <param name="result">The mapped FHIR gender</param>
    /// <returns>True if mapping was successful</returns>
    public static bool TryMapGender(string? genderValue, Dictionary<string, string> genderMappings, out AdministrativeGender result)
    {
        result = AdministrativeGender.Unknown;

        if (string.IsNullOrWhiteSpace(genderValue))
            return true; // Empty maps to Unknown

        var cleanGender = genderValue.Trim();

        // Try custom mappings first
        if (genderMappings.TryGetValue(cleanGender, out var mappedValue))
        {
            return Enum.TryParse<AdministrativeGender>(mappedValue, true, out result);
        }

        // Default mappings
        var defaultMappings = new Dictionary<string, AdministrativeGender>(StringComparer.OrdinalIgnoreCase)
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
            { "4", AdministrativeGender.Unknown }
        };

        if (defaultMappings.TryGetValue(cleanGender, out result))
            return true;

        // If no mapping found, default to Unknown
        result = AdministrativeGender.Unknown;
        return true;
    }

    /// <summary>
    /// Normalizes phone numbers to international format
    /// </summary>
    /// <param name="phoneNumber">The phone number to normalize</param>
    /// <param name="defaultCountryPrefix">Default country prefix to apply</param>
    /// <param name="result">The normalized phone number</param>
    /// <returns>True if normalization was successful</returns>
    public static bool TryNormalizePhoneNumber(string? phoneNumber, string defaultCountryPrefix, out string result)
    {
        result = string.Empty;

        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        // Remove all non-digit characters except + at the beginning
        var normalized = Regex.Replace(phoneNumber.Trim(), @"[^\d+]", "");

        if (string.IsNullOrEmpty(normalized))
            return false;

        // Handle different phone number formats
        if (normalized.StartsWith("+"))
        {
            // Already has country code
            result = normalized;
            return true;
        }

        if (normalized.Length == 10)
        {
            // Assume US number, add +1 prefix
            result = defaultCountryPrefix + normalized;
            return true;
        }

        if (normalized.Length == 11 && normalized.StartsWith("1"))
        {
            // US number with 1 prefix, add +
            result = "+" + normalized;
            return true;
        }

        // For other lengths, assume it needs the default country prefix
        if (normalized.Length >= 7 && normalized.Length <= 15)
        {
            result = defaultCountryPrefix + normalized;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Validates and normalizes email addresses
    /// </summary>
    /// <param name="email">The email address to validate</param>
    /// <param name="result">The normalized email address</param>
    /// <returns>True if email is valid</returns>
    public static bool TryValidateEmail(string? email, out string result)
    {
        result = string.Empty;

        if (string.IsNullOrWhiteSpace(email))
            return false;

        var cleanEmail = email.Trim().ToLowerInvariant();

        try
        {
            var addr = new System.Net.Mail.MailAddress(cleanEmail);
            if (addr.Address == cleanEmail)
            {
                result = cleanEmail;
                return true;
            }
        }
        catch
        {
            // Invalid email format
        }

        return false;
    }

    /// <summary>
    /// Normalizes postal codes based on country-specific formats
    /// </summary>
    /// <param name="postalCode">The postal code to normalize</param>
    /// <param name="countryCode">The country code for format rules</param>
    /// <param name="result">The normalized postal code</param>
    /// <returns>True if normalization was successful</returns>
    public static bool TryNormalizePostalCode(string? postalCode, string countryCode, out string result)
    {
        result = string.Empty;

        if (string.IsNullOrWhiteSpace(postalCode))
            return false;

        var cleanCode = postalCode.Trim().ToUpperInvariant();

        switch (countryCode.ToUpperInvariant())
        {
            case "US":
                return TryNormalizeUSPostalCode(cleanCode, out result);
            case "CA":
                return TryNormalizeCanadianPostalCode(cleanCode, out result);
            default:
                // For other countries, just return the cleaned code
                result = cleanCode;
                return true;
        }
    }

    /// <summary>
    /// Normalizes US postal codes (ZIP and ZIP+4)
    /// </summary>
    private static bool TryNormalizeUSPostalCode(string postalCode, out string result)
    {
        result = string.Empty;

        // Remove all non-digits
        var digits = Regex.Replace(postalCode, @"[^\d]", "");

        if (digits.Length == 5)
        {
            result = digits;
            return true;
        }

        if (digits.Length == 9)
        {
            result = $"{digits.Substring(0, 5)}-{digits.Substring(5, 4)}";
            return true;
        }

        return false;
    }

    /// <summary>
    /// Normalizes Canadian postal codes
    /// </summary>
    private static bool TryNormalizeCanadianPostalCode(string postalCode, out string result)
    {
        result = string.Empty;

        // Remove spaces and convert to uppercase
        var normalized = postalCode.Replace(" ", "").ToUpperInvariant();

        // Canadian postal code pattern: A1A 1A1
        if (Regex.IsMatch(normalized, @"^[A-Z]\d[A-Z]\d[A-Z]\d$"))
        {
            result = $"{normalized.Substring(0, 3)} {normalized.Substring(3, 3)}";
            return true;
        }

        return false;
    }

    /// <summary>
    /// Converts vendor observation status to FHIR ObservationStatus
    /// </summary>
    /// <param name="statusValue">The vendor status value</param>
    /// <param name="statusMappings">Custom status mappings from configuration</param>
    /// <param name="result">The mapped FHIR observation status</param>
    /// <returns>True if mapping was successful</returns>
    public static bool TryMapObservationStatus(string? statusValue, Dictionary<string, string> statusMappings, out ObservationStatus result)
    {
        result = ObservationStatus.Unknown;

        if (string.IsNullOrWhiteSpace(statusValue))
            return true; // Empty maps to Unknown

        var cleanStatus = statusValue.Trim();

        // Try custom mappings first
        if (statusMappings.TryGetValue(cleanStatus, out var mappedValue))
        {
            return Enum.TryParse<ObservationStatus>(mappedValue, true, out result);
        }

        // Default mappings
        var defaultMappings = new Dictionary<string, ObservationStatus>(StringComparer.OrdinalIgnoreCase)
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

        if (defaultMappings.TryGetValue(cleanStatus, out result))
            return true;

        // If no mapping found, default to Unknown
        result = ObservationStatus.Unknown;
        return true;
    }

    /// <summary>
    /// Safely parses numeric values from observation data
    /// </summary>
    /// <param name="valueString">The value string to parse</param>
    /// <param name="result">The parsed decimal value</param>
    /// <returns>True if parsing was successful</returns>
    public static bool TryParseObservationValue(string? valueString, out decimal result)
    {
        result = 0;

        if (string.IsNullOrWhiteSpace(valueString))
            return false;

        var cleanValue = valueString.Trim();

        // Handle special cases
        if (cleanValue.Equals("NULL", StringComparison.OrdinalIgnoreCase) ||
            cleanValue.Equals("N/A", StringComparison.OrdinalIgnoreCase) ||
            cleanValue.Equals("UNKNOWN", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // Remove common formatting characters
        cleanValue = cleanValue.Replace(",", "").Replace("$", "").Replace("%", "");

        // Try to parse as decimal
        if (decimal.TryParse(cleanValue, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
            return true;

        return false;
    }

    /// <summary>
    /// Validates field length against configuration limits
    /// </summary>
    /// <param name="fieldName">The name of the field</param>
    /// <param name="value">The field value</param>
    /// <param name="maxLengths">Dictionary of field max lengths from configuration</param>
    /// <returns>True if field length is within limits</returns>
    public static bool ValidateFieldLength(string fieldName, string? value, Dictionary<string, int> maxLengths)
    {
        if (string.IsNullOrEmpty(value))
            return true;

        if (maxLengths.TryGetValue(fieldName, out var maxLength))
        {
            return value.Length <= maxLength;
        }

        // If no specific limit defined, allow any length
        return true;
    }

    /// <summary>
    /// Creates a sanitized value for logging that doesn't expose PHI
    /// </summary>
    /// <param name="value">The original value</param>
    /// <param name="fieldName">The field name for context</param>
    /// <returns>A sanitized representation safe for logging</returns>
    public static string CreateSanitizedLogValue(object? value, string fieldName)
    {
        if (value == null)
            return "[null]";

        var valueString = value.ToString();
        if (string.IsNullOrEmpty(valueString))
            return "[empty]";

        // For PHI-sensitive fields, only log type and length
        var phiFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "FirstName", "LastName", "Email", "PhoneNumber", "DateOfBirth",
            "Street", "PatientId", "MRN", "SSN"
        };

        if (phiFields.Contains(fieldName))
        {
            return $"[{value.GetType().Name}:{valueString.Length}chars]";
        }

        // For non-PHI fields, we can log more detail but still be cautious
        if (valueString.Length > 50)
        {
            return $"[{value.GetType().Name}:{valueString.Length}chars]";
        }

        return valueString;
    }
}