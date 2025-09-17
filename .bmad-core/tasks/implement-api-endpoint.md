# Task: Implement API Endpoint with Live FHIR Validation

## Overview
This task guides the implementation of complete API endpoints that integrate vendor APIs, apply data mapping transformations, and validate FHIR resources before returning compliant responses.

## Inputs Required
- **Mapping Service**: IDataMappingService implementation from previous story
- **FHIR Profiles**: Project-specific FHIR profiles for validation
- **Vendor API Specifications**: Integration Partner Profile with endpoint details
- **Authentication Configuration**: SMART on FHIR v2 setup requirements

## Outputs Generated
- **Complete API Controller**: ASP.NET Core controller with FHIR endpoints
- **Vendor API Client**: HTTP client service for external data retrieval
- **Validation Service**: FHIR validation using Firely SDK
- **Authentication Service**: SMART on FHIR v2 implementation
- **Error Handling**: Structured error responses and OperationOutcome
- **Integration Tests**: Comprehensive test coverage

## Implementation Workflow

### Step 1: Create API Controller Foundation
- Generate PatientController with GET /Patient/{id} endpoint
- Configure dependency injection for required services
- Implement async endpoint methods following coding standards
- Add proper HTTP client configuration with Polly resilience policies

### Step 2: Integrate Vendor API Client
- Create vendor-specific API client service
- Implement HTTP client with timeout configuration (30s vendor API, 10s auth)
- Add Polly resilience patterns (retry 3x, circuit breaker)
- Handle vendor API authentication and data retrieval

### Step 3: Apply Data Transformation
- Inject IDataMappingService into controller
- Call mapping service to transform vendor data to FHIR resources
- Handle mapping exceptions and transformation errors
- Add comprehensive logging for transformation tracking (no PHI)

### Step 4: Implement FHIR Validation
- Configure Firely SDK validation services
- Integrate profile-based validation before response
- Add validation error handling and detailed error messages
- Generate OperationOutcome for validation failures

### Step 5: Implement Authentication
- Create OAuth 2.0 client configuration for SMART authorization
- Implement JWT token validation middleware
- Add scope-based authorization for FHIR resources (patient/*.read, patient/*.write)
- Generate authentication service for token acquisition and refresh

### Step 6: Create Error Handling
- Implement structured error response models
- Add appropriate HTTP status codes for different error types
- Include detailed validation error information
- Ensure no PHI appears in error logs

### Step 7: Generate Integration Tests
- Create PatientControllerTests.cs with comprehensive test coverage
- Test successful FHIR resource retrieval and validation
- Test vendor API failure scenarios and mapping errors
- Test authentication and authorization scenarios

## Technical Requirements

### Coding Standards Compliance
- **Language**: C# 12 targeting .NET 8.0 runtime
- **Framework**: ASP.NET Core 8.0 for web API functionality
- **Naming**: PascalCase for classes/methods, camelCase for variables
- **Critical Rules**:
  - NO PHI IN LOGS (absolutely critical)
  - USE RESILIENCY PATTERNS (Polly for vendor API calls)
  - VALIDATE ALL INPUTS (API request validation)
  - USE CUSTOM EXCEPTIONS (FhirValidationException, VendorApiException)
  - ASYNC EVERYWHERE (async operations for I/O-bound calls)

### Performance Targets
- API Response Time: <500ms for simple Patient resource retrieval
- Transformation Performance: <100ms for single resource mapping
- Concurrent Users: Support minimum 50 concurrent requests
- Memory Usage: <200MB baseline, <500MB under load
- Validation Performance: <50ms for FHIR resource validation

### Security Requirements
- SMART on FHIR v2 compliance
- JWT token validation with vendor public keys
- Scope-based authorization enforcement
- Secure token storage with encryption at rest
- Proper CORS, CSP, and HSTS headers

## Validation Checklist
- [ ] API controller implements all required endpoints
- [ ] Vendor API client has proper resilience patterns
- [ ] Data mapping service integration works correctly
- [ ] FHIR validation using Firely SDK is functional
- [ ] SMART on FHIR v2 authentication is implemented
- [ ] Structured error responses are generated
- [ ] All integration tests pass
- [ ] No PHI appears in logs
- [ ] Performance targets are met
- [ ] Security requirements are satisfied