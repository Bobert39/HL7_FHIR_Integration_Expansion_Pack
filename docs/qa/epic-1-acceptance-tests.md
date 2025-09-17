# Epic 1: Foundation & Core Agent Setup - Acceptance Test Cases

## Test Suite Overview

**Epic Goal**: Establish the foundational structure of the expansion pack and define the core AI agent personas (Specialist, Informaticist, Security Analyst, Integration Analyst) and their capabilities.

**Test Execution Date**: _______________
**Tester Name**: _______________
**Environment**: BMad Framework v___

## Story 1.1: Initialize Project & Define Agents

### Test Case 1.1.1: Project Structure Validation
**Objective**: Verify correct monorepo structure and directory hierarchy

**Pre-conditions**:
- Fresh clone of repository
- BMad framework installed

**Test Steps**:
1. Navigate to project root directory
2. Verify existence of `package.json` file
3. Check for `.bmad-core` directory at root level
4. Validate subdirectory structure within `.bmad-core`

**Expected Results**:
- [ ] `package.json` exists with correct metadata (name, version, description)
- [ ] `.bmad-core` directory exists at root
- [ ] Following subdirectories exist: `agents`, `workflows`, `tasks`, `templates`, `data`
- [ ] Directory permissions allow read/write operations

**Actual Results**: _________________
**Status**: [ ] PASS [ ] FAIL

---

### Test Case 1.1.2: Agent File Creation and Structure
**Objective**: Verify all four agent definition files exist with correct structure

**Test Steps**:
1. Navigate to `.bmad-core/agents/` directory
2. List all files in the directory
3. Open each agent file and validate structure
4. Verify AgentConfiguration data model compliance

**Expected Results**:
- [ ] `fhir-interoperability-specialist.md` exists
- [ ] `clinical-informaticist.md` exists
- [ ] `healthcare-it-security-analyst.md` exists
- [ ] `healthcare-system-integration-analyst.md` exists
- [ ] Each file contains: `id`, `name`, `title`, `icon`, `persona` fields
- [ ] Persona object includes: `role`, `style`, `core_principles`

**Actual Results**: _________________
**Status**: [ ] PASS [ ] FAIL

---

### Test Case 1.1.3: Agent Persona Definition Validation
**Objective**: Verify agent personas are properly defined and complete

**Test Steps**:
1. For each agent file, validate persona completeness
2. Check for detailed role descriptions
3. Verify core principles are defined
4. Validate dependency references format

**Expected Results**:
- [ ] FHIR Specialist has technical interoperability focus
- [ ] Clinical Informaticist has clinical workflow focus
- [ ] Security Analyst has compliance and security focus
- [ ] Integration Analyst has vendor API focus
- [ ] Each persona has minimum 3 core principles
- [ ] Dependencies section properly formatted (tasks, templates, workflows)

**Actual Results**: _________________
**Status**: [ ] PASS [ ] FAIL

---

## Story 1.2: Define Workflow & Task Placeholders

### Test Case 1.2.1: Workflow File Creation and Structure
**Objective**: Verify workflow YAML files exist with correct structure

**Test Steps**:
1. Navigate to `.bmad-core/workflows/` directory
2. List all YAML files
3. Parse each YAML file for validity
4. Verify WorkflowDefinition data model compliance

**Expected Results**:
- [ ] `specification-workflow.yaml` exists and is valid YAML
- [ ] `development-workflow.yaml` exists and is valid YAML
- [ ] `research-workflow.yaml` exists and is valid YAML
- [ ] Each file contains: `id`, `name`, `description`, `sequence` fields
- [ ] Sequence array contains step definitions

**Actual Results**: _________________
**Status**: [ ] PASS [ ] FAIL

---

### Test Case 1.2.2: Task File Creation and Structure
**Objective**: Verify task placeholder files exist with correct structure

**Test Steps**:
1. Navigate to `.bmad-core/tasks/` directory
2. List all markdown files
3. Validate each task file structure
4. Verify TaskDefinition data model compliance

**Expected Results**:
- [ ] `create-fhir-profile.md` exists
- [ ] `document-clinical-requirements.md` exists
- [ ] `conduct-security-assessment.md` exists
- [ ] `initial-scoping.md` exists
- [ ] Each file contains: `id`, `description`, `inputs`, `outputs` sections
- [ ] Task descriptions are clear and actionable

**Actual Results**: _________________
**Status**: [ ] PASS [ ] FAIL

---

### Test Case 1.2.3: Workflow-Task-Agent Integration
**Objective**: Verify workflows reference tasks and agents correctly

**Test Steps**:
1. For each workflow, validate agent references
2. Verify task references in workflow sequences
3. Check agent files for workflow dependencies
4. Validate cross-references are bidirectional

**Expected Results**:
- [ ] Specification workflow references Clinical Informaticist and FHIR Specialist
- [ ] Research workflow references Integration Analyst
- [ ] Development workflow references FHIR Specialist and Security Analyst
- [ ] Agent files list their associated workflows in dependencies
- [ ] No orphaned tasks or broken references

**Actual Results**: _________________
**Status**: [ ] PASS [ ] FAIL

---

## Story 1.3: Define Template Placeholders

### Test Case 1.3.1: Template File Creation and Structure
**Objective**: Verify template files exist with correct structure

**Test Steps**:
1. Navigate to `.bmad-core/templates/` directory
2. List all template files
3. Validate each template file structure
4. Verify TemplateDefinition data model compliance

**Expected Results**:
- [ ] `fhir-profile.tmpl.yaml` exists
- [ ] `implementation-guide.tmpl.md` exists
- [ ] `integration-partner-profile.tmpl.md` exists
- [ ] Each file contains: `id`, `format`, `structure` fields
- [ ] Templates have placeholder variables defined

**Actual Results**: _________________
**Status**: [ ] PASS [ ] FAIL

---

### Test Case 1.3.2: Agent-Template Dependency Validation
**Objective**: Verify agents correctly reference their template dependencies

**Test Steps**:
1. Open FHIR Specialist agent file
2. Check template dependencies section
3. Repeat for Clinical Informaticist
4. Repeat for Integration Analyst
5. Verify Security Analyst template references (if any)

**Expected Results**:
- [ ] FHIR Specialist references `fhir-profile.tmpl.yaml`
- [ ] FHIR Specialist references `implementation-guide.tmpl.md`
- [ ] Clinical Informaticist references `implementation-guide.tmpl.md`
- [ ] Integration Analyst references `integration-partner-profile.tmpl.md`
- [ ] All template references resolve to actual files

**Actual Results**: _________________
**Status**: [ ] PASS [ ] FAIL

---

## Integration Test Cases

### Test Case INT-1: Agent Activation Test
**Objective**: Verify agents can be activated through BMad framework

**Test Steps**:
1. Execute BMad command to list available agents
2. Attempt to activate FHIR Specialist agent
3. Verify agent responds with expected persona
4. Test agent command execution

**Expected Results**:
- [ ] All four agents appear in agent list
- [ ] Agent activation successful
- [ ] Agent displays correct name and title
- [ ] Agent help command shows available tasks/workflows

**Actual Results**: _________________
**Status**: [ ] PASS [ ] FAIL

---

### Test Case INT-2: Workflow Orchestration Test
**Objective**: Verify workflows can orchestrate agent interactions

**Test Steps**:
1. Load specification workflow
2. Verify workflow steps are parseable
3. Check agent references resolve correctly
4. Validate task sequence is logical

**Expected Results**:
- [ ] Workflow loads without errors
- [ ] All agent references are valid
- [ ] Task sequence follows logical progression
- [ ] No circular dependencies detected

**Actual Results**: _________________
**Status**: [ ] PASS [ ] FAIL

---

### Test Case INT-3: End-to-End Foundation Test
**Objective**: Verify the complete foundation supports a simple operation

**Test Steps**:
1. Activate Clinical Informaticist agent
2. Request list of available tasks
3. Select "document-clinical-requirements" task
4. Verify task loads and displays inputs/outputs
5. Confirm template references are accessible

**Expected Results**:
- [ ] Agent activation successful
- [ ] Task list displays correctly
- [ ] Task selection works
- [ ] Task structure is complete
- [ ] No missing dependencies

**Actual Results**: _________________
**Status**: [ ] PASS [ ] FAIL

---

## Test Execution Checklist

### Pre-Test Setup
- [ ] Repository cloned fresh
- [ ] BMad framework installed and configured
- [ ] Test environment documented
- [ ] Test data prepared (if needed)

### During Testing
- [ ] Execute tests in sequence
- [ ] Document actual results immediately
- [ ] Capture screenshots/logs for failures
- [ ] Note any deviations from expected behavior

### Post-Test Activities
- [ ] Compile test results summary
- [ ] Log defects in issue tracker
- [ ] Update story status based on results
- [ ] Create remediation plan for failures

---

## Test Summary

**Total Test Cases**: 12
**Passed**: ___
**Failed**: ___
**Blocked**: ___
**Pass Rate**: ___%

### Critical Issues Found
1. _________________________________
2. _________________________________
3. _________________________________

### Recommendations
- [ ] Epic 1 Ready for Closure
- [ ] Minor fixes required before closure
- [ ] Major issues require resolution
- [ ] Retest needed after fixes

**Test Lead Signature**: _______________
**Date**: _______________

## Automated Test Script

```bash
#!/bin/bash
# Epic 1 Automated Validation Script

echo "Starting Epic 1 Foundation Validation Tests..."

# Test 1.1.1: Project Structure
echo "Test 1.1.1: Validating project structure..."
if [ -f "package.json" ] && [ -d ".bmad-core" ]; then
    echo "✓ Project structure valid"
else
    echo "✗ Project structure invalid"
    exit 1
fi

# Test 1.1.2: Agent Files
echo "Test 1.1.2: Checking agent files..."
AGENTS=("fhir-interoperability-specialist" "clinical-informaticist" "healthcare-it-security-analyst" "healthcare-system-integration-analyst")
for agent in "${AGENTS[@]}"; do
    if [ -f ".bmad-core/agents/${agent}.md" ]; then
        echo "✓ ${agent}.md exists"
    else
        echo "✗ ${agent}.md missing"
        exit 1
    fi
done

# Test 1.2.1: Workflow Files
echo "Test 1.2.1: Checking workflow files..."
WORKFLOWS=("specification-workflow" "development-workflow" "research-workflow")
for workflow in "${WORKFLOWS[@]}"; do
    if [ -f ".bmad-core/workflows/${workflow}.yaml" ]; then
        echo "✓ ${workflow}.yaml exists"
    else
        echo "✗ ${workflow}.yaml missing"
        exit 1
    fi
done

# Test 1.3.1: Template Files
echo "Test 1.3.1: Checking template files..."
if [ -f ".bmad-core/templates/fhir-profile.tmpl.yaml" ] && \
   [ -f ".bmad-core/templates/implementation-guide.tmpl.md" ] && \
   [ -f ".bmad-core/templates/integration-partner-profile.tmpl.md" ]; then
    echo "✓ All templates exist"
else
    echo "✗ Templates missing"
    exit 1
fi

echo "All automated tests passed!"
```