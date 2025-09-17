# Section 6 of 12: External APIs

This expansion pack does not directly call external APIs in a traditional runtime sense. Instead, its primary architectural function is to orchestrate the user's interaction with an external toolchain and to generate code that will, in turn, interact with EHR vendor APIs. The following are the key external systems this pack is designed to manage:

1. **Simplifier.net**  
   - *Purpose:* The central, cloud-based repository for collaborative FHIR project management, profile publishing, and documentation.  
   - *Responsible Agents:* FHIR Interoperability Specialist, Clinical Informaticist.  
   - *Interaction Method:* The agents provide step-by-step guidance for the user to perform actions (e.g., "publish profile," "review comments") via the Simplifier.net web interface.

2. **Forge FHIR Profile Editor**  
   - *Purpose:* The desktop application used for the technical creation and validation of FHIR profiles (StructureDefinitions).  
   - *Responsible Agent:* FHIR Interoperability Specialist.  
   - *Interaction Method:* The agent provides a detailed, guided workflow for the user to operate the Forge desktop application.

3. **Firely Terminal**  
   - *Purpose:* The command-line tool used for scripting, batch validation of FHIR resources, and package management.  
   - *Responsible Agents:* FHIR Interoperability Specialist, Healthcare System Integration Analyst.  
   - *Interaction Method:* The agents generate and provide specific command-line scripts for the user to execute in Firely Terminal.

4. **Target EHR Vendor FHIR APIs (e.g., Epic, OpenEMR, Eyefinity)**  
   - *Purpose:* These are the ultimate targets of the integration service.  
   - *Responsible Agents:* Healthcare System Integration Analyst (for research), FHIR Interoperability Specialist (for implementation).  
   - *Interaction Method:* The agents guide the generation of C#/.NET code that will make live RESTful API calls to these external systems.

---
