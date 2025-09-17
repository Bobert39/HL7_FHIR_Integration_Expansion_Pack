# Section 4 of 12: Data Models

These are the conceptual data models that define the structure of the expansion pack itself. They are not database schemas but rather the high-level information structures that the BMad framework uses to understand and operate the agents.

## AgentConfiguration (.md)

- **Purpose:** To define the persona, capabilities, and dependencies of a single AI agent. This model corresponds directly to an agent's markdown file.
- **Key Attributes:**
  - `id`: string - The unique identifier for the agent (e.g., "clinical-informaticist").
  - `name`: string - The human-friendly name of the agent.
  - `persona`: object - A structured object containing the agent's role, style, and core principles.
  - `dependencies`: object - Lists of tasks, templates, and workflows the agent is authorized to use.

## WorkflowDefinition (.yaml)

- **Purpose:** To define a multi-step, sequential process that an agent can guide a user through.
- **Key Attributes:**
  - `id`: string - The unique identifier for the workflow (e.g., "specification-workflow").
  - `name`: string - The human-friendly name of the workflow.
  - `sequence`: array - An ordered list of steps, defining the agents and tasks involved in the process.

## TaskDefinition (.md)

- **Purpose:** To define a single, atomic, and repeatable action an agent can perform.
- **Key Attributes:**
  - `id`: string - The unique identifier for the task (e.g., "create-fhir-profile").
  - `description`: string - A detailed description of the task's objective and method.
  - `inputs`: object - The required inputs for the task (e.g., a clinical requirements document).
  - `outputs`: object - The expected artifacts produced by the task (e.g., a FHIR StructureDefinition file).

## TemplateDefinition (.yaml or .md)

- **Purpose:** To define a reusable, standardized artifact, such as a document or a piece of code, that agents can use to generate consistent outputs.
- **Key Attributes:**
  - `id`: string - The unique identifier for the template.
  - `format`: string - The output format (e.g., "markdown", "csharp").
  - `structure`: object - The content and placeholder variables for the template.

---
