# ADR-003: Authentication Strategy

**Status:** Proposed
**Date:** 2026-08-04
**Decision Makers:** MedicHp Architecture Team

---

## Context

MedicHp handles sensitive Protected Health Information (PHI) and must comply with healthcare data security standards. The authentication system must support multiple user roles (Super Admin, Doctor, Patient, Clinic Staff) across three client platforms (website, dashboard, mobile) while maintaining a seamless and secure user experience.

## Decision

We will implement **JWT (JSON Web Tokens) with Refresh Token rotation** as the primary authentication mechanism, backed by ASP.NET Core Identity.

### Key Design Choices

1. **JWT Access Tokens** — Short-lived (15 minutes), stateless tokens for API authorization
2. **Refresh Tokens** — Long-lived, stored server-side in PostgreSQL, with single-use rotation
3. **Role-Based Access Control (RBAC)** — Claims-based authorization with role hierarchy
4. **Secure Token Storage** — HttpOnly cookies for web, SecureStore for mobile
5. **Multi-device support** — Independent sessions per device with revocation capability

## Rationale

- **JWT** provides stateless verification, enabling horizontal API scaling without shared session state
- **Refresh token rotation** mitigates token theft; each refresh token is single-use
- **Server-side refresh storage** enables immediate session revocation for compromised accounts
- **RBAC via claims** integrates natively with ASP.NET Core's authorization middleware

## Consequences

### Positive
- Stateless access tokens enable API scaling without session affinity
- Immediate revocation capability for security incidents
- Native ASP.NET Core middleware integration reduces custom code

### Negative
- Requires Redis or database lookup for refresh token validation
- Token rotation adds complexity to client-side token management
- Clock skew must be handled carefully across distributed services

## Alternatives Considered

| Alternative              | Reason Rejected                                                |
|--------------------------|----------------------------------------------------------------|
| Session-based auth       | Doesn't scale well across multiple API instances               |
| OAuth2 + OIDC (external) | Adds third-party dependency; premature for initial release     |
| API keys                 | Not suitable for end-user authentication                       |

## References

- [docs/Specifications/SECURITY_GUIDELINES.md](../Specifications/SECURITY_GUIDELINES.md)
- [docs/AI/Prompt_03_Authentication.md](../AI/Prompt_03_Authentication.md)
