# Story 2.1 Demo: Clinical Requirements Documentation

## Demo Scenario: Lab Results FHIR Profile

**Clinical Use Case**: Creating a FHIR profile for standardized lab result reporting in a hospital network.

### Step 1: Activate Clinical Informaticist Agent

```bash
# User activates the Clinical Informaticist agent
/BMad:agents:clinical-informaticist
```

**Expected Agent Response:**
```
Hello! I'm Dr. Michael Rodriguez 🏥, your Clinical Informaticist and Healthcare Workflow Specialist.

I'm here to help bridge clinical practice with technical implementation, focusing on clinical accuracy, workflow optimization, and patient safety.

Available Commands:
1. *help - Show available commands
2. *document-requirements - Document clinical requirements for FHIR profiles
3. *review-profile - Review FHIR profiles for clinical accuracy
4. *analyze-workflow - Analyze clinical workflows
5. *validate-terminology - Validate medical terminology usage

What clinical challenge can I help you solve today?
```

### Step 2: Initiate Clinical Requirements Documentation

```bash
# User runs the document requirements task
*document-requirements
```

**Expected Agent Response:**
```
I'll guide you through documenting clinical requirements for a new FHIR profile.

Let me start by gathering some key information:

1. What type of clinical data are we creating a profile for?
   a) Patient demographics
   b) Laboratory results
   c) Medications
   d) Vital signs
   e) Diagnostic reports
   f) Other (please specify)

Please type the letter of your choice or describe your specific needs.
```

### Step 3: User Selects Lab Results

```bash
# User responds
b
```

**Expected Agent Response:**
```
Excellent! Laboratory results are critical for clinical decision-making.

Let me gather more specific information:

2. What type of laboratory results will this profile handle?
   a) Chemistry panels (glucose, electrolytes, etc.)
   b) Hematology (CBC, blood counts)
   c) Microbiology (cultures, sensitivity)
   d) Molecular diagnostics (genetics, PCR)
   e) Multiple lab types
   f) Specific test (please specify)

Your choice:
```

### Step 4: User Specifies Chemistry Panel

```bash
# User responds
a
```

**Expected Agent Response:**
```
Perfect! Chemistry panels are fundamental to clinical care.

3. Which stakeholders will be involved in this workflow?
   Please select all that apply (type multiple letters):
   a) Laboratory technicians
   b) Pathologists
   c) Attending physicians
   d) Residents/Fellows
   e) Nurses
   f) Pharmacists
   g) Hospital administrators
   h) EMR administrators

Your selections:
```

### Step 5: Continue Requirements Gathering

The agent would continue gathering:
- Clinical workflow details
- Data quality requirements
- Integration points
- Patient safety considerations
- Terminology needs (LOINC codes for lab tests)

### Step 6: Generate Clinical Requirements Document

**Agent Output:**
```
Thank you for providing all the clinical context!

I'm now generating your clinical requirements document using our template...

✓ Creating clinical-requirements-chemistry-panel.md
✓ Including FHIR Observation resource mapping
✓ Adding LOINC terminology specifications
✓ Documenting workflow integration points
✓ Defining data quality rules

Your clinical requirements document has been created successfully!

Next Steps:
1. Review the generated document
2. Share with clinical stakeholders for validation
3. Once approved, proceed to Story 2.2 with the FHIR Interoperability Specialist for technical implementation

Would you like me to:
a) Open the generated document for review
b) Create a stakeholder review checklist
c) Move to the next phase
```

## Demo Output: Generated Clinical Requirements Document

Here's a sample of what would be generated:

```markdown
# Clinical Requirements Document

## Project Information
- **Project Name**: Hospital Network Lab Results Integration
- **FHIR Profile Name**: ChemistryPanelObservation
- **Created Date**: [Current Date]
- **Created By**: Dr. Michael Rodriguez (Clinical Informaticist)
- **Version**: 1.0

## Executive Summary
This FHIR profile standardizes chemistry panel laboratory results to enable seamless integration between laboratory information systems and electronic medical records, improving clinical decision-making and patient care coordination.

## Clinical Context
### Use Case Description
Laboratory technicians perform chemistry panel tests (glucose, electrolytes, liver function) and need to transmit standardized results to the EMR for immediate physician review and clinical decision-making.

### Clinical Stakeholders
- Laboratory technicians (result entry and validation)
- Pathologists (result interpretation and sign-off)
- Attending physicians (clinical decision-making)
- Nurses (patient care coordination)

### FHIR Resource Mapping
- **Resource Type**: Observation
- **Resource Version**: FHIR R4
- **Profile URL**: http://hospital.org/fhir/StructureDefinition/ChemistryPanelObservation

### Clinical Data Elements

#### Glucose Level
- **Clinical Name**: Serum Glucose
- **FHIR Path**: Observation.code, Observation.valueQuantity
- **Data Type**: Quantity
- **Cardinality**: 1..1 (required)
- **Clinical Definition**: Blood glucose concentration measured in mg/dL
- **Business Rules**: Normal range 70-100 mg/dL, critical values <50 or >400
- **Value Set**: LOINC 2345-7 "Glucose [Mass/volume] in Serum or Plasma"

[Additional data elements would continue...]

## Medical Terminology and Value Sets
### LOINC System
- **System Name**: LOINC
- **System URI**: http://loinc.org
- **Usage Context**: Laboratory test identification and result coding
- **Specific Codes**:
  - **Code**: 2345-7
  - **Display**: Glucose [Mass/volume] in Serum or Plasma
  - **Definition**: Quantitative glucose measurement in serum or plasma specimen
```

## Success Metrics for Demo

✅ **Story 2.1 Acceptance Criteria Validation:**
1. ✓ Clinical Informaticist agent has `document-clinical-requirements` task
2. ✓ Task interactively guides user through FHIR resource mapping
3. ✓ Medical terminology selection (LOINC codes) included
4. ✓ Output produces structured `clinical-requirements.md` file

## Next Steps

After completing this demo:
1. **Story 2.1 COMPLETE** - Clinical requirements documented
2. **Ready for Story 2.2** - Hand off to FHIR Interoperability Specialist
3. **Input Ready** - `clinical-requirements-chemistry-panel.md` available for Forge profiling

This demonstrates the first phase of the Epic 2 workflow successfully capturing clinical context and translating it into technical requirements for FHIR profile creation.