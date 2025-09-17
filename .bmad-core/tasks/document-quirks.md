# document-quirks

## Task Configuration

```yaml
id: document-quirks
name: Document Data Model and Identify Quirks
description: Analyze sample API response payloads to document the vendor system's data model, identify FHIR resource mappings, and capture non-standard behaviors for development team awareness
agent: healthcare-system-integration-analyst
category: technical-analysis
complexity: high
estimated-time: 3-4 hours
dependencies:
  - initial-scoping.md
  - technical-research.md
templates:
  - data-model-analysis.tmpl.md
  - fhir-mapping-guide.tmpl.md
  - data-quirks-documentation.tmpl.md
  - completed-integration-profile.tmpl.md
inputs:
  - Enhanced Integration Partner Profile from technical-research task
  - Sample API response payloads (JSON/XML) from vendor system
  - Vendor API documentation for reference
outputs:
  - Completed Integration Partner Profile with comprehensive data model documentation
  - FHIR resource mapping specifications
  - Data quirks and non-standard behavior documentation
elicit: true
```

## Task Workflow

### Phase 1: Preparation and Context Gathering
1. **Load Enhanced Integration Partner Profile**
   - Review existing vendor documentation from initial-scoping and technical-research
   - Understand API authentication and endpoint configurations
   - Note any previously identified data format specifications

2. **Gather Sample API Response Payloads**
   - Request user to provide sample API responses (JSON/XML)
   - If not available, guide user to make test API calls using documented endpoints
   - Ensure samples cover key data types: Patient, Encounter, Observation, Medication, etc.

### Phase 2: Data Model Analysis

3. **Parse and Analyze Sample Payloads**
   - Use data-model-analysis.tmpl.md template
   - Document structure of each data type/resource
   - Identify all fields, data types, and relationships
   - Note any nested objects or arrays
   - Capture field naming conventions and patterns

4. **Document Data Field Inventory**
   ```markdown
   ## Data Model Documentation

   ### Patient Resource Structure
   - Field Name | Data Type | Required | Example Value | Notes

   ### Encounter Resource Structure
   - Field Name | Data Type | Required | Example Value | Notes

   ### Observation Resource Structure
   - Field Name | Data Type | Required | Example Value | Notes
   ```

### Phase 3: FHIR Mapping Identification

5. **Map Vendor Fields to FHIR Resources**
   - Use fhir-mapping-guide.tmpl.md template
   - For each vendor field, identify corresponding FHIR element
   - Document mapping confidence levels (Direct, Transform Required, Complex, Unclear)
   - Note any fields that don't have clear FHIR equivalents

6. **Create FHIR Mapping Specification**
   ```markdown
   ## FHIR Mapping Specifications

   ### Patient Resource Mappings
   | Vendor Field | FHIR Element | Mapping Type | Transformation Notes |
   |--------------|--------------|--------------|----------------------|
   | patient_id   | Patient.identifier | Direct | Use system: "vendor-system" |
   | first_name   | Patient.name.given | Direct | - |
   | last_name    | Patient.name.family | Direct | - |
   | dob          | Patient.birthDate | Transform | Convert MM/DD/YYYY to YYYY-MM-DD |

   ### Encounter Resource Mappings
   [Similar structure]

   ### Observation Resource Mappings
   [Similar structure]
   ```

### Phase 4: Non-Standard Behavior Documentation

7. **Identify Data Quirks and Anomalies**
   - Use data-quirks-documentation.tmpl.md template
   - Document non-standard date/time formats
   - Identify custom code systems or value sets
   - Note unusual null/empty value representations
   - Capture any data validation rules or constraints
   - Document character encoding issues or special characters

8. **Document Vendor-Specific Behaviors**
   ```markdown
   ## Data Quirks and Non-Standard Behaviors

   ### Date/Time Handling
   - Issue: Dates use non-standard format "MM/DD/YYYY HH:MM AM/PM"
   - Impact: Requires custom parsing before FHIR conversion
   - Solution: Implement DateTimeParser utility class

   ### Custom Code Systems
   - Issue: Vendor uses proprietary codes for gender ("M", "F", "U")
   - Impact: Requires mapping to FHIR AdministrativeGender value set
   - Solution: Create GenderCodeMapper with lookup table

   ### Null Value Handling
   - Issue: Empty strings used instead of null values
   - Impact: May cause validation errors in FHIR resources
   - Solution: Pre-process data to convert empty strings to null

   ### Special Characters
   - Issue: Names may contain special characters not properly escaped
   - Impact: JSON parsing errors possible
   - Solution: Implement character encoding sanitization
   ```

### Phase 5: Performance and Reliability Considerations

9. **Document API Response Characteristics**
   - Average response times for different endpoints
   - Maximum payload sizes observed
   - Rate limiting behaviors
   - Error response formats and codes
   - Retry recommendations

10. **Identify Integration Risks**
    ```markdown
    ## Integration Risks and Mitigation

    ### Data Quality Risks
    - Risk: Inconsistent data formats across different API versions
    - Mitigation: Implement version detection and adaptive parsing

    ### Performance Risks
    - Risk: Large payload sizes (>10MB) for bulk data requests
    - Mitigation: Implement pagination and streaming processing

    ### Reliability Risks
    - Risk: API timeout issues during peak hours
    - Mitigation: Implement exponential backoff retry logic
    ```

### Phase 6: Integration Partner Profile Completion

11. **Compile Comprehensive Documentation**
    - Use completed-integration-profile.tmpl.md template
    - Merge all findings into final Integration Partner Profile
    - Ensure all sections are complete and validated
    - Add executive summary of key findings and recommendations

12. **Create Development Team Handoff Package**
    ```markdown
    ## Development Team Handoff

    ### Priority Implementation Notes
    1. Custom date/time parser required (see Section X)
    2. Gender code mapping table needed (see Section Y)
    3. Special character handling in all string fields
    4. Implement retry logic with exponential backoff

    ### FHIR Resource Priority
    1. Patient (core resource, most mappings validated)
    2. Encounter (complex mappings, needs review)
    3. Observation (custom code systems require attention)

    ### Testing Recommendations
    - Use provided sample payloads for unit testing
    - Implement edge case testing for identified quirks
    - Performance test with large payloads
    ```

## User Interaction Points

### Required User Inputs
1. **Sample API Response Payloads**
   - "Please provide sample API response payloads in JSON or XML format"
   - "Include responses for Patient, Encounter, and Observation if available"

2. **Vendor Documentation Clarification**
   - "Do you have any additional vendor documentation about data formats?"
   - "Are there known issues or quirks the vendor has communicated?"

3. **Business Rule Validation**
   - "Please confirm the following FHIR mapping is correct: [mapping]"
   - "How should we handle [specific quirk]?"

### Validation Checkpoints
- After Phase 2: "Please review the data model documentation for accuracy"
- After Phase 3: "Please validate these FHIR mappings with clinical team if needed"
- After Phase 4: "Please confirm these quirks match your testing experience"
- Final: "Please review the completed Integration Partner Profile"

## Success Criteria
- [ ] All sample payloads analyzed and documented
- [ ] FHIR mappings identified for all critical data fields
- [ ] Non-standard behaviors comprehensively documented
- [ ] Integration Partner Profile completed with all sections
- [ ] Development team handoff package created
- [ ] All findings validated with user/vendor documentation

## Error Handling
- If sample payloads unavailable: Guide user through API testing to obtain them
- If FHIR mapping unclear: Document as "Requires Clinical Review" with detailed notes
- If quirk impact unknown: Flag as "High Risk" for development team attention
- If vendor documentation conflicts: Document both versions and flag for clarification