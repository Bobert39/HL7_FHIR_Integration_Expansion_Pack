<!-- Powered by BMAD™ Core -->

# Author Implementation Guide

## ⚠️ CRITICAL EXECUTION NOTICE ⚠️

**THIS IS AN EXECUTABLE WORKFLOW - NOT REFERENCE MATERIAL**

When this task is invoked:

1. **DISABLE ALL EFFICIENCY OPTIMIZATIONS** - This workflow requires full user interaction
2. **MANDATORY STEP-BY-STEP EXECUTION** - Each section must be processed sequentially with user feedback
3. **ELICITATION IS REQUIRED** - When `elicit: true`, you MUST use the 1-9 format and wait for user response
4. **NO SHORTCUTS ALLOWED** - Complete implementation guides cannot be created without following this workflow

**VIOLATION INDICATOR:** If you create a complete Implementation Guide without user interaction, you have violated this workflow.

## Purpose

Guide users through creating a comprehensive FHIR Implementation Guide by assembling all project artifacts into a professional publication ready for Simplifier.net distribution.

## Inputs Required

- Clinical requirements documentation from Epic 1
- FHIR profiles and extensions from Epic 3
- Integration service implementation from Epic 4
- Validation suite results from Story 5.1
- Security assessment from Story 5.2
- Implementation Guide Template at `.bmad-core/templates/implementation-guide.tmpl.md`

## Expected Outputs

- Complete Implementation Guide document in `docs/implementation-guide/`
- FHIR package structure for Simplifier.net publication in `fhir-package/`
- Publication instructions in `docs/publication/`
- Versioned IG with semantic versioning (Major.Minor.Patch format)
- External partner access strategy and documentation

## Workflow Steps

### Step 1: Project Artifact Assembly

**Objective:** Gather all required project artifacts for IG compilation

**Actions:**
1. **Inventory Clinical Requirements**
   - Locate Epic 1 clinical requirements documentation in `docs/`
   - Extract clinical use cases and workflow descriptions
   - Identify stakeholder roles and responsibilities
   - Document success criteria and acceptance requirements

2. **Collect FHIR Technical Artifacts**
   - Gather all FHIR profiles created in Epic 3 from `src/` and `docs/`
   - Collect FHIR extensions and value sets
   - Retrieve example FHIR resources demonstrating profile usage
   - Document profile canonical URLs and dependencies

3. **Extract Implementation Evidence**
   - Review integration service code from `src/FhirIntegrationService/`
   - Extract API endpoint specifications and request/response examples
   - Document authentication and authorization implementation
   - Collect validation and error handling patterns

4. **Gather Quality Assurance Results**
   - Include validation suite results from Story 5.1
   - Integrate security assessment findings from Story 5.2
   - Document testing procedures and quality gates
   - Extract compliance and regulatory considerations

**Deliverable:** Comprehensive artifact inventory with organized content ready for IG assembly

### Step 2: Implementation Guide Template Instantiation

**Objective:** Use the IG template to create structured documentation

**Actions:**
1. **Initialize Template Processing**
   - Load Implementation Guide Template from `.bmad-core/templates/implementation-guide.tmpl.md`
   - Configure interactive mode with advanced elicitation
   - Set primary agent as FHIR Interoperability Specialist
   - Enable secondary agents: Clinical Informaticist, Healthcare IT Security Analyst

2. **Process Template Sections with User Interaction**
   - **Guide Overview Section** (elicit: true)
     - Define IG name, title, and version (semantic versioning)
     - Establish scope and purpose statement
     - Identify target audiences (Clinical Teams, Technical Implementers, etc.)

   - **Clinical Context Section** (elicit: true)
     - Document clinical use cases from Epic 1
     - Define stakeholder roles and responsibilities
     - Establish success criteria and workflow requirements

   - **Technical Specifications Section** (elicit: true)
     - Specify FHIR R4 version compliance
     - List all FHIR profiles with purposes and URLs
     - Document API endpoints with request/response formats

   - **Security Considerations Section** (elicit: true)
     - Define SMART on FHIR authentication method
     - Document authorization and access control approach
     - Specify data encryption and audit logging requirements
     - Address PHI handling procedures

   - **Implementation Examples Section** (elicit: true)
     - Provide C# code examples following project coding standards
     - Define test scenarios and validation procedures
     - Include concrete implementation guidance

**Critical Elicitation Requirements:**
- Each section with `elicit: true` MUST follow the 1-9 numbered options format
- Present detailed rationale explaining trade-offs and decisions
- Wait for user response before proceeding to next section
- Apply user feedback and iterate on content

**Deliverable:** Complete Implementation Guide document generated from template with user-validated content

### Step 3: FHIR Package Preparation

**Objective:** Create publication-ready FHIR package for Simplifier.net

**Actions:**
1. **Create FHIR Package Structure**
   - Create `fhir-package/` directory with standard FHIR package layout
   - Generate `package.json` manifest with semantic version (v1.0.0 for initial release)
   - Include all FHIR profiles, extensions, and value sets
   - Add example resources demonstrating profile usage

2. **Configure Package Metadata**
   - Set canonical URL for stable reference: `http://example.org/fhir/ig/hl7-fhir-expansion-pack`
   - Define package dependencies (base FHIR R4, Firely .NET SDK references)
   - Specify version constraints and compatibility requirements
   - Document package purpose and scope

3. **Validate Package Completeness**
   - Verify all referenced profiles are included
   - Check canonical URL consistency across resources
   - Validate resource interdependencies
   - Ensure example resources validate against profiles

**Deliverable:** Complete FHIR package ready for Simplifier.net upload

### Step 4: Publication Instructions Creation

**Objective:** Provide step-by-step guidance for Simplifier.net publication

**Actions:**
1. **Create Publication Checklist**
   - Document pre-publication validation requirements
   - List required Simplifier.net account permissions
   - Define publication approval workflow
   - Establish quality assurance checkpoints

2. **Generate Simplifier.net Instructions**
   - Provide step-by-step Simplifier.net project setup guidance
   - Document FHIR package upload procedures
   - Specify visibility and access permission configuration
   - Include canonical URL setup and verification steps

3. **Define External Partner Access Strategy**
   - Healthcare organizations implementing FHIR
   - EHR vendors seeking integration patterns
   - Healthcare IT consultants and clinical informaticists
   - Health system integration teams
   - Create partner onboarding documentation
   - Establish feedback collection and integration process

**Deliverable:** Comprehensive publication instructions in `docs/publication/`

### Step 5: Quality Assurance and Validation

**Objective:** Ensure IG completeness and publication readiness

**Actions:**
1. **Content Validation**
   - Verify all project artifacts are integrated
   - Check technical accuracy of code examples
   - Validate clinical relevance of use cases
   - Ensure security compliance with healthcare standards

2. **Navigation and Accessibility Testing**
   - Test IG document navigation and linking
   - Verify resource downloads and usage examples
   - Validate external references and canonical URLs
   - Check accessibility compliance for professional publication

3. **Publication Readiness Assessment**
   - Confirm FHIR package validation passes
   - Verify Simplifier.net compatibility
   - Test external accessibility requirements
   - Validate stable URL functionality

**Deliverable:** Validated, publication-ready Implementation Guide

## File Structure Created

```
docs/
├── implementation-guide/
│   ├── [guide-name]-implementation-guide.md
│   ├── clinical-context.md
│   ├── technical-specifications.md
│   └── security-considerations.md
├── publication/
│   ├── simplifier-publication-guide.md
│   ├── external-partner-access.md
│   └── publication-checklist.md
└── artifacts/
    ├── clinical-requirements-summary.md
    ├── fhir-profiles-inventory.md
    └── implementation-evidence.md

fhir-package/
├── package.json
├── profiles/
├── extensions/
├── valuesets/
└── examples/
```

## Semantic Versioning Strategy

### Version Format: Major.Minor.Patch

- **Major (X.0.0)**: Breaking changes to profiles or API contracts
  - FHIR profile structure changes affecting existing implementations
  - API endpoint modifications requiring client updates
  - Security model changes affecting authentication/authorization

- **Minor (X.Y.0)**: New features without breaking changes
  - New FHIR profiles or extensions
  - Additional API endpoints
  - Enhanced security features maintaining backward compatibility
  - New implementation examples and documentation sections

- **Patch (X.Y.Z)**: Bug fixes and documentation updates
  - Error corrections in profiles or examples
  - Documentation clarifications and improvements
  - Example resource corrections
  - Security vulnerability patches

### Version Release Process

1. **Pre-release Validation**: All quality gates must pass
2. **Semantic Version Assignment**: Follow breaking change assessment
3. **Release Documentation**: Update changelog and migration guides
4. **Simplifier.net Publication**: Update canonical URLs with version
5. **External Communication**: Notify partners of significant updates

## Maintenance and Distribution Strategy

### Maintenance Procedures
- **Quarterly Reviews**: Regular content updates and accuracy validation
- **Annual Major Updates**: Comprehensive review and enhancement cycles
- **Patch Releases**: Critical fixes and security updates as needed

### Feedback Integration
- **GitHub Issues**: Technical feedback and bug reports
- **Simplifier.net Comments**: Community feedback and usage questions
- **Direct Stakeholder Communication**: Partner-specific requirements and enhancements

### Distribution Channels
- **Primary**: Simplifier.net for FHIR community access and collaboration
- **Secondary**: GitHub releases for version control and issue tracking
- **Tertiary**: Organization websites and conference presentations

## Success Criteria

1. **Complete IG Assembly**: All project artifacts successfully integrated
2. **Template Compliance**: Full utilization of Implementation Guide Template
3. **Publication Readiness**: FHIR package validates and ready for Simplifier.net
4. **External Accessibility**: Stable canonical URL and partner access configured
5. **Quality Assurance**: All validation tests pass and documentation is professional-grade
6. **Maintenance Framework**: Update procedures and feedback collection established

## Validation Requirements

- All FHIR profiles validate against R4 specification
- Code examples compile and execute successfully
- Security considerations address healthcare compliance requirements
- Documentation meets professional publication standards
- External links and references are accessible and current