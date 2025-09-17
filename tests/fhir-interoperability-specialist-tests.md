# FHIR Interoperability Specialist Agent Tests

## Test Suite: Agent Configuration Validation

### Test Case 1: Agent Definition Structure Enhancement
**Purpose**: Validate that the FHIR Interoperability Specialist agent has proper structure with Forge workflow capabilities

**Test Steps**:
1. Load `/bmad-core/agents/fhir-interoperability-specialist.md`
2. Verify YAML block contains required fields:
   - `agent.name`: "Dr. Sarah Chen"
   - `agent.id`: "fhir-interoperability-specialist"
   - `agent.title`: "FHIR Interoperability Specialist"
   - `agent.icon`: "🔗"
   - `agent.whenToUse`: Contains FHIR profile creation and validation
3. Verify persona section includes:
   - `role`: Senior FHIR Technical Specialist & Standards Expert
   - `focus`: Technical accuracy, FHIR compliance, interoperability patterns
   - `core_principles`: Contains standards compliance and interoperability focus

**Expected Results**:
- All required fields present and properly formatted
- Agent definition follows BMad framework standards
- FHIR technical expertise and Forge capabilities properly represented

### Test Case 2: Enhanced Command Structure Validation
**Purpose**: Verify agent has proper command structure including Forge workflow commands

**Test Steps**:
1. Verify commands section includes required commands:
   - `help`: Help display functionality
   - `create-profile`: Maps to create-fhir-profile task
   - `create-profile-in-forge`: Maps to create-profile-in-forge task (NEW)
   - `validate-resources`: FHIR resource validation capability
   - `generate-ig`: Implementation guide generation
   - `analyze-compatibility`: FHIR version compatibility analysis
   - `exit`: Proper exit functionality
2. Verify dependencies section references:
   - `tasks`: create-fhir-profile.md, create-profile-in-forge.md (NEW)
   - `templates`: fhir-profile.tmpl.yaml, implementation-guide.tmpl.md, forge-workflow-guide.tmpl.md (NEW), structure-definition-validation.tmpl.md (NEW)
   - `workflows`: specification-workflow.yaml, development-workflow.yaml

**Expected Results**:
- All commands properly defined with correct syntax
- New Forge-specific command added and properly mapped
- Dependencies correctly reference existing and new files
- Command descriptions align with FHIR technical domain expertise

## Test Suite: Forge Workflow Integration

### Test Case 3: Create Profile in Forge Task Integration
**Purpose**: Validate integration between agent and create-profile-in-forge task

**Test Steps**:
1. Verify task file exists at `/bmad-core/tasks/create-profile-in-forge.md`
2. Verify task has proper structure:
   - Task ID matches agent dependency reference
   - Assigned agent matches fhir-interoperability-specialist
   - Process steps include comprehensive Forge workflow guidance
   - Prerequisites include clinical-requirements.md input
3. Verify task outputs align with Forge tool capabilities and StructureDefinition generation

**Expected Results**:
- Task file exists and is properly structured
- Task assigned to correct agent
- Task process supports comprehensive Forge-based profile creation
- Output format compatible with Simplifier.net publication workflow

### Test Case 4: Forge Workflow Template Integration
**Purpose**: Verify Forge workflow template supports systematic profile creation

**Test Steps**:
1. Verify template file exists at `/bmad-core/templates/forge-workflow-guide.tmpl.md`
2. Verify template includes required Forge workflow sections:
   - Pre-Profile Setup and environment preparation
   - Base Resource Selection and metadata configuration
   - Element Constraint Definition with clinical mapping
   - Terminology Binding Configuration
   - Advanced Constraints and Extensions
   - Forge Validation Process and quality assurance
3. Verify template structure supports step-by-step Forge usage guidance

**Expected Results**:
- Template file exists and is properly formatted
- All required Forge workflow sections present
- Template structure supports systematic profile development
- Format enables both technical and clinical stakeholder engagement

### Test Case 5: Validation Template Integration
**Purpose**: Verify StructureDefinition validation template supports comprehensive quality assurance

**Test Steps**:
1. Verify template file exists at `/bmad-core/templates/structure-definition-validation.tmpl.md`
2. Verify template includes comprehensive validation sections:
   - Structural Validation (metadata, resource structure, element constraints)
   - Content Validation (terminology binding, invariants, extensions)
   - Clinical Validation (requirements mapping, safety considerations)
   - Technical Validation (Forge results, example instances, implementation testing)
   - Documentation Validation (completeness, stakeholder review)
   - Publication Readiness (pre-publication checklist, export packaging)
3. Verify template format supports systematic validation documentation

**Expected Results**:
- Template file exists and is comprehensive
- All validation aspects properly covered
- Template structure supports quality assurance process
- Format enables audit trail and approval documentation

## Test Suite: Clinical Requirements Integration

### Test Case 6: Clinical-Requirements Input Processing
**Purpose**: Validate task properly processes clinical-requirements.md input

**Test Steps**:
1. Verify create-profile-in-forge task specifies clinical-requirements.md as input
2. Verify task process includes clinical requirements analysis steps
3. Verify workflow maps clinical data elements to FHIR elements
4. Verify terminology requirements from clinical context are captured
5. Verify business rules translation to FHIR invariants

**Expected Results**:
- Task properly receives and processes clinical requirements
- Clinical context preserved through technical implementation
- Clinical terminology properly mapped to FHIR coding systems
- Business rules systematically converted to technical constraints

### Test Case 7: Workflow Continuity with Story 2.1
**Purpose**: Verify seamless handoff from Clinical Informaticist output

**Test Steps**:
1. Verify task input format compatible with clinical-requirements template
2. Verify clinical data elements properly mapped to FHIR paths
3. Verify terminology systems (LOINC, SNOMED CT, ICD-10) properly supported
4. Verify clinical constraints translated to FHIR profile constraints
5. Verify audit trail maintained from clinical requirements to technical profile

**Expected Results**:
- Seamless workflow progression from Story 2.1 output
- Clinical context fully preserved in technical implementation
- No loss of clinical requirements during technical translation
- Traceability maintained from clinical needs to FHIR constraints

## Test Suite: FHIR Standards Compliance

### Test Case 8: FHIR R4 Compliance Validation
**Purpose**: Verify task and templates ensure FHIR R4 compliance

**Test Steps**:
1. Verify task process includes FHIR R4 specification adherence
2. Verify validation steps confirm StructureDefinition compliance
3. Verify terminology binding follows FHIR R4 patterns
4. Verify invariant expressions use valid FHIRPath syntax
5. Verify example instances validate against FHIR R4 rules

**Expected Results**:
- All generated profiles comply with FHIR R4 specification
- Validation process catches non-compliant constructs
- Terminology bindings follow FHIR standards
- Technical artifacts ready for FHIR ecosystem integration

### Test Case 9: Forge Tool Integration
**Purpose**: Verify workflow properly utilizes Forge tool capabilities

**Test Steps**:
1. Verify workflow includes Forge-specific features utilization:
   - Profile Editor for visual constraint definition
   - Element Inspector for detailed configuration
   - Validation Engine for real-time feedback
   - Example Editor for instance creation and testing
   - Package Management for dependency handling
2. Verify troubleshooting guidance for common Forge issues
3. Verify export process for StructureDefinition files

**Expected Results**:
- Workflow takes full advantage of Forge capabilities
- Users guided through Forge-specific features effectively
- Common issues anticipated and addressed
- Export process produces valid FHIR artifacts

## Test Suite: End-to-End Workflow Validation

### Test Case 10: Complete Profile Creation Workflow
**Purpose**: Validate complete workflow from clinical requirements to StructureDefinition

**Test Steps**:
1. Simulate agent activation with fhir-interoperability-specialist
2. Execute create-profile-in-forge command with sample clinical-requirements.md
3. Verify task guides user through:
   - Clinical requirements analysis and base resource selection
   - Systematic constraint application in Forge
   - Terminology binding configuration
   - Validation and quality assurance process
   - StructureDefinition export and documentation
4. Verify output produces valid StructureDefinition ready for publication

**Expected Results**:
- Agent activates properly with FHIR technical persona
- Task execution follows logical FHIR profile creation process
- Output format suitable for input to publication workflow (Story 2.3)
- Technical quality ensures FHIR compliance and implementability

### Test Case 11: Multi-Template Coordination
**Purpose**: Verify proper coordination between workflow and validation templates

**Test Steps**:
1. Verify forge-workflow-guide template provides systematic development guidance
2. Verify structure-definition-validation template provides comprehensive quality assurance
3. Verify templates complement each other without overlap or gaps
4. Verify template usage supports iterative development and validation
5. Verify templates enable clinical and technical stakeholder engagement

**Expected Results**:
- Templates work together to support complete profile development lifecycle
- No gaps or overlaps between template coverage
- Templates enable both development and validation workflows
- Stakeholder engagement properly supported throughout process

## Quality Gates

### Agent Configuration Quality
- [ ] Agent YAML syntax valid and complete with Forge capabilities
- [ ] All referenced dependencies exist and are accessible
- [ ] Agent persona aligns with FHIR technical specialist role
- [ ] Commands properly map to Forge workflow task executions

### Task Integration Quality
- [ ] create-profile-in-forge task exists and is properly structured
- [ ] Task process includes all required Forge workflow steps
- [ ] Task inputs align with clinical requirements format
- [ ] Task outputs suitable for publication workflow handoff

### Template Quality
- [ ] forge-workflow-guide template comprehensive and systematic
- [ ] structure-definition-validation template covers all quality aspects
- [ ] Templates support both technical and clinical stakeholder needs
- [ ] Template format enables audit trail and approval documentation

### FHIR Standards Compliance
- [ ] All workflow steps ensure FHIR R4 compliance
- [ ] Validation process comprehensive and thorough
- [ ] Forge tool capabilities properly utilized
- [ ] Output artifacts ready for FHIR ecosystem integration

### Workflow Integration
- [ ] Seamless handoff from Story 2.1 clinical requirements
- [ ] Clinical context preserved through technical implementation
- [ ] Output format suitable for Story 2.3 publication workflow
- [ ] End-to-end workflow produces implementable FHIR profiles

---

**Test Execution Notes**:
- Tests should be executed in BMad framework environment with Forge tool access
- Agent activation should be tested with proper FHIR technical persona adoption
- Task execution should validate Forge-specific guidance quality and accuracy
- Template usage should produce properly formatted workflow and validation documents
- Integration testing should verify seamless Epic 2 workflow progression