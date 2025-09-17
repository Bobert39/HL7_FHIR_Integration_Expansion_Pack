# Vendor Technical Questions Template
# Template for generating structured questions for vendor technical teams

template:
  id: vendor-technical-questions-template
  name: Vendor Technical Questions Template
  version: 1.0
  format: markdown
  purpose: Generate structured questions for vendor technical teams regarding integration capabilities

content_structure: |
  # Technical Questions for {{system_name}} Integration

  **Date:** {{date_created}}
  **Requesting Organization:** {{requesting_organization}}
  **Contact:** {{contact_name}} ({{contact_email}})
  **Integration Purpose:** {{integration_purpose}}

  ## Authentication and Security

  ### 1. Authentication Methods
  - What authentication methods does your API support? (OAuth2, SMART on FHIR, API Keys, Certificate-based, etc.)
  - Do you support SMART on FHIR v2 for patient-authorized access?
  - What is the process for obtaining API credentials?
  - Do you provide separate credentials for sandbox and production environments?

  ### 2. Security Requirements
  - What security certifications does your API platform maintain? (SOC 2, HIPAA, etc.)
  - Are there specific network security requirements? (VPN, IP whitelisting, etc.)
  - What encryption standards are required for data in transit?
  - Do you have specific requirements for client-side security implementations?

  ### 3. Compliance and Audit
  - What audit logging is available for API access?
  - Do you provide BAA (Business Associate Agreement) for integration partners?
  - Are there specific patient consent requirements for data access?
  - What compliance frameworks does your integration platform support?

  ## API Capabilities and Technical Specifications

  ### 4. API Standards and Formats
  - What version of FHIR do you support? (R4, STU3, DSTU2)
  - Do you support HL7 v2.x messaging? If so, which versions?
  - What data formats are supported? (JSON, XML, HL7)
  - Are there any custom API extensions or non-standard implementations?

  ### 5. Available Endpoints and Operations
  - What FHIR resources are available through your API? (Patient, Observation, Condition, etc.)
  - Do you support bulk data export (FHIR Bulk Data Access)?
  - What CRUD operations are supported for each resource type?
  - Are there any read-only or write-only restrictions on specific data types?

  ### 6. Data Scope and Access
  - What clinical data types are accessible? (Demographics, clinical notes, lab results, medications, etc.)
  - Are there different levels of data access based on user permissions?
  - Can we access historical data? If so, how far back?
  - Are there any data elements that are not available via API?

  ## Performance and Limitations

  ### 7. Rate Limits and Throttling
  - What are the rate limits for API calls? (per minute, hour, day)
  - Are rate limits applied per credential set or per organization?
  - How are rate limit exceeded scenarios handled?
  - Are there burst allowances for higher temporary volumes?

  ### 8. Performance Characteristics
  - What are typical response times for API calls?
  - What is your API uptime SLA?
  - Are there scheduled maintenance windows that affect API availability?
  - What is the maximum payload size for API requests?

  ### 9. Volume and Scalability
  - What data volumes can the API handle? (patients, transactions per day)
  - Are there any limitations on bulk data operations?
  - How do you handle large result sets? (pagination, streaming)
  - Are there any restrictions on concurrent connections?

  ## Development and Testing

  ### 10. Sandbox Environment
  - Do you provide a sandbox environment for development and testing?
  - Is the sandbox environment functionally equivalent to production?
  - What test data is available in the sandbox?
  - Are there any limitations or differences in the sandbox environment?

  ### 11. Documentation and Support
  - Where can we access detailed API documentation?
  - Do you provide code samples or SDKs for integration?
  - What technical support is available during development?
  - Are there developer forums or community resources?

  ### 12. Implementation Process
  - What is the typical timeline for API credential provisioning?
  - Are there any certification or testing requirements before production access?
  - What is the process for promoting from sandbox to production?
  - Do you provide integration testing support?

  ## Business and Operational

  ### 13. Costs and Licensing
  - Are there any costs associated with API access?
  - Are there transaction-based fees or volume-based pricing?
  - What is included in the base API access fee?
  - Are there additional costs for premium features or higher rate limits?

  ### 14. Support and Maintenance
  - What level of technical support is provided?
  - What are your support hours and response time SLAs?
  - How are API updates and changes communicated to integration partners?
  - Do you provide advance notice of breaking changes?

  ### 15. Partnership and Onboarding
  - What is the typical onboarding timeline for new integration partners?
  - Are there any partnership agreements or contracts required?
  - Do you have a formal partner program or certification process?
  - Are there any restrictions on the types of integrations or use cases supported?

  ## Additional Information

  ### 16. Future Roadmap
  - Are there planned enhancements to the API platform?
  - Do you have plans to support additional FHIR versions or resources?
  - Are there any deprecated features or endpoints we should be aware of?
  - What is your timeline for major API version updates?

  ### 17. Integration Examples
  - Can you provide examples of similar integrations you've implemented?
  - Are there any integration patterns or architectures you recommend?
  - Do you have preferred integration partners or vendors you work with?
  - Are there any specific technical approaches you recommend or discourage?

  ## Our Specific Requirements

  ### Integration Overview
  {{integration_requirements}}

  ### Specific Questions Based on Our Use Case
  {{custom_questions}}

  ---

  **Thank you for your time and assistance. We look forward to your responses and to potentially partnering with you for this integration project.**

  **Next Steps:** Based on your responses, we will evaluate technical feasibility and proceed with detailed integration planning if suitable.

usage_instructions: |
  This template generates comprehensive technical questions for vendor engagement.
  Customize the {{integration_requirements}} and {{custom_questions}} sections
  based on the specific integration needs identified during the initial scoping phase.

  Use this template to ensure all critical technical questions are covered when
  contacting vendor technical teams.