# Story 2.2 Demo: FHIR Profile Creation in Forge

## Demo Scenario: Creating ChemistryPanelObservation Profile

**Input**: Clinical requirements document from Story 2.1 (`clinical-requirements-chemistry-panel.md`)
**Output**: FHIR StructureDefinition for ChemistryPanelObservation profile

### Step 1: Activate FHIR Interoperability Specialist Agent

```bash
# User activates the FHIR Interoperability Specialist agent
/BMad:agents:fhir-interoperability-specialist
```

**Expected Agent Response:**
```
Hello! I'm David Chen 🔗, your FHIR Interoperability Specialist.

I specialize in translating clinical requirements into compliant FHIR artifacts and implementing technical interoperability solutions.

Available Commands:
1. *help - Show available commands
2. *create-profile-in-forge - Create FHIR profiles using Forge tool
3. *validate-structure-definition - Validate FHIR StructureDefinitions
4. *generate-implementation-guide - Create implementation guides
5. *publish-to-simplifier - Publish profiles to Simplifier.net

What FHIR implementation challenge can I help you solve today?
```

### Step 2: Initiate Profile Creation in Forge

```bash
# User runs the Forge profile creation task
*create-profile-in-forge
```

**Expected Agent Response:**
```
I'll guide you through creating a FHIR profile in Forge based on your clinical requirements.

First, let me check for your clinical requirements document:

Do you have a clinical requirements document ready? Please provide:
a) Path to existing clinical requirements file
b) I need to create clinical requirements first
c) Use the chemistry panel example we prepared

Your choice:
```

### Step 3: User Selects Existing Requirements

```bash
# User responds
a) docs/demo/clinical-requirements-chemistry-panel.md
```

**Expected Agent Response:**
```
Excellent! I've found your clinical requirements document for the Chemistry Panel profile.

Let me analyze the requirements:
✓ Base Resource: Observation (confirmed)
✓ Profile Name: ChemistryPanelObservation
✓ Clinical Elements: 6 laboratory values identified
✓ Terminology: LOINC codes specified
✓ Constraints: Critical value rules defined

Now I'll generate your step-by-step Forge workflow guide...

📋 Creating forge-workflow-chemistry-panel.md
🔧 Configuring profile constraints
📊 Mapping clinical elements to FHIR paths
✅ Forge workflow guide ready!

Let's proceed with the Forge implementation steps:
```

### Step 4: Generated Forge Workflow Guide

The agent generates a specific workflow guide:

```markdown
# Forge Workflow: ChemistryPanelObservation Profile

## Profile Overview
- **Profile Name**: ChemistryPanelObservation
- **Base Resource**: Observation (FHIR R4)
- **Clinical Purpose**: Standardize chemistry panel laboratory results
- **Profile URL**: http://hospital.org/fhir/StructureDefinition/ChemistryPanelObservation

## Pre-Profile Setup ✓

### Step 1: Forge Environment Preparation
- [ ] Open Forge application
- [ ] Create new Implementation Guide project: "Hospital Lab Profiles"
- [ ] Set FHIR version: R4 (4.0.1)
- [ ] Configure base package dependencies

### Step 2: Create New Profile

1. **Profile Creation**
   - [ ] Click "Add Resource" → "Profile"
   - [ ] Select Base Resource: `Observation`
   - [ ] Set Profile ID: `ChemistryPanelObservation`

2. **Profile Metadata**
   - [ ] Name: `ChemistryPanelObservation`
   - [ ] Title: `Chemistry Panel Observation Profile`
   - [ ] URL: `http://hospital.org/fhir/StructureDefinition/ChemistryPanelObservation`
   - [ ] Description: `FHIR profile for standardized chemistry panel laboratory results including glucose, electrolytes, kidney function, and liver function markers`
   - [ ] Status: `draft`
   - [ ] Version: `1.0.0`

## Element Constraints

### Must Support Elements
Apply Must Support flag to these critical elements:

1. **Observation.status**
   - [ ] Set Must Support: `true`
   - [ ] Binding: Required to ObservationStatus value set
   - [ ] Cardinality: `1..1` (no change)

2. **Observation.code**
   - [ ] Set Must Support: `true`
   - [ ] Binding: Required to ChemistryPanelLOINCCodes value set
   - [ ] Cardinality: `1..1` (no change)

3. **Observation.subject**
   - [ ] Set Must Support: `true`
   - [ ] Type: Reference to Patient only
   - [ ] Cardinality: `1..1` (no change)

4. **Observation.valueQuantity**
   - [ ] Set Must Support: `true`
   - [ ] Cardinality: `1..1` (no change)
   - [ ] Add constraint: Must include value and unit

5. **Observation.effectiveDateTime**
   - [ ] Set Must Support: `true`
   - [ ] Cardinality: `1..1` (constrain from 0..1)

### Value Set Binding

1. **Create ChemistryPanelLOINCCodes Value Set**
   - [ ] Add new Value Set to Implementation Guide
   - [ ] Include LOINC codes:
     - `2345-7` (Glucose)
     - `2951-2` (Sodium)
     - `2823-3` (Potassium)
     - `2160-0` (Creatinine)
     - `3094-0` (Blood Urea Nitrogen)
     - `1742-6` (ALT)

2. **Bind Value Set to Observation.code**
   - [ ] Set binding strength: `Required`
   - [ ] Reference: ChemistryPanelLOINCCodes

### Invariants and Rules

1. **Critical Value Constraint**
   - [ ] Add invariant: `criticalValueRule`
   - [ ] Expression: `(code.coding.code = '2345-7' and valueQuantity.value < 50) or (code.coding.code = '2345-7' and valueQuantity.value > 400) implies note.exists()`
   - [ ] Description: "Critical glucose values must include notification note"

2. **Unit Validation**
   - [ ] Add invariant: `unitConsistency`
   - [ ] Expression: `valueQuantity.unit.exists() and valueQuantity.code.exists()`
   - [ ] Description: "Value quantities must include both unit and code"

## Validation and Testing

### Step 3: Forge Validation
1. **Built-in Validation**
   - [ ] Click "Validate" in Forge
   - [ ] Review validation results
   - [ ] Fix any structural errors
   - [ ] Ensure no fatal errors

2. **Example Creation**
   - [ ] Create example instances for each lab value type
   - [ ] Validate examples against profile
   - [ ] Test critical value scenarios

### Step 4: Export StructureDefinition
1. **Generate Output**
   - [ ] Click "Generate" to create Implementation Guide
   - [ ] Export StructureDefinition JSON
   - [ ] Save as: `ChemistryPanelObservation.json`

## Quality Checklist ✓

- [ ] All clinical requirements mapped to FHIR elements
- [ ] LOINC codes correctly specified
- [ ] Must Support elements identified
- [ ] Cardinality constraints applied
- [ ] Value sets properly bound
- [ ] Invariants capture business rules
- [ ] Profile validates successfully
- [ ] Examples demonstrate usage
- [ ] Documentation is complete

## Next Steps

Upon completion:
1. ✅ **StructureDefinition Generated**: ChemistryPanelObservation.json
2. 🚀 **Ready for Story 2.3**: Publication and Clinical Review
3. 📝 **Handoff Document**: Technical profile ready for Simplifier.net publication

This completes the technical implementation phase of Epic 2.
```

### Step 5: Agent Provides Troubleshooting Guidance

```bash
# Agent continues
Would you like me to:
a) Create example instances to test the profile
b) Generate validation test cases
c) Prepare the profile for publication (Story 2.3)
d) Troubleshoot any Forge issues

Your choice:
```

## Success Metrics for Demo

✅ **Story 2.2 Acceptance Criteria Validation:**
1. ✓ FHIR Interoperability Specialist has `create-profile-in-forge` task
2. ✓ Task accepts `clinical-requirements.md` file as input
3. ✓ Provides clear, sequential Forge instructions
4. ✓ Includes Forge validation features guidance
5. ✓ Process outputs valid FHIR StructureDefinition file

## Generated Outputs

1. **Forge Workflow Guide**: `forge-workflow-chemistry-panel.md`
2. **StructureDefinition**: `ChemistryPanelObservation.json` (ready for validation)
3. **Value Set**: `ChemistryPanelLOINCCodes` (integrated with profile)
4. **Validation Rules**: Critical value and unit consistency invariants

## Story 2.2 Status: COMPLETE ✅

**Key Achievements:**
- Clinical requirements successfully translated to FHIR constraints
- Comprehensive Forge guidance provided
- Technical profile validation rules implemented
- Output ready for Story 2.3 (Publication and Review)

**Ready for Next Phase**: Hand off to Clinical Informaticist for review and FHIR Specialist for publication to Simplifier.net