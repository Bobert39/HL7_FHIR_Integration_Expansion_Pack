# Section 12 of 12: Coding Standards

These standards are mandatory for all AI agents generating C#/.NET code for this project. They are designed to be minimal but critical, ensuring that all generated code is consistent, secure, and maintainable. The AI dev agent will be strictly constrained to follow these rules.

## Core Standards

- **Languages & Runtimes:** All code must be written in C# 12 and target the .NET 8.0 runtime.
- **Style & Linting:** Projects will include a standard `.editorconfig` file to enforce consistent formatting. AI agents must adhere to the rules defined within it.
- **Test Organization:** All test files must be located in a separate test project (e.g., `MyProject.Tests`) and the test file's name must correspond to the class it is testing (e.g., `FhirMappingService.cs` -> `FhirMappingServiceTests.cs`).

## Naming Conventions

| Element                | Convention   | Example                          |
|------------------------|-------------|----------------------------------|
| Classes, Interfaces, Enums | PascalCase   | `public class FhirMappingService` |
| Methods                | PascalCase   | `public async Task<Patient> GetPatientById()` |
| Public Properties      | PascalCase   | `public string PatientId { get; set; }` |
| Local Variables        | camelCase    | `var patientRecord = ...`        |
| Private Fields         | _camelCase   | `private readonly ILogger _logger;` |
| Interfaces             | IPascalCase  | `public interface IFhirService`  |

## Critical Rules

- **NO PHI IN LOGS:** Under no circumstances shall Protected Health Information (PHI) be logged. Use the configured Serilog filters to prevent this.
- **USE RESILIENCY PATTERNS:** All external HTTP calls to vendor APIs must be wrapped in a Polly resilience policy that includes both a Retry and a Circuit Breaker pattern.
- **VALIDATE ALL INPUTS:** All data received from external sources (e.g., API request bodies) must be validated at the boundary of the service before any processing occurs.
- **USE CUSTOM EXCEPTIONS:** For all business logic or validation failures, throw a specific custom exception (e.g., `FhirValidationException`) instead of a generic Exception.
- **ASYNC EVERYWHERE:** All I/O-bound operations (database calls, API calls) must be async from top to bottom. Use `async/await` and never `.Result` or `.Wait()`.
