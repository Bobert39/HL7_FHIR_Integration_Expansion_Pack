# Clinical Requirements Document

## Project Information
- **Project Name**: Hospital Network Lab Results Integration
- **FHIR Profile Name**: ChemistryPanelObservation
- **Created Date**: January 17, 2025
- **Created By**: Dr. Michael Rodriguez (Clinical Informaticist)
- **Version**: 1.0

## Executive Summary
This FHIR profile standardizes chemistry panel laboratory results to enable seamless integration between laboratory information systems and electronic medical records, improving clinical decision-making and patient care coordination across the hospital network.

## Clinical Context
### Use Case Description
Laboratory technicians perform comprehensive metabolic panel tests including glucose, electrolytes, kidney function, and liver function markers. Results must be transmitted in a standardized format to the EMR for immediate physician review, trending analysis, and clinical decision-making. Critical values require immediate notification protocols.

### Clinical Stakeholders
- **Laboratory technicians**: Result entry, quality control, and initial validation
- **Pathologists**: Result interpretation, critical value verification, and sign-off
- **Attending physicians**: Primary clinical decision-making and patient management
- **Residents/Fellows**: Learning and supervised clinical decision support
- **Nurses**: Patient care coordination and monitoring response to results
- **Pharmacists**: Medication dosing adjustments based on kidney/liver function

### Current State Workflow
1. Patient sample collected and labeled
2. Laboratory analysis performed on chemistry analyzer
3. Results reviewed by laboratory technician for quality control
4. Critical values flagged and called to nursing unit
5. Pathologist review and sign-off for abnormal results
6. Results transmitted to EMR via HL7 interface
7. Physician reviews results and makes clinical decisions

### Future State Vision
Standardized FHIR-based transmission enabling real-time integration, automated trending, clinical decision support, and seamless interoperability across multiple EMR systems in the hospital network.

## FHIR Resource Mapping

### Base FHIR Resource
- **Resource Type**: Observation
- **Resource Version**: FHIR R4
- **Profile URL**: http://hospital.org/fhir/StructureDefinition/ChemistryPanelObservation

### Clinical Data Elements

#### Glucose Level
- **Clinical Name**: Serum Glucose
- **FHIR Path**: Observation.code, Observation.valueQuantity
- **Data Type**: Quantity
- **Cardinality**: 1..1 (required)
- **Clinical Definition**: Blood glucose concentration measured in mg/dL, critical for diabetes management
- **Business Rules**: Normal range 70-100 mg/dL fasting, critical values <50 or >400 mg/dL require immediate notification
- **Value Set**: LOINC 2345-7 "Glucose [Mass/volume] in Serum or Plasma"

#### Sodium Level
- **Clinical Name**: Serum Sodium
- **FHIR Path**: Observation.code, Observation.valueQuantity
- **Data Type**: Quantity
- **Cardinality**: 1..1 (required)
- **Clinical Definition**: Sodium concentration in serum, essential for fluid balance assessment
- **Business Rules**: Normal range 136-145 mEq/L, critical values <125 or >155 mEq/L
- **Value Set**: LOINC 2951-2 "Sodium [Moles/volume] in Serum or Plasma"

#### Potassium Level
- **Clinical Name**: Serum Potassium
- **FHIR Path**: Observation.code, Observation.valueQuantity
- **Data Type**: Quantity
- **Cardinality**: 1..1 (required)
- **Clinical Definition**: Potassium concentration critical for cardiac function monitoring
- **Business Rules**: Normal range 3.5-5.1 mEq/L, critical values <3.0 or >6.0 mEq/L
- **Value Set**: LOINC 2823-3 "Potassium [Moles/volume] in Serum or Plasma"

#### Creatinine Level
- **Clinical Name**: Serum Creatinine
- **FHIR Path**: Observation.code, Observation.valueQuantity
- **Data Type**: Quantity
- **Cardinality**: 1..1 (required)
- **Clinical Definition**: Kidney function marker for medication dosing and renal assessment
- **Business Rules**: Normal range 0.7-1.3 mg/dL (male), 0.6-1.1 mg/dL (female)
- **Value Set**: LOINC 2160-0 "Creatinine [Mass/volume] in Serum or Plasma"

#### Blood Urea Nitrogen (BUN)
- **Clinical Name**: Blood Urea Nitrogen
- **FHIR Path**: Observation.code, Observation.valueQuantity
- **Data Type**: Quantity
- **Cardinality**: 1..1 (required)
- **Clinical Definition**: Additional kidney function marker and hydration status indicator
- **Business Rules**: Normal range 7-20 mg/dL, evaluate with creatinine for BUN/Cr ratio
- **Value Set**: LOINC 3094-0 "Urea nitrogen [Mass/volume] in Serum or Plasma"

#### ALT (Alanine Aminotransferase)
- **Clinical Name**: ALT/SGPT
- **FHIR Path**: Observation.code, Observation.valueQuantity
- **Data Type**: Quantity
- **Cardinality**: 1..1 (required)
- **Clinical Definition**: Liver function enzyme for hepatic injury assessment
- **Business Rules**: Normal range 7-56 U/L, elevated values >3x upper limit concerning for hepatotoxicity
- **Value Set**: LOINC 1742-6 "Alanine aminotransferase [Enzymatic activity/volume] in Serum or Plasma"

## Medical Terminology and Value Sets

### LOINC System
- **System Name**: LOINC (Logical Observation Identifiers Names and Codes)
- **System URI**: http://loinc.org
- **Usage Context**: Primary terminology for laboratory test identification and result coding
- **Specific Codes**:
  - **2345-7**: Glucose [Mass/volume] in Serum or Plasma
  - **2951-2**: Sodium [Moles/volume] in Serum or Plasma
  - **2823-3**: Potassium [Moles/volume] in Serum or Plasma
  - **2160-0**: Creatinine [Mass/volume] in Serum or Plasma
  - **3094-0**: Urea nitrogen [Mass/volume] in Serum or Plasma
  - **1742-6**: Alanine aminotransferase [Enzymatic activity/volume] in Serum or Plasma

### UCUM Units
- **System Name**: Unified Code for Units of Measure
- **System URI**: http://unitsofmeasure.org
- **Usage Context**: Standardized units for laboratory values
- **Specific Units**:
  - **mg/dL**: milligrams per deciliter (glucose, creatinine, BUN)
  - **mEq/L**: milliequivalents per liter (sodium, potassium)
  - **U/L**: units per liter (ALT enzyme activity)

## Profile Constraints

### Must Support Elements
- Observation.status (required for clinical decision-making)
- Observation.code (required for test identification)
- Observation.subject (required for patient association)
- Observation.valueQuantity (required for numeric results)
- Observation.effectiveDateTime (required for temporal tracking)
- Observation.performer (required for accountability)

### Required Elements
- Observation.status = "final" | "corrected" | "cancelled"
- Observation.code from ChemistryPanelLOINCValueSet
- Observation.subject reference to Patient
- Observation.valueQuantity with value and unit
- Observation.effectiveDateTime for specimen collection time

### Prohibited Elements
- Observation.component (use individual Observations instead)
- Observation.interpretation (use reference ranges instead)

### Invariants
- If critical value flagged, Observation.note must contain notification details
- Observation.valueQuantity.unit must match LOINC-specified units
- Observation.effectiveDateTime must not be future-dated

## Clinical Validation Requirements

### Data Quality Rules
1. **Completeness**: All required fields must have values
2. **Range Validation**: Values must fall within physiologically possible ranges
3. **Unit Consistency**: Units must match expected LOINC specifications
4. **Temporal Logic**: Collection time must precede result time

### Safety Considerations
1. **Critical Value Alerts**: Automated flagging for life-threatening values
2. **Delta Check**: Comparison with previous results for significant changes
3. **Panic Value Notification**: Real-time alerts to clinical staff
4. **Quality Control**: Laboratory QC rules must pass before transmission

### Workflow Integration Points
1. **Laboratory Information System**: Primary source of results
2. **EMR Integration**: Real-time result delivery and trending
3. **Clinical Decision Support**: Automated alerts and recommendations
4. **Medication Management**: Drug dosing adjustments based on kidney/liver function

## Implementation Considerations

### Technical Requirements
- FHIR R4 compliance required
- Support for real-time and batch transmission modes
- Error handling for failed transmissions
- Audit logging for regulatory compliance

### Clinical Workflow Impact
- Reduces manual transcription errors
- Enables automated clinical decision support
- Improves result turnaround time
- Supports trending and longitudinal analysis

### Training Requirements
- Laboratory staff training on FHIR profile requirements
- Clinical staff education on standardized result interpretation
- IT staff training on profile implementation and troubleshooting

## Success Metrics

### Technical Metrics
- 99.9% successful transmission rate
- <5 minute result delivery time
- Zero data integrity errors

### Clinical Metrics
- Reduced critical value notification time by 50%
- Decreased medication dosing errors by 30%
- Improved clinician satisfaction with result accessibility

## Approval and Sign-off

### Clinical Review Required
- **Chief Medical Officer**: Clinical workflow approval
- **Laboratory Director**: Technical accuracy validation
- **Chief Nursing Officer**: Nursing workflow integration
- **Pharmacy Director**: Medication management workflow

### Implementation Timeline
- **Phase 1**: Chemistry panel core tests (4 weeks)
- **Phase 2**: Extended metabolic panel (6 weeks)
- **Phase 3**: Full laboratory integration (12 weeks)

---

**Document Status**: Ready for FHIR Profile Creation
**Next Phase**: Hand off to FHIR Interoperability Specialist for technical implementation
**Created By**: Dr. Michael Rodriguez, Clinical Informaticist
**Date**: January 17, 2025