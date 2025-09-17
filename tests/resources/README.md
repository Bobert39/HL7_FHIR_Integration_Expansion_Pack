# FHIR Test Resources

This directory contains synthetic FHIR resources for validation testing. All data is synthetic and contains no Protected Health Information (PHI).

## Test Data Categories

### Valid Resources
- `patient-valid-001.json` - Valid Patient resource with complete demographics
- `observation-valid-001.json` - Valid Observation resource (vital signs)
- `practitioner-valid-001.xml` - Valid Practitioner resource in XML format

### Invalid Resources (for negative testing)
- `patient-invalid-001.json` - Patient resource with validation errors
- `observation-invalid-001.json` - Observation resource with validation errors

## Resource Types Covered
- **Patient**: Demographics, identifiers, contact information
- **Observation**: Vital signs, laboratory results
- **Practitioner**: Healthcare provider information

## Validation Testing
These resources are used by the FHIR Validation Suite to test:
- Profile-based validation
- Structure definition compliance
- Terminology validation
- Cardinality constraints
- Data type validation

## Data Generation
For larger test datasets, use the synthetic data generation scripts:
- Synthea for realistic patient population generation
- Firely .NET SDK synthetic data generators
- Custom edge case generators for boundary testing

## Performance Testing Datasets
- Small datasets (10-50 resources): Unit testing
- Medium datasets (100-500 resources): Integration testing
- Large datasets (1000+ resources): Performance validation

## Compliance
- All test data is synthetic and non-PHI compliant
- Follows FHIR R4 specification
- Aligned with project coding standards (NO PHI IN LOGS)
- Supports both JSON and XML FHIR formats