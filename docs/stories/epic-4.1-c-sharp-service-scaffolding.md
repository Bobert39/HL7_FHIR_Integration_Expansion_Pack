# Epic 4.1: Generate C# Service Scaffolding with Firely SDK

**Story ID:** Epic 4.1
**Epic:** Development & Implementation Workflow
**Priority:** High - Critical Path
**Estimated Effort:** 4-6 hours
**Status:** Ready for Implementation

---

## User Story

**As a healthcare integration developer,**
I want a properly structured .NET service project with Firely SDK integration,
So that I have a clean foundation to build FHIR-compliant data transformation services.

---

## Story Context

### Existing System Integration

- **Integrates with:** OpenEMR API (documented in integration partner profile)
- **Technology:** C#/.NET 6+, Firely .NET SDK, ASP.NET Core Web API
- **Follows pattern:** BMad expansion pack modular service architecture
- **Touch points:** ChemistryPanelObservation FHIR profile, OpenEMR patient data API

### Dependencies

**Input Artifacts:**
- `docs/demo/completed-integration-partner-profile-openemr.md` - OpenEMR API documentation
- `docs/demo/ChemistryPanelObservation.json` - FHIR profile for implementation
- `.bmad-core/templates/` - Project templates and patterns

**Prerequisites:**
- .NET 6+ SDK installed
- Visual Studio or VS Code with C# extension
- Basic understanding of FHIR R4 standard

---

## Acceptance Criteria

### Functional Requirements

1. **Service Structure:** Create standard .NET Web API project with Controllers, Services, Models folders
2. **Firely Integration:** Firely .NET SDK properly installed and configured as dependency
3. **Health Check:** Basic API controller with `/health` endpoint returns service status

### Integration Requirements

4. **Existing workflows continue:** BMad expansion pack agents and tasks remain operational
5. **Template pattern:** Service follows BMad standard file organization principles
6. **Profile integration:** Project references ChemistryPanelObservation StructureDefinition

### Quality Requirements

7. **Testing foundation:** Unit test project included with basic health check test
8. **Documentation:** README with setup instructions and Firely SDK integration notes
9. **Validation:** Service builds and runs successfully with health endpoint accessible

---

## Technical Implementation Guide

### Project Structure
```
src/FhirIntegrationService/
├── Controllers/
│   └── HealthController.cs
├── Services/
│   └── (ready for Epic 4.2)
├── Models/
│   └── (ready for Epic 4.2)
├── Program.cs
├── appsettings.json
└── FhirIntegrationService.csproj

tests/FhirIntegrationService.Tests/
├── Controllers/
│   └── HealthControllerTests.cs
└── FhirIntegrationService.Tests.csproj
```

### Required NuGet Packages
- `Hl7.Fhir.R4` (Firely .NET SDK)
- `Microsoft.AspNetCore.App`
- `Swashbuckle.AspNetCore` (for API documentation)
- `Microsoft.NET.Test.Sdk` (test project)
- `xUnit` (test framework)

### Health Check Implementation
```csharp
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            service = "FHIR Integration Service",
            fhirVersion = "R4"
        });
    }
}
```

---

## Technical Notes

- **Integration Approach:** Standalone service that will consume OpenEMR API and output FHIR resources
- **Existing Pattern Reference:** Follow .NET Web API standard project template with Firely SDK integration
- **Key Constraints:** Must be compatible with Firely SDK version requirements and FHIR R4 standard
- **Configuration:** Include appsettings for OpenEMR API connection (from integration partner profile)

---

## Definition of Done

- [ ] .NET Web API project created with standard structure
- [ ] Firely .NET SDK dependency added and configured
- [ ] Basic health check endpoint implemented and tested
- [ ] Unit test project included with passing health check test
- [ ] Service builds without errors and starts successfully
- [ ] README documentation with setup and integration instructions
- [ ] Project follows BMad expansion pack organizational standards
- [ ] Configuration ready for Epic 4.2 data mapping implementation

---

## Risk Assessment

### Primary Risk
Firely SDK version compatibility or configuration issues

### Mitigation Strategy
- Use latest stable Firely SDK version with documented .NET 6+ compatibility
- Follow official Firely documentation for initial setup
- Test basic FHIR resource creation in health check

### Rollback Plan
Simple project deletion - no existing system impact since this is new service creation

---

## Success Criteria

**Epic 4.1 is complete when:**

1. ✅ Service runs locally with accessible health check endpoint
2. ✅ Firely SDK successfully loads and basic FHIR operations work
3. ✅ Unit tests pass and build pipeline is green
4. ✅ Documentation enables next developer to continue with Epic 4.2
5. ✅ Project structure supports planned data mapping and API endpoint features

---

## Next Steps

**After Epic 4.1 completion:**
- Hand off to Epic 4.2: Implement Data Mapping Logic
- Use this service as foundation for OpenEMR → FHIR transformation
- Leverage established patterns for rapid Epic 4.2-4.3 development

---

**Generated by BMad PM Agent | Product Manager: John 📋**
**Date:** 2024-09-17
**Change Log:** Initial story creation for Epic 4 Development & Implementation Workflow