# StructureDefinition Validation Checklist

## Pre-Validation Preparation

### Profile Information
- **Profile Name**: `[Profile Name]`
- **Profile URL**: `[Profile URL]`
- **Base Resource**: `[Base FHIR Resource]`
- **FHIR Version**: `[FHIR Version - R4]`
- **Profile Status**: `[draft/active/retired]`
- **Validation Date**: `[Date]`
- **Validated By**: `[Validator Name/Role]`

### Validation Environment
- **Forge Version**: `[Version Number]`
- **FHIR Package Version**: `[Package Version]`
- **Terminology Server**: `[Server URL if applicable]`
- **Additional Dependencies**: `[List any additional packages]`

## Structural Validation

### 1. Profile Metadata Validation
- [ ] **Profile URL** is unique and follows organizational conventions
- [ ] **Profile Name** is descriptive and follows naming conventions
- [ ] **Title** clearly describes the profile purpose
- [ ] **Description** provides comprehensive profile overview
- [ ] **Status** is appropriate for profile maturity level
- [ ] **Version** follows semantic versioning if applicable
- [ ] **Publisher** information is complete and accurate
- [ ] **Contact** information is provided and current

### 2. Resource Structure Validation
- [ ] **Base Resource** is correctly specified and appropriate for use case
- [ ] **Profile Kind** is set correctly (resource/datatype/extension)
- [ ] **Abstract Flag** is set appropriately
- [ ] **Context** is properly defined if profile is a data type or extension
- [ ] **Derivation** is set correctly (specialization/constraint)

### 3. Element Constraint Validation
- [ ] **Cardinality Constraints** are valid and not more restrictive than base
- [ ] **Data Type Constraints** are appropriate and valid
- [ ] **Must Support Flags** are set on clinically important elements
- [ ] **Fixed Values** are used appropriately and are valid
- [ ] **Pattern Values** are correctly applied where needed
- [ ] **Default Values** are set appropriately if applicable

## Content Validation

### 4. Terminology Binding Validation
- [ ] **Value Set Bindings** reference valid and accessible value sets
- [ ] **Code System References** use correct and standard URIs
- [ ] **Binding Strength** is appropriate for each coded element
  - [ ] Required bindings for essential clinical concepts
  - [ ] Extensible bindings for comprehensive but expandable sets
  - [ ] Preferred bindings for recommended but flexible choices
  - [ ] Example bindings for guidance without enforcement
- [ ] **Terminology Server Connectivity** confirmed if external references used

### 5. Invariant and Business Rule Validation
- [ ] **FHIRPath Expressions** are syntactically correct
- [ ] **Invariant Logic** correctly implements intended business rules
- [ ] **Severity Levels** are appropriate (error/warning/information)
- [ ] **Human Descriptions** are clear and helpful for implementers
- [ ] **Invariant Keys** are unique within the profile
- [ ] **Expression Testing** completed with realistic data

### 6. Extension Validation (if applicable)
- [ ] **Extension URLs** are unique and follow organizational conventions
- [ ] **Extension Context** correctly specifies where extension can be used
- [ ] **Extension Structure** is valid and appropriate for data being captured
- [ ] **Extension Cardinality** is set correctly
- [ ] **Extension Documentation** is complete and clear

## Clinical Validation

### 7. Clinical Requirements Mapping
- [ ] **All Clinical Requirements** from clinical-requirements.md are addressed
- [ ] **Clinical Data Elements** are properly mapped to FHIR elements
- [ ] **Clinical Terminology** is correctly bound and accessible
- [ ] **Clinical Business Rules** are implemented as invariants or constraints
- [ ] **Clinical Workflows** are supported by profile structure
- [ ] **Missing Requirements** are identified and addressed or documented

### 8. Safety and Quality Validation
- [ ] **Patient Safety** considerations are addressed in constraints
- [ ] **Data Quality Rules** are implemented to prevent invalid data
- [ ] **Required Safety Elements** are marked as must support or required
- [ ] **Prohibited Unsafe Elements** are appropriately constrained (0..0)
- [ ] **Clinical Decision Support** requirements are supported

## Technical Validation

### 9. Forge Validation Results
- [ ] **No Validation Errors** reported by Forge
- [ ] **No Validation Warnings** or warnings are documented and justified
- [ ] **Terminology Resolution** successful for all bound elements
- [ ] **Dependency Resolution** successful for all referenced profiles/extensions
- [ ] **Package Validation** confirms all dependencies are available

### 10. Example Instance Validation
- [ ] **Positive Examples** validate successfully against profile
- [ ] **Edge Case Examples** validate correctly
- [ ] **Negative Examples** fail validation as expected
- [ ] **Real-world Scenarios** are represented in examples
- [ ] **Example Documentation** explains clinical context and usage

### 11. Implementation Testing
- [ ] **FHIR Validator (Java)** validation passes if available
- [ ] **Firely .NET SDK** validation passes if applicable
- [ ] **Target System Testing** completed if specific systems targeted
- [ ] **Interoperability Testing** completed with relevant systems
- [ ] **Performance Testing** completed for large-scale implementations

## Documentation Validation

### 12. Profile Documentation Completeness
- [ ] **Implementation Guide Sections** are complete and accurate
- [ ] **Element Definitions** provide clear short and definition text
- [ ] **Usage Notes** explain implementation considerations
- [ ] **Mapping Information** shows relationships to clinical concepts
- [ ] **Examples and Scenarios** demonstrate practical usage
- [ ] **Known Issues** are documented if any exist

### 13. Clinical Stakeholder Review
- [ ] **Clinical Review Completed** by appropriate clinical experts
- [ ] **Clinical Accuracy Confirmed** by domain experts
- [ ] **Workflow Integration Validated** by end users
- [ ] **Terminology Accuracy Confirmed** by clinical terminology experts
- [ ] **Safety Review Completed** by clinical safety experts

## Publication Readiness

### 14. Pre-Publication Checklist
- [ ] **All Validation Steps Completed** successfully
- [ ] **Clinical Approval Obtained** from appropriate stakeholders
- [ ] **Technical Review Completed** by FHIR experts
- [ ] **Documentation Finalized** and reviewed for accuracy
- [ ] **Version Control** properly managed and documented
- [ ] **Change Log** updated with all modifications

### 15. Export and Packaging
- [ ] **StructureDefinition JSON** exported and validated
- [ ] **File Size** appropriate and not excessively large
- [ ] **JSON Syntax** valid and properly formatted
- [ ] **Dependencies** properly declared and available
- [ ] **Package Manifest** updated if part of implementation guide

## Validation Results Summary

### Issues Identified
| Severity | Issue Description | Resolution | Status |
|----------|------------------|------------|--------|
| Error    | [Description]    | [Solution] | [Fixed/Open] |
| Warning  | [Description]    | [Justification/Solution] | [Addressed/Accepted] |

### Validation Metrics
- **Total Validation Errors**: [Number] (Must be 0 for publication)
- **Total Validation Warnings**: [Number]
- **Warnings Addressed**: [Number]
- **Warnings Accepted**: [Number] (with justification)
- **Example Instances Validated**: [Number]
- **Clinical Requirements Covered**: [Percentage]

### Sign-Off

#### Technical Validation
- **FHIR Technical Specialist**: `[Name and Date]`
- **Validation Summary**: `[Brief summary of technical validation results]`

#### Clinical Validation
- **Clinical Domain Expert**: `[Name and Date]`
- **Clinical Summary**: `[Brief summary of clinical validation results]`

#### Final Approval
- **Profile Ready for Publication**: `[Yes/No]`
- **Publication Authorized By**: `[Name, Role, and Date]`
- **Next Steps**: `[Description of next actions - publication, implementation, etc.]`

---

**Validation Complete**: Profile has been thoroughly validated and is ready for publication to Simplifier.net registry and implementation by healthcare systems.

**Maintenance Note**: This validation checklist should be repeated for any significant profile updates or when moving from draft to active status.