# ADR-005: Doctor Registration

**Status:** Proposed
**Date:** 2026-08-04
**Decision Makers:** MedCore Architecture Team

---

## Context

Doctor registration requires a higher level of trust and verification compared to patient onboarding. Doctors access sensitive patient records, issue prescriptions, and represent the medical credibility of the platform. The registration process must verify professional credentials while remaining efficient enough to attract practitioners.

## Decision

We will implement an **admin-verified doctor onboarding flow** with credential review before account activation.

### Registration Stages

1. **Stage 1 — Account Application** (doctor-initiated)
   - Full name, email, phone number, password
   - Medical license number and issuing authority
   - Specialization(s) and years of experience
   - Upload: medical license document, profile photo, national ID

2. **Stage 2 — Admin Verification** (admin-initiated)
   - Super Admin reviews submitted credentials
   - License verification against issuing authority (manual initially, automated later)
   - Status transitions: `Pending` → `Approved` / `Rejected` / `More Info Required`

3. **Stage 3 — Profile Setup** (post-approval)
   - Availability schedule configuration
   - Consultation fee settings
   - Clinic affiliation (if applicable)
   - Bio and specialization details

## Rationale

- **Admin verification gate** ensures only legitimate practitioners access patient data
- **Document upload** provides an auditable credential trail
- **Post-approval profile setup** avoids wasted effort for rejected applicants
- **Status workflow** enables transparent communication with applicants

## Consequences

### Positive
- Platform credibility is maintained through verified practitioner profiles
- Audit trail of credential verification for compliance
- Flexible status workflow handles edge cases (expired licenses, re-verification)

### Negative
- Manual verification creates admin workload and onboarding latency
- Rejected doctors may leave negative impressions of the platform
- Document storage adds infrastructure and compliance considerations (encryption, retention)

## Alternatives Considered

| Alternative                        | Reason Rejected                                              |
|------------------------------------|--------------------------------------------------------------|
| Self-registration (no verification)| Unacceptable risk for healthcare platform credibility        |
| Third-party credential API only    | Not universally available across all target regions           |
| Invitation-only registration       | Limits platform growth; too restrictive for initial launch    |

## References

- [docs/AI/Prompt_06_Doctor.md](../AI/Prompt_06_Doctor.md)
- [docs/Specifications/BUSINESS_RULES.md](../Specifications/BUSINESS_RULES.md)
