# Section 6 of 8: Epic 2 Specification & Profiling Workflow

## Epic Goal

This epic implements the first core capability of the expansion pack: guiding a user through the creation of a FHIR specification. It will activate the Clinical Informaticist and FHIR Interoperability Specialist agents to perform their defined tasks, resulting in a validated, human-readable FHIR profile published to Simplifier.net. This delivers the pack's primary 'design and document' value.

---

### Story 2.1: Elicit and Document Clinical Requirements

**As a BMad user,**  
I want to use the Clinical Informaticist agent to perform data mapping and terminology management,  
so that I can produce a clear requirements document for a new FHIR profile.

**Acceptance Criteria:**

- The Clinical Informaticist agent has a `document-clinical-requirements` task.
- The task interactively guides the user to map a clinical data element to a FHIR resource and select appropriate medical terminology (e.g., LOINC, SNOMED codes).
- The task's output is a structured markdown file (`clinical-requirements.md`) that clearly defines the constraints and value sets for the profile.

---

### Story 2.2: Guide FHIR Profile Creation in Forge

**As a BMad user,**  
I want the FHIR Interoperability Specialist agent to provide a step-by-step guide for creating a profile in Forge,  
so that I can accurately translate the clinical requirements into a technical FHIR artifact.

**Acceptance Criteria:**

- The FHIR Interoperability Specialist agent has a `create-profile-in-forge` task that accepts a `clinical-requirements.md` file as input.
- The agent provides clear, sequential instructions on how to use Forge to constrain a base FHIR resource according to the requirements.
- The guidance includes instructions on how to use Forge's built-in validation features.
- The final output of the process is a valid FHIR StructureDefinition file.

---

### Story 2.3: Guide Profile Publication and Review

**As a BMad user,**  
I want agents to guide me through publishing the new profile to Simplifier.net and performing a collaborative review,  
so that the profile is centrally documented, versioned, and clinically validated.

**Acceptance Criteria:**

- The FHIR Interoperability Specialist agent has a `publish-to-simplifier` task that guides the user on uploading the StructureDefinition file.
- The task prompts the user to engage the Clinical Informaticist agent for review once the profile is published.
- The Clinical Informaticist agent has a `review-simplifier-profile` task that guides the user on how to interpret the human-readable view on Simplifier.net.
- The review task concludes with a formal approval step, confirming the profile meets the clinical requirements.

---
