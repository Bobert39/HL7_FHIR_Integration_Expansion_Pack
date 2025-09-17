# generate-scaffolding

## Task Definition
This task generates a complete C#/.NET service project scaffolding for FHIR integration, including the necessary project structure, dependencies, and basic implementations.

## Inputs Required
- Integration Partner Profile (vendor API specifications and data model documentation)
- Project name for the scaffolding
- Target .NET version (default: 8.0)

## Outputs Generated
- Complete .NET project with proper structure (Controllers, Services, Models directories)
- Firely .NET SDK 5.x dependency configuration
- Basic health check endpoint implementation
- Unit test project with testing framework setup
- Configuration files (.editorconfig, appsettings.json, launchSettings.json)
- Containerization foundation (Dockerfile, docker-compose.yml)

## Task Workflow Steps

### Step 1: Analyze Integration Partner Profile
- Parse Integration Partner Profile for vendor-specific requirements
- Extract API specifications and data model information
- Identify vendor-specific configuration requirements

### Step 2: Generate Project Structure
- Create main project directory with proper naming convention
- Generate Controllers/ directory with base controller structure
- Generate Services/ directory with interface patterns
- Generate Models/ directory for data structures
- Generate Configuration/ directory for settings

### Step 3: Configure Core Dependencies
- Add Firely .NET SDK 5.x package reference
- Configure ASP.NET Core 8.0 framework
- Set up dependency injection for FHIR services
- Add required NuGet packages (Polly, Serilog, xUnit)

### Step 4: Implement Health Check Endpoint
- Generate HealthController with /health endpoint
- Implement basic health check response
- Add health check middleware configuration
- Include dependency validation logic

### Step 5: Generate Configuration Files
- Create .editorconfig for coding standards compliance
- Generate appsettings.json with FHIR configuration sections
- Create launchSettings.json for development environment
- Set up logging configuration with PHI protection

### Step 6: Create Test Project Structure
- Generate companion test project with proper naming convention
- Add testing framework references (xUnit, FluentAssertions, Moq)
- Create HealthControllerTests.cs with comprehensive tests
- Configure test coverage reporting (coverlet.collector)

### Step 7: Prepare Containerization Foundation
- Generate Dockerfile with .NET 8.0 runtime and SDK stages
- Create .dockerignore file for build optimization
- Add docker-compose.yml for local development environment
- Configure container health checks and environment variables

## Implementation Instructions

This task should generate production-ready C# code that follows the established coding standards:
- C# 12 targeting .NET 8.0 runtime
- PascalCase for classes/methods, camelCase for variables, _camelCase for private fields
- NO PHI IN LOGS (use Serilog filters)
- USE RESILIENCY PATTERNS (Polly for external HTTP calls)
- VALIDATE ALL INPUTS at service boundaries
- USE CUSTOM EXCEPTIONS for business logic failures
- ASYNC EVERYWHERE for I/O operations

## Validation Criteria
- Project compiles successfully with no errors
- Health check endpoint returns 200 OK status
- All unit tests pass
- Docker container builds and runs successfully
- Firely .NET SDK integration is properly configured