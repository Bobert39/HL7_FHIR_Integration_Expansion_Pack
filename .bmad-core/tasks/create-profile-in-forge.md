# Create FHIR Profile in Forge Task

## Purpose
Guide users through the step-by-step process of creating a FHIR profile in Forge tool, translating clinical requirements into a technical FHIR StructureDefinition artifact.

## Task Definition
- **ID**: create-profile-in-forge
- **Description**: Provide comprehensive guidance for using Forge to constrain base FHIR resources according to clinical requirements and generate valid StructureDefinition files
- **Assigned Agent**: fhir-interoperability-specialist
- **Estimated Duration**: 2-4 hours per profile
- **Prerequisites**: Clinical requirements document (clinical-requirements.md) from Clinical Informaticist

## Inputs
- `clinical-requirements.md` file from Clinical Informaticist agent
- Base FHIR resource selection (Patient, Observation, DiagnosticReport, etc.)
- Target FHIR version (R4 by default)
- Implementation context and constraints
- Terminology system requirements (LOINC, SNOMED CT, ICD-10)

## Process Steps

### 1. Pre-Profile Planning
**1.1 Clinical Requirements Analysis**
- Review clinical-requirements.md document thoroughly
- Identify base FHIR resource type from clinical context
- Extract data elements requiring profiling constraints
- Understand clinical terminology and value set requirements

**1.2 Forge Environment Setup**
- Ensure Forge tool is installed and updated to latest version
- Verify FHIR package dependencies are available
- Configure workspace for new profile creation
- Set up validation and publishing settings

### 2. Base Resource Selection and Configuration
**2.1 Base Resource Identification**
- Open Forge and create new Implementation Guide project
- Select appropriate base FHIR resource based on clinical requirements
- Review base resource elements and their cardinalities
- Understand inheritance implications for profiling

**2.2 Profile Metadata Configuration**
- Set profile URL following organization conventions
- Define profile name, title, and description from clinical context
- Configure profile status (draft, active, retired)
- Set experimental flag and version numbering
- Add publisher and contact information

### 3. Element Constraint Definition
**3.1 Must Support Elements**
- Identify elements that must be supported by implementing systems
- Mark critical data elements as mustSupport in Forge
- Document rationale for mustSupport decisions
- Consider implementation burden vs. clinical value

**3.2 Cardinality Constraints**
- Review default cardinalities from base resource
- Apply tighter constraints based on clinical requirements
- Set required elements (1..1, 1..*) for essential data
- Prohibit elements (0..0) that should not be used

**3.3 Data Type Refinements**
- Constrain complex data types to specific patterns
- Apply slice definitions for repeating elements
- Configure choice types to specific allowed types
- Set fixed values for invariant elements

### 4. Terminology Binding and Value Sets
**4.1 Value Set Configuration**
- Create or reference existing value sets for coded elements
- Configure binding strength (required, extensible, preferred, example)
- Document terminology system selection rationale
- Validate code system URIs and versions

**4.2 Code System Integration**
- Reference standard code systems (LOINC, SNOMED CT, ICD-10)
- Configure local code systems if needed
- Set up concept maps for terminology translations
- Validate terminology server connectivity

### 5. Invariant and Extension Definition
**5.1 Business Rules Implementation**
- Define FHIRPath invariants for complex business rules
- Implement cross-element validation constraints
- Test invariant expressions for accuracy
- Document constraint rationale and examples

**5.2 Extension Development**
- Create extensions for data elements not in base resource
- Define extension context and cardinality
- Configure extension data types and constraints
- Document extension usage and examples

### 6. Forge Validation Features
**6.1 Built-in Validation**
- Use Forge's real-time validation feedback
- Resolve structural validation errors
- Address terminology binding warnings
- Verify invariant expression syntax

**6.2 Example Instance Creation**
- Create example instances demonstrating profile usage
- Validate examples against profile constraints
- Test edge cases and boundary conditions
- Document example scenarios and use cases

### 7. Documentation and Narrative
**7.1 Profile Documentation**
- Add comprehensive profile descriptions
- Document design decisions and rationale
- Include implementation guidance and notes
- Reference clinical requirements and use cases

**7.2 Element-Level Documentation**
- Provide short and definition text for constrained elements
- Add implementation comments and guidance
- Include mapping information to clinical concepts
- Document any deviations from base resource

### 8. Quality Assurance and Testing
**8.1 Validation Suite Execution**
- Run complete profile validation in Forge
- Test against FHIR validation rules
- Verify terminology binding resolution
- Check invariant expression execution

**8.2 Implementation Testing**
- Create test data conforming to profile
- Validate test instances against constraints
- Test with FHIR validation tools (Java validator, .NET SDK)
- Verify interoperability with target systems

## Outputs
- Valid FHIR StructureDefinition file (.json format)
- Profile documentation and implementation notes
- Example instances demonstrating profile usage
- Validation report confirming FHIR compliance
- Forge project file for future modifications
- Technical implementation guide sections

## Quality Gates
- Profile validates successfully in Forge without errors
- All clinical requirements mapped to profile constraints
- Terminology bindings resolve correctly
- Example instances validate against profile
- StructureDefinition conforms to FHIR R4 specification
- Profile is ready for publication to Simplifier.net

## Forge-Specific Features Utilization

### Core Forge Capabilities
- **Profile Editor**: Visual constraint definition and inheritance management
- **Element Inspector**: Detailed element configuration and documentation
- **Validation Engine**: Real-time validation feedback and error resolution
- **Example Editor**: Instance creation and validation testing
- **Package Management**: FHIR package dependency handling

### Advanced Forge Features
- **Slice Editor**: Complex slicing configuration for repeating elements
- **Extension Designer**: Custom extension creation and management
- **Invariant Editor**: FHIRPath expression development and testing
- **Mapping Editor**: Clinical concept to FHIR element mapping
- **Publication Tools**: StructureDefinition export and sharing

### Validation and Testing Tools
- **Structure Validation**: FHIR conformance and constraint checking
- **Terminology Validation**: Code system and value set verification
- **Instance Validation**: Example testing against profile constraints
- **Dependency Checking**: FHIR package and profile dependency validation

## Troubleshooting Common Issues

### Validation Errors
- **Cardinality Conflicts**: Review inherited constraints vs. applied constraints
- **Terminology Errors**: Verify code system URIs and binding strength
- **Invariant Failures**: Test FHIRPath expressions with example data
- **Extension Issues**: Check extension context and cardinality settings

### Performance Optimization
- **Large Profiles**: Use slicing judiciously to maintain performance
- **Complex Invariants**: Optimize FHIRPath expressions for efficiency
- **Terminology Loading**: Cache frequently used value sets locally
- **Validation Speed**: Use incremental validation during development

## Success Criteria
- Complete translation of clinical requirements to FHIR profile constraints
- Valid StructureDefinition file generated and exported
- All Forge validation checks pass without errors or warnings
- Profile ready for publication to Simplifier.net registry
- Implementation guidance documented for consuming systems
- Clinical stakeholder review and approval process initiated

## Integration Points
- **Input from Clinical Informaticist**: Clinical requirements and terminology guidance
- **Output to Publication Workflow**: StructureDefinition ready for Simplifier.net
- **Feedback to Clinical Review**: Profile available for clinical validation
- **Implementation Support**: Technical guidance for system integration