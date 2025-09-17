# FHIR Profiles Inventory
*Compiled from Epic 3 and related implementation for Implementation Guide*

## Overview

This document inventories all FHIR profiles, extensions, and value sets created during the HL7 FHIR Integration Expansion Pack project, serving as input for the Implementation Guide technical specifications section.

## FHIR Version Compliance

**Base FHIR Version:** R4 (4.0.1)
**Implementation Guide Base:** HL7 FHIR R4

## Core FHIR Profiles Used

### 1. Patient Resource Profile

**Profile Name:** Standard Patient Profile
**Profile Purpose:** Patient demographic and contact information for healthcare integration
**Profile URL:** `http://hl7.org/fhir/StructureDefinition/Patient`
**Base Resource:** Patient

**Key Elements:**
- identifier: Medical Record Number (MRN) with hospital-specific system
- active: Patient record status
- name: Official patient name with given and family names
- telecom: Phone and email contact information
- gender: Administrative gender
- birthDate: Date of birth
- address: Home address with complete postal information
- maritalStatus: Marital status using standard value set
- contact: Emergency contact information
- communication: Preferred language for communication

**Usage Context:**
Used in patient data exchange scenarios for demographic synchronization between healthcare systems.

### 2. Observation Resource Profile

**Profile Name:** Vital Signs Observation Profile
**Profile Purpose:** Clinical observations and measurements with standardized coding
**Profile URL:** `http://hl7.org/fhir/StructureDefinition/Observation`
**Base Resource:** Observation

**Key Elements:**
- status: Observation status (final, preliminary, cancelled)
- category: Observation category (vital-signs, laboratory, imaging)
- code: LOINC-coded observation type
- subject: Reference to Patient resource
- effectiveDateTime: When observation was taken
- valueQuantity: Numerical value with unit of measure
- performer: Healthcare provider who performed observation
- component: Additional observation components (e.g., diastolic BP)

**Usage Context:**
Used for clinical data exchange including vital signs, laboratory results, and other clinical measurements.

### 3. Practitioner Resource Profile

**Profile Name:** Healthcare Provider Profile
**Profile Purpose:** Healthcare provider information for clinical context
**Profile URL:** `http://hl7.org/fhir/StructureDefinition/Practitioner`
**Base Resource:** Practitioner

**Key Elements:**
- identifier: Provider identification numbers (NPI, license numbers)
- active: Provider active status
- name: Provider name information
- telecom: Provider contact information
- qualification: Professional qualifications and certifications

**Usage Context:**
Referenced in clinical observations and care activities to identify responsible providers.

## Custom Extensions

### Healthcare Integration Extensions

**Extension Name:** Integration Metadata Extension
**Extension URL:** `http://example.org/fhir/StructureDefinition/integration-metadata`
**Purpose:** Track integration-specific metadata for resource synchronization

**Elements:**
- sourceSystem: Originating system identifier
- lastSyncTimestamp: Last synchronization timestamp
- syncStatus: Current synchronization status
- dataQualityScore: Data quality assessment score

## Value Sets and Code Systems

### Custom Value Sets

**Value Set Name:** Integration System Types
**Value Set URL:** `http://example.org/fhir/ValueSet/integration-system-types`
**Purpose:** Standardized codes for different healthcare system types

**Codes:**
- EHR: Electronic Health Record system
- LIS: Laboratory Information System
- RIS: Radiology Information System
- PACS: Picture Archiving and Communication System
- HIE: Health Information Exchange

### Standard Code Systems Used

**LOINC:** `http://loinc.org`
- Used for observation codes and clinical terminology
- Primary system for laboratory and clinical observations

**SNOMED CT:** `http://snomed.info/sct`
- Used for clinical concepts and procedures
- Primary system for clinical terminology

**ICD-10-CM:** `http://hl7.org/fhir/sid/icd-10-cm`
- Used for diagnosis codes
- Primary system for condition coding

**CPT:** `http://www.ama-assn.org/go/cpt`
- Used for procedure codes
- Primary system for procedure and service coding

## Example Resources

### Patient Example

**File:** `tests/resources/patient-valid-001.json`
**Description:** Comprehensive patient record demonstrating standard Patient profile usage
**Key Features:**
- Complete demographic information
- Multiple identifiers including MRN
- Contact information and emergency contacts
- Address and communication preferences

### Observation Example

**File:** `tests/resources/observation-valid-001.json`
**Description:** Vital signs observation with blood pressure measurement
**Key Features:**
- LOINC-coded vital signs observation
- Structured blood pressure measurement with components
- Reference to patient and performing practitioner
- Standard units of measure

## Implementation Patterns

### Resource Validation

**Validation Service:** `FhirValidationService`
**Profile Validation:** Resources validated against specified profiles
**Default Profiles:** Standard FHIR R4 profiles used as baseline

**Validation Features:**
- Comprehensive validation result reporting
- Issue severity classification (Fatal, Error, Warning, Information)
- Detailed location information for validation issues
- Performance monitoring for validation operations

### Data Mapping

**Mapping Service:** `DataMappingService`
**Purpose:** Transform data between different system formats and FHIR
**Supported Transformations:**
- Legacy system data to FHIR resources
- FHIR resource normalization
- Cross-system identifier mapping

### Security and Privacy

**Authentication:** SMART on FHIR OAuth 2.0
**Authorization:** Role-based access control
**Data Protection:** PHI handling with audit logging

**Security Features:**
- Secure token management
- Audit trail for all resource access
- Data encryption in transit and at rest
- Compliance with healthcare security standards

## Quality Assurance

### Profile Validation Testing

**Test Coverage:**
- Valid resource examples pass validation
- Invalid resource examples fail with appropriate errors
- Edge cases and boundary conditions tested
- Performance testing for large resource sets

**Validation Test Files:**
- `tests/resources/patient-valid-001.json`: Valid patient example
- `tests/resources/patient-invalid-001.json`: Invalid patient example
- `tests/resources/observation-valid-001.json`: Valid observation example
- `tests/resources/observation-invalid-001.json`: Invalid observation example

### Compliance Testing

**FHIR Compliance:**
- All profiles conform to FHIR R4 specification
- Resource validation against official FHIR schemas
- Canonical URL consistency across profiles

**Integration Testing:**
- End-to-end resource exchange testing
- Cross-system interoperability validation
- Performance and scalability testing

## Profile Dependencies

### Base FHIR Dependencies

**Required FHIR Packages:**
- `hl7.fhir.r4.core`: Base FHIR R4 specification
- `hl7.fhir.us.core`: US Core implementation guide profiles

**External Dependencies:**
- Firely .NET SDK 5.x for FHIR processing
- FHIR validation engine for profile compliance
- Terminology services for code system validation

### Profile Relationships

**Patient → Observation:** Patients are subjects of clinical observations
**Practitioner → Observation:** Practitioners perform clinical observations
**Organization → All:** Organizational context for all clinical activities

## Canonical URLs

### Base URLs

**Implementation Guide:** `http://example.org/fhir/ig/hl7-fhir-expansion-pack`
**Profile Base:** `http://example.org/fhir/StructureDefinition/`
**ValueSet Base:** `http://example.org/fhir/ValueSet/`
**Extension Base:** `http://example.org/fhir/StructureDefinition/`

### Versioning Strategy

**URL Pattern:** `{base-url}/{resource-type}/{id}|{version}`
**Version Format:** Semantic versioning (Major.Minor.Patch)
**Stability:** URLs maintained for backward compatibility

---

*This inventory was compiled from project implementation including Epic 3 FHIR profile development, validation suite implementation, and integration service development throughout the HL7 FHIR Integration Expansion Pack project.*