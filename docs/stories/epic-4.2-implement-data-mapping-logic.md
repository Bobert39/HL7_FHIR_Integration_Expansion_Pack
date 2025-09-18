# Epic 4.2: Implement Data Mapping Logic - OpenEMR to FHIR ChemistryPanel Transformation

**Story ID:** Epic 4.2
**Epic:** Development & Implementation Workflow
**Priority:** High - Critical Path
**Estimated Effort:** 6-8 hours
**Status:** Ready for Implementation
**Depends On:** Epic 4.1 (C# Service Scaffolding)

---

## User Story

**As a healthcare integration developer,**
I want a robust data mapping service that transforms OpenEMR Observation data into ChemistryPanelObservation FHIR profiles,
So that laboratory results are standardized, validated, and compliant with our clinical requirements.

---

## Story Context

### Existing System Integration

- **Integrates with:** OpenEMR Observation API (validated in Epic 3)
- **Technology:** C#/.NET service from Epic 4.1, Firely .NET SDK
- **Follows pattern:** BMad transformation service architecture
- **Touch points:** ChemistryPanelObservation profile validation, unit translation, timezone handling

### Input Dependencies

**Required Artifacts:**
- `docs/demo/completed-integration-partner-profile-openemr.md` - OpenEMR quirks and API patterns
- `docs/demo/ChemistryPanelObservation.json` - Target FHIR profile specification
- Epic 4.1 service scaffolding with Firely SDK integration
- OpenEMR sample observation data from Epic 3 testing

**Prerequisites:**
- Epic 4.1 completed with working health check endpoint
- Firely .NET SDK configured and functional
- Understanding of FHIR R4 Observation resource structure

---

## Acceptance Criteria

### Core Mapping Requirements

1. **Resource Transformation:** Convert OpenEMR Observation JSON to valid ChemistryPanelObservation FHIR resource
2. **Unit Code Translation:** Implement mapping table for non-UCUM units → standard UCUM codes
3. **Timezone Normalization:** Convert all timestamps to UTC with proper timezone handling
4. **Reference Resolution:** Handle both contained and external patient references from OpenEMR

### Data Quality Requirements

5. **FHIR Validation:** All output resources pass Firely SDK validation against ChemistryPanelObservation profile
6. **Critical Value Logic:** Implement business rules for critical values with mandatory note requirements
7. **Error Handling:** Graceful handling of malformed input data with detailed error reporting
8. **Logging:** Comprehensive transformation logging for troubleshooting and audit

### Performance Requirements

9. **Transformation Speed:** <100ms per observation transformation
10. **Memory Efficiency:** Process large batches without memory leaks
11. **Unit Test Coverage:** ≥90% coverage for all transformation logic

---

## Technical Implementation Architecture

### Core Service Interface

```csharp
public interface IFhirMappingService
{
    Task<ChemistryPanelObservation> MapObservationAsync(OpenEmrObservation input);
    Task<ValidationResult> ValidateMappingAsync(ChemistryPanelObservation output);
    Task<List<ChemistryPanelObservation>> MapBatchAsync(List<OpenEmrObservation> inputs);
    Task<TransformationReport> GetTransformationReportAsync(List<OpenEmrObservation> inputs);
}

public class OpenEmrToFhirMappingService : IFhirMappingService
{
    private readonly IUnitCodeTranslator _unitTranslator;
    private readonly ITimezoneNormalizer _timezoneHandler;
    private readonly IFhirValidator _validator;
    private readonly ILogger<OpenEmrToFhirMappingService> _logger;

    // Implementation details...
}
```

### Supporting Services

**Unit Code Translation Service:**
```csharp
public interface IUnitCodeTranslator
{
    string TranslateToUcum(string inputUnit);
    bool IsUcumCompliant(string unit);
    Dictionary<string, string> GetTranslationMappings();
    TranslationResult TryTranslate(string inputUnit, out string ucumUnit);
}
```

**Timezone Normalization Service:**
```csharp
public interface ITimezoneNormalizer
{
    DateTime NormalizeToUtc(string dateTimeString, string sourceTimeZone = null);
    bool IsUtcTimestamp(string dateTimeString);
    TimezoneNormalizationResult ProcessTimestamp(string input);
}
```

**FHIR Validation Service:**
```csharp
public interface IFhirValidator
{
    Task<ValidationResult> ValidateResourceAsync(Resource resource, string profileUrl);
    Task<ValidationResult> ValidateChemistryPanelAsync(ChemistryPanelObservation observation);
    List<ValidationIssue> GetValidationIssues(Resource resource);
}
```

---

## Data Quirks Handling Strategy

### Based on Epic 3 OpenEMR Research

**1. Unit Code Translation Table (Priority 1)**
```csharp
private static readonly Dictionary<string, string> UnitMappings = new()
{
    // Common glucose units
    { "mg/dl", "mg/dL" },
    { "mg/dL", "mg/dL" },           // Already correct
    { "mg%", "mg/dL" },             // Legacy format

    // Electrolyte units
    { "meq/l", "mEq/L" },
    { "mEq/l", "mEq/L" },
    { "mmol/l", "mmol/L" },

    // Protein units
    { "g/dl", "g/dL" },
    { "gm/dl", "g/dL" },

    // Enzyme units
    { "iu/l", "U/L" },
    { "IU/L", "U/L" },
    { "u/l", "U/L" },

    // Additional mappings from OpenEMR testing
    { "units/l", "U/L" },
    { "pg/ml", "pg/mL" },
    { "ng/ml", "ng/mL" },
    { "mg/l", "mg/L" }
};
```

**2. Critical Value Business Rules**
```csharp
public class CriticalValueRules
{
    private static readonly Dictionary<string, CriticalRange> CriticalRanges = new()
    {
        // Glucose (LOINC: 2345-7)
        { "2345-7", new CriticalRange { LowCritical = 50, HighCritical = 400, Unit = "mg/dL" } },

        // Potassium (LOINC: 2823-3)
        { "2823-3", new CriticalRange { LowCritical = 3.0, HighCritical = 6.0, Unit = "mEq/L" } },

        // Sodium (LOINC: 2951-2)
        { "2951-2", new CriticalRange { LowCritical = 120, HighCritical = 160, Unit = "mEq/L" } },

        // Creatinine (LOINC: 2160-0)
        { "2160-0", new CriticalRange { LowCritical = null, HighCritical = 5.0, Unit = "mg/dL" } }
    };

    public bool IsCriticalValue(string loincCode, decimal value, string unit);
    public string GenerateCriticalValueNote(string loincCode, decimal value, string unit);
}
```

**3. Timezone Normalization Logic**
```csharp
public class TimezoneNormalizer : ITimezoneNormalizer
{
    public DateTime NormalizeToUtc(string dateTimeString, string sourceTimeZone = null)
    {
        // Handle OpenEMR patterns:
        // 1. "2024-01-17T10:30:00" (local time, no timezone)
        // 2. "2024-01-17T10:30:00Z" (UTC)
        // 3. "2024-01-17T10:30:00-05:00" (with timezone offset)
        // 4. "2024-01-17 10:30:00" (space separator)

        // Convert all to UTC for standardization
    }
}
```

---

## Project File Structure

```
src/FhirIntegrationService/
├── Services/
│   ├── Mapping/
│   │   ├── IFhirMappingService.cs
│   │   ├── OpenEmrToFhirMappingService.cs
│   │   ├── MappingProfile.cs
│   │   └── TransformationReport.cs
│   ├── Translation/
│   │   ├── IUnitCodeTranslator.cs
│   │   ├── UnitCodeTranslator.cs
│   │   └── UnitMappingTable.cs
│   ├── Validation/
│   │   ├── IFhirValidator.cs
│   │   ├── FhirProfileValidator.cs
│   │   └── CriticalValueValidator.cs
│   └── Utilities/
│       ├── ITimezoneNormalizer.cs
│       ├── TimezoneNormalizer.cs
│       └── ReferenceResolver.cs
├── Models/
│   ├── OpenEmr/
│   │   ├── OpenEmrObservation.cs
│   │   └── OpenEmrPatientReference.cs
│   ├── Fhir/
│   │   └── ChemistryPanelObservation.cs
│   └── Transformation/
│       ├── TransformationResult.cs
│       ├── ValidationResult.cs
│       └── CriticalRange.cs
└── Configuration/
    ├── MappingConfiguration.cs
    └── ValidationConfiguration.cs

tests/FhirIntegrationService.Tests/
├── Services/
│   ├── Mapping/
│   │   ├── OpenEmrToFhirMappingServiceTests.cs
│   │   └── TransformationReportTests.cs
│   ├── Translation/
│   │   └── UnitCodeTranslatorTests.cs
│   ├── Validation/
│   │   └── FhirProfileValidatorTests.cs
│   └── Utilities/
│       └── TimezoneNormalizerTests.cs
├── TestData/
│   ├── OpenEmrSamples/
│   │   ├── glucose-observation.json
│   │   ├── electrolyte-panel.json
│   │   └── liver-function-tests.json
│   └── ExpectedFhirOutputs/
│       ├── glucose-chemistry-panel.json
│       └── electrolyte-chemistry-panel.json
└── Integration/
    └── EndToEndMappingTests.cs
```

---

## Implementation Phases

### Phase 1: Core Transformation (Hours 1-3)
- [ ] Implement basic OpenEmrToFhirMappingService
- [ ] Create core transformation logic for Observation → ChemistryPanelObservation
- [ ] Implement basic unit tests with sample data
- [ ] Verify Firely SDK integration works correctly

### Phase 2: Quirks Handling (Hours 4-5)
- [ ] Implement UnitCodeTranslator with comprehensive mapping table
- [ ] Implement TimezoneNormalizer for OpenEMR patterns
- [ ] Add reference resolution logic for patient references
- [ ] Create unit tests for all quirks handling

### Phase 3: Business Rules & Validation (Hours 6-7)
- [ ] Implement critical value business rules
- [ ] Integrate FHIR profile validation using Firely SDK
- [ ] Add comprehensive error handling and logging
- [ ] Create integration tests with real OpenEMR sample data

### Phase 4: Performance & Polish (Hour 8)
- [ ] Optimize for <100ms transformation performance
- [ ] Implement batch processing with memory management
- [ ] Complete unit test coverage to ≥90%
- [ ] Add performance benchmarking tests

---

## Sample Input/Output

### Input: OpenEMR Observation
```json
{
  "resourceType": "Observation",
  "id": "openemr-glucose-123",
  "status": "final",
  "code": {
    "coding": [{
      "system": "http://loinc.org",
      "code": "2345-7",
      "display": "Glucose [Mass/volume] in Serum or Plasma"
    }]
  },
  "subject": {
    "reference": "Patient/openemr-patient-456"
  },
  "effectiveDateTime": "2024-01-17T10:30:00",
  "valueQuantity": {
    "value": 450,
    "unit": "mg/dl",
    "system": "http://unitsofmeasure.org",
    "code": "mg/dl"
  }
}
```

### Expected Output: ChemistryPanelObservation
```json
{
  "resourceType": "Observation",
  "id": "chemistry-panel-glucose-123",
  "meta": {
    "profile": ["http://hospital.org/fhir/StructureDefinition/ChemistryPanelObservation"]
  },
  "status": "final",
  "code": {
    "coding": [{
      "system": "http://loinc.org",
      "code": "2345-7",
      "display": "Glucose [Mass/volume] in Serum or Plasma"
    }]
  },
  "subject": {
    "reference": "Patient/openemr-patient-456"
  },
  "effectiveDateTime": "2024-01-17T15:30:00Z",
  "performer": [{
    "reference": "Organization/hospital-lab"
  }],
  "valueQuantity": {
    "value": 450,
    "unit": "mg/dL",
    "system": "http://unitsofmeasure.org",
    "code": "mg/dL"
  },
  "note": [{
    "text": "CRITICAL VALUE: Glucose 450 mg/dL exceeds critical threshold (>400). Physician notified per protocol."
  }]
}
```

---

## Definition of Done

- [ ] OpenEmrToFhirMappingService implemented with all interface methods
- [ ] Unit code translation service with comprehensive mapping table
- [ ] Timezone normalization service handling all OpenEMR patterns
- [ ] Reference resolution logic for patient references
- [ ] Critical value business rules implementation with note generation
- [ ] FHIR validation integration using Firely SDK against ChemistryPanelObservation profile
- [ ] Comprehensive unit tests with ≥90% coverage
- [ ] Integration tests using real OpenEMR sample data from Epic 3
- [ ] Performance tests meeting <100ms transformation time requirement
- [ ] Error handling and logging framework implemented
- [ ] Batch processing capability with memory management
- [ ] Documentation updated with transformation logic and mapping tables

---

## Risk Assessment & Mitigation

### Primary Risks

**1. Complex Unit Mapping**
- **Risk:** Some OpenEMR units may not have direct UCUM equivalents
- **Mitigation:** Implement fallback logging for unmapped units, use original with warning flag
- **Monitoring:** Track unmapped unit frequencies in transformation reports

**2. Business Rule Edge Cases**
- **Risk:** Critical value thresholds may need clinical validation
- **Mitigation:** Implement configurable thresholds, clinical review gate before production
- **Monitoring:** Alert on any critical value rule failures

**3. Performance with Large Batches**
- **Risk:** Memory usage with high-volume transformations
- **Mitigation:** Implement streaming processing, memory profiling, batch size limits
- **Monitoring:** Performance metrics and memory usage tracking

### Rollback Strategy
- Unit service failures → Log errors, return original units with validation warnings
- Timezone failures → Use original timestamp with timezone annotation
- Critical value failures → Process observation without note, log for review
- Complete transformation failure → Return detailed error report for manual review

---

## Success Criteria

**Epic 4.2 is complete when:**

1. ✅ **100% Transformation Success:** All valid OpenEMR observations transform successfully
2. ✅ **FHIR Compliance:** All outputs pass ChemistryPanelObservation profile validation
3. ✅ **Quirks Handled:** Unit translation and timezone normalization working correctly
4. ✅ **Performance Targets:** <100ms transformation time achieved consistently
5. ✅ **Quality Gates:** ≥90% test coverage with comprehensive error handling
6. ✅ **Business Rules:** Critical value detection and note generation working
7. ✅ **Production Ready:** Service integrates cleanly with Epic 4.1 scaffolding

---

## Next Steps After Completion

**After Epic 4.2 completion:**
- Hand off to Epic 4.3: Create Production API Endpoints
- Use mapping service as core of patient data retrieval API
- Integrate transformation reports into API response metadata
- Leverage established patterns for additional FHIR resource types

---

**Generated by BMad PM Agent | Product Manager: John 📋**
**Date:** 2024-09-17
**Change Log:** Initial story creation for Epic 4.2 - Data Mapping Implementation
**Based on:** Epic 3 OpenEMR research, Epic 2 FHIR profiles, Epic 4.1 service foundation