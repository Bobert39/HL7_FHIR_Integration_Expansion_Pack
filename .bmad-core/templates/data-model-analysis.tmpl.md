# Data Model Analysis Template

## Template Configuration
```yaml
id: data-model-analysis
name: Data Model Analysis Template
description: Structured template for analyzing vendor API response payloads and documenting data models
category: technical-documentation
format: markdown
version: 1.0
```

## Vendor System Information
- **System Name**: [Vendor System Name]
- **API Version**: [Version Number]
- **Analysis Date**: [Date]
- **Analyst**: Healthcare System Integration Analyst

## Data Model Documentation

### Resource: Patient
#### Field Inventory
| Field Path | Data Type | Required | Nullable | Example Value | Format/Pattern | Notes |
|------------|-----------|----------|----------|---------------|----------------|--------|
| patient_id | string | Yes | No | "PAT123456" | Alphanumeric | Primary identifier |
| first_name | string | Yes | No | "John" | Text | Given name |
| last_name | string | Yes | No | "Doe" | Text | Family name |
| date_of_birth | string | Yes | No | "01/15/1980" | MM/DD/YYYY | Non-standard date format |
| gender | string | Yes | No | "M" | Single char | M/F/U/O |
| ssn | string | No | Yes | "123-45-6789" | XXX-XX-XXXX | Masked in responses |
| address | object | No | Yes | {...} | Complex object | See nested structure |
| address.street | string | No | Yes | "123 Main St" | Text | Street address |
| address.city | string | No | Yes | "Boston" | Text | City name |
| address.state | string | No | Yes | "MA" | 2-char code | State abbreviation |
| address.zip | string | No | Yes | "02134" | 5 or 9 digits | ZIP or ZIP+4 |
| phone_numbers | array | No | Yes | [...] | Array of objects | Multiple phone types |
| email | string | No | Yes | "john@example.com" | Email format | Primary email |

#### Relationships
- **Has Many**: Encounters, Observations, Medications
- **Belongs To**: Organization (primary care provider)
- **References**: Practitioners (care team)

#### Data Constraints
- patient_id must be unique across system
- date_of_birth cannot be future date
- gender limited to defined value set
- At least one name component required

### Resource: Encounter
#### Field Inventory
| Field Path | Data Type | Required | Nullable | Example Value | Format/Pattern | Notes |
|------------|-----------|----------|----------|---------------|----------------|--------|
| encounter_id | string | Yes | No | "ENC789012" | Alphanumeric | Primary identifier |
| patient_id | string | Yes | No | "PAT123456" | Alphanumeric | FK to Patient |
| encounter_date | string | Yes | No | "03/15/2024 14:30" | MM/DD/YYYY HH:MM | 24-hour time |
| encounter_type | string | Yes | No | "OFFICE" | Code | Custom codes |
| status | string | Yes | No | "completed" | Enum | arrived/in-progress/completed |
| provider_id | string | Yes | No | "DOC456" | Alphanumeric | FK to Provider |
| location | string | No | Yes | "Main Campus" | Text | Facility name |
| diagnosis_codes | array | No | Yes | ["Z00.00"] | ICD-10 codes | Primary and secondary |
| chief_complaint | string | No | Yes | "Annual checkup" | Free text | Patient reported |
| notes | string | No | Yes | "..." | Long text | Clinical notes |

### Resource: Observation
#### Field Inventory
| Field Path | Data Type | Required | Nullable | Example Value | Format/Pattern | Notes |
|------------|-----------|----------|----------|---------------|----------------|--------|
| observation_id | string | Yes | No | "OBS345678" | Alphanumeric | Primary identifier |
| patient_id | string | Yes | No | "PAT123456" | Alphanumeric | FK to Patient |
| encounter_id | string | No | Yes | "ENC789012" | Alphanumeric | FK to Encounter |
| observation_date | string | Yes | No | "03/15/2024" | MM/DD/YYYY | Date only |
| observation_time | string | No | Yes | "14:45:00" | HH:MM:SS | Separate time field |
| code | string | Yes | No | "8302-2" | LOINC code | Sometimes proprietary |
| description | string | Yes | No | "Height" | Text | Human readable |
| value | string | Yes | No | "175" | Varies | Polymorphic field |
| unit | string | No | Yes | "cm" | UCUM | Sometimes missing |
| reference_range | string | No | Yes | "150-200" | Text | Not structured |
| status | string | Yes | No | "final" | Enum | preliminary/final/amended |
| performer | string | No | Yes | "DOC456" | Alphanumeric | Who performed |

## Data Type Patterns

### Date/Time Formats
| Field Type | Format Found | Standard Format | Conversion Required |
|------------|--------------|-----------------|---------------------|
| Date only | MM/DD/YYYY | YYYY-MM-DD | Yes |
| DateTime | MM/DD/YYYY HH:MM | YYYY-MM-DDT HH:MM:SSZ | Yes |
| Time only | HH:MM:SS | HH:MM:SS | No |
| Timestamp | Unix epoch (ms) | ISO 8601 | Yes |

### Code Systems
| Domain | Vendor System | Standard System | Mapping Required |
|--------|---------------|-----------------|------------------|
| Gender | M/F/U/O | FHIR AdministrativeGender | Yes |
| Encounter Type | Custom codes | FHIR Encounter.class | Yes |
| Observation Status | Custom terms | FHIR ObservationStatus | Yes |
| Units | Mixed formats | UCUM | Yes |

### Null/Empty Value Representations
| Pattern | Meaning | FHIR Handling |
|---------|---------|---------------|
| "" (empty string) | No value | Convert to null |
| "N/A" | Not applicable | Omit field |
| "Unknown" | Unknown value | Use DataAbsentReason |
| -1 or 999 | Sentinel value | Convert to null |
| Missing field | Not collected | Omit from resource |

## Nested Object Structures

### Address Object
```json
{
  "street": "string",
  "street2": "string (optional)",
  "city": "string",
  "state": "string (2-char)",
  "zip": "string",
  "country": "string (optional, defaults to 'USA')",
  "type": "string (home/work/temp)"
}
```

### Phone Object
```json
{
  "number": "string",
  "type": "string (home/work/mobile)",
  "primary": "boolean",
  "extension": "string (optional)"
}
```

## Array Structures

### Diagnosis Codes Array
- Can be empty array []
- Usually contains ICD-10 codes
- First element is primary diagnosis
- No explicit primary flag

### Medications Array
- Complex nested structure
- Contains both active and discontinued
- Status field determines current state
- May reference external medication database

## Special Characteristics

### Character Encoding
- UTF-8 encoding used
- Special characters in names not always escaped
- Newlines in text fields use \n
- Some fields may contain HTML tags

### Field Presence Patterns
- Required fields always present, even if null
- Optional fields may be omitted entirely
- Empty arrays represented as [] not null
- Empty objects represented as {} not null

### Size Limitations
| Field Type | Maximum Size | Notes |
|------------|--------------|--------|
| Identifiers | 50 chars | Alphanumeric only |
| Names | 100 chars | Per component |
| Free text | 4000 chars | Notes, comments |
| Arrays | 100 elements | Practical limit |

## Validation Rules Discovered

### Business Rules
1. Patient must have at least one identifier
2. Encounter must reference valid patient
3. Observations should reference encounter when available
4. Dates cannot be in future (except appointments)
5. Deleted records marked with status, not removed

### Data Integrity Rules
1. Referential integrity not always enforced
2. Orphaned records possible (deleted patient)
3. Duplicate checking on identifiers only
4. Case sensitivity varies by field

## Performance Characteristics

### Response Times
| Operation | Avg Time | Max Time | Payload Size |
|-----------|----------|----------|--------------|
| Single Patient | 200ms | 500ms | 2-5 KB |
| Patient List (100) | 800ms | 2000ms | 200-500 KB |
| Encounter History | 400ms | 1000ms | 10-50 KB |
| Bulk Export | 30s | 5min | 1-100 MB |

### Pagination
- Default page size: 100
- Maximum page size: 1000
- Uses offset/limit pattern
- No cursor-based pagination

## Notes and Observations

### Key Findings
1. [Important discovery about data model]
2. [Significant deviation from standards]
3. [Performance or reliability concern]
4. [Integration complexity identified]

### Recommendations
1. [Suggested approach for handling quirk]
2. [Performance optimization opportunity]
3. [Data quality improvement needed]
4. [Risk mitigation strategy]

### Questions for Vendor
1. [Clarification needed on field usage]
2. [Documentation gap identified]
3. [Inconsistency requiring explanation]
4. [Future compatibility concern]