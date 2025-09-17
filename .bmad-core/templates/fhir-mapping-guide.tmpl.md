# FHIR Mapping Guide Template

## Template Configuration
```yaml
id: fhir-mapping-guide
name: FHIR Resource Mapping Guide
description: Template for documenting vendor field to FHIR element mappings with transformation requirements
category: technical-documentation
format: markdown
version: 1.0
fhir-version: R4
```

## Mapping Overview
- **Vendor System**: [System Name]
- **FHIR Version**: R4
- **Mapping Date**: [Date]
- **Mapped By**: Healthcare System Integration Analyst
- **Review Status**: [Draft/Reviewed/Approved]

## Mapping Confidence Levels
- **Direct**: 1-to-1 mapping, no transformation needed
- **Simple**: Basic transformation (format change, type conversion)
- **Complex**: Multiple steps, business logic, or conditional mapping
- **Uncertain**: Requires clinical/business review
- **Not Mappable**: No FHIR equivalent identified

## Patient Resource Mappings

### Core Demographics
| Vendor Field | FHIR Element | Confidence | Transformation | Notes |
|--------------|--------------|------------|----------------|--------|
| patient_id | Patient.identifier[0].value | Direct | None | Primary identifier |
| [System Name] | Patient.identifier[0].system | Direct | Fixed value: "urn:oid:x.x.x" | |
| medical_record_num | Patient.identifier[1].value | Direct | None | MRN |
| first_name | Patient.name[0].given[0] | Direct | None | |
| middle_name | Patient.name[0].given[1] | Direct | None | Optional |
| last_name | Patient.name[0].family | Direct | None | |
| name_prefix | Patient.name[0].prefix[0] | Direct | None | Dr., Mr., Ms., etc. |
| name_suffix | Patient.name[0].suffix[0] | Direct | None | Jr., III, etc. |
| date_of_birth | Patient.birthDate | Simple | MM/DD/YYYY → YYYY-MM-DD | Date format conversion |
| gender | Patient.gender | Complex | See gender mapping table | Custom codes |
| ssn | Patient.identifier[2].value | Simple | Add dashes if missing | Type: "SS" |
| deceased_flag | Patient.deceasedBoolean | Simple | Y/N → true/false | |
| deceased_date | Patient.deceasedDateTime | Simple | Date format conversion | If known |

### Contact Information
| Vendor Field | FHIR Element | Confidence | Transformation | Notes |
|--------------|--------------|------------|----------------|--------|
| address.street | Patient.address[0].line[0] | Direct | None | |
| address.street2 | Patient.address[0].line[1] | Direct | None | Apt/Suite |
| address.city | Patient.address[0].city | Direct | None | |
| address.state | Patient.address[0].state | Direct | None | |
| address.zip | Patient.address[0].postalCode | Direct | None | |
| address.country | Patient.address[0].country | Simple | Default to "USA" if empty | |
| address.type | Patient.address[0].use | Simple | Map home/work/temp | |
| phone_home | Patient.telecom[0].value | Simple | Format standardization | |
| phone_home | Patient.telecom[0].system | Direct | Fixed: "phone" | |
| phone_home | Patient.telecom[0].use | Direct | Fixed: "home" | |
| phone_mobile | Patient.telecom[1].value | Simple | Format standardization | |
| email | Patient.telecom[2].value | Direct | None | |
| email | Patient.telecom[2].system | Direct | Fixed: "email" | |

### Clinical Attributes
| Vendor Field | FHIR Element | Confidence | Transformation | Notes |
|--------------|--------------|------------|----------------|--------|
| race | Patient.extension[us-core-race] | Complex | Map to OMB codes | US Core extension |
| ethnicity | Patient.extension[us-core-ethnicity] | Complex | Map to OMB codes | US Core extension |
| language | Patient.communication[0].language | Simple | Map to ISO 639 | |
| marital_status | Patient.maritalStatus | Simple | Map to FHIR value set | |
| religion | Patient.extension[religion] | Simple | Map to v3 ReligiousAffiliation | |

## Encounter Resource Mappings

### Encounter Core
| Vendor Field | FHIR Element | Confidence | Transformation | Notes |
|--------------|--------------|------------|----------------|--------|
| encounter_id | Encounter.identifier[0].value | Direct | None | |
| patient_id | Encounter.subject | Simple | Reference: "Patient/{id}" | |
| encounter_date | Encounter.period.start | Simple | DateTime conversion | |
| discharge_date | Encounter.period.end | Simple | DateTime conversion | |
| encounter_type | Encounter.class | Complex | Map to FHIR encounter class | |
| encounter_status | Encounter.status | Simple | Map to FHIR status | |
| admission_type | Encounter.type[0] | Complex | Map to FHIR encounter type | |
| service_type | Encounter.serviceType | Complex | Map to SNOMED codes | |

### Participants
| Vendor Field | FHIR Element | Confidence | Transformation | Notes |
|--------------|--------------|------------|----------------|--------|
| attending_provider | Encounter.participant[0].individual | Simple | Reference: "Practitioner/{id}" | |
| attending_provider | Encounter.participant[0].type | Direct | Fixed: "ATND" | |
| referring_provider | Encounter.participant[1].individual | Simple | Reference: "Practitioner/{id}" | |
| referring_provider | Encounter.participant[1].type | Direct | Fixed: "REF" | |

### Location
| Vendor Field | FHIR Element | Confidence | Transformation | Notes |
|--------------|--------------|------------|----------------|--------|
| facility | Encounter.location[0].location | Simple | Reference: "Location/{id}" | |
| room | Encounter.location[0].location.display | Simple | Concatenate facility + room | |
| bed | Encounter.location[0].extension | Complex | Custom extension needed | |

### Diagnoses
| Vendor Field | FHIR Element | Confidence | Transformation | Notes |
|--------------|--------------|------------|----------------|--------|
| diagnosis_codes[0] | Encounter.diagnosis[0].condition | Complex | Create Condition resource | Primary |
| diagnosis_codes[1+] | Encounter.diagnosis[1+].condition | Complex | Create Condition resources | Secondary |
| diagnosis_codes[n].code | Encounter.diagnosis[n].use | Simple | Determine primary/secondary | |

## Observation Resource Mappings

### Observation Core
| Vendor Field | FHIR Element | Confidence | Transformation | Notes |
|--------------|--------------|------------|----------------|--------|
| observation_id | Observation.identifier[0].value | Direct | None | |
| patient_id | Observation.subject | Simple | Reference: "Patient/{id}" | |
| encounter_id | Observation.encounter | Simple | Reference: "Encounter/{id}" | |
| observation_date | Observation.effectiveDateTime | Simple | Combine date + time fields | |
| observation_time | Observation.effectiveDateTime | Simple | Combine with date field | |
| observation_status | Observation.status | Simple | Map to FHIR status | |

### Observation Coding
| Vendor Field | FHIR Element | Confidence | Transformation | Notes |
|--------------|--------------|------------|----------------|--------|
| code | Observation.code.coding[0].code | Direct | None | LOINC preferred |
| code_system | Observation.code.coding[0].system | Simple | Map to standard URI | |
| description | Observation.code.text | Direct | None | Display text |
| category | Observation.category[0] | Complex | Map to FHIR categories | |

### Observation Value
| Vendor Field | FHIR Element | Confidence | Transformation | Notes |
|--------------|--------------|------------|----------------|--------|
| value (numeric) | Observation.valueQuantity.value | Simple | Parse to decimal | |
| unit | Observation.valueQuantity.unit | Direct | None | |
| unit | Observation.valueQuantity.code | Complex | Map to UCUM | |
| unit | Observation.valueQuantity.system | Direct | Fixed: "http://unitsofmeasure.org" | |
| value (text) | Observation.valueString | Direct | None | For text results |
| value (coded) | Observation.valueCodeableConcept | Complex | Map to appropriate codes | |

### Reference Ranges
| Vendor Field | FHIR Element | Confidence | Transformation | Notes |
|--------------|--------------|------------|----------------|--------|
| reference_low | Observation.referenceRange[0].low | Simple | Parse numeric value | |
| reference_high | Observation.referenceRange[0].high | Simple | Parse numeric value | |
| reference_text | Observation.referenceRange[0].text | Direct | None | If not structured |

### Interpretation
| Vendor Field | FHIR Element | Confidence | Transformation | Notes |
|--------------|--------------|------------|----------------|--------|
| abnormal_flag | Observation.interpretation[0] | Complex | Map H/L/N to FHIR codes | |
| critical_flag | Observation.interpretation[1] | Complex | Map to critical codes | |

## Medication Resource Mappings

### Medication Core
| Vendor Field | FHIR Element | Confidence | Transformation | Notes |
|--------------|--------------|------------|----------------|--------|
| medication_id | MedicationRequest.identifier[0].value | Direct | None | |
| patient_id | MedicationRequest.subject | Simple | Reference: "Patient/{id}" | |
| encounter_id | MedicationRequest.encounter | Simple | Reference: "Encounter/{id}" | |
| medication_name | MedicationRequest.medicationCodeableConcept.text | Direct | None | |
| medication_code | MedicationRequest.medicationCodeableConcept.coding[0].code | Direct | None | RxNorm preferred |
| status | MedicationRequest.status | Simple | Map to FHIR status | |
| intent | MedicationRequest.intent | Simple | Default: "order" | |

### Dosage Instructions
| Vendor Field | FHIR Element | Confidence | Transformation | Notes |
|--------------|--------------|------------|----------------|--------|
| dose_amount | MedicationRequest.dosageInstruction[0].doseAndRate[0].doseQuantity.value | Simple | Parse numeric | |
| dose_unit | MedicationRequest.dosageInstruction[0].doseAndRate[0].doseQuantity.unit | Direct | None | |
| frequency | MedicationRequest.dosageInstruction[0].timing | Complex | Parse frequency text | |
| route | MedicationRequest.dosageInstruction[0].route | Simple | Map to SNOMED | |
| instructions | MedicationRequest.dosageInstruction[0].text | Direct | None | Free text |

## Transformation Rules

### Date/Time Transformations
```javascript
// Vendor format: MM/DD/YYYY or MM/DD/YYYY HH:MM
// FHIR format: YYYY-MM-DD or YYYY-MM-DDThh:mm:ss+zz:zz

function transformDate(vendorDate) {
  // Implementation details
  return fhirDate;
}
```

### Gender Mapping Table
| Vendor Code | FHIR Code | Display |
|-------------|-----------|---------|
| M | male | Male |
| F | female | Female |
| U | unknown | Unknown |
| O | other | Other |
| N/A | unknown | Unknown |

### Status Mapping Tables

#### Encounter Status
| Vendor Status | FHIR Status |
|---------------|-------------|
| scheduled | planned |
| arrived | arrived |
| in_progress | in-progress |
| completed | finished |
| cancelled | cancelled |

#### Observation Status
| Vendor Status | FHIR Status |
|---------------|-------------|
| preliminary | preliminary |
| final | final |
| corrected | corrected |
| cancelled | cancelled |

## Complex Transformation Examples

### Creating References
```csharp
// Transform vendor patient_id to FHIR reference
public Reference CreatePatientReference(string vendorPatientId)
{
    return new Reference
    {
        Reference = $"Patient/{vendorPatientId}",
        Display = GetPatientDisplay(vendorPatientId)
    };
}
```

### Handling Null Values
```csharp
// Vendor uses empty string for null
public string TransformNullableField(string vendorValue)
{
    if (string.IsNullOrWhiteSpace(vendorValue) || vendorValue == "N/A")
        return null;
    return vendorValue;
}
```

### Array to FHIR List
```csharp
// Transform vendor diagnosis array to FHIR
public List<Encounter.DiagnosisComponent> TransformDiagnoses(string[] vendorDiagnoses)
{
    var fhirDiagnoses = new List<Encounter.DiagnosisComponent>();
    for (int i = 0; i < vendorDiagnoses.Length; i++)
    {
        fhirDiagnoses.Add(new Encounter.DiagnosisComponent
        {
            Condition = CreateConditionReference(vendorDiagnoses[i]),
            Use = i == 0 ? new CodeableConcept("http://terminology.hl7.org/CodeSystem/diagnosis-role", "AD", "Admission diagnosis")
                        : new CodeableConcept("http://terminology.hl7.org/CodeSystem/diagnosis-role", "DD", "Discharge diagnosis")
        });
    }
    return fhirDiagnoses;
}
```

## Unmapped Fields

### Fields with No FHIR Equivalent
| Vendor Field | Reason | Recommendation |
|--------------|--------|----------------|
| internal_flags | System specific | Store in custom extension if needed |
| legacy_id | Deprecated field | Ignore unless required for migration |
| custom_field_1 | Business specific | Create extension or ignore |

### Fields Requiring Business Decision
| Vendor Field | Options | Decision Needed |
|--------------|---------|-----------------|
| priority_code | Could map to multiple FHIR elements | Determine business meaning |
| custom_status | Non-standard values | Define mapping rules |
| override_flag | System specific | Determine if needed in FHIR |

## Validation Requirements

### Required Fields for Valid FHIR Resources
- **Patient**: identifier, name OR given, gender
- **Encounter**: status, class, subject
- **Observation**: status, code, subject, effectiveDateTime OR effectivePeriod
- **MedicationRequest**: status, intent, medicationCodeableConcept OR medicationReference, subject

### Data Quality Checks
1. Verify all required FHIR fields can be populated
2. Validate code system mappings against standard terminologies
3. Ensure date/time formats are consistently transformed
4. Check reference integrity between resources
5. Validate value ranges and units for observations

## Implementation Notes

### Performance Considerations
- Cache frequently used mappings (codes, references)
- Batch transformations when possible
- Consider lazy loading for large nested structures

### Error Handling
- Log unmapped fields for review
- Provide default values where safe
- Fail fast on critical mapping errors
- Maintain mapping audit trail

### Testing Requirements
- Unit tests for each transformation function
- Integration tests with sample vendor data
- Validation against FHIR profiles
- Performance tests with bulk data

## Review and Approval

### Review Checklist
- [ ] All critical vendor fields mapped
- [ ] Transformations documented clearly
- [ ] Complex mappings have examples
- [ ] Unmapped fields justified
- [ ] Clinical review completed where needed
- [ ] Technical review completed
- [ ] Performance impact assessed

### Approval
- **Technical Reviewer**: [Name] - Date: [Date]
- **Clinical Reviewer**: [Name] - Date: [Date]
- **Project Manager**: [Name] - Date: [Date]