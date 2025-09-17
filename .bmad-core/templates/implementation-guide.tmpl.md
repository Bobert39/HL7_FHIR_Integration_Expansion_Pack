# Implementation Guide Template
# Template for generating comprehensive FHIR implementation guides

template:
  id: implementation-guide-template
  name: FHIR Implementation Guide Template
  version: 1.0
  format: markdown
  output:
    format: markdown
    filename: "{{guide_name}}-implementation-guide.md"
    title: "{{guide_title}} - Implementation Guide"

workflow:
  mode: interactive
  elicitation: advanced-elicitation

agent_config:
  primary_agent: fhir-interoperability-specialist
  secondary_agents: [clinical-informaticist, healthcare-it-security-analyst]
  editable_sections:
    - guide_overview
    - clinical_context
    - technical_specifications
    - security_considerations
    - implementation_examples

sections:
  - id: guide_overview
    title: Implementation Guide Overview
    type: structured-content
    instruction: Provide comprehensive overview of the implementation guide
    elicit: true
    fields:
      - name: guide_name
        type: text
        instruction: "Implementation guide identifier"
      - name: guide_title
        type: text
        instruction: "Human-readable guide title"
      - name: version
        type: text
        instruction: "Guide version (semantic versioning)"
        default: "1.0.0"
      - name: scope
        type: long-text
        instruction: "Scope and purpose of this implementation guide"
      - name: target_audience
        type: multi-select
        choices: ["Clinical Teams", "Technical Implementers", "System Administrators", "Compliance Officers", "Project Managers"]
        instruction: "Select target audiences for this guide"

  - id: clinical_context
    title: Clinical Context and Requirements
    type: structured-content
    instruction: Document the clinical context and requirements
    elicit: true
    fields:
      - name: clinical_use_cases
        type: repeating-section
        instruction: "Describe primary clinical use cases"
        fields:
          - name: use_case_title
            type: text
          - name: clinical_workflow
            type: long-text
          - name: data_requirements
            type: long-text
      - name: stakeholders
        type: repeating-section
        instruction: "Identify key stakeholders and their roles"
        fields:
          - name: stakeholder_role
            type: text
          - name: responsibilities
            type: long-text
          - name: success_criteria
            type: long-text

  - id: technical_specifications
    title: Technical Specifications
    type: structured-content
    instruction: Define technical implementation requirements
    elicit: true
    fields:
      - name: fhir_version
        type: choice
        choices: ["R4", "R5"]
        instruction: "FHIR version for this implementation"
        default: "R4"
      - name: profiles_used
        type: repeating-section
        instruction: "List FHIR profiles used in this implementation"
        fields:
          - name: profile_name
            type: text
          - name: profile_purpose
            type: text
          - name: profile_url
            type: text
      - name: api_endpoints
        type: repeating-section
        instruction: "Document API endpoints and their specifications"
        fields:
          - name: endpoint_path
            type: text
          - name: http_method
            type: choice
            choices: ["GET", "POST", "PUT", "DELETE", "PATCH"]
          - name: endpoint_description
            type: long-text
          - name: request_format
            type: code-block
          - name: response_format
            type: code-block

  - id: security_considerations
    title: Security and Privacy Considerations
    type: structured-content
    instruction: Document security requirements and implementation guidance
    elicit: true
    fields:
      - name: authentication_method
        type: choice
        choices: ["OAuth 2.0", "SMART on FHIR", "API Keys", "Certificate-based", "Other"]
        instruction: "Primary authentication method"
      - name: authorization_approach
        type: long-text
        instruction: "Authorization and access control approach"
      - name: data_encryption
        type: long-text
        instruction: "Data encryption requirements and implementation"
      - name: audit_logging
        type: long-text
        instruction: "Audit logging requirements and procedures"
      - name: phi_handling
        type: long-text
        instruction: "Protected Health Information handling procedures"

  - id: implementation_examples
    title: Implementation Examples
    type: structured-content
    instruction: Provide concrete implementation examples and code samples
    elicit: true
    fields:
      - name: code_examples
        type: repeating-section
        instruction: "Provide implementation code examples"
        fields:
          - name: example_title
            type: text
          - name: programming_language
            type: choice
            choices: ["C#", "Java", "Python", "JavaScript", "Other"]
          - name: code_sample
            type: code-block
          - name: explanation
            type: long-text
      - name: test_scenarios
        type: repeating-section
        instruction: "Define test scenarios and validation procedures"
        fields:
          - name: scenario_name
            type: text
          - name: test_description
            type: long-text
          - name: expected_outcome
            type: long-text

content_structure: |
  # {{guide_title}} - Implementation Guide

  ## Version {{version}}

  ## Table of Contents
  1. [Overview](#overview)
  2. [Clinical Context](#clinical-context)
  3. [Technical Specifications](#technical-specifications)
  4. [Security Considerations](#security-considerations)
  5. [Implementation Examples](#implementation-examples)
  6. [Testing and Validation](#testing-and-validation)
  7. [Support and Resources](#support-and-resources)

  ## Overview

  ### Scope and Purpose
  {{scope}}

  ### Target Audience
  {{#each target_audience}}
  - {{this}}
  {{/each}}

  ### FHIR Version
  This implementation guide is based on **HL7 FHIR {{fhir_version}}**.

  ## Clinical Context

  ### Clinical Use Cases
  {{#each clinical_use_cases}}
  #### {{use_case_title}}
  **Workflow:** {{clinical_workflow}}

  **Data Requirements:** {{data_requirements}}
  {{/each}}

  ### Stakeholders
  {{#each stakeholders}}
  #### {{stakeholder_role}}
  **Responsibilities:** {{responsibilities}}

  **Success Criteria:** {{success_criteria}}
  {{/each}}

  ## Technical Specifications

  ### FHIR Profiles
  {{#each profiles_used}}
  - **{{profile_name}}**: {{profile_purpose}}
    - URL: `{{profile_url}}`
  {{/each}}

  ### API Endpoints
  {{#each api_endpoints}}
  #### {{http_method}} {{endpoint_path}}
  {{endpoint_description}}

  **Request Format:**
  ```json
  {{request_format}}
  ```

  **Response Format:**
  ```json
  {{response_format}}
  ```
  {{/each}}

  ## Security Considerations

  ### Authentication
  **Method:** {{authentication_method}}

  ### Authorization
  {{authorization_approach}}

  ### Data Encryption
  {{data_encryption}}

  ### Audit Logging
  {{audit_logging}}

  ### PHI Handling
  {{phi_handling}}

  ## Implementation Examples

  ### Code Examples
  {{#each code_examples}}
  #### {{example_title}}
  {{explanation}}

  ```{{programming_language}}
  {{code_sample}}
  ```
  {{/each}}

  ### Test Scenarios
  {{#each test_scenarios}}
  #### {{scenario_name}}
  **Description:** {{test_description}}

  **Expected Outcome:** {{expected_outcome}}
  {{/each}}

  ## Testing and Validation

  ### Validation Tools
  - FHIR Validator
  - Simplifier.net
  - Forge

  ### Testing Procedures
  1. Profile validation against FHIR specification
  2. Resource instance validation against profiles
  3. API endpoint testing
  4. Security testing
  5. Performance testing

  ## Support and Resources

  ### Documentation
  - [HL7 FHIR Specification](http://hl7.org/fhir/)
  - [Firely .NET SDK Documentation](https://docs.fire.ly/)

  ### Contact Information
  For technical support and questions, please contact: {{contact_email}}

validation:
  required_sections:
    - guide_overview
    - clinical_context
    - technical_specifications
    - security_considerations
  quality_gates:
    - "All clinical use cases must be clearly documented"
    - "Technical specifications must include concrete examples"
    - "Security considerations must address PHI protection"
    - "Implementation examples must be validated and tested"