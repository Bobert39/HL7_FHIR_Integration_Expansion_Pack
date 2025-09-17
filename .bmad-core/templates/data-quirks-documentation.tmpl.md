# Data Quirks Documentation Template

## Template Configuration
```yaml
id: data-quirks-documentation
name: Non-Standard Behavior and Data Quirks Documentation
description: Template for documenting vendor-specific quirks, non-standard behaviors, and integration gotchas
category: technical-documentation
format: markdown
version: 1.0
criticality: high
```

## Overview
- **Vendor System**: [System Name]
- **Documentation Date**: [Date]
- **Documented By**: Healthcare System Integration Analyst
- **Risk Level**: [Low/Medium/High/Critical]
- **Impact Scope**: [Data Quality/Performance/Reliability/Security]

## Executive Summary
Brief description of the most critical quirks that development team must be aware of before starting implementation.

### Top 5 Critical Quirks
1. [Most critical quirk affecting data integrity]
2. [Second most critical quirk]
3. [Third most critical quirk]
4. [Fourth most critical quirk]
5. [Fifth most critical quirk]

## Date and Time Handling Quirks

### Non-Standard Date Formats
| Field | Expected Format | Actual Format | Example | Impact | Mitigation |
|-------|----------------|---------------|---------|---------|------------|
| Patient.birthDate | YYYY-MM-DD | MM/DD/YYYY | "03/15/1980" | Parse errors | Custom parser required |
| Encounter.date | ISO 8601 | MM/DD/YYYY HH:MM | "03/15/2024 14:30" | FHIR validation fails | DateTime converter needed |
| Observation.timestamp | Unix epoch | Proprietary format | "20240315143000" | Cannot parse | Format detection logic |

### Timezone Handling
- **Issue**: System stores all times in local timezone without timezone indicator
- **Impact**: Ambiguous times during DST transitions, incorrect UTC conversion
- **Mitigation**:
  - Determine facility timezone from configuration
  - Apply timezone during transformation
  - Handle DST transition edge cases

### Date Range Inconsistencies
- **Issue**: Start dates can be after end dates in encounters
- **Impact**: Invalid FHIR Period resources
- **Mitigation**: Validate and swap if necessary, log for review

## Code System Quirks

### Proprietary Code Systems
| Domain | Standard Expected | Vendor System | Mapping Complexity |
|--------|------------------|---------------|-------------------|
| Gender | FHIR AdministrativeGender | M/F/U/X/N | Simple mapping table |
| Marital Status | FHIR MaritalStatus | 20+ custom codes | Complex mapping with defaults |
| Race | OMB Race Categories | 50+ granular codes | Hierarchical mapping required |
| Language | ISO 639 | 3-letter custom codes | Lookup table needed |

### Missing Code System Identifiers
- **Issue**: Observation codes provided without system identifier
- **Impact**: Cannot determine if LOINC, SNOMED, or proprietary
- **Mitigation**:
  - Pattern matching to detect code system
  - Maintain known code database
  - Default to proprietary if unknown

### Code Version Inconsistencies
- **Issue**: ICD codes mix ICD-9 and ICD-10 without indication
- **Impact**: Incorrect clinical meaning if wrong version assumed
- **Mitigation**:
  - Check code pattern (ICD-9: xxx.xx, ICD-10: XXX.XX)
  - Validate against known code ranges
  - Flag ambiguous codes for manual review

## Data Structure Quirks

### Inconsistent Field Presence
| Scenario | Expected | Actual | Impact |
|----------|----------|--------|--------|
| Optional fields | Omitted if null | Present with "" | Validation errors |
| Empty arrays | [] | Omitted entirely | Null reference errors |
| Null numbers | null | -1 or 999999 | Incorrect calculations |
| Boolean fields | true/false | "Y"/"N"/"" | Type conversion errors |

### Polymorphic Fields
```yaml
# Field 'value' changes type based on 'value_type' field
Examples:
  - value_type: "numeric", value: "123.45" (string containing number)
  - value_type: "text", value: "Normal" (string)
  - value_type: "coded", value: "A,B,C" (comma-delimited codes)
  - value_type: "date", value: "03/15/2024" (string date)
```

### Nested Object Inconsistencies
- **Issue**: Address object structure varies by patient type
- **Variations**:
  - Inpatient: Full address object with all fields
  - Outpatient: Flattened address fields at root level
  - Emergency: Address as single concatenated string
- **Mitigation**: Detect patient type and apply appropriate parsing

## Character Encoding and Text Quirks

### Special Character Handling
| Issue | Example | Impact | Solution |
|-------|---------|---------|----------|
| Unescaped quotes in JSON | `"name": "John "Johnny" Doe"` | JSON parse errors | Pre-process escaping |
| HTML in text fields | `<b>Important</b> note` | Display issues | Strip or convert HTML |
| Line breaks | Mixed \n, \r\n, <br> | Formatting issues | Normalize to \n |
| Unicode issues | Smart quotes, em-dashes | Encoding errors | UTF-8 normalization |

### Text Field Limitations
- **Issue**: Notes field truncated at 4000 characters without warning
- **Impact**: Clinical information loss
- **Mitigation**: Check length before submission, implement continuation pattern

### Case Sensitivity Inconsistencies
| Field Type | Case Sensitive | Example | Impact |
|------------|---------------|---------|--------|
| Patient IDs | No | pat123 = PAT123 | Duplicate detection issues |
| Provider IDs | Yes | doc001 ≠ DOC001 | Failed lookups |
| Code values | Sometimes | Depends on code system | Mapping failures |

## Null and Empty Value Representations

### Null Value Patterns
```yaml
Patterns Found:
  - Empty string: ""
  - Literal null: null
  - String "null": "null"
  - String "N/A": "N/A"
  - String "None": "None"
  - String "Unknown": "Unknown"
  - Number -1: -1
  - Number 999: 999 or 9999
  - Missing field: Field omitted from response

Recommended Handling:
  - Create NullValueParser utility
  - Map all patterns to proper null
  - Document in data dictionary
```

### Required Fields That Can Be Empty
| Field | Marked Required | Can Be Empty | Default Handling |
|-------|----------------|--------------|------------------|
| Patient.email | Yes | Yes ("") | Treat as null |
| Encounter.reason | Yes | Yes ([]) | Omit from FHIR |
| Observation.performer | Yes | Yes ("Unknown") | Use Organization reference |

## Data Validation Quirks

### Validation Rule Inconsistencies
- **Issue**: API accepts invalid data but fails silently
- **Examples**:
  - Future dates accepted for birth date
  - Invalid SSN formats stored as-is
  - Negative values for age
- **Mitigation**: Client-side validation before submission

### Referential Integrity Issues
| Relationship | Issue | Frequency | Impact |
|--------------|-------|-----------|--------|
| Encounter → Patient | Patient may not exist | Rare | Orphaned encounters |
| Observation → Encounter | Encounter may be deleted | Common | Lost context |
| Order → Provider | Provider may be inactive | Common | Invalid references |

### Unique Constraint Violations
- **Issue**: System allows duplicate patient records with same demographics
- **Impact**: Data integrity issues, wrong patient selection
- **Mitigation**: Implement matching algorithm, flag potential duplicates

## Performance and Behavioral Quirks

### Response Time Variability
| Operation | Normal | During Peak | Timeout Threshold |
|-----------|--------|-------------|-------------------|
| Single record | 200ms | 5000ms | 30s |
| List (100 records) | 1s | 30s | 60s |
| Search | 500ms | 10s | 30s |

### Pagination Quirks
- **Issue**: Page size parameter ignored, always returns 50
- **Impact**: More API calls needed than expected
- **Mitigation**: Adjust pagination logic to expect 50

### Rate Limiting Behaviors
```yaml
Observed Patterns:
  - No documented rate limits
  - Actual limit: 100 requests/minute
  - No rate limit headers provided
  - 429 responses return HTML, not JSON
  - Reset time unknown

Recommendations:
  - Implement conservative rate limiting (60/min)
  - Exponential backoff on 429
  - Monitor for HTML responses
```

### Caching Behaviors
- **Issue**: API returns cached data for up to 5 minutes
- **Impact**: Updates not immediately visible
- **Mitigation**: Implement cache-busting parameters or wait period

## Error Handling Quirks

### Non-Standard Error Responses
| Scenario | Expected | Actual | Parsing Required |
|----------|----------|--------|------------------|
| 404 Not Found | JSON error | HTML 404 page | HTML detection |
| 500 Server Error | JSON error | Plain text | Text parsing |
| Validation Error | 400 with details | 200 with error in body | Check success field |
| Authentication | 401 | 403 | Map 403 to auth error |

### Silent Failures
- **Issue**: API returns success but doesn't save data
- **Indicators**:
  - Response has `"success": true` but `"warnings": ["Data not saved"]`
  - No error message provided
  - Have to check if data persisted separately
- **Mitigation**: Always verify writes with subsequent read

### Partial Success Handling
```json
// Bulk operation response example
{
  "success": true,
  "total": 100,
  "processed": 97,
  "failed": 3,
  "failures": [
    {"index": 45, "error": "Invalid date"},
    {"index": 67, "error": "Duplicate"},
    {"index": 89, "error": "Unknown"}
  ]
}
```

## Security and Compliance Quirks

### Authentication Quirks
- **Issue**: Token expiration time not provided in response
- **Impact**: Cannot proactively refresh tokens
- **Mitigation**: Track token age, refresh on 401 response

### Data Leakage Issues
| Issue | Example | Risk | Mitigation |
|-------|---------|------|------------|
| Verbose errors | Stack traces in production | Information disclosure | Error filtering |
| Debug headers | Internal IPs in headers | Network mapping | Header stripping |
| Hidden fields | Deleted data still returned | Privacy violation | Response filtering |

### Audit Logging Gaps
- **Issue**: Not all operations are logged
- **Missing**: Read operations, failed authentications
- **Impact**: Incomplete audit trail
- **Mitigation**: Implement client-side audit logging

## Integration Gotchas

### Undocumented Features
1. **Hidden Parameters**: `_includeDeleted=true` returns soft-deleted records
2. **Special Headers**: `X-Debug-Mode: true` returns additional metadata
3. **Bulk Operations**: Undocumented `/bulk` endpoint exists
4. **Export Feature**: `/export` endpoint for full database dump

### Version-Specific Behaviors
| API Version | Behavior | Impact |
|-------------|----------|--------|
| v1 | Returns XML by default | Parse errors if expecting JSON |
| v2 | Breaking changes in field names | Mapping failures |
| v3 | Deprecates critical endpoints | Feature loss |

### Environment Differences
| Environment | Difference | Impact |
|-------------|------------|--------|
| Development | No SSL required | Security testing gaps |
| Staging | Different data format | Test failures |
| Production | Stricter validation | Unexpected errors |

## Recommended Workarounds

### High Priority Workarounds
1. **Date Parser Utility**: Handle all date format variations
2. **Code Mapper Service**: Centralize code system mappings
3. **Null Handler**: Normalize all null representations
4. **Response Wrapper**: Handle various error formats
5. **Retry Logic**: Handle transient failures

### Data Quality Monitors
```csharp
// Example monitoring checks
public class DataQualityMonitor
{
    public void CheckIncomingData(VendorData data)
    {
        LogIfFutureDates(data);
        LogIfInvalidReferences(data);
        LogIfDuplicates(data);
        LogIfMissingRequiredFields(data);
        LogIfSuspiciousValues(data);
    }
}
```

## Testing Recommendations

### Edge Cases to Test
1. DST transition dates
2. Leap year dates
3. Maximum length strings
4. Special characters in all text fields
5. Null/empty variations
6. Large bulk operations
7. Concurrent requests
8. Token expiration handling
9. Network timeout scenarios
10. Partial failure scenarios

### Load Testing Considerations
- Test with production-like data volumes
- Simulate peak hour traffic patterns
- Test rate limiting boundaries
- Measure response time degradation
- Test connection pool limits

## Development Team Checklist

### Before Starting Development
- [ ] Review all critical quirks in this document
- [ ] Set up data validation layer
- [ ] Implement custom parsers for dates and codes
- [ ] Create error handling wrapper
- [ ] Design retry strategy
- [ ] Plan for data quality monitoring

### During Development
- [ ] Log all unmapped fields
- [ ] Track transformation failures
- [ ] Monitor API response times
- [ ] Validate FHIR resources before sending
- [ ] Test with production-like data

### Before Deployment
- [ ] Verify all quirks are handled
- [ ] Load test with realistic volumes
- [ ] Confirm error handling works
- [ ] Validate audit logging
- [ ] Document any new quirks discovered

## Risk Assessment

### High Risk Items
| Quirk | Probability | Impact | Mitigation Priority |
|-------|------------|--------|-------------------|
| Data loss from truncation | High | Critical | Immediate |
| Invalid clinical codes | Medium | High | High |
| Reference integrity issues | High | Medium | High |
| Silent failures | Medium | High | High |

### Medium Risk Items
| Quirk | Probability | Impact | Mitigation Priority |
|-------|------------|--------|-------------------|
| Performance degradation | Medium | Medium | Medium |
| Timezone issues | Low | Medium | Medium |
| Caching delays | High | Low | Medium |

## Vendor Communication Log

### Issues Reported to Vendor
| Date | Issue | Vendor Response | Resolution |
|------|-------|----------------|------------|
| [Date] | Date format inconsistency | Acknowledged | Won't fix |
| [Date] | Rate limiting undocumented | Will document | Pending |
| [Date] | Silent failures | Under investigation | Open |

### Vendor Recommended Workarounds
1. [Vendor suggested workaround]
2. [Another vendor recommendation]

## Document Maintenance

### Review Schedule
- **Weekly**: During active development
- **Monthly**: During testing phase
- **Quarterly**: After go-live

### Update History
| Date | Author | Changes |
|------|--------|---------|
| [Date] | [Name] | Initial documentation |
| [Date] | [Name] | Added section X |

### Next Review Date: [Date]

## Appendix

### Sample Problematic Payloads
```json
// Example of problematic response
{
  "patient": {
    "id": "PAT123",
    "name": "John \"Johnny\" Doe",
    "birth_date": "13/45/2024",  // Invalid date
    "gender": "X",  // Unexpected code
    "age": -1,  // Sentinel value
    "address": ""  // Empty string instead of object
  }
}
```

### Utility Code Samples
```csharp
// Date parser utility
public static class QuirkyDateParser
{
    public static DateTime? ParseVendorDate(string vendorDate)
    {
        // Implementation handling all quirky formats
    }
}

// Null handler utility
public static class NullHandler
{
    public static bool IsEffectivelyNull(string value)
    {
        return string.IsNullOrWhiteSpace(value) ||
               value == "N/A" ||
               value == "null" ||
               value == "Unknown";
    }
}
```