# implement-data-mapping

**Task Type**: Implementation
**Agent**: FHIR Interoperability Specialist
**Complexity**: High
**Estimated Duration**: 3-4 hours

## Purpose

Guide the user through implementing C# data mapping and transformation logic that converts vendor-specific data formats to FHIR-compliant resources, handling documented data quirks and ensuring proper validation.

## Prerequisites

- Integration Partner Profile document completed (contains vendor data model specifications)
- FHIR StructureDefinition files for target FHIR resources
- .NET 8.0 project with Firely .NET SDK 5.x configured
- Basic C# service structure established

## Inputs

1. **Integration Partner Profile**: Document containing:
   - Vendor data model specifications
   - Field mappings to FHIR elements
   - Documented data quirks and anomalies
   - Sample vendor data payloads

2. **FHIR StructureDefinition**: Defines the target FHIR resource structure and constraints

3. **Vendor Data Models**: Specific data formats from the target EHR system

## Outputs

1. **IDataMappingService Interface**: Comprehensive interface for data transformation operations
2. **DataMappingService Implementation**: Concrete implementation with vendor-specific logic
3. **Data Quirks Handling**: Specialized logic for vendor-specific data anomalies
4. **Unit Tests**: Comprehensive test suite covering all mapping scenarios
5. **Configuration Classes**: Mapping configuration and validation rules

## Workflow Steps

### Step 1: Analyze Integration Partner Profile
- Review vendor data model specifications
- Identify field mappings to FHIR elements
- Document data quirks and transformation requirements
- Note validation constraints and business rules

### Step 2: Implement Core Data Mapping Service
- Create DataMappingService class implementing IDataMappingService
- Implement TransformPatientAsync method with field-by-field mapping
- Implement TransformObservationAsync method for clinical data
- Add vendor data validation methods
- Implement FHIR resource validation using Firely SDK

### Step 3: Implement Data Quirks Handling
- Create HandleDataQuirkAsync method for vendor-specific anomalies
- Implement date format normalization (vendor formats → FHIR dateTime)
- Add gender code transformation (vendor codes → FHIR administrative-gender)
- Handle phone number format standardization
- Implement address component standardization
- Add insurance/coverage code mapping to standard terminologies
- Create null/empty value handling logic

### Step 4: Add Mapping Configuration
- Create mapping configuration classes for vendor data models
- Implement data type conversion helpers with type safety
- Add structured logging for transformation tracking (no PHI)
- Create mapping result objects with success/failure states
- Add validation metrics and transformation statistics

### Step 5: Implement Error Handling
- Create custom exception hierarchy:
  - DataMappingException (base exception)
  - VendorDataValidationException (invalid input data)
  - FhirResourceCreationException (FHIR resource assembly failures)
  - DataQuirkHandlingException (vendor-specific transformation failures)
- Add comprehensive error logging (PHI-safe)
- Implement graceful degradation for non-critical field failures
- Add retry logic for transient transformation issues

### Step 6: Generate Unit Tests
- Create DataMappingServiceTests.cs in test project
- Add test cases for all required field mappings
- Create test cases for data quirks handling scenarios
- Add test cases for error scenarios and edge cases
- Implement validation tests for FHIR resource compliance
- Add performance tests for transformation operations

### Step 7: Integration Testing
- Test with sample vendor data from Integration Partner Profile
- Validate FHIR resource output against StructureDefinition
- Verify all documented data quirks are handled correctly
- Test error scenarios and edge cases
- Validate logging and metrics collection

## Technical Requirements

### Coding Standards
- **Language**: C# 12 targeting .NET 8.0 runtime
- **Naming**: PascalCase for classes/methods, camelCase for variables, _camelCase for private fields
- **NO PHI IN LOGS**: Critical healthcare compliance requirement
- **Async Operations**: All I/O-bound operations must be async
- **Input Validation**: Validate all vendor data at service boundaries
- **Custom Exceptions**: Use specific custom exceptions for business logic failures

### Dependencies
- Firely .NET SDK 5.x for FHIR resource operations
- Microsoft.Extensions.Logging for structured logging
- Polly for resilience patterns (external API calls)
- System.Text.Json for JSON data handling

### Performance Requirements
- Transform single patient record: < 100ms
- Transform batch of 100 records: < 5 seconds
- Memory usage: < 50MB for typical transformation operations
- Support concurrent transformations

## Validation Criteria

### Functional Validation
- [ ] All vendor data fields correctly mapped to FHIR elements
- [ ] All documented data quirks handled appropriately
- [ ] Generated FHIR resources validate against StructureDefinition
- [ ] Error scenarios handled gracefully with appropriate exceptions
- [ ] Null and empty values handled consistently

### Technical Validation
- [ ] Follows C# 12/.NET 8.0 coding standards
- [ ] No PHI exposed in logs or error messages
- [ ] All operations properly async/await
- [ ] Comprehensive unit test coverage (>90%)
- [ ] Performance requirements met

### Quality Validation
- [ ] Code follows SOLID principles
- [ ] Proper separation of concerns
- [ ] Comprehensive error handling
- [ ] Structured logging with appropriate levels
- [ ] Documentation and code comments

## Common Issues and Solutions

### Issue: Date Format Variations
**Solution**: Implement flexible date parsing with multiple format attempts, fallback to null for invalid dates with warning logging.

### Issue: Vendor Code Mapping
**Solution**: Create translation dictionaries for vendor-specific codes to FHIR valuesets, with extension fallback for unknown codes.

### Issue: Performance with Large Datasets
**Solution**: Implement batch processing, streaming for large datasets, and optimize FHIR resource creation patterns.

### Issue: FHIR Validation Failures
**Solution**: Implement progressive validation - validate core fields first, handle optional fields gracefully, provide detailed validation feedback.

## Follow-up Tasks

After completion, consider:
- Performance optimization based on real-world usage patterns
- Additional vendor data format support
- Enhanced metrics and monitoring
- Integration with external terminology services
- Automated testing with production-like data volumes

## Notes

- Maintain strict PHI compliance throughout implementation
- Use Integration Partner Profile as authoritative source for vendor requirements
- Prioritize data quality and FHIR compliance over transformation speed
- Document all assumptions and design decisions for future maintenance