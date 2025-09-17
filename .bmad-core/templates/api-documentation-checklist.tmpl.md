# API Documentation Discovery Checklist Template
# Template for systematic documentation discovery and evaluation

template:
  id: api-documentation-checklist-template
  name: API Documentation Discovery Checklist
  version: 1.0
  format: markdown
  purpose: Systematic checklist for discovering and evaluating target system API documentation

content_structure: |
  # API Documentation Discovery Checklist for {{system_name}}

  **Date:** {{date_created}}
  **System:** {{system_name}}
  **Vendor:** {{vendor_name}}
  **Reviewer:** {{reviewer_name}}

  ## General Documentation Discovery

  ### Primary Resources
  - [ ] **Main vendor website found**
    - URL: {{vendor_website}}
    - Navigation to developer section: {{developer_section_path}}

  - [ ] **Developer portal/documentation site located**
    - URL: {{developer_portal_url}}
    - Registration required: {{registration_required}}
    - Access level: {{access_level}} (Public, Registration Required, Partner Only)

  - [ ] **API documentation found**
    - URL: {{api_docs_url}}
    - Format: {{docs_format}} (Interactive, PDF, HTML, Wiki)
    - Last updated: {{last_updated}}
    - Completeness: {{completeness_rating}} (Complete, Partial, Minimal)

  ### Authentication Documentation
  - [ ] **Authentication methods documented**
    - OAuth2 support: {{oauth2_support}}
    - SMART on FHIR support: {{smart_fhir_support}}
    - API key authentication: {{api_key_auth}}
    - Other methods: {{other_auth_methods}}

  - [ ] **Authentication flow documented**
    - Step-by-step guide available: {{auth_guide_available}}
    - Code examples provided: {{auth_examples_available}}
    - Scope definitions: {{scope_definitions_available}}

  ### FHIR Documentation (if applicable)
  - [ ] **FHIR implementation guide found**
    - URL: {{fhir_guide_url}}
    - FHIR version: {{fhir_version}}
    - Implementation guide format: {{fhir_guide_format}}
    - Capability statement available: {{capability_statement_available}}

  - [ ] **FHIR resource documentation**
    - Supported resources listed: {{supported_resources_listed}}
    - Resource profiles documented: {{resource_profiles_documented}}
    - Search parameters documented: {{search_params_documented}}
    - Custom extensions documented: {{custom_extensions_documented}}

  ### API Endpoints and Operations
  - [ ] **Endpoint documentation**
    - Base URL provided: {{base_url_provided}}
    - Endpoint list complete: {{endpoint_list_complete}}
    - HTTP methods documented: {{http_methods_documented}}
    - Request/response examples: {{request_response_examples}}

  - [ ] **Data model documentation**
    - Data schemas available: {{data_schemas_available}}
    - Field definitions provided: {{field_definitions_provided}}
    - Required vs optional fields: {{required_fields_documented}}
    - Data validation rules: {{validation_rules_documented}}

  ## Technical Specifications

  ### Rate Limits and Performance
  - [ ] **Rate limiting documented**
    - Rate limits specified: {{rate_limits_specified}}
    - Rate limit headers documented: {{rate_limit_headers_documented}}
    - Throttling behavior described: {{throttling_behavior_described}}
    - Burst allowances mentioned: {{burst_allowances_mentioned}}

  - [ ] **Performance characteristics**
    - Response time expectations: {{response_time_expectations}}
    - Uptime SLA documented: {{uptime_sla_documented}}
    - Maintenance windows specified: {{maintenance_windows_specified}}

  ### Error Handling
  - [ ] **Error documentation**
    - Error codes listed: {{error_codes_listed}}
    - Error message formats: {{error_message_formats}}
    - Error handling best practices: {{error_handling_practices}}
    - Retry guidance provided: {{retry_guidance_provided}}

  ### Security and Compliance
  - [ ] **Security documentation**
    - HTTPS requirements: {{https_requirements}}
    - IP whitelisting mentioned: {{ip_whitelisting_mentioned}}
    - Certificate requirements: {{certificate_requirements}}
    - Security best practices: {{security_best_practices}}

  - [ ] **Compliance information**
    - HIPAA compliance mentioned: {{hipaa_compliance_mentioned}}
    - SOC 2 certification: {{soc2_certification}}
    - BAA availability: {{baa_availability}}
    - Audit capabilities: {{audit_capabilities}}

  ## Development Resources

  ### Testing and Sandbox
  - [ ] **Sandbox environment**
    - Sandbox availability: {{sandbox_availability}}
    - Sandbox access process: {{sandbox_access_process}}
    - Test data availability: {{test_data_availability}}
    - Sandbox limitations documented: {{sandbox_limitations}}

  - [ ] **Testing guidance**
    - Testing best practices: {{testing_best_practices}}
    - Test scenarios provided: {{test_scenarios_provided}}
    - Validation tools available: {{validation_tools_available}}

  ### Code Examples and SDKs
  - [ ] **Code samples**
    - Programming languages covered: {{programming_languages}}
    - Authentication examples: {{auth_code_examples}}
    - CRUD operation examples: {{crud_examples}}
    - Error handling examples: {{error_handling_examples}}

  - [ ] **SDKs and libraries**
    - Official SDKs available: {{official_sdks}}
    - Supported languages: {{sdk_languages}}
    - Community libraries mentioned: {{community_libraries}}
    - Installation instructions: {{installation_instructions}}

  ### Support Resources
  - [ ] **Developer support**
    - Support contact information: {{support_contact_info}}
    - Developer forums available: {{developer_forums}}
    - Community resources: {{community_resources}}
    - FAQ section: {{faq_section}}

  - [ ] **Getting started guide**
    - Quick start guide available: {{quick_start_guide}}
    - Step-by-step tutorials: {{step_by_step_tutorials}}
    - Prerequisites documented: {{prerequisites_documented}}
    - Complete integration examples: {{complete_integration_examples}}

  ## Documentation Quality Assessment

  ### Completeness Score (1-5 scale)
  - Overall documentation completeness: {{completeness_score}}/5
  - Technical detail level: {{technical_detail_score}}/5
  - Code example quality: {{code_example_score}}/5
  - Up-to-date accuracy: {{accuracy_score}}/5

  ### Accessibility
  - Search functionality: {{search_functionality}}
  - Mobile-friendly: {{mobile_friendly}}
  - Navigation ease: {{navigation_ease}}
  - Download options: {{download_options}}

  ### Missing or Unclear Information
  {{missing_information}}

  ## Contact Information Discovered

  ### Technical Contacts
  - Developer support email: {{dev_support_email}}
  - Technical sales contact: {{tech_sales_contact}}
  - Partnership inquiries: {{partnership_contact}}
  - API support phone: {{api_support_phone}}

  ### Business Contacts
  - General sales: {{general_sales_contact}}
  - Account management: {{account_management_contact}}
  - Business development: {{business_dev_contact}}

  ## Next Steps and Recommendations

  ### Immediate Actions
  - [ ] {{immediate_action_1}}
  - [ ] {{immediate_action_2}}
  - [ ] {{immediate_action_3}}

  ### Questions for Vendor
  {{questions_for_vendor}}

  ### Documentation Gaps
  {{documentation_gaps}}

  ### Overall Assessment
  **Integration Feasibility:** {{integration_feasibility}} (High, Medium, Low)
  **Documentation Quality:** {{documentation_quality}} (Excellent, Good, Fair, Poor)
  **Recommended Next Step:** {{recommended_next_step}}

  ## Notes
  {{additional_notes}}

usage_instructions: |
  This checklist should be used during the initial system discovery phase to
  systematically evaluate the available API documentation for a target system.

  Complete each checkbox item and fill in the associated information. Use
  "Unknown" or "Not Found" for items that cannot be located.

  The completed checklist will help identify gaps in available documentation
  and prepare targeted questions for vendor engagement.