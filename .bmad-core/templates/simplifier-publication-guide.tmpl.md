# Simplifier.net Publication Guide

## Template: simplifier-publication-guide
**Version**: 1.0
**Purpose**: Step-by-step guide for publishing FHIR profiles to Simplifier.net
**Usage**: Referenced by publish-to-simplifier task

---

## Profile Information
- **Profile Name**: {profile_name}
- **Version**: {profile_version}
- **Description**: {profile_description}
- **Clinical Purpose**: {clinical_purpose}
- **Canonical URL**: {canonical_url}

## Pre-Publication Checklist

### StructureDefinition Validation
- [ ] **Valid FHIR Format**: StructureDefinition file is valid JSON/XML
- [ ] **Canonical URL**: Unique canonical URL defined
- [ ] **Profile Name**: Follows naming conventions (PascalCase, descriptive)
- [ ] **Version**: Semantic versioning format (e.g., 1.0.0)
- [ ] **Status**: Appropriate status (draft, active, retired)
- [ ] **Description**: Clear, comprehensive description of clinical purpose
- [ ] **Contact Information**: Author/maintainer contact details included
- [ ] **Copyright**: Copyright and license information specified
- [ ] **Publisher**: Organization/individual publisher identified

### Profile Content Validation
- [ ] **Base Profile**: Correctly derived from appropriate base resource
- [ ] **Element Constraints**: All constraints properly defined
- [ ] **Cardinality**: Appropriate min/max cardinality for clinical use
- [ ] **Data Types**: Correct data types specified
- [ ] **Value Sets**: Terminology bindings properly configured
- [ ] **Extensions**: Custom extensions properly defined and referenced
- [ ] **Examples**: Representative examples included (recommended)
- [ ] **Documentation**: Element definitions include clinical guidance

### Clinical Validation
- [ ] **Requirements Alignment**: Profile addresses clinical requirements
- [ ] **Workflow Integration**: Supports intended clinical workflows
- [ ] **User Experience**: Reasonable documentation burden for clinicians
- [ ] **Safety Considerations**: Patient safety implications addressed

## Simplifier.net Upload Instructions

### Step 1: Account Access
1. Navigate to https://simplifier.net
2. Sign in with your credentials
3. Select the appropriate organization/project: **{organization_name}**
4. Verify you have publishing permissions

### Step 2: File Upload
1. Click **"Add Resource"** or **"Upload"** button
2. Select your StructureDefinition file: `{file_path}`
3. Wait for upload confirmation
4. Review auto-generated metadata for accuracy

### Step 3: Publication Configuration
- **Publication Status**: Set to `{publication_status}`
- **Access Permissions**: Configure as `{access_permissions}`
- **Categories/Tags**: Add relevant tags: `{tags}`
- **Project Assignment**: Assign to project: `{project_name}`

### Step 4: Metadata Review
Verify the following information is correct:
- **Name**: {profile_name}
- **Version**: {profile_version}
- **Canonical URL**: {canonical_url}
- **Description**: {profile_description}
- **Publisher**: {publisher}
- **Contact**: {contact_info}

### Step 5: Publication
1. Click **"Publish"** to make profile publicly available
2. Confirm publication in modal dialog
3. Wait for publication confirmation message
4. Note the publication timestamp: _{publication_timestamp}_

## Post-Publication Verification

### Accessibility Check
- [ ] **Profile URL**: Verify profile accessible at {profile_url}
- [ ] **Human-readable View**: Confirm readable documentation generated
- [ ] **Search**: Verify profile appears in Simplifier.net search results
- [ ] **Canonical URL**: Test canonical URL resolves correctly
- [ ] **Version Display**: Confirm version information displayed correctly

### Documentation Quality
- [ ] **Element Descriptions**: All elements have clear descriptions
- [ ] **Examples**: Examples display correctly (if included)
- [ ] **Value Sets**: Terminology bindings render properly
- [ ] **Constraints**: Profile constraints clearly documented
- [ ] **Navigation**: Profile sections easily navigable

### Integration Testing
- [ ] **FHIR Validation**: Profile validates using FHIR validators
- [ ] **Tool Compatibility**: Profile works with FHIR development tools
- [ ] **API Access**: Profile accessible via Simplifier.net API
- [ ] **Export**: Profile can be exported in multiple formats

## Publication Record

### Publication Details
- **Publication Date**: {publication_date}
- **Published By**: {publisher_name}
- **Simplifier.net URL**: {simplifier_url}
- **Human-readable URL**: {human_readable_url}
- **Canonical URL**: {canonical_url}
- **Version**: {version}
- **Status**: {status}

### Quality Metrics
- **Validation Status**: {validation_status}
- **Documentation Completeness**: {documentation_score}%
- **Example Coverage**: {example_coverage}
- **Terminology Binding**: {terminology_status}

### Next Steps
1. **Clinical Review**: Initiate clinical validation process
2. **Implementation Planning**: Begin implementation guide development
3. **Community Feedback**: Monitor for community comments and feedback
4. **Version Management**: Plan for future version updates

## Troubleshooting

### Common Upload Issues
- **File Size Too Large**: Compress or split large profiles
- **Invalid Format**: Validate FHIR syntax using validator tools
- **Permission Denied**: Contact organization administrator for permissions
- **Duplicate URL**: Modify canonical URL or increment version

### Publication Problems
- **Server Errors**: Retry after brief delay, contact Simplifier.net support
- **Metadata Issues**: Review and correct profile metadata
- **Visibility Problems**: Check publication status and permissions
- **Search Issues**: Allow time for indexing, verify tags and categories

### Access Issues
- **Profile Not Found**: Verify URL spelling and publication status
- **Permission Errors**: Check sharing settings and organization permissions
- **Version Conflicts**: Ensure version numbers are unique and properly incremented

## Support Resources
- **Simplifier.net Documentation**: https://docs.simplifier.net
- **FHIR Specification**: https://hl7.org/fhir/
- **Community Support**: Simplifier.net community forums
- **Technical Support**: Contact Simplifier.net support team

---

**Template Version**: 1.0
**Last Updated**: {current_date}
**Maintained By**: FHIR Interoperability Specialist Agent