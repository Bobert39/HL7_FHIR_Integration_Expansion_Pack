# review-simplifier-profile

## Task Definition
**ID**: review-simplifier-profile
**Description**: Clinical validation and approval of published FHIR profile on Simplifier.net
**Agent**: Clinical Informaticist
**Elicit**: true

## Task Overview
This task guides the Clinical Informaticist through reviewing a published FHIR profile on Simplifier.net, validating it against original clinical requirements, and providing formal clinical approval with documentation.

## Inputs Required
- Published Simplifier.net profile URL
- Human-readable view URL
- Original clinical requirements document
- Profile publication metadata

## Outputs Produced
- Clinical validation report
- Formal approval document
- Clinical compliance checklist results
- Recommendations for profile improvements (if needed)

## Prerequisites
- Published FHIR profile accessible on Simplifier.net
- Original clinical requirements document available
- Clinical domain expertise for validation
- Access to clinical workflow documentation

## Step-by-Step Workflow

### Step 1: Gather Review Information
**Elicit Information:**
```
Please provide the following information for clinical review:
1. Published Simplifier.net profile URL: [User Input]
2. Human-readable view URL: [User Input]
3. Path to original clinical requirements document: [User Input]
4. Clinical use case being addressed: [User Input]
5. Target clinical workflows: [User Input]
6. Intended user personas (clinicians, roles): [User Input]
```

### Step 2: Access and Review Published Profile
Guide reviewer through Simplifier.net interface:

1. **Access Published Profile**
   - Open provided Simplifier.net URL
   - Verify profile loads correctly
   - Review publication metadata
   - Check version and status information

2. **Review Human-Readable View**
   - Access human-readable documentation
   - Review profile description and purpose
   - Examine element definitions and constraints
   - Validate terminology bindings and value sets

3. **Examine Technical Details**
   - Review StructureDefinition elements
   - Validate cardinality constraints
   - Check data types and extensions
   - Verify profile derivation hierarchy

### Step 3: Clinical Requirements Validation
Use template: `clinical-review-checklist.tmpl.md`

**Requirement Traceability Analysis:**
```
For each clinical requirement from the original document:
1. Identify corresponding FHIR profile elements
2. Validate constraint appropriateness
3. Verify clinical workflow alignment
4. Check data capture completeness
5. Assess usability for intended users
```

**Clinical Validation Checklist:**
- [ ] Profile addresses all stated clinical requirements
- [ ] Data elements align with clinical workflow needs
- [ ] Cardinality constraints match clinical reality
- [ ] Terminology bindings are clinically appropriate
- [ ] Required vs. optional elements match clinical priorities
- [ ] Profile supports intended clinical use cases
- [ ] Data capture burden is reasonable for clinicians
- [ ] Profile enables quality measurement and reporting
- [ ] Patient safety considerations are addressed
- [ ] Compliance with relevant clinical guidelines

### Step 4: Clinical Workflow Assessment
Evaluate profile against real-world clinical scenarios:

1. **Workflow Integration**
   - Assess data collection points in clinical workflow
   - Evaluate documentation burden on clinicians
   - Check alignment with existing EHR workflows
   - Verify support for clinical decision-making

2. **User Experience Validation**
   - Consider data entry requirements for clinical staff
   - Evaluate information display and retrieval needs
   - Assess impact on clinical efficiency
   - Check accessibility for different user roles

3. **Clinical Safety Review**
   - Identify potential safety risks
   - Validate critical data element requirements
   - Check alert and notification requirements
   - Assess data validation and quality controls

### Step 5: Generate Clinical Validation Report
Use template: `profile-approval-document.tmpl.md`

**Report Components:**
1. **Executive Summary**
   - Clinical validation outcome
   - Key findings and recommendations
   - Approval status and conditions

2. **Requirements Traceability Matrix**
   - Original requirement → FHIR element mapping
   - Validation status for each requirement
   - Gap analysis and recommendations

3. **Clinical Assessment Results**
   - Workflow integration assessment
   - User experience evaluation
   - Safety and quality implications
   - Compliance with clinical standards

4. **Recommendations**
   - Approved elements and constraints
   - Suggested modifications (if any)
   - Implementation guidance
   - Future enhancement opportunities

### Step 6: Formal Approval Process
**Approval Decision Matrix:**

**APPROVED**: Profile meets all clinical requirements and is ready for implementation
- All validation criteria met
- No significant clinical concerns
- Supports intended clinical workflows
- Aligns with clinical best practices

**APPROVED WITH CONDITIONS**: Profile approved with specific modifications required
- Minor adjustments needed for clinical alignment
- Non-critical workflow considerations
- Recommendations for future versions
- Conditional implementation guidance

**REQUIRES REVISION**: Profile needs significant changes before clinical approval
- Major clinical requirements not met
- Workflow integration concerns
- Patient safety considerations
- Return to FHIR Interoperability Specialist for revision

### Step 7: Document Approval Decision
Create formal approval documentation:

```
CLINICAL APPROVAL DECISION
Profile: [Profile Name] v[Version]
Simplifier.net URL: [Profile URL]
Review Date: [Current Date]
Reviewer: [Clinical Informaticist Name/Role]

DECISION: [APPROVED | APPROVED WITH CONDITIONS | REQUIRES REVISION]

CLINICAL VALIDATION SUMMARY:
- Requirements Coverage: [Percentage]%
- Workflow Alignment: [Assessment]
- Safety Assessment: [Assessment]
- Implementation Readiness: [Assessment]

CONDITIONS/RECOMMENDATIONS:
[List any conditions or recommendations]

CLINICAL APPROVAL SIGNATURE:
[Digital signature/timestamp]
```

## Validation Criteria

### Clinical Requirements Validation
- All original clinical requirements addressed
- FHIR elements appropriately constrained
- Clinical workflows properly supported
- User experience considerations addressed

### Quality and Safety Validation
- Patient safety implications assessed
- Data quality controls validated
- Clinical decision support enabled
- Regulatory compliance maintained

### Implementation Readiness
- Profile ready for clinical implementation
- Documentation sufficient for implementers
- Training requirements identified
- Change management considerations addressed

## Error Handling

### Access Issues
- **Profile not accessible**: Verify URL and publication status
- **Requirements document missing**: Request document from FHIR Specialist
- **Insufficient clinical context**: Request additional clinical information

### Validation Issues
- **Requirements gaps identified**: Document gaps and recommend revisions
- **Clinical workflow concerns**: Provide specific feedback for improvement
- **Safety concerns**: Escalate immediately with detailed documentation

### Documentation Issues
- **Incomplete profile documentation**: Request additional documentation
- **Unclear clinical purpose**: Seek clarification from original requirements
- **Missing implementation guidance**: Recommend additional documentation

## Success Criteria
- [ ] Published profile fully reviewed and validated
- [ ] All clinical requirements assessed for coverage
- [ ] Formal approval decision documented
- [ ] Clinical validation report completed
- [ ] Recommendations provided for any identified issues
- [ ] Approval documentation signed and dated

## Dependencies
- **Templates**: clinical-review-checklist.tmpl.md, profile-approval-document.tmpl.md
- **External Resources**: Simplifier.net profile, clinical requirements document
- **Clinical Knowledge**: Domain expertise for validation
- **Agent Coordination**: FHIR Interoperability Specialist (for revision requests)

## Notes
- This task ensures clinical validation of technical FHIR profiles
- Provides formal approval process for regulatory compliance
- Bridges technical implementation with clinical practice requirements
- Establishes audit trail for clinical validation decisions
- Supports continuous improvement through feedback and recommendations