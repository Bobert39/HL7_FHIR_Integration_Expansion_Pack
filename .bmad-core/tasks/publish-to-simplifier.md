# publish-to-simplifier

## Task Definition
**ID**: publish-to-simplifier
**Description**: Guide user through publishing FHIR StructureDefinition to Simplifier.net with clinical review coordination
**Agent**: FHIR Interoperability Specialist
**Elicit**: true

## Task Overview
This task guides the user through the process of publishing a FHIR StructureDefinition file to Simplifier.net for centralized documentation and versioning, then coordinates with the Clinical Informaticist for clinical validation and approval.

## Inputs Required
- FHIR StructureDefinition file (.json or .xml format)
- Profile metadata (name, version, description, purpose)
- Simplifier.net account credentials
- Original clinical requirements document for validation reference

## Outputs Produced
- Published profile URL on Simplifier.net
- Publication confirmation with human-readable view
- Clinical review coordination handoff
- Publication audit trail

## Prerequisites
- Valid Simplifier.net account with publishing permissions
- Completed FHIR StructureDefinition file from Forge
- Clinical requirements document for validation reference
- Internet connectivity for Simplifier.net access

## Step-by-Step Workflow

### Step 1: Validate Input Files
```
Validate that the StructureDefinition file is:
- Valid JSON or XML format
- Contains required FHIR resource elements
- Has proper profile metadata (url, name, status, description)
- Passes basic FHIR validation rules
```

**Elicit Information:**
```
Please provide the following information:
1. Path to your FHIR StructureDefinition file: [User Input]
2. Profile name: [User Input]
3. Profile version: [User Input]
4. Profile description: [User Input]
5. Clinical use case/purpose: [User Input]
6. Simplifier.net organization/project name: [User Input]
```

### Step 2: Pre-Publication Checklist
Use template: `simplifier-publication-guide.tmpl.md`

Verify the following items are complete:
- [ ] StructureDefinition has unique canonical URL
- [ ] Profile name follows naming conventions
- [ ] Version number is properly formatted (semantic versioning)
- [ ] Description clearly explains clinical purpose
- [ ] Contact information is included
- [ ] Copyright and license information is specified
- [ ] Profile elements have appropriate cardinality
- [ ] All constraints are properly documented

### Step 3: Simplifier.net Upload Process
Guide user through the publication steps:

1. **Login to Simplifier.net**
   - Navigate to https://simplifier.net
   - Sign in with credentials
   - Select appropriate organization/project

2. **Upload StructureDefinition**
   - Click "Add Resource" or "Upload"
   - Select the StructureDefinition file
   - Verify file upload success
   - Review auto-generated metadata

3. **Configure Publication Settings**
   - Set publication status (draft, active, retired)
   - Configure access permissions
   - Add relevant tags and categories
   - Set up version management

4. **Publish and Verify**
   - Click "Publish" to make profile available
   - Verify profile appears in project listing
   - Access human-readable view
   - Confirm canonical URL accessibility

### Step 4: Generate Publication Documentation
Create publication record with:
- Publication timestamp
- Published profile URL
- Human-readable view URL
- Version information
- Publication settings summary

### Step 5: Coordinate Clinical Review
**Inter-Agent Coordination:**

Generate handoff message for Clinical Informaticist:
```
CLINICAL REVIEW REQUEST
Profile Published: [Profile Name] v[Version]
Simplifier.net URL: [Published URL]
Human-readable View: [Human-readable URL]
Original Requirements: [Requirements Document Path]
Publication Date: [Timestamp]

Please initiate clinical review using the *review-simplifier-profile command.
```

**Action**: Prompt user to engage Clinical Informaticist agent for review:
```
Your FHIR profile has been successfully published to Simplifier.net!

Published Profile: [Profile URL]
Human-readable View: [Human-readable URL]

NEXT STEP: Please activate the Clinical Informaticist agent and use the
*review-simplifier-profile command to perform clinical validation and approval.

The Clinical Informaticist will need:
- The published profile URL above
- The original clinical requirements document
- Access to the human-readable view for validation
```

## Validation Criteria

### Technical Validation
- StructureDefinition file is valid FHIR format
- Profile validates against FHIR R4 base specification
- Canonical URL is unique and accessible
- All required metadata fields are populated

### Publication Validation
- Profile successfully uploads to Simplifier.net
- Human-readable view generates correctly
- Profile is accessible via canonical URL
- Version information is correctly displayed

### Process Validation
- User provides all required input information
- Pre-publication checklist completed
- Clinical review coordination initiated
- Publication audit trail documented

## Error Handling

### File Validation Errors
- **Invalid FHIR format**: Guide user to validate file using Forge or FHIR validator
- **Missing metadata**: Prompt user to complete required fields
- **Duplicate canonical URL**: Suggest URL modification or version increment

### Publication Errors
- **Upload failure**: Check file size, format, and network connectivity
- **Permission denied**: Verify Simplifier.net account permissions
- **Server errors**: Suggest retry with backoff strategy

### Coordination Errors
- **Requirements document missing**: Request path to clinical requirements
- **Clinical Informaticist unavailable**: Provide manual review checklist as fallback

## Success Criteria
- [ ] StructureDefinition file successfully published to Simplifier.net
- [ ] Human-readable view is accessible and displays correctly
- [ ] Clinical review coordination successfully initiated
- [ ] Publication audit trail created and documented
- [ ] User can access published profile via canonical URL

## Dependencies
- **Templates**: simplifier-publication-guide.tmpl.md
- **External Tools**: Simplifier.net, FHIR validation services
- **Agent Coordination**: Clinical Informaticist agent (review-simplifier-profile task)
- **Input Files**: FHIR StructureDefinition, clinical requirements document

## Notes
- This task bridges technical FHIR implementation with clinical validation
- Ensures proper documentation and versioning through Simplifier.net
- Establishes audit trail for regulatory compliance
- Facilitates collaborative review between technical and clinical stakeholders