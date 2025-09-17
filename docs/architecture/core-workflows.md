# Section 7 of 12: Core Workflows

This section visualizes the primary operational sequences of the expansion pack using Mermaid sequence diagrams. These diagrams illustrate the step-by-step collaboration between the user and the specialized AI agents to achieve the project's key objectives.

## Workflow 1: Specification & Profiling

This workflow details the process of creating a new, clinically validated FHIR profile.

```mermaid
sequenceDiagram
    participant User
    participant Clinical Informaticist
    participant FHIR Specialist
    participant Forge
    participant Simplifier.net

    User->>+Clinical Informaticist: Activate to define clinical needs
    Clinical Informaticist-->>User: Guide data mapping & terminology selection
    User-->>Clinical Informaticist: Provide requirements
    Clinical Informaticist-->>User: Produce clinical-requirements.md
    deactivate Clinical Informaticist

    User->>+FHIR Specialist: Activate with requirements doc
    FHIR Specialist-->>User: Guide profile creation in Forge
    User->>Forge: Create/constrain profile
    Forge-->>User: Produce StructureDefinition file
    
    FHIR Specialist-->>User: Guide publication to Simplifier.net
    User->>Simplifier.net: Publish StructureDefinition
    Simplifier.net-->>User: Confirm publication
    
    FHIR Specialist-->>User: Prompt for clinical review
    deactivate FHIR Specialist

    User->>+Clinical Informaticist: Activate for profile review
    Clinical Informaticist-->>User: Guide review on Simplifier.net
    User-->>Clinical Informaticist: Provide final approval
    Clinical Informaticist-->>User: Confirm profile is validated
    deactivate Clinical Informaticist
```

## Workflow 2: Integration Research

This workflow shows how the Integration Analyst researches a target vendor system.

```mermaid
sequenceDiagram
    participant User
    participant Integration Analyst
    participant Postman
    participant Vendor API

    User->>+Integration Analyst: Activate with target system name (e.g., "Epic")
    Integration Analyst-->>User: Guide initial scoping (find docs, contacts)
    User-->>Integration Analyst: Provide research findings
    Integration Analyst-->>User: Produce partial Integration Partner Profile

    Integration Analyst-->>User: Guide API endpoint & auth testing
    User->>Postman: Make API calls to Vendor API
    Postman->>Vendor API: Request data
    Vendor API-->>Postman: Return response
    Postman-->>User: Display payload

    Integration Analyst-->>User: Guide documentation of results & quirks
    User-->>Integration Analyst: Provide analysis of payload
    Integration Analyst-->>User: Produce completed Integration Partner Profile
    deactivate Integration Analyst
```

## Workflow 3: Development and Validation

This workflow visualizes the end-to-end process of generating, implementing, and validating the integration service code.

```mermaid
sequenceDiagram
    participant User
    participant FHIR Specialist
    participant Security Analyst
    participant C#/.NET Service
    participant Firely Terminal

    User->>+FHIR Specialist: Activate with profiles & research docs
    FHIR Specialist-->>User: Guide C#/.NET project scaffolding
    User-->>C#/.NET Service: Create base project structure
    FHIR Specialist-->>User: Guide data mapping logic implementation
    User-->>C#/.NET Service: Write mapping code
    FHIR Specialist-->>User: Guide API endpoint implementation
    User-->>C#/.NET Service: Write endpoint code
    deactivate FHIR Specialist

    User->>+FHIR Specialist: Activate to create validation suite
    FHIR Specialist-->>User: Provide automated test script
    User->>Firely Terminal: Execute validation against C#/.NET Service
    Firely Terminal-->>User: Report validation results
    deactivate FHIR Specialist

    User->>+Security Analyst: Activate for compliance review
    Security Analyst-->>User: Guide security & HIPAA checklist
    User-->>Security Analyst: Confirm compliance checks
    Security Analyst-->>User: Produce completed Security Checklist
    deactivate Security Analyst
```

---
