# Forge Workflow Templates Tests

## Test Suite: Forge Workflow Guide Template Validation

### Test Case 1: Template Structure and Completeness
**Purpose**: Validate forge-workflow-guide template has comprehensive structure for Forge-based profile development

**Test Steps**:
1. Load `/bmad-core/templates/forge-workflow-guide.tmpl.md`
2. Verify template includes required sections:
   - Profile Creation Workflow (systematic development approach)
   - Pre-Profile Setup (preparation and environment configuration)
   - Profile Development Process (step-by-step Forge guidance)
   - Forge Validation Process (quality assurance using Forge tools)
   - Quality Assurance and Documentation (comprehensive validation)
   - Troubleshooting Common Issues (Forge-specific problem resolution)
   - Success Criteria Checklist (completion verification)
3. Verify logical flow from setup through completion and validation

**Expected Results**:
- All required sections present and properly organized
- Template structure supports systematic Forge-based profile development
- Logical progression from preparation through validation and completion
- Comprehensive coverage of Forge tool capabilities and best practices

### Test Case 2: Pre-Profile Setup and Environment Configuration
**Purpose**: Verify template adequately guides Forge environment preparation

**Test Steps**:
1. Verify Pre-Profile Setup section includes:
   - Review Clinical Requirements (clinical-requirements.md processing)
   - Forge Environment Preparation (installation, project setup, dependencies)
   - Base resource identification and planning steps
   - Terminology and value set requirements analysis
2. Verify checklist format enables systematic preparation
3. Verify setup guidance supports successful Forge workflow execution

**Expected Results**:
- Environment setup thoroughly documented and actionable
- Clinical requirements integration properly planned
- Forge-specific preparation steps comprehensive and accurate
- Checklist format enables systematic execution and verification

### Test Case 3: Profile Development Process Structure
**Purpose**: Validate template supports detailed step-by-step Forge profile development

**Test Steps**:
1. Verify Profile Development Process includes systematic steps:
   - Step 1: Base Resource Selection and Metadata configuration
   - Step 2: Element Constraint Definition with clinical mapping
   - Step 3: Terminology Binding Configuration for coded elements
   - Step 4: Advanced Constraints and Extensions development
2. Verify each step includes:
   - Clear objectives and outcomes
   - Specific Forge tool instructions and guidance
   - Clinical data element mapping templates
   - Validation checkpoints and quality measures
3. Verify step-by-step format supports iterative development

**Expected Results**:
- Profile development process systematic and comprehensive
- Forge tool instructions accurate and detailed
- Clinical mapping templates enable systematic data element processing
- Iterative development approach supports quality and refinement

### Test Case 4: Forge Validation Process Integration
**Purpose**: Verify template leverages Forge validation capabilities effectively

**Test Steps**:
1. Verify Forge Validation Process section includes:
   - Step 5: Real-Time Validation using Forge built-in validation
   - Step 6: Example Instance Creation and Testing in Forge
   - Comprehensive validation checklist covering all aspects
   - Forge-specific validation features utilization guidance
2. Verify validation approach covers:
   - Structural validation (profile structure and constraints)
   - Terminology validation (code systems and value sets)
   - Invariant validation (FHIRPath expressions and business rules)
   - Example validation (instance testing against profile)
3. Verify validation process ensures FHIR compliance and quality

**Expected Results**:
- Forge validation capabilities comprehensively utilized
- Validation process systematic and thorough
- Quality assurance measures appropriate for FHIR profile development
- Validation checklist enables verification and documentation

## Test Suite: Structure Definition Validation Template

### Test Case 5: Validation Template Comprehensive Coverage
**Purpose**: Validate structure-definition-validation template covers all validation aspects

**Test Steps**:
1. Load `/bmad-core/templates/structure-definition-validation.tmpl.md`
2. Verify template includes comprehensive validation sections:
   - Pre-Validation Preparation (environment and profile information)
   - Structural Validation (metadata, resource structure, element constraints)
   - Content Validation (terminology binding, invariants, extensions)
   - Clinical Validation (requirements mapping, safety considerations)
   - Technical Validation (Forge results, implementation testing)
   - Documentation Validation (completeness, stakeholder review)
   - Publication Readiness (pre-publication checklist, export packaging)
3. Verify validation methodology aligns with FHIR standards and best practices

**Expected Results**:
- All validation aspects comprehensively covered
- Validation methodology systematic and thorough
- FHIR standards compliance properly addressed
- Publication readiness criteria clear and actionable

### Test Case 6: Clinical Validation Integration
**Purpose**: Verify validation template properly integrates clinical validation requirements

**Test Steps**:
1. Verify Clinical Validation section includes:
   - Clinical Requirements Mapping (traceability to clinical-requirements.md)
   - Safety and Quality Validation (patient safety and data quality)
   - Clinical stakeholder review and approval process
   - Clinical terminology accuracy verification
   - Clinical workflow integration validation
2. Verify clinical validation criteria measurable and meaningful
3. Verify clinical approval process clearly defined and documented

**Expected Results**:
- Clinical validation comprehensively addressed
- Patient safety explicitly prioritized and verified
- Clinical stakeholder engagement systematically supported
- Clinical approval process enables quality assurance and compliance

### Test Case 7: Technical Validation Comprehensiveness
**Purpose**: Validate template covers comprehensive technical validation requirements

**Test Steps**:
1. Verify Technical Validation section includes:
   - Forge Validation Results (built-in validation feedback)
   - Example Instance Validation (profile compliance testing)
   - Implementation Testing (FHIR validator and SDK testing)
   - Interoperability Testing (cross-system compatibility)
   - Performance Testing (large-scale implementation considerations)
2. Verify technical validation tools properly utilized
3. Verify validation results documentation supports audit and compliance

**Expected Results**:
- Technical validation systematic and comprehensive
- FHIR validation tools properly utilized
- Implementation and interoperability testing properly addressed
- Validation documentation supports audit trail and compliance

## Test Suite: Template Integration and Coordination

### Test Case 8: Template Coordination and Complementarity
**Purpose**: Verify forge-workflow-guide and structure-definition-validation templates work together effectively

**Test Steps**:
1. Verify templates complement each other without overlap:
   - Workflow guide focuses on development process and Forge usage
   - Validation template focuses on quality assurance and compliance verification
   - Clear handoff points between development and validation phases
2. Verify template integration supports iterative development:
   - Validation checkpoints integrated throughout workflow guide
   - Validation template supports ongoing quality assurance
   - Templates enable both development and validation perspectives
3. Verify templates support different stakeholder needs:
   - Technical implementers can follow workflow guide systematically
   - Quality assurance teams can use validation template for verification
   - Clinical stakeholders can engage through both templates

**Expected Results**:
- Templates work together seamlessly without gaps or overlaps
- Iterative development and validation properly supported
- Multiple stakeholder perspectives accommodated
- Template coordination enhances overall workflow effectiveness

### Test Case 9: Clinical Requirements Integration Across Templates
**Purpose**: Verify both templates properly integrate clinical requirements from Story 2.1

**Test Steps**:
1. Verify workflow guide template:
   - Includes clinical requirements review and analysis steps
   - Maps clinical data elements to FHIR elements systematically
   - Preserves clinical context through technical implementation
   - Enables clinical stakeholder engagement during development
2. Verify validation template:
   - Validates clinical requirements mapping completeness
   - Verifies clinical safety and quality considerations
   - Enables clinical stakeholder review and approval
   - Maintains traceability from clinical needs to technical constraints
3. Verify templates maintain clinical context throughout Forge workflow

**Expected Results**:
- Clinical requirements properly integrated across both templates
- Clinical context preserved from development through validation
- Clinical stakeholder engagement systematically supported
- Traceability maintained from clinical needs to technical implementation

## Test Suite: Forge Tool Alignment

### Test Case 10: Forge Tool Feature Utilization
**Purpose**: Verify templates properly leverage comprehensive Forge tool capabilities

**Test Steps**:
1. Verify workflow guide leverages Forge features:
   - Profile Editor for visual constraint definition
   - Element Inspector for detailed element configuration
   - Validation Engine for real-time feedback
   - Example Editor for instance creation and testing
   - Package Management for dependency handling
   - Advanced features (Slice Editor, Extension Designer, etc.)
2. Verify validation template utilizes Forge validation capabilities:
   - Built-in validation results integration
   - Terminology validation through Forge
   - Example instance validation in Forge environment
   - Export validation and quality verification
3. Verify Forge-specific guidance accuracy and completeness

**Expected Results**:
- Forge tool capabilities comprehensively utilized across templates
- Forge-specific guidance accurate and actionable
- Advanced Forge features properly leveraged for complex profiling needs
- Templates maximize Forge tool effectiveness for profile development

### Test Case 11: Troubleshooting and Performance Optimization
**Purpose**: Verify templates include comprehensive Forge troubleshooting and optimization guidance

**Test Steps**:
1. Verify workflow guide includes troubleshooting for:
   - Validation errors and resolution strategies
   - Performance issues and optimization techniques
   - Common Forge usage problems and solutions
   - Forge-specific best practices integration
2. Verify validation template includes troubleshooting for:
   - Validation failure diagnosis and resolution
   - Technical validation tool integration issues
   - Performance validation and optimization verification
   - Publication readiness troubleshooting
3. Verify troubleshooting guidance practical and actionable

**Expected Results**:
- Troubleshooting guidance comprehensive and practical
- Common issues anticipated and addressed proactively
- Performance optimization properly integrated
- Best practices support efficient and effective Forge usage

## Quality Gates

### Template Structure Quality
- [ ] All required sections present and logically organized
- [ ] Forge tool capabilities comprehensively covered
- [ ] Clinical requirements integration systematic and thorough
- [ ] Validation methodology comprehensive and standards-compliant

### Forge Tool Integration
- [ ] Forge features systematically leveraged across templates
- [ ] Forge-specific guidance accurate and actionable
- [ ] Advanced capabilities properly utilized
- [ ] Troubleshooting guidance comprehensive and practical

### Clinical Integration Quality
- [ ] Clinical requirements properly integrated across templates
- [ ] Clinical context preserved through technical implementation
- [ ] Clinical stakeholder engagement systematically supported
- [ ] Clinical validation comprehensive and meaningful

### Template Coordination
- [ ] Templates complement each other without gaps or overlaps
- [ ] Iterative development and validation properly supported
- [ ] Multiple stakeholder perspectives accommodated
- [ ] Template integration enhances workflow effectiveness

### Output Quality Support
- [ ] Templates enable high-quality StructureDefinition development
- [ ] FHIR standards compliance systematically ensured
- [ ] Publication readiness criteria clear and achievable
- [ ] Implementation guidance comprehensive and practical

---

**Test Execution Notes**:
- Template validation should include Forge tool experts and clinical domain experts
- Template structure should be tested with realistic profile development scenarios
- Integration testing should verify seamless coordination between templates
- Forge-specific guidance should be validated against actual Forge tool capabilities
- Clinical integration should be verified with clinical stakeholders and use cases