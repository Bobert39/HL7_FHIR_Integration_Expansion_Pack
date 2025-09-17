# Integration Partner Profile Template
# Template for documenting healthcare system integration partner profiles (Initial Discovery Phase)

template:
  id: integration-partner-profile-template
  name: Integration Partner Profile Template - Initial Discovery
  version: 1.0
  format: markdown
  purpose: Initial discovery and documentation of target healthcare system for integration planning

# Simple template for initial system scoping phase
content_structure: |
  # {{system_name}} Integration Partner Profile

  **Date Created:** {{date_created}}
  **Status:** Initial Discovery
  **Next Phase:** Technical Research and API Testing

  ## System Overview

  **System Name:** {{system_name}}
  **Vendor/Organization:** {{vendor_name}}
  **System Type:** {{system_type}} (e.g., EHR, Practice Management, Laboratory, etc.)
  **Website:** {{vendor_website}}

  ## Integration Goals

  **Integration Type:** {{integration_type}} (Read-only, Write-only, Bidirectional)
  **Primary Use Case:** {{use_case}}
  **Expected Data Volume:** {{data_volume}}
  **Timeline:** {{timeline}}

  ## Public Documentation Found

  ### Developer Resources
  - **Main Developer Portal:** {{developer_portal_url}}
  - **API Documentation:** {{api_docs_url}}
  - **FHIR Implementation Guide:** {{fhir_guide_url}}
  - **Authentication Guide:** {{auth_docs_url}}
  - **Rate Limits Documentation:** {{rate_limits_url}}

  ### Additional Resources
  - **Community Forums:** {{community_forums}}
  - **Sample Code/SDKs:** {{sample_code_url}}
  - **Sandbox Environment:** {{sandbox_url}}

  ## Vendor Contacts

  ### Technical Contacts
  - **Developer Support Email:** {{dev_support_email}}
  - **Technical Sales Contact:** {{tech_sales_contact}}
  - **Partnership Contact:** {{partnership_contact}}

  ### Existing Relationships
  - **Current Vendor Relationship:** {{existing_relationship}}
  - **Account Manager:** {{account_manager}}

  ## Initial Technical Assessment

  ### Suspected Capabilities
  - **FHIR Support:** {{fhir_support}} (R4, STU3, DSTU2, Unknown, None)
  - **Authentication Methods:** {{auth_methods}} (OAuth2, SMART on FHIR, API Keys, etc.)
  - **API Types:** {{api_types}} (REST, SOAP, HL7 v2.x, Custom)

  ### Requirements Assessment

  **Data Needed:**
  {{data_requirements}}

  **Security Requirements:**
  {{security_requirements}}

  **Compliance Needs:**
  {{compliance_requirements}}

  ## Key Questions for Vendor

  ### Technical Questions
  1. What authentication methods are supported for API access?
  2. What are the rate limits for API calls?
  3. Is there a sandbox environment available for testing?
  4. What FHIR resources are supported (if applicable)?
  5. What is the process for obtaining API credentials?
  6. Are there any specific security requirements or certifications needed?

  ### Business Questions
  1. What is the typical onboarding timeline for new integration partners?
  2. Are there any integration fees or licensing costs?
  3. What level of technical support is provided during implementation?
  4. Are there any restrictions on data usage or patient consent requirements?

  ## Next Steps

  - [ ] Contact vendor technical team with prepared questions
  - [ ] Request access to sandbox environment and API credentials
  - [ ] Review detailed API documentation
  - [ ] Proceed to technical research and API testing phase
  - [ ] Update this profile with confirmed technical details

  ## Notes

  {{additional_notes}}

usage_instructions: |
  This template is used during the initial-scoping task to document findings from public research
  and prepare for vendor engagement. Fill in the placeholders with information gathered during
  the discovery process. Use "Unknown" or "TBD" for information not yet available.

  The completed profile will be used as input for the technical-research task where API testing
  and detailed technical validation will be performed.