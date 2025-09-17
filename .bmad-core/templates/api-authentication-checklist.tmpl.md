# API Authentication Checklist Template
# Template for validating authentication flow and security requirements

template:
  id: api-authentication-checklist-template
  name: API Authentication Validation Checklist
  version: 1.0
  format: markdown
  purpose: Systematic validation of authentication flows and security requirements

content_structure: |
  # API Authentication Validation Checklist for {{system_name}}

  **Date:** {{date_created}}
  **System:** {{system_name}}
  **Authentication Type:** {{auth_type}}
  **Validator:** {{validator_name}}
  **Environment:** {{environment}} (Sandbox/Production)

  ## Authentication Method Overview

  ### Identified Authentication Type
  - [ ] **OAuth 2.0** - {{oauth_version}}
    - Grant Type: {{grant_type}}
    - Authorization Code Flow: {{auth_code_flow}}
    - Client Credentials Flow: {{client_credentials_flow}}
    - Refresh Token Support: {{refresh_token_support}}

  - [ ] **SMART on FHIR** - {{smart_version}}
    - Launch Context: {{launch_context}}
    - Scope Support: {{scope_support}}
    - Patient Authorization: {{patient_authorization}}

  - [ ] **API Key Authentication**
    - Header-based: {{header_based}}
    - Query Parameter: {{query_parameter}}
    - Custom Location: {{custom_location}}

  - [ ] **Certificate-based Authentication**
    - Client Certificates: {{client_certificates}}
    - mTLS Support: {{mtls_support}}

  - [ ] **Other Authentication Method**
    - Method: {{other_auth_method}}
    - Details: {{other_auth_details}}

  ## Pre-Authentication Setup

  ### Credential Management
  - [ ] **Client Registration Completed**
    - Client ID received: {{client_id_received}}
    - Client Secret received: {{client_secret_received}}
    - Registration date: {{registration_date}}
    - Contact for credential issues: {{credential_contact}}

  - [ ] **Environment Configuration**
    - Sandbox environment access: {{sandbox_access}}
    - Production environment access: {{production_access}}
    - Environment differences documented: {{env_differences}}

  - [ ] **Network Requirements**
    - API endpoint accessibility verified: {{endpoint_accessible}}
    - SSL/TLS requirements met: {{ssl_requirements}}
    - IP whitelisting configured: {{ip_whitelisting}}
    - Firewall rules updated: {{firewall_rules}}

  ## OAuth 2.0 Flow Validation (if applicable)

  ### Authorization Code Flow Testing
  - [ ] **Authorization Request**
    - Authorization URL accessible: {{auth_url_accessible}}
    - Required parameters accepted: {{auth_params_accepted}}
    - State parameter supported: {{state_param_supported}}
    - PKCE support (if required): {{pkce_support}}

  - [ ] **Authorization Response**
    - Authorization code received: {{auth_code_received}}
    - State parameter validated: {{state_validated}}
    - Error handling tested: {{error_handling_tested}}

  - [ ] **Token Exchange**
    - Token endpoint accessible: {{token_endpoint_accessible}}
    - Authorization code exchange successful: {{code_exchange_successful}}
    - Access token received: {{access_token_received}}
    - Refresh token received: {{refresh_token_received}}
    - Token expiration documented: {{token_expiration}}

  ### Client Credentials Flow Testing
  - [ ] **Direct Token Request**
    - Client credentials accepted: {{credentials_accepted}}
    - Scope parameter supported: {{scope_supported}}
    - Token response received: {{token_response_received}}
    - Token format validated: {{token_format_validated}}

  ### Token Management
  - [ ] **Access Token Validation**
    - Token format: {{token_format}} (JWT, opaque, etc.)
    - Token expiration time: {{token_expiration_time}}
    - Token scope verification: {{token_scope_verification}}
    - Token introspection endpoint: {{token_introspection}}

  - [ ] **Refresh Token Testing**
    - Refresh token functionality: {{refresh_functionality}}
    - Refresh grant type supported: {{refresh_grant_supported}}
    - New access token issued: {{new_token_issued}}
    - Refresh token rotation: {{refresh_token_rotation}}

  ## SMART on FHIR Validation (if applicable)

  ### SMART App Launch
  - [ ] **Launch Context**
    - EHR launch supported: {{ehr_launch_supported}}
    - Standalone launch supported: {{standalone_launch_supported}}
    - Launch parameters handled: {{launch_params_handled}}

  - [ ] **Scope Management**
    - Patient scope support: {{patient_scope_support}}
    - User scope support: {{user_scope_support}}
    - System scope support: {{system_scope_support}}
    - Custom scopes available: {{custom_scopes}}

  - [ ] **Authorization Server Metadata**
    - Well-known endpoint accessible: {{wellknown_accessible}}
    - Capability statement available: {{capability_statement}}
    - Supported grant types documented: {{grant_types_documented}}

  ## API Key Authentication Validation (if applicable)

  ### API Key Testing
  - [ ] **Key Configuration**
    - API key received: {{api_key_received}}
    - Key format validated: {{key_format_validated}}
    - Key placement tested: {{key_placement_tested}}
    - Key rotation process documented: {{key_rotation_process}}

  - [ ] **Request Authentication**
    - Header authentication successful: {{header_auth_successful}}
    - Query parameter authentication: {{query_auth_successful}}
    - Request signing required: {{request_signing_required}}

  ## Security Validation

  ### Transport Security
  - [ ] **HTTPS Requirements**
    - All endpoints use HTTPS: {{https_all_endpoints}}
    - TLS version requirements: {{tls_version_requirements}}
    - Certificate validation performed: {{cert_validation}}
    - HSTS headers present: {{hsts_headers}}

  - [ ] **Request Security**
    - Request signing implemented: {{request_signing}}
    - Timestamp validation: {{timestamp_validation}}
    - Nonce handling: {{nonce_handling}}
    - Replay attack protection: {{replay_protection}}

  ### Error Handling Security
  - [ ] **Authentication Errors**
    - Invalid credentials handling: {{invalid_creds_handling}}
    - Expired token handling: {{expired_token_handling}}
    - Rate limiting on auth endpoints: {{auth_rate_limiting}}
    - Error message security: {{error_message_security}}

  ## Rate Limiting and Performance

  ### Authentication Rate Limits
  - [ ] **Token Endpoint Limits**
    - Rate limits documented: {{token_rate_limits}}
    - Rate limit headers present: {{rate_limit_headers}}
    - Burst allowances: {{burst_allowances}}
    - Rate limit exceeded behavior: {{rate_limit_exceeded}}

  ### Performance Characteristics
  - [ ] **Response Times**
    - Authentication response time: {{auth_response_time}}ms
    - Token validation time: {{token_validation_time}}ms
    - Refresh token time: {{refresh_token_time}}ms
    - Performance under load: {{performance_under_load}}

  ## Compliance and Audit

  ### Audit Logging
  - [ ] **Authentication Audit**
    - Login attempts logged: {{login_attempts_logged}}
    - Failed authentication logged: {{failed_auth_logged}}
    - Token usage tracked: {{token_usage_tracked}}
    - Audit log format: {{audit_log_format}}

  ### Compliance Requirements
  - [ ] **Healthcare Compliance**
    - HIPAA compliance verified: {{hipaa_compliance}}
    - User consent handling: {{user_consent_handling}}
    - Patient authorization: {{patient_authorization_verified}}
    - Data access controls: {{data_access_controls}}

  ## Testing Results Summary

  ### Successful Authentication Tests
  {{successful_auth_tests}}

  ### Failed Authentication Tests
  {{failed_auth_tests}}

  ### Security Issues Identified
  {{security_issues_identified}}

  ### Performance Issues
  {{performance_issues}}

  ## Authentication Flow Documentation

  ### Working Authentication Flow
  ```
  {{working_auth_flow}}
  ```

  ### Required Headers
  ```
  {{required_headers}}
  ```

  ### Sample Requests
  **Token Request:**
  ```
  {{sample_token_request}}
  ```

  **Authenticated API Request:**
  ```
  {{sample_authenticated_request}}
  ```

  ## Recommendations

  ### Security Recommendations
  {{security_recommendations}}

  ### Implementation Recommendations
  {{implementation_recommendations}}

  ### Operational Recommendations
  {{operational_recommendations}}

  ## Integration Partner Profile Updates

  ### Authentication Section Updates
  - **Confirmed Authentication Method**: {{confirmed_auth_method}}
  - **Working Endpoints**: {{working_endpoints}}
  - **Token Specifications**: {{token_specifications}}
  - **Security Requirements**: {{security_requirements}}
  - **Performance Characteristics**: {{performance_characteristics}}

  ## Next Steps

  ### Immediate Actions
  - [ ] Update Integration Partner Profile with confirmed authentication details
  - [ ] Document any authentication quirks or non-standard implementations
  - [ ] Prepare authentication implementation plan
  - [ ] Schedule security review if needed

  ### Follow-up Testing
  - [ ] Test authentication in production environment (when ready)
  - [ ] Validate authentication under higher load conditions
  - [ ] Test token refresh and expiration scenarios
  - [ ] Verify ongoing compliance requirements

  ## Troubleshooting Guide

  ### Common Issues and Solutions
  {{troubleshooting_guide}}

  ### Vendor Contact Information
  - **Authentication Support**: {{auth_support_contact}}
  - **Technical Issues**: {{technical_support_contact}}
  - **Security Questions**: {{security_contact}}

  ## Notes and Additional Information
  {{additional_notes}}

usage_instructions: |
  This checklist provides comprehensive validation of authentication flows and security
  requirements for healthcare system APIs.

  Complete each section based on the authentication method identified for the target system.
  Document all test results and update the Integration Partner Profile with confirmed
  authentication details.

  Pay special attention to security requirements and compliance considerations for
  healthcare data access.