# Create Profile in Forge Task Tests

## Test Suite: Task Definition Validation

### Test Case 1: Task Structure and Forge Focus
**Purpose**: Validate task definition has proper structure with Forge tool emphasis

**Test Steps**:
1. Load `/bmad-core/tasks/create-profile-in-forge.md`
2. Verify task metadata:
   - ID: "create-profile-in-forge"
   - Assigned Agent: "fhir-interoperability-specialist"
   - Description includes Forge tool guidance and StructureDefinition generation
   - Duration estimate appropriate for Forge-based profile creation (2-4 hours)
   - Prerequisites specify clinical-requirements.md input
3. Verify task includes Forge-specific technical considerations

**Expected Results**:
- Task properly identified and assigned to FHIR technical agent
- Description reflects Forge tool expertise requirements
- Duration estimate realistic for Forge-based profile development
- Forge technical context properly represented

### Test Case 2: Input Requirements for Forge Workflow
**Purpose**: Verify task defines appropriate inputs for Forge-based profile creation

**Test Steps**:
1. Verify inputs section includes:
   - `clinical-requirements.md` file from Clinical Informaticist agent
   - Base FHIR resource selection guidance
   - Target FHIR version specification (R4 by default)
   - Implementation context and constraints
   - Terminology system requirements for Forge binding
2. Verify inputs support comprehensive Forge workflow execution

**Expected Results**:
- All necessary Forge workflow input types identified
- Clinical requirements properly specified as primary input
- FHIR technical context properly captured for Forge usage
- Terminology requirements aligned with Forge binding capabilities

### Test Case 3: Forge-Specific Process Steps Validation
**Purpose**: Validate task process follows Forge tool best practices and capabilities

**Test Steps**:
1. Verify process includes Forge environment setup:
   - Forge installation and update verification
   - Implementation Guide project creation
   - FHIR package dependency configuration
   - Validation and publishing settings
2. Verify Forge workflow steps:
   - Base resource selection and profile metadata configuration
   - Element constraint definition using Forge editors
   - Terminology binding using Forge value set tools
   - Invariant definition using Forge FHIRPath editor
   - Validation using Forge built-in validation engine
3. Verify Forge quality assurance process:
   - Real-time validation feedback utilization
   - Example instance creation and testing
   - StructureDefinition export and verification

**Expected Results**:
- Process follows systematic Forge-based profile development methodology
- Forge tool capabilities comprehensively utilized
- Quality assurance leverages Forge validation features
- Output supports technical implementation requirements

## Test Suite: Forge Tool Integration

### Test Case 4: Forge Features Utilization
**Purpose**: Verify task leverages comprehensive Forge tool capabilities

**Test Steps**:
1. Verify Core Forge Capabilities coverage:
   - Profile Editor for visual constraint definition
   - Element Inspector for detailed element configuration
   - Validation Engine for real-time validation feedback
   - Example Editor for instance creation and validation
   - Package Management for FHIR package dependency handling
2. Verify Advanced Forge Features coverage:
   - Slice Editor for complex slicing configuration
   - Extension Designer for custom extension creation
   - Invariant Editor for FHIRPath expression development
   - Mapping Editor for clinical concept to FHIR element mapping
   - Publication Tools for StructureDefinition export and sharing
3. Verify Validation and Testing Tools usage:
   - Structure Validation for FHIR conformance checking
   - Terminology Validation for code system verification
   - Instance Validation for example testing
   - Dependency Checking for profile dependency validation

**Expected Results**:
- Task systematically guides users through all relevant Forge features
- Advanced capabilities properly utilized for complex profiling needs
- Validation tools comprehensively leveraged for quality assurance
- Forge tool capabilities maximize profile development efficiency

### Test Case 5: Forge Troubleshooting and Optimization
**Purpose**: Validate task includes comprehensive Forge-specific troubleshooting

**Test Steps**:
1. Verify troubleshooting guidance for common Forge issues:
   - Cardinality Conflicts and resolution strategies
   - Terminology Errors and binding troubleshooting
   - Invariant Failures and FHIRPath debugging
   - Extension Issues and context configuration problems
2. Verify performance optimization guidance:
   - Large Profiles and slicing optimization
   - Complex Invariants and FHIRPath efficiency
   - Terminology Loading and local caching strategies
   - Validation Speed and incremental validation techniques
3. Verify Forge-specific best practices integration

**Expected Results**:
- Common Forge issues anticipated and addressed
- Performance considerations properly integrated
- Troubleshooting guidance practical and actionable
- Best practices support efficient Forge workflow execution

## Test Suite: Clinical Requirements Integration

### Test Case 6: Clinical-Requirements Processing
**Purpose**: Verify task systematically processes clinical requirements for Forge implementation

**Test Steps**:
1. Verify clinical requirements analysis process:
   - Clinical-requirements.md document review and understanding
   - Base FHIR resource identification from clinical context
   - Data elements extraction requiring Forge constraint application
   - Clinical terminology and value set requirements capture
2. Verify clinical-to-technical translation methodology:
   - Clinical data elements mapped to FHIR element paths
   - Clinical constraints converted to Forge constraint configurations
   - Clinical terminology mapped to FHIR terminology binding
   - Clinical business rules translated to FHIRPath invariants

**Expected Results**:
- Clinical requirements systematically analyzed and processed
- Clinical context preserved through technical implementation
- Translation methodology ensures no loss of clinical intent
- Forge configuration accurately reflects clinical needs

### Test Case 7: Clinical Validation Integration
**Purpose**: Validate task supports clinical stakeholder engagement throughout Forge workflow

**Test Steps**:
1. Verify clinical validation checkpoints:
   - Clinical requirements review before Forge development
   - Clinical terminology validation during binding configuration
   - Clinical business rules verification during invariant development
   - Clinical example scenario validation during instance testing
2. Verify clinical stakeholder communication:
   - Clinical context documentation for technical implementers
   - Clinical approval process for constraint decisions
   - Clinical feedback integration during iterative development
   - Clinical sign-off preparation for profile completion

**Expected Results**:
- Clinical validation properly integrated throughout Forge workflow
- Clinical stakeholder engagement systematically supported
- Clinical approval process clearly defined and actionable
- Clinical context maintained for technical implementation teams

## Test Suite: Output Quality Validation

### Test Case 8: StructureDefinition Output Quality
**Purpose**: Verify task outputs high-quality StructureDefinition files

**Test Steps**:
1. Verify StructureDefinition output requirements:
   - Valid FHIR StructureDefinition file in JSON format
   - Profile documentation and implementation notes
   - Example instances demonstrating profile usage
   - Validation report confirming FHIR compliance
   - Forge project file for future modifications
2. Verify output quality measures:
   - Profile validates successfully in Forge without errors
   - All clinical requirements mapped to profile constraints
   - Terminology bindings resolve correctly
   - Example instances validate against profile
   - StructureDefinition conforms to FHIR R4 specification

**Expected Results**:
- Output format suitable for FHIR ecosystem integration
- Quality measures ensure implementable and compliant profiles
- Documentation supports implementation and maintenance
- Example instances demonstrate realistic usage scenarios

### Test Case 9: Publication Readiness
**Purpose**: Verify task output is ready for Simplifier.net publication

**Test Steps**:
1. Verify publication preparation:
   - StructureDefinition export from Forge in correct format
   - Profile metadata complete for registry publication
   - Documentation suitable for public consumption
   - Example instances included for implementer guidance
   - Dependencies properly declared and accessible
2. Verify publication quality gates:
   - Profile ready for publication to Simplifier.net registry
   - Implementation guidance documented for consuming systems
   - Clinical stakeholder review and approval process initiated
   - Technical validation completed and documented

**Expected Results**:
- Output immediately suitable for Simplifier.net publication
- Publication quality meets registry standards
- Clinical and technical approval processes properly supported
- Implementation guidance enables successful adoption

## Test Suite: Workflow Integration

### Test Case 10: Story 2.1 Integration
**Purpose**: Validate seamless handoff from Clinical Informaticist output

**Test Steps**:
1. Verify clinical-requirements.md input format compatibility
2. Verify clinical data elements properly processed for Forge implementation
3. Verify terminology systems properly supported in Forge workflow
4. Verify clinical constraints accurately translated to FHIR profile constraints
5. Verify audit trail maintained from clinical requirements to technical profile

**Expected Results**:
- Seamless workflow progression from Story 2.1 clinical requirements
- Clinical context fully preserved in Forge-based technical implementation
- No loss of clinical intent during technical translation
- Traceability maintained from clinical needs to FHIR constraints

### Test Case 11: Story 2.3 Preparation
**Purpose**: Verify task output properly prepares for publication and review workflow

**Test Steps**:
1. Verify StructureDefinition format suitable for Simplifier.net upload
2. Verify clinical context preserved for clinical review workflow
3. Verify technical documentation supports implementation guidance
4. Verify validation artifacts support quality assurance process
5. Verify handoff documentation enables Story 2.3 execution

**Expected Results**:
- Task output format compatible with Story 2.3 publication workflow
- Clinical context available for clinical review and approval
- Technical quality supports immediate publication
- Workflow handoff enables seamless Epic 2 completion

## Quality Gates

### Task Definition Quality
- [ ] Task properly structured with Forge tool focus
- [ ] Inputs comprehensive for Forge-based profile creation
- [ ] Process steps systematically leverage Forge capabilities
- [ ] Outputs support FHIR ecosystem integration

### Forge Tool Integration
- [ ] Forge features comprehensively utilized
- [ ] Advanced capabilities properly leveraged
- [ ] Troubleshooting guidance practical and complete
- [ ] Performance optimization properly addressed

### Clinical Requirements Translation
- [ ] Clinical requirements systematically processed
- [ ] Clinical-to-technical translation methodology sound
- [ ] Clinical validation properly integrated
- [ ] Clinical stakeholder engagement systematically supported

### Output Quality Assurance
- [ ] StructureDefinition output meets FHIR standards
- [ ] Publication readiness confirmed
- [ ] Implementation guidance comprehensive
- [ ] Quality gates ensure technical and clinical compliance

### Workflow Integration
- [ ] Story 2.1 handoff seamless and complete
- [ ] Clinical context preserved through technical implementation
- [ ] Story 2.3 preparation proper and thorough
- [ ] Epic 2 workflow progression logically supported

---

**Test Execution Notes**:
- Task testing should include Forge tool availability and functionality validation
- Process execution should be tested with realistic clinical requirements scenarios
- Output quality should be validated using FHIR validation tools and Forge validation
- Integration testing should verify seamless workflow progression within Epic 2
- Forge-specific features should be tested for proper utilization and guidance quality