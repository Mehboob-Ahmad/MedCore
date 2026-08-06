# ADR-004: Patient Registration

**Status:** Proposed
**Date:** 2026-08-04
**Decision Makers:** MedCore Architecture Team

---

## Context

Patient registration is the primary onboarding entry point for the MedCore platform. The process must balance a low-friction user experience with the collection of essential health information and identity verification required by healthcare regulations.

## Decision

We will implement a **multi-step progressive registration flow** with deferred profile completion.

### Registration Stages

1. **Stage 1 — Account Creation** (required, immediate)
   - Full name, email, phone number, password
   - Email or SMS verification (OTP)
   - Terms of service and privacy policy acceptance

2. **Stage 2 — Basic Health Profile** (prompted after first login)
   - Date of birth, gender, blood type
   - Emergency contact information
   - Primary language preference

3. **Stage 3 — Extended Medical History** (optional, prompted contextually)
   - Known allergies, chronic conditions
   - Current medications
   - Insurance information

## Rationale

- **Progressive disclosure** reduces initial registration abandonment
- **Staged data collection** allows patients to use basic features immediately
- **Deferred profile completion** increases conversion rates while still gathering necessary data
- **OTP verification** establishes identity trust without requiring document uploads at onboarding

## Consequences

### Positive
- Lower barrier to entry increases patient sign-up conversion
- Patients can book appointments immediately after Stage 1
- Incomplete profiles are surfaced via dashboard prompts, not gates

### Negative
- Doctors may encounter patients with incomplete medical histories
- Additional backend logic to handle partially-complete profiles
- Must design UI to gracefully handle missing optional data

## Alternatives Considered

| Alternative                    | Reason Rejected                                              |
|--------------------------------|--------------------------------------------------------------|
| Single-page full registration  | Too many fields; high abandonment rate                       |
| Social login only              | Insufficient identity verification for healthcare context     |
| Document-based KYC             | Too heavy for initial onboarding; deferred to later phase     |

## References

- [docs/AI/Prompt_05_Patient.md](../AI/Prompt_05_Patient.md)
- [docs/Specifications/PRODUCT_REQUIREMENTS.md](../Specifications/PRODUCT_REQUIREMENTS.md)
