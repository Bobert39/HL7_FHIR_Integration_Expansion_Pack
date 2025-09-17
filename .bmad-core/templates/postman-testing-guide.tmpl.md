# Postman Testing Guide Template
# Template for systematic API testing using Postman

template:
  id: postman-testing-guide-template
  name: Postman API Testing Guide
  version: 1.0
  format: markdown
  purpose: Step-by-step guide for testing healthcare system APIs using Postman

content_structure: |
  # Postman Testing Guide for {{system_name}} API

  **Date:** {{date_created}}
  **System:** {{system_name}}
  **API Base URL:** {{api_base_url}}
  **Authentication Type:** {{auth_type}}
  **Tester:** {{tester_name}}

  ## Prerequisites

  ### Required Information
  - [ ] API Base URL: `{{api_base_url}}`
  - [ ] Authentication credentials received from vendor
  - [ ] Postman installed and configured
  - [ ] Integration Partner Profile available for reference

  ### Credentials Checklist
  - [ ] **Client ID**: {{client_id}}
  - [ ] **Client Secret**: {{client_secret}} (keep secure)
  - [ ] **Scope**: {{oauth_scope}}
  - [ ] **Authorization Endpoint**: {{auth_endpoint}}
  - [ ] **Token Endpoint**: {{token_endpoint}}

  ## Postman Collection Setup

  ### Step 1: Create New Collection
  1. Open Postman
  2. Click "New" → "Collection"
  3. Name: "{{system_name}} API Testing"
  4. Description: "API testing for {{system_name}} integration"

  ### Step 2: Configure Collection Variables
  Add the following variables at collection level:
  ```
  base_url: {{api_base_url}}
  client_id: {{client_id}}
  client_secret: {{client_secret}}
  scope: {{oauth_scope}}
  auth_url: {{auth_endpoint}}
  token_url: {{token_endpoint}}
  access_token: (will be set during authentication)
  ```

  ### Step 3: Set Up Authentication
  #### For OAuth2 Authentication:
  1. Go to Collection → Authorization tab
  2. Type: OAuth 2.0
  3. Configure:
     - Grant Type: Authorization Code or Client Credentials
     - Auth URL: `{{auth_url}}`
     - Access Token URL: `{{token_url}}`
     - Client ID: `{{client_id}}`
     - Client Secret: `{{client_secret}}`
     - Scope: `{{scope}}`

  #### For API Key Authentication:
  1. Go to Collection → Authorization tab
  2. Type: API Key
  3. Configure:
     - Key: {{api_key_header}}
     - Value: {{api_key_value}}
     - Add to: Header

  ## Authentication Testing

  ### Test 1: Token Acquisition
  **Request Name**: "Get Access Token"
  **Method**: POST
  **URL**: `{{token_url}}`
  **Headers**:
  ```
  Content-Type: application/x-www-form-urlencoded
  ```
  **Body** (for Client Credentials):
  ```
  grant_type=client_credentials
  client_id={{client_id}}
  client_secret={{client_secret}}
  scope={{scope}}
  ```

  **Expected Response**:
  ```json
  {
    "access_token": "eyJ...",
    "token_type": "Bearer",
    "expires_in": 3600,
    "scope": "patient/*.read"
  }
  ```

  **Post-request Script** (to save token):
  ```javascript
  if (pm.response.code === 200) {
      const response = pm.response.json();
      pm.collectionVariables.set("access_token", response.access_token);
      console.log("Access token saved:", response.access_token);
  }
  ```

  **Test Results**:
  - [ ] Authentication successful (200 response)
  - [ ] Access token received and saved
  - [ ] Token expiration noted: {{token_expiration}}
  - [ ] Any errors or warnings: {{auth_errors}}

  ## API Endpoint Testing

  ### Test 2: Patient Search (FHIR)
  **Request Name**: "Search Patients"
  **Method**: GET
  **URL**: `{{base_url}}/Patient`
  **Headers**:
  ```
  Authorization: Bearer {{access_token}}
  Accept: application/fhir+json
  ```
  **Query Parameters**:
  ```
  _count=10
  family=Smith
  ```

  **Expected Response**:
  ```json
  {
    "resourceType": "Bundle",
    "type": "searchset",
    "total": 1,
    "entry": [...]
  }
  ```

  **Test Results**:
  - [ ] Request successful (200 response)
  - [ ] FHIR Bundle returned
  - [ ] Patient resources in response
  - [ ] Response time: {{patient_search_time}}ms
  - [ ] Any issues: {{patient_search_issues}}

  ### Test 3: Get Specific Patient
  **Request Name**: "Get Patient by ID"
  **Method**: GET
  **URL**: `{{base_url}}/Patient/{{patient_id}}`
  **Headers**:
  ```
  Authorization: Bearer {{access_token}}
  Accept: application/fhir+json
  ```

  **Expected Response**:
  ```json
  {
    "resourceType": "Patient",
    "id": "{{patient_id}}",
    "name": [...],
    "birthDate": "1980-01-01"
  }
  ```

  **Test Results**:
  - [ ] Patient record retrieved successfully
  - [ ] Complete patient demographics available
  - [ ] Response format: {{patient_response_format}}
  - [ ] Response time: {{patient_get_time}}ms
  - [ ] Available data fields documented below

  ### Test 4: Additional Endpoints
  **Test additional endpoints as identified:**

  #### {{endpoint_name_1}}
  **Method**: {{endpoint_method_1}}
  **URL**: `{{base_url}}/{{endpoint_path_1}}`
  **Results**: {{endpoint_1_results}}

  #### {{endpoint_name_2}}
  **Method**: {{endpoint_method_2}}
  **URL**: `{{base_url}}/{{endpoint_path_2}}`
  **Results**: {{endpoint_2_results}}

  ## Error Testing

  ### Test 5: Invalid Authentication
  **Request Name**: "Test Invalid Token"
  **Method**: GET
  **URL**: `{{base_url}}/Patient`
  **Headers**:
  ```
  Authorization: Bearer invalid_token_123
  Accept: application/fhir+json
  ```

  **Expected Response**: 401 Unauthorized
  **Test Results**:
  - [ ] Proper error response received
  - [ ] Error message format: {{error_format}}
  - [ ] Error details: {{error_details}}

  ### Test 6: Rate Limiting
  **Request Name**: "Test Rate Limits"
  **Method**: Multiple rapid requests to any endpoint

  **Test Results**:
  - [ ] Rate limiting behavior observed
  - [ ] Rate limit headers present: {{rate_limit_headers}}
  - [ ] Rate limit exceeded response: {{rate_limit_response}}
  - [ ] Reset time behavior: {{rate_limit_reset}}

  ## Data Structure Analysis

  ### Patient Record Analysis
  **Sample Patient Response**:
  ```json
  {{sample_patient_response}}
  ```

  ### Available Data Fields
  **Demographics**:
  - [ ] Patient ID: {{patient_id_format}}
  - [ ] Name: {{name_format}}
  - [ ] Birth Date: {{birth_date_format}}
  - [ ] Gender: {{gender_format}}
  - [ ] Address: {{address_format}}
  - [ ] Phone: {{phone_format}}
  - [ ] Email: {{email_format}}

  **Clinical Data**:
  - [ ] Medical Record Number: {{mrn_format}}
  - [ ] Allergies: {{allergies_available}}
  - [ ] Medications: {{medications_available}}
  - [ ] Conditions: {{conditions_available}}
  - [ ] Observations: {{observations_available}}

  ### Custom Extensions
  **Vendor-specific fields identified**:
  {{custom_extensions}}

  ## Performance Metrics

  ### Response Time Analysis
  - **Authentication**: {{auth_response_time}}ms
  - **Patient Search**: {{search_response_time}}ms
  - **Patient Retrieve**: {{retrieve_response_time}}ms
  - **Average Response Time**: {{average_response_time}}ms

  ### Concurrent Request Testing
  - **Max Concurrent Requests**: {{max_concurrent}}
  - **Performance Degradation Point**: {{performance_degradation}}
  - **Error Rate at High Load**: {{error_rate_high_load}}

  ## Final Test Summary

  ### Successful Tests
  {{successful_tests}}

  ### Failed Tests
  {{failed_tests}}

  ### API Capabilities Confirmed
  - [ ] Authentication method: {{confirmed_auth_method}}
  - [ ] Base URL: {{confirmed_base_url}}
  - [ ] FHIR version: {{confirmed_fhir_version}}
  - [ ] Available resources: {{confirmed_resources}}
  - [ ] Rate limits: {{confirmed_rate_limits}}

  ### Integration Partner Profile Updates
  **Updates to make in Integration Partner Profile**:
  1. **API Base URL**: Update to confirmed working URL
  2. **Authentication Method**: Update with tested and working method
  3. **Endpoints and Operations**: Add list of tested and confirmed endpoints
  4. **Response Formats**: Document actual response formats and structures
  5. **Performance Characteristics**: Add response time and limitation information
  6. **Error Handling**: Document error response formats and codes

  ## Recommendations

  ### Technical Recommendations
  {{technical_recommendations}}

  ### Implementation Considerations
  {{implementation_considerations}}

  ### Next Steps
  1. Update Integration Partner Profile with confirmed details
  2. Document any vendor-specific implementation quirks
  3. Plan development approach based on confirmed capabilities
  4. Schedule follow-up testing for additional endpoints if needed

  ## Troubleshooting Notes
  {{troubleshooting_notes}}

usage_instructions: |
  This guide provides a systematic approach to testing healthcare system APIs using Postman.
  Follow each step in order, documenting results and updating the Integration Partner Profile
  with confirmed technical details.

  Customize the endpoint testing section based on the specific API capabilities
  identified in the vendor documentation and Integration Partner Profile.