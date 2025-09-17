# Endpoint Testing Protocol Template
# Template for systematic API endpoint testing and validation

template:
  id: endpoint-testing-protocol-template
  name: API Endpoint Testing Protocol
  version: 1.0
  format: markdown
  purpose: Systematic protocol for testing and validating API endpoints and operations

content_structure: |
  # API Endpoint Testing Protocol for {{system_name}}

  **Date:** {{date_created}}
  **System:** {{system_name}}
  **API Base URL:** {{api_base_url}}
  **FHIR Version:** {{fhir_version}}
  **Tester:** {{tester_name}}
  **Environment:** {{environment}}

  ## Testing Overview

  ### Test Scope
  **Primary Focus:**
  - [ ] Patient data access and retrieval
  - [ ] Clinical data endpoints (observations, conditions, medications)
  - [ ] Administrative data (appointments, organizations)
  - [ ] Custom or vendor-specific endpoints

  **Authentication Status:**
  - Authentication method: {{auth_method}}
  - Token obtained: {{token_obtained}}
  - Token expires: {{token_expiration}}

  ## Patient Data Endpoints

  ### Test 1: Patient Search
  **Endpoint**: `GET {{api_base_url}}/Patient`
  **Purpose**: Test patient search functionality and pagination

  #### Test Cases
  1. **Basic Search**
     - URL: `{{api_base_url}}/Patient?_count=10`
     - Expected: Bundle with patient list
     - Result: {{patient_search_basic_result}}
     - Response Time: {{patient_search_time}}ms

  2. **Name-based Search**
     - URL: `{{api_base_url}}/Patient?family=Smith&given=John`
     - Expected: Matching patients
     - Result: {{patient_search_name_result}}

  3. **Date-based Search**
     - URL: `{{api_base_url}}/Patient?birthdate=ge1990-01-01`
     - Expected: Patients born after 1990
     - Result: {{patient_search_date_result}}

  4. **Identifier Search**
     - URL: `{{api_base_url}}/Patient?identifier={{test_identifier}}`
     - Expected: Specific patient by ID
     - Result: {{patient_search_id_result}}

  **Summary:**
  - [ ] Patient search functional: {{patient_search_functional}}
  - [ ] Pagination working: {{pagination_working}}
  - [ ] Search parameters supported: {{search_params_supported}}
  - [ ] Performance acceptable: {{performance_acceptable}}

  ### Test 2: Individual Patient Retrieval
  **Endpoint**: `GET {{api_base_url}}/Patient/{id}`
  **Purpose**: Test individual patient record retrieval

  #### Test Cases
  1. **Valid Patient ID**
     - URL: `{{api_base_url}}/Patient/{{test_patient_id}}`
     - Expected: Complete patient resource
     - Result: {{patient_get_valid_result}}
     - Response Time: {{patient_get_time}}ms

  2. **Invalid Patient ID**
     - URL: `{{api_base_url}}/Patient/invalid-id-123`
     - Expected: 404 Not Found
     - Result: {{patient_get_invalid_result}}

  3. **Patient with Includes**
     - URL: `{{api_base_url}}/Patient/{{test_patient_id}}?_include=Patient:organization`
     - Expected: Patient with included organization
     - Result: {{patient_include_result}}

  **Summary:**
  - [ ] Individual retrieval working: {{individual_retrieval_working}}
  - [ ] Error handling appropriate: {{error_handling_appropriate}}
  - [ ] Include parameters supported: {{include_params_supported}}

  ## Clinical Data Endpoints

  ### Test 3: Observations
  **Endpoint**: `GET {{api_base_url}}/Observation`
  **Purpose**: Test clinical observation data access

  #### Test Cases
  1. **Patient-specific Observations**
     - URL: `{{api_base_url}}/Observation?patient={{test_patient_id}}`
     - Expected: Observations for specific patient
     - Result: {{observation_patient_result}}

  2. **Category-based Search**
     - URL: `{{api_base_url}}/Observation?category=vital-signs&patient={{test_patient_id}}`
     - Expected: Vital signs observations
     - Result: {{observation_category_result}}

  3. **Date Range Search**
     - URL: `{{api_base_url}}/Observation?patient={{test_patient_id}}&date=ge2023-01-01`
     - Expected: Recent observations
     - Result: {{observation_date_result}}

  **Summary:**
  - [ ] Observation access working: {{observation_access_working}}
  - [ ] Clinical data available: {{clinical_data_available}}
  - [ ] Data quality acceptable: {{data_quality_acceptable}}

  ### Test 4: Conditions/Diagnoses
  **Endpoint**: `GET {{api_base_url}}/Condition`
  **Purpose**: Test condition and diagnosis data access

  #### Test Cases
  1. **Patient Conditions**
     - URL: `{{api_base_url}}/Condition?patient={{test_patient_id}}`
     - Expected: Patient's conditions/diagnoses
     - Result: {{condition_patient_result}}

  2. **Active Conditions**
     - URL: `{{api_base_url}}/Condition?patient={{test_patient_id}}&clinical-status=active`
     - Expected: Only active conditions
     - Result: {{condition_active_result}}

  **Summary:**
  - [ ] Condition data accessible: {{condition_data_accessible}}
  - [ ] Diagnosis information complete: {{diagnosis_info_complete}}

  ### Test 5: Medications
  **Endpoint**: `GET {{api_base_url}}/MedicationRequest` or `{{api_base_url}}/MedicationStatement`
  **Purpose**: Test medication data access

  #### Test Cases
  1. **Patient Medications**
     - URL: `{{api_base_url}}/MedicationRequest?patient={{test_patient_id}}`
     - Expected: Patient's medication orders
     - Result: {{medication_request_result}}

  2. **Active Medications**
     - URL: `{{api_base_url}}/MedicationRequest?patient={{test_patient_id}}&status=active`
     - Expected: Currently active medications
     - Result: {{medication_active_result}}

  **Summary:**
  - [ ] Medication data accessible: {{medication_data_accessible}}
  - [ ] Medication details complete: {{medication_details_complete}}

  ## Administrative Endpoints

  ### Test 6: Organizations
  **Endpoint**: `GET {{api_base_url}}/Organization`
  **Purpose**: Test organization/provider data access

  #### Test Cases
  1. **Organization Search**
     - URL: `{{api_base_url}}/Organization?name={{org_name}}`
     - Expected: Matching organizations
     - Result: {{organization_search_result}}

  2. **Organization Details**
     - URL: `{{api_base_url}}/Organization/{{test_org_id}}`
     - Expected: Complete organization information
     - Result: {{organization_details_result}}

  **Summary:**
  - [ ] Organization data accessible: {{organization_data_accessible}}
  - [ ] Provider information available: {{provider_info_available}}

  ### Test 7: Practitioners
  **Endpoint**: `GET {{api_base_url}}/Practitioner`
  **Purpose**: Test practitioner/provider data access

  #### Test Cases
  1. **Practitioner Search**
     - URL: `{{api_base_url}}/Practitioner?name={{practitioner_name}}`
     - Expected: Matching practitioners
     - Result: {{practitioner_search_result}}

  **Summary:**
  - [ ] Practitioner data accessible: {{practitioner_data_accessible}}

  ## Custom and Extended Endpoints

  ### Test 8: Custom Endpoints
  **Purpose**: Test vendor-specific or custom endpoints

  #### Custom Endpoint 1
  - **Endpoint**: `{{custom_endpoint_1}}`
  - **Purpose**: {{custom_endpoint_1_purpose}}
  - **Result**: {{custom_endpoint_1_result}}

  #### Custom Endpoint 2
  - **Endpoint**: `{{custom_endpoint_2}}`
  - **Purpose**: {{custom_endpoint_2_purpose}}
  - **Result**: {{custom_endpoint_2_result}}

  ## CRUD Operations Testing

  ### Test 9: Create Operations (if supported)
  **Purpose**: Test data creation capabilities

  #### Patient Creation Test
  - **Method**: POST
  - **Endpoint**: `{{api_base_url}}/Patient`
  - **Payload**: {{patient_create_payload}}
  - **Result**: {{patient_create_result}}
  - **Supported**: {{create_operations_supported}}

  ### Test 10: Update Operations (if supported)
  **Purpose**: Test data modification capabilities

  #### Patient Update Test
  - **Method**: PUT/PATCH
  - **Endpoint**: `{{api_base_url}}/Patient/{{test_patient_id}}`
  - **Payload**: {{patient_update_payload}}
  - **Result**: {{patient_update_result}}
  - **Supported**: {{update_operations_supported}}

  ## Error Handling and Edge Cases

  ### Test 11: Error Response Testing
  **Purpose**: Validate error handling and response formats

  #### Test Cases
  1. **Invalid Resource Type**
     - URL: `{{api_base_url}}/InvalidResource`
     - Expected: 404 Not Found
     - Result: {{invalid_resource_result}}

  2. **Malformed Query**
     - URL: `{{api_base_url}}/Patient?invalid-param=value`
     - Expected: 400 Bad Request or ignore
     - Result: {{malformed_query_result}}

  3. **Unauthorized Access**
     - URL: `{{api_base_url}}/Patient` (without token)
     - Expected: 401 Unauthorized
     - Result: {{unauthorized_access_result}}

  4. **Forbidden Resource**
     - URL: `{{api_base_url}}/Patient/{{restricted_patient_id}}`
     - Expected: 403 Forbidden
     - Result: {{forbidden_resource_result}}

  **Summary:**
  - [ ] Error responses appropriate: {{error_responses_appropriate}}
  - [ ] Error format consistent: {{error_format_consistent}}
  - [ ] Error messages helpful: {{error_messages_helpful}}

  ## Performance and Scalability Testing

  ### Test 12: Performance Characteristics
  **Purpose**: Measure API performance and identify limitations

  #### Response Time Analysis
  - **Average Response Time**: {{average_response_time}}ms
  - **95th Percentile**: {{p95_response_time}}ms
  - **Slowest Endpoint**: {{slowest_endpoint}}
  - **Fastest Endpoint**: {{fastest_endpoint}}

  #### Pagination Performance
  - **Small Result Sets** (≤10 records): {{small_set_performance}}ms
  - **Medium Result Sets** (11-100 records): {{medium_set_performance}}ms
  - **Large Result Sets** (>100 records): {{large_set_performance}}ms

  #### Concurrent Request Testing
  - **Max Concurrent Requests Tested**: {{max_concurrent_tested}}
  - **Error Rate at High Concurrency**: {{error_rate_high_concurrency}}%
  - **Performance Degradation**: {{performance_degradation}}

  ### Test 13: Rate Limiting Validation
  **Purpose**: Understand and document rate limiting behavior

  #### Rate Limit Testing
  - **Rate Limit Headers Present**: {{rate_limit_headers_present}}
  - **Documented Rate Limits**: {{documented_rate_limits}}
  - **Actual Rate Limits Observed**: {{observed_rate_limits}}
  - **Rate Limit Reset Behavior**: {{rate_limit_reset_behavior}}
  - **Burst Allowance**: {{burst_allowance}}

  ## Data Quality and Structure Analysis

  ### Test 14: Data Structure Validation
  **Purpose**: Analyze data quality and structure consistency

  #### FHIR Compliance
  - [ ] Resources conform to FHIR specification: {{fhir_compliance}}
  - [ ] Required fields present: {{required_fields_present}}
  - [ ] Data types correct: {{data_types_correct}}
  - [ ] Reference integrity maintained: {{reference_integrity}}

  #### Data Completeness
  - **Patient Demographics Completeness**: {{patient_demographics_completeness}}%
  - **Clinical Data Completeness**: {{clinical_data_completeness}}%
  - **Administrative Data Completeness**: {{admin_data_completeness}}%

  #### Custom Extensions
  - **Custom Fields Identified**: {{custom_fields_identified}}
  - **Vendor Extensions Used**: {{vendor_extensions_used}}
  - **Non-standard Implementations**: {{non_standard_implementations}}

  ## Test Results Summary

  ### Successfully Tested Endpoints
  {{successful_endpoints}}

  ### Failed or Unavailable Endpoints
  {{failed_endpoints}}

  ### Endpoint Performance Summary
  | Endpoint | Method | Avg Response Time | Success Rate | Notes |
  |----------|--------|------------------|--------------|-------|
  {{endpoint_performance_table}}

  ### Data Availability Summary
  | Data Type | Available | Quality | Completeness | Notes |
  |-----------|-----------|---------|--------------|-------|
  | Patient Demographics | {{patient_demo_available}} | {{patient_demo_quality}} | {{patient_demo_completeness}} | {{patient_demo_notes}} |
  | Clinical Observations | {{clinical_obs_available}} | {{clinical_obs_quality}} | {{clinical_obs_completeness}} | {{clinical_obs_notes}} |
  | Medications | {{medications_available}} | {{medications_quality}} | {{medications_completeness}} | {{medications_notes}} |
  | Conditions/Diagnoses | {{conditions_available}} | {{conditions_quality}} | {{conditions_completeness}} | {{conditions_notes}} |

  ## Integration Partner Profile Updates

  ### Confirmed API Details
  **Working Base URL**: {{confirmed_base_url}}
  **Supported FHIR Version**: {{confirmed_fhir_version}}
  **Authentication Method**: {{confirmed_auth_method}}

  ### Supported Operations
  {{supported_operations}}

  ### Performance Characteristics
  - **Average Response Time**: {{avg_response_time}}ms
  - **Rate Limits**: {{confirmed_rate_limits}}
  - **Concurrent Request Limit**: {{concurrent_limit}}
  - **Uptime Observed**: {{observed_uptime}}

  ### Data Access Capabilities
  {{data_access_capabilities}}

  ## Recommendations and Next Steps

  ### Technical Recommendations
  {{technical_recommendations}}

  ### Implementation Considerations
  {{implementation_considerations}}

  ### Performance Optimization Opportunities
  {{performance_optimization}}

  ### Data Quality Improvements Needed
  {{data_quality_improvements}}

  ## Issues and Limitations Identified

  ### Technical Issues
  {{technical_issues}}

  ### Data Quality Issues
  {{data_quality_issues}}

  ### Performance Limitations
  {{performance_limitations}}

  ### Compliance Concerns
  {{compliance_concerns}}

  ## Contact Information for Follow-up
  - **Technical Support**: {{technical_support_contact}}
  - **API Questions**: {{api_questions_contact}}
  - **Data Issues**: {{data_issues_contact}}

  ## Additional Notes and Observations
  {{additional_notes}}

usage_instructions: |
  This protocol provides comprehensive testing coverage for healthcare system APIs.

  Execute tests in the order presented, documenting results for each test case.
  Pay special attention to data quality, performance characteristics, and any
  vendor-specific implementations that deviate from standard FHIR.

  Use results to update the Integration Partner Profile with confirmed technical
  capabilities and limitations.