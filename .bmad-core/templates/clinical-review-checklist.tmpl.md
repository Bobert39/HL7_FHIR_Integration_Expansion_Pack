# Clinical Review Checklist for FHIR Profile

## Template: clinical-review-checklist
**Version**: 1.0
**Purpose**: Comprehensive clinical validation checklist for published FHIR profiles
**Usage**: Referenced by review-simplifier-profile task

---

## Profile Under Review
- **Profile Name**: {profile_name}
- **Version**: {profile_version}
- **Simplifier.net URL**: {profile_url}
- **Human-readable URL**: {human_readable_url}
- **Review Date**: {review_date}
- **Reviewer**: {reviewer_name} ({reviewer_role})

## Clinical Requirements Validation

### Requirements Traceability
For each original clinical requirement, verify:

| Requirement ID | Original Requirement | FHIR Element(s) | Status | Notes |
|----------------|---------------------|-----------------|--------|-------|
| {req_id_1} | {requirement_text_1} | {fhir_elements_1} | ✅/❌/⚠️ | {notes_1} |
| {req_id_2} | {requirement_text_2} | {fhir_elements_2} | ✅/❌/⚠️ | {notes_2} |
| {req_id_3} | {requirement_text_3} | {fhir_elements_3} | ✅/❌/⚠️ | {notes_3} |

**Legend**: ✅ Fully Addressed | ❌ Not Addressed | ⚠️ Partially Addressed

### Clinical Data Elements Review
- [ ] **Essential Data Elements**: All clinically essential data captured
- [ ] **Data Completeness**: Profile captures complete clinical picture
- [ ] **Data Accuracy**: Constraints ensure clinical data accuracy
- [ ] **Data Granularity**: Appropriate level of detail for clinical use
- [ ] **Missing Elements**: No critical clinical data elements missing

## Clinical Workflow Assessment

### Workflow Integration
- [ ] **Data Collection**: Aligns with natural clinical data collection points
- [ ] **Clinical Decision Support**: Supports clinical decision-making processes
- [ ] **Care Coordination**: Enables effective care team communication
- [ ] **Quality Measurement**: Supports quality metrics and reporting
- [ ] **Documentation Workflow**: Integrates smoothly with clinical documentation

### User Experience Evaluation
- [ ] **Data Entry Burden**: Reasonable documentation burden for clinicians
- [ ] **Information Retrieval**: Easy access to relevant clinical information
- [ ] **Workflow Efficiency**: Does not significantly slow clinical processes
- [ ] **User Role Alignment**: Appropriate for intended clinical user roles
- [ ] **Training Requirements**: Reasonable learning curve for clinical staff

### Clinical Context Validation
- [ ] **Care Setting Appropriateness**: Suitable for intended care environments
- [ ] **Patient Population**: Appropriate for target patient demographics
- [ ] **Clinical Specialty**: Aligns with specialty-specific workflows
- [ ] **Care Episodes**: Supports complete care episode documentation
- [ ] **Longitudinal Care**: Enables tracking across care continuum

## Patient Safety and Quality

### Safety Considerations
- [ ] **Critical Data Elements**: Essential safety data required appropriately
- [ ] **Alert Requirements**: Supports necessary clinical alerts and warnings
- [ ] **Error Prevention**: Constraints help prevent data entry errors
- [ ] **Risk Mitigation**: Addresses identified clinical risks
- [ ] **Safety Monitoring**: Enables safety event tracking and reporting

### Quality and Compliance
- [ ] **Clinical Guidelines**: Aligns with relevant clinical practice guidelines
- [ ] **Quality Measures**: Supports quality indicator calculation
- [ ] **Regulatory Compliance**: Meets applicable regulatory requirements
- [ ] **Standards Adherence**: Complies with professional standards
- [ ] **Evidence-Based Practice**: Supports evidence-based clinical decisions

### Data Integrity and Validation
- [ ] **Data Validation Rules**: Appropriate constraints for data quality
- [ ] **Required vs Optional**: Correct balance of required and optional elements
- [ ] **Value Set Appropriateness**: Clinical terminology properly constrained
- [ ] **Data Type Validation**: Correct data types for clinical concepts
- [ ] **Consistency Checks**: Cross-element validation rules appropriate

## Technical Clinical Assessment

### Terminology and Coding
- [ ] **Value Set Coverage**: Terminology covers clinical use cases completely
- [ ] **Code System Selection**: Appropriate code systems for clinical concepts
- [ ] **Terminology Binding**: Correct binding strength (required/preferred/example)
- [ ] **Local Extensions**: Necessary extensions for local clinical needs
- [ ] **Interoperability**: Standard terminologies used where possible

### Cardinality and Constraints
- [ ] **Minimum Cardinality**: Required elements match clinical necessities
- [ ] **Maximum Cardinality**: Upper bounds appropriate for clinical reality
- [ ] **Conditional Requirements**: Context-dependent requirements properly defined
- [ ] **Business Rules**: Clinical business rules correctly implemented
- [ ] **Constraint Rationale**: Clear clinical justification for all constraints

### Profile Structure
- [ ] **Base Resource Selection**: Appropriate FHIR resource as foundation
- [ ] **Extension Usage**: Custom extensions necessary and well-defined
- [ ] **Profile Hierarchy**: Correct derivation from base profiles
- [ ] **Element Definitions**: Clear clinical definitions for all elements
- [ ] **Examples**: Representative clinical examples provided

## Implementation Considerations

### Clinical Adoption
- [ ] **Change Management**: Profile changes manageable for clinical staff
- [ ] **Training Needs**: Reasonable training requirements identified
- [ ] **Implementation Timeline**: Realistic implementation expectations
- [ ] **Pilot Testing**: Opportunities for pilot testing identified
- [ ] **Feedback Mechanisms**: Processes for ongoing clinical feedback

### System Integration
- [ ] **EHR Compatibility**: Profile implementable in typical EHR systems
- [ ] **Interface Requirements**: Clear data exchange requirements
- [ ] **Performance Impact**: Minimal impact on system performance
- [ ] **Migration Strategy**: Clear path from existing data structures
- [ ] **Maintenance Requirements**: Sustainable ongoing maintenance needs

### Clinical Governance
- [ ] **Approval Process**: Clear clinical approval and sign-off process
- [ ] **Version Control**: Appropriate versioning strategy for clinical updates
- [ ] **Change Control**: Process for managing clinical requirement changes
- [ ] **Documentation**: Comprehensive implementation documentation
- [ ] **Support**: Ongoing clinical support during implementation

## Overall Clinical Assessment

### Clinical Validation Summary
**Requirements Coverage**: {coverage_percentage}% of clinical requirements addressed
**Workflow Alignment**: {workflow_score}/10 - Profile alignment with clinical workflows
**Safety Assessment**: {safety_score}/10 - Patient safety considerations addressed
**Quality Impact**: {quality_score}/10 - Expected impact on care quality
**Implementation Readiness**: {readiness_score}/10 - Readiness for clinical implementation

### Key Findings
1. **Strengths**:
   - {strength_1}
   - {strength_2}
   - {strength_3}

2. **Areas for Improvement**:
   - {improvement_1}
   - {improvement_2}
   - {improvement_3}

3. **Critical Issues** (if any):
   - {critical_issue_1}
   - {critical_issue_2}

### Clinical Recommendations
- **Priority 1 (Critical)**: {priority_1_recommendations}
- **Priority 2 (Important)**: {priority_2_recommendations}
- **Priority 3 (Enhancement)**: {priority_3_recommendations}

## Approval Decision

### Decision Matrix
Based on clinical validation criteria:

- **APPROVED**: ✅ Profile meets all clinical requirements and is ready for implementation
- **APPROVED WITH CONDITIONS**: ⚠️ Profile approved with specific modifications required
- **REQUIRES REVISION**: ❌ Profile needs significant changes before clinical approval

### Decision: {approval_decision}

### Conditions/Requirements (if applicable):
1. {condition_1}
2. {condition_2}
3. {condition_3}

### Clinical Approval
- **Approver**: {approver_name}
- **Title/Role**: {approver_title}
- **Approval Date**: {approval_date}
- **Digital Signature**: {digital_signature}

---

**Checklist Version**: 1.0
**Last Updated**: {current_date}
**Maintained By**: Clinical Informaticist Agent