# Publish to Simplifier Task Tests

## Test Suite: Task Definition Validation

### Test Case 1: Task Structure Validation
**Purpose**: Validate that the publish-to-simplifier task has proper structure and metadata

**Test Steps**:
1. Load `/bmad-core/tasks/publish-to-simplifier.md`
2. Verify task definition contains required fields:
   - `id`: "publish-to-simplifier"
   - `description`: Simplifier.net publication guidance with clinical review coordination
   - `agent`: FHIR Interoperability Specialist
   - `elicit`: true (requires user interaction)
3. Verify task overview clearly describes publication process
4. Verify inputs, outputs, and prerequisites are properly defined

**Expected Results**:
- All required fields present and properly formatted
- Task definition follows BMad framework TaskDefinition data model
- Clear purpose and scope defined for Simplifier.net publication
- Elicit flag properly set for interactive guidance

### Test Case 2: Input and Output Specification
**Purpose**: Verify task properly defines required inputs and expected outputs

**Test Steps**:
1. Verify inputs required section includes:
   - FHIR StructureDefinition file (.json or .xml format)
   - Profile metadata (name, version, description, purpose)
   - Simplifier.net account credentials
   - Original clinical requirements document for validation reference
2. Verify outputs produced section includes:
   - Published profile URL on Simplifier.net
   - Publication confirmation with human-readable view
   - Clinical review coordination handoff
   - Publication audit trail
3. Verify prerequisites section covers all necessary preparations

**Expected Results**:
- All required inputs clearly specified with format requirements
- Expected outputs clearly defined and measurable
- Prerequisites ensure task can be successfully executed
- Input/output format compatible with Epic 2 workflow handoff

## Test Suite: Workflow Process Validation

### Test Case 3: Step-by-Step Workflow Completeness
**Purpose**: Validate that the task workflow covers all necessary publication steps

**Test Steps**:
1. Verify Step 1 (Validate Input Files):
   - FHIR format validation
   - Required metadata presence
   - Basic FHIR validation rules
   - Elicit information section with proper user input prompts
2. Verify Step 2 (Pre-Publication Checklist):
   - References simplifier-publication-guide.tmpl.md template
   - Covers canonical URL, naming conventions, versioning
   - Includes metadata completeness checks
3. Verify Step 3 (Simplifier.net Upload Process):
   - Login and navigation guidance
   - File upload instructions
   - Publication settings configuration
   - Verification steps
4. Verify Step 4 (Generate Publication Documentation)
5. Verify Step 5 (Coordinate Clinical Review)

**Expected Results**:
- All publication steps covered systematically
- Each step includes clear, actionable instructions
- Template references are correct and accessible
- User interaction prompts follow BMad elicit patterns

### Test Case 4: Simplifier.net Integration Guidance
**Purpose**: Verify task provides accurate Simplifier.net platform guidance

**Test Steps**:
1. Verify login and navigation instructions are current
2. Verify upload process matches Simplifier.net UI/workflow
3. Verify publication settings configuration covers:
   - Publication status options
   - Access permissions
   - Version management
   - Project assignment
4. Verify verification steps confirm successful publication
5. Verify human-readable view accessibility testing

**Expected Results**:
- Instructions match current Simplifier.net interface
- All required publication settings covered
- Verification steps ensure publication success
- Human-readable view properly accessible for clinical review

### Test Case 5: Inter-Agent Coordination
**Purpose**: Validate coordination mechanism with Clinical Informaticist agent

**Test Steps**:
1. Verify handoff message generation includes:
   - Profile name and version
   - Published Simplifier.net URL
   - Human-readable view URL
   - Original requirements document reference
   - Publication timestamp
2. Verify prompt for Clinical Informaticist engagement:
   - Clear instruction to activate Clinical Informaticist agent
   - Specific command reference (*review-simplifier-profile)
   - Required information for clinical review
3. Verify audit trail creation for inter-agent coordination

**Expected Results**:
- Handoff message contains all required information
- Clinical Informaticist engagement clearly guided
- Coordination maintains workflow continuity
- Audit trail supports Epic 2 workflow traceability

## Test Suite: Validation and Error Handling

### Test Case 6: Input Validation Logic
**Purpose**: Verify task properly validates input files and metadata

**Test Steps**:
1. Test FHIR format validation:
   - Valid JSON/XML format check
   - Required FHIR resource elements presence
   - Basic FHIR R4 compliance validation
2. Test metadata validation:
   - Profile name format and uniqueness
   - Version number semantic versioning
   - Description completeness
   - Canonical URL uniqueness
3. Test clinical requirements document validation

**Expected Results**:
- Invalid formats properly detected and rejected
- Metadata validation catches incomplete information
- Clear error messages guide user correction
- Validation prevents publication of invalid profiles

### Test Case 7: Error Handling Scenarios
**Purpose**: Verify task handles common error scenarios gracefully

**Test Steps**:
1. Test file validation errors:
   - Invalid FHIR format handling
   - Missing metadata handling
   - Duplicate canonical URL handling
2. Test publication errors:
   - Upload failure handling
   - Permission denied handling
   - Server error handling with retry guidance
3. Test coordination errors:
   - Missing requirements document handling
   - Clinical Informaticist unavailable fallback

**Expected Results**:
- All error scenarios properly handled
- Clear error messages with corrective guidance
- Fallback strategies provided where appropriate
- Error handling maintains workflow integrity

### Test Case 8: Validation Criteria Completeness
**Purpose**: Verify validation criteria cover all quality aspects

**Test Steps**:
1. Verify technical validation criteria:
   - FHIR format validation
   - Profile validation against FHIR R4
   - Canonical URL accessibility
   - Metadata completeness
2. Verify publication validation criteria:
   - Successful upload confirmation
   - Human-readable view generation
   - Profile accessibility via canonical URL
   - Version information display
3. Verify process validation criteria:
   - User input completeness
   - Pre-publication checklist completion
   - Clinical review coordination initiation
   - Publication audit trail documentation

**Expected Results**:
- All validation aspects properly covered
- Criteria ensure publication quality and compliance
- Process validation ensures workflow integrity
- Audit trail supports regulatory requirements

## Test Suite: Template Integration

### Test Case 9: Simplifier Publication Guide Template Usage
**Purpose**: Verify proper integration with simplifier-publication-guide template

**Test Steps**:
1. Verify template reference in Step 2 (Pre-Publication Checklist)
2. Verify template file exists at `/bmad-core/templates/simplifier-publication-guide.tmpl.md`
3. Verify template content aligns with task requirements:
   - Publication checklist items match task validation criteria
   - Simplifier.net instructions align with task Step 3
   - Template format supports task execution
4. Verify template placeholder usage for dynamic content

**Expected Results**:
- Template properly referenced and accessible
- Template content supports task execution requirements
- Template format enables systematic publication guidance
- Dynamic content placeholders properly utilized

### Test Case 10: Template Coordination with Agent Dependencies
**Purpose**: Verify template reference aligns with agent dependency configuration

**Test Steps**:
1. Verify FHIR Interoperability Specialist agent dependencies include:
   - `simplifier-publication-guide.tmpl.md` in templates section
2. Verify template reference consistency between agent and task
3. Verify template accessibility through BMad framework resolution
4. Verify template format follows BMad TemplateDefinition standards

**Expected Results**:
- Agent dependencies properly include required template
- Template references consistent across agent and task
- Template accessible through BMad IDE-FILE-RESOLUTION
- Template format complies with BMad standards

## Test Suite: Success Criteria and Dependencies

### Test Case 11: Success Criteria Validation
**Purpose**: Verify success criteria are comprehensive and measurable

**Test Steps**:
1. Verify success criteria checklist includes:
   - StructureDefinition successfully published to Simplifier.net
   - Human-readable view accessible and displays correctly
   - Clinical review coordination successfully initiated
   - Publication audit trail created and documented
   - Profile accessible via canonical URL
2. Verify criteria are measurable and verifiable
3. Verify criteria align with Epic 2 workflow objectives

**Expected Results**:
- All success criteria are specific and measurable
- Criteria cover all aspects of successful publication
- Success criteria support workflow progression to clinical review
- Criteria enable audit and compliance verification

### Test Case 12: Dependencies and Integration
**Purpose**: Verify task dependencies are properly specified and accessible

**Test Steps**:
1. Verify template dependencies:
   - simplifier-publication-guide.tmpl.md exists and is accessible
2. Verify external tool dependencies:
   - Simplifier.net platform availability
   - FHIR validation services integration
3. Verify agent coordination dependencies:
   - Clinical Informaticist agent (review-simplifier-profile task)
4. Verify input file dependencies:
   - FHIR StructureDefinition format compatibility
   - Clinical requirements document format compatibility

**Expected Results**:
- All dependencies properly identified and accessible
- External tool integration requirements clearly specified
- Agent coordination dependencies enable workflow continuity
- Input format dependencies ensure compatibility with Epic 2

## Test Suite: End-to-End Integration

### Test Case 13: Epic 2 Workflow Integration
**Purpose**: Verify task integrates properly within Epic 2 workflow context

**Test Steps**:
1. Verify task input format compatible with Story 2.2 output:
   - FHIR StructureDefinition from Forge workflow
   - Clinical requirements from Story 2.1
2. Verify task output enables Story 2.3 clinical review:
   - Published profile URL format
   - Clinical review coordination handoff
3. Verify workflow progression maintains Epic 2 objectives:
   - Validated FHIR profile publication
   - Human-readable documentation on Simplifier.net
   - Clinical validation coordination

**Expected Results**:
- Seamless integration with Epic 2 workflow progression
- Input/output format compatibility maintained
- Epic 2 objectives supported through task execution
- Workflow continuity preserved across story boundaries

### Test Case 14: Quality Gate Integration
**Purpose**: Verify task supports Epic 2 quality assurance requirements

**Test Steps**:
1. Verify validation process supports quality gates:
   - Technical validation ensures FHIR compliance
   - Publication validation ensures accessibility
   - Process validation ensures workflow integrity
2. Verify audit trail supports Epic 2 traceability requirements
3. Verify error handling maintains Epic 2 quality standards
4. Verify success criteria align with Epic 2 delivery criteria

**Expected Results**:
- Quality gates properly integrated throughout task execution
- Audit trail supports Epic 2 traceability requirements
- Error handling maintains Epic 2 quality standards
- Success criteria enable Epic 2 delivery verification

## Quality Gates

### Task Definition Quality
- [ ] Task YAML/markdown syntax valid and complete
- [ ] All required TaskDefinition fields present and accurate
- [ ] Task description clearly defines scope and purpose
- [ ] Elicit flag properly set for interactive guidance

### Workflow Process Quality
- [ ] All publication steps systematically covered
- [ ] Simplifier.net integration guidance accurate and current
- [ ] Inter-agent coordination properly implemented
- [ ] User interaction patterns follow BMad elicit standards

### Validation and Error Handling Quality
- [ ] Input validation comprehensive and accurate
- [ ] Error scenarios properly handled with clear guidance
- [ ] Validation criteria cover all quality aspects
- [ ] Success criteria specific, measurable, and complete

### Template Integration Quality
- [ ] Template references accurate and accessible
- [ ] Template content supports task execution requirements
- [ ] Template format complies with BMad standards
- [ ] Agent dependency configuration consistent

### Epic 2 Workflow Integration Quality
- [ ] Input format compatible with Story 2.2 output
- [ ] Output format enables Story 2.3 clinical review
- [ ] Workflow progression maintains Epic 2 objectives
- [ ] Quality gates support Epic 2 delivery requirements

---

**Test Execution Notes**:
- Tests should be executed in BMad framework environment with Simplifier.net access
- Task execution should validate Simplifier.net integration accuracy
- Template usage should produce properly formatted publication guidance
- Agent coordination should verify Clinical Informaticist handoff functionality
- Integration testing should confirm Epic 2 workflow progression continuity