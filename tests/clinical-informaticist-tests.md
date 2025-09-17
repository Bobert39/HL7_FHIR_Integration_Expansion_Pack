# Clinical Informaticist Agent Tests

## Test Suite: Agent Configuration Validation

### Test Case 1: Agent Definition Structure
**Purpose**: Validate that the Clinical Informaticist agent has proper structure and required fields

**Test Steps**:
1. Load `/bmad-core/agents/clinical-informaticist.md`
2. Verify YAML block contains required fields:
   - `agent.name`: "Dr. Michael Rodriguez"
   - `agent.id`: "clinical-informaticist"
   - `agent.title`: "Clinical Informaticist"
   - `agent.icon`: "🏥"
   - `agent.whenToUse`: Contains clinical workflow analysis
3. Verify persona section includes:
   - `role`: Clinical Domain Expert & Healthcare Workflow Specialist
   - `focus`: Clinical accuracy, workflow optimization, patient safety
   - `core_principles`: Contains patient safety and clinical workflow understanding

**Expected Results**:
- All required fields present and properly formatted
- Agent definition follows BMad framework standards
- Clinical focus and expertise properly represented

### Test Case 2: Command Structure Validation
**Purpose**: Verify agent has proper command structure and dependencies

**Test Steps**:
1. Verify commands section includes required commands:
   - `help`: Help display functionality
   - `document-requirements`: Maps to document-clinical-requirements task
   - `analyze-workflow`: Clinical workflow analysis capability
   - `validate-clinical`: Clinical validation functionality
   - `exit`: Proper exit functionality
2. Verify dependencies section references:
   - `tasks`: document-clinical-requirements.md
   - `templates`: implementation-guide.tmpl.md
   - `workflows`: specification-workflow.yaml

**Expected Results**:
- All commands properly defined with correct syntax
- Dependencies correctly reference existing files
- Command descriptions align with clinical domain expertise

## Test Suite: Task Execution Validation

### Test Case 3: Document Clinical Requirements Task Integration
**Purpose**: Validate integration between agent and document-clinical-requirements task

**Test Steps**:
1. Verify task file exists at `/bmad-core/tasks/document-clinical-requirements.md`
2. Verify task has proper structure:
   - Task ID matches agent dependency reference
   - Assigned agent matches clinical-informaticist
   - Process steps include stakeholder engagement and workflow analysis
3. Verify task outputs align with clinical requirements template structure

**Expected Results**:
- Task file exists and is properly structured
- Task assigned to correct agent
- Task process supports clinical requirements gathering
- Output format compatible with clinical-requirements template

### Test Case 4: Template Integration Validation
**Purpose**: Verify clinical requirements template supports task outputs

**Test Steps**:
1. Verify template file exists at `/bmad-core/templates/clinical-requirements.tmpl.md`
2. Verify template includes required sections:
   - Clinical Context and Use Case Description
   - FHIR Resource Mapping
   - Medical Terminology and Value Sets
   - Profile Constraints
   - Clinical Validation Requirements
3. Verify template supports structured data capture for FHIR profiling

**Expected Results**:
- Template file exists and is properly formatted
- All required sections present for clinical requirements
- Template structure supports systematic data capture
- Format supports handoff to FHIR Interoperability Specialist

## Test Suite: Workflow Integration

### Test Case 5: End-to-End Workflow Validation
**Purpose**: Validate complete workflow from agent activation to clinical requirements output

**Test Steps**:
1. Simulate agent activation with clinical-informaticist
2. Execute document-requirements command
3. Verify task guides user through:
   - Stakeholder engagement process
   - Clinical workflow analysis
   - Requirements documentation using template
   - Clinical validation steps
4. Verify output produces structured clinical-requirements.md file

**Expected Results**:
- Agent activates properly with clinical persona
- Task execution follows logical clinical requirements gathering process
- Output format suitable for input to FHIR profile creation
- Clinical validation ensures accuracy and completeness

### Test Case 6: Clinical Domain Expertise Validation
**Purpose**: Verify agent demonstrates appropriate clinical domain knowledge

**Test Steps**:
1. Verify agent persona includes healthcare-specific language and concepts
2. Verify task process includes clinical best practices:
   - Patient safety considerations
   - Healthcare compliance requirements
   - Clinical workflow understanding
   - Evidence-based practice principles
3. Verify output template captures clinical context and terminology

**Expected Results**:
- Agent demonstrates clinical domain expertise in language and approach
- Task process reflects healthcare industry best practices
- Template structure supports clinical validation and review
- Output suitable for clinical stakeholder review and approval

## Quality Gates

### Configuration Validation
- [ ] Agent YAML syntax valid and complete
- [ ] All referenced dependencies exist and are accessible
- [ ] Agent persona aligns with clinical informaticist role
- [ ] Commands properly map to task executions

### Task Integration Validation
- [ ] document-clinical-requirements task exists and is properly structured
- [ ] Task process includes all required clinical workflow steps
- [ ] Task outputs align with template structure
- [ ] Clinical validation steps ensure quality and accuracy

### Template Validation
- [ ] clinical-requirements template exists and is comprehensive
- [ ] Template captures all necessary clinical context
- [ ] Template structure supports FHIR resource mapping
- [ ] Template format enables clinical stakeholder review

### Workflow Completeness
- [ ] End-to-end workflow produces valid clinical requirements document
- [ ] Output suitable for handoff to FHIR Interoperability Specialist
- [ ] Clinical validation ensures requirements accuracy
- [ ] Process supports iterative refinement and stakeholder feedback

---

**Test Execution Notes**:
- Tests should be executed in BMad framework environment
- Agent activation should be tested with proper persona adoption
- Task execution should validate user interaction and guidance quality
- Template usage should produce properly formatted clinical requirements documents