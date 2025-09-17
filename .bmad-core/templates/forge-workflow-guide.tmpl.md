# Forge Workflow Guide

## Profile Creation Workflow

### Pre-Profile Setup
**Objective**: Prepare for profile creation with proper planning and environment setup

**Steps:**
1. **Review Clinical Requirements**
   - [ ] Read and understand `clinical-requirements.md` document
   - [ ] Identify base FHIR resource type needed
   - [ ] List all data elements requiring constraints
   - [ ] Note terminology and value set requirements

2. **Forge Environment Preparation**
   - [ ] Verify Forge installation and latest version
   - [ ] Set up new Implementation Guide project
   - [ ] Configure FHIR package dependencies
   - [ ] Test Forge validation environment

### Profile Development Process

#### Step 1: Base Resource Selection and Metadata
**Objective**: Configure profile foundation and metadata

**Tasks:**
1. **Create New Profile**
   - [ ] Open Forge and create new profile
   - [ ] Select base FHIR resource: `[Base Resource Type]`
   - [ ] Configure profile URL: `[Profile URL]`
   - [ ] Set profile name: `[Profile Name]`

2. **Profile Metadata Configuration**
   - [ ] Title: `[Profile Title]`
   - [ ] Description: `[Clinical Context Description]`
   - [ ] Status: `[draft/active/retired]`
   - [ ] Version: `[Version Number]`
   - [ ] Publisher: `[Organization Name]`
   - [ ] Contact Information: `[Contact Details]`

#### Step 2: Element Constraint Definition
**Objective**: Apply clinical requirements as FHIR constraints

**Clinical Data Elements Mapping:**

**Data Element 1: [Clinical Element Name]**
- [ ] FHIR Path: `[FHIR Element Path]`
- [ ] Cardinality: `[Original]` → `[Constrained]`
- [ ] Data Type: `[FHIR Data Type]`
- [ ] Must Support: `[Yes/No]`
- [ ] Clinical Definition: `[Clinical Meaning]`
- [ ] Business Rules: `[Validation Rules]`

**Data Element 2: [Clinical Element Name]**
- [ ] FHIR Path: `[FHIR Element Path]`
- [ ] Cardinality: `[Original]` → `[Constrained]`
- [ ] Data Type: `[FHIR Data Type]`
- [ ] Must Support: `[Yes/No]`
- [ ] Clinical Definition: `[Clinical Meaning]`
- [ ] Business Rules: `[Validation Rules]`

[Continue for additional data elements]

#### Step 3: Terminology Binding Configuration
**Objective**: Configure coded elements with appropriate terminology

**Terminology System 1: [System Name]**
- [ ] Element Path: `[FHIR Element Path]`
- [ ] Code System: `[System URI]`
- [ ] Binding Strength: `[required/extensible/preferred/example]`
- [ ] Value Set: `[Value Set URL or Local Definition]`
- [ ] Clinical Context: `[Usage Context]`

**Terminology System 2: [System Name]**
- [ ] Element Path: `[FHIR Element Path]`
- [ ] Code System: `[System URI]`
- [ ] Binding Strength: `[required/extensible/preferred/example]`
- [ ] Value Set: `[Value Set URL or Local Definition]`
- [ ] Clinical Context: `[Usage Context]`

#### Step 4: Advanced Constraints and Extensions
**Objective**: Implement complex business rules and custom elements

**Invariants and Business Rules:**
1. **Rule 1: [Business Rule Description]**
   - [ ] FHIRPath Expression: `[FHIRPath Code]`
   - [ ] Severity: `[error/warning/information]`
   - [ ] Human Description: `[User-Friendly Description]`

2. **Rule 2: [Business Rule Description]**
   - [ ] FHIRPath Expression: `[FHIRPath Code]`
   - [ ] Severity: `[error/warning/information]`
   - [ ] Human Description: `[User-Friendly Description]`

**Extensions (if needed):**
1. **Extension 1: [Extension Purpose]**
   - [ ] Extension URL: `[Extension URL]`
   - [ ] Context: `[Where Extension Can Be Used]`
   - [ ] Data Type: `[Extension Data Type]`
   - [ ] Cardinality: `[Extension Cardinality]`

### Forge Validation Process

#### Step 5: Real-Time Validation
**Objective**: Use Forge's built-in validation to ensure profile quality

**Validation Checklist:**
- [ ] **Structural Validation**: No errors in profile structure
- [ ] **Cardinality Validation**: All cardinality constraints are valid
- [ ] **Data Type Validation**: All data types are appropriate and constrained correctly
- [ ] **Terminology Validation**: All code systems and value sets resolve correctly
- [ ] **Invariant Validation**: All FHIRPath expressions are syntactically correct
- [ ] **Extension Validation**: All extensions are properly defined and referenced

#### Step 6: Example Instance Creation and Testing
**Objective**: Create and validate example instances against the profile

**Example Instance 1: [Scenario Description]**
- [ ] Create example instance in Forge
- [ ] Populate all required elements
- [ ] Include realistic clinical data
- [ ] Validate against profile constraints
- [ ] Document any validation issues

**Example Instance 2: [Scenario Description]**
- [ ] Create example instance in Forge
- [ ] Test edge cases and boundary conditions
- [ ] Validate against profile constraints
- [ ] Verify terminology bindings work correctly

### Quality Assurance and Documentation

#### Step 7: Profile Documentation
**Objective**: Ensure comprehensive documentation for implementers

**Documentation Tasks:**
- [ ] **Profile Description**: Clear explanation of profile purpose and scope
- [ ] **Implementation Notes**: Guidance for developers and implementers
- [ ] **Clinical Context**: Reference to original clinical requirements
- [ ] **Element Definitions**: Short and definition text for all constrained elements
- [ ] **Usage Examples**: Clinical scenarios demonstrating profile usage
- [ ] **Mapping Information**: Relationships to clinical concepts and other standards

#### Step 8: Final Validation and Export
**Objective**: Ensure profile is ready for publication and implementation

**Final Validation Steps:**
- [ ] Complete Forge validation passes without errors or warnings
- [ ] All clinical requirements are properly represented in constraints
- [ ] Example instances validate successfully
- [ ] Profile documentation is complete and accurate
- [ ] Profile is ready for review by clinical stakeholders

**Export and Preparation for Publication:**
- [ ] Export StructureDefinition as JSON file
- [ ] Verify exported file structure and content
- [ ] Prepare profile for upload to Simplifier.net
- [ ] Generate implementation guide documentation sections
- [ ] Package profile with dependencies and examples

## Troubleshooting Common Issues

### Validation Errors
**Error: Cardinality Constraint Conflict**
- **Cause**: Attempting to apply stricter constraint than base resource allows
- **Solution**: Review base resource cardinality and adjust constraints accordingly
- **Prevention**: Always check base resource constraints before applying new ones

**Error: Terminology Binding Resolution Failure**
- **Cause**: Code system URL incorrect or value set not accessible
- **Solution**: Verify code system URLs and ensure terminology servers are accessible
- **Prevention**: Use standard terminology system URLs and test connectivity

**Error: FHIRPath Expression Syntax Error**
- **Cause**: Invalid FHIRPath syntax in invariant expressions
- **Solution**: Test FHIRPath expressions with example data before applying
- **Prevention**: Use Forge's expression editor and validation tools

### Performance Issues
**Issue: Slow Profile Loading**
- **Cause**: Large number of constraints or complex slicing definitions
- **Solution**: Optimize constraints and consider profile decomposition
- **Prevention**: Design profiles with performance in mind

**Issue: Validation Timeout**
- **Cause**: Complex invariant expressions or large value sets
- **Solution**: Simplify expressions and use local value set definitions
- **Prevention**: Test performance during development, not just at the end

## Success Criteria Checklist

### Technical Validation
- [ ] Profile validates in Forge without errors
- [ ] All constraints properly applied and functioning
- [ ] Terminology bindings resolve correctly
- [ ] Invariant expressions execute successfully
- [ ] Example instances validate against profile

### Clinical Validation
- [ ] All clinical requirements represented in profile constraints
- [ ] Clinical terminology properly captured and bound
- [ ] Business rules correctly implemented as invariants
- [ ] Profile supports intended clinical workflows
- [ ] Clinical stakeholders can review and understand profile

### Implementation Readiness
- [ ] Profile documentation complete and clear
- [ ] Implementation guidance provided for developers
- [ ] Example instances demonstrate realistic usage
- [ ] Profile ready for publication to registry
- [ ] Integration testing completed successfully

---

**Workflow Completion**: When all checklist items are completed, the profile is ready for clinical review and publication to Simplifier.net registry.

**Next Steps**: Initiate clinical validation workflow with Clinical Informaticist for review and approval before publication.