# Section 3 of 12: Tech Stack

This section defines the definitive technology stack for both the expansion pack itself and the integration services it will generate. These choices are derived directly from the PRD's technical assumptions and are considered the single source of truth for all development. All AI agents will be constrained to use these specific technologies and versions.

| Category                | Technology         | Version    | Purpose                                               | Rationale                                                                                 |
|-------------------------|-------------------|------------|-------------------------------------------------------|-------------------------------------------------------------------------------------------|
| Language (Generated Code) | C#                | 12         | Primary language for generated integration services.   | Directly aligns with the Firely .NET SDK, providing seamless integration and strong typing for healthcare data models. |
| Runtime (Generated Code)  | .NET              | 8.0 (LTS)  | Executes the generated C# code.                       | Current Long-Term Support (LTS) version, ensuring long-term support, performance, and stability. |
| Framework (Generated Code)| ASP.NET Core      | 8.0        | To build the web APIs for the integration services.    | The standard, high-performance framework for building web APIs with C#/.NET.              |
| Core SDK (Toolchain)      | Firely .NET SDK   | 5.x        | Core library for parsing, validating, and interacting with FHIR resources. | A non-negotiable core dependency defined by the project's goals.                          |
| Dev Tools (Toolchain)     | Forge, Simplifier.net, Firely Terminal | Latest | Profiling, collaboration, and validation.              | The core external tools the expansion pack is designed to orchestrate.                    |
| API Standard              | HL7 FHIR          | R4         | The data interchange standard.                        | The specified version for healthcare interoperability.                                    |
| Authorization Standard    | SMART on FHIR     | v2         | Secure, user-consented access to clinical data.       | The industry standard for FHIR app authorization.                                         |
| Pack Definition Language  | Markdown, YAML    | N/A        | Defining agents, workflows, tasks, and templates.     | The standard for the BMad framework.                                                      |

---
