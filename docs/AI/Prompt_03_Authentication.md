# Prompt 03 — Authentication System

> AI prompt for implementing the MedCore authentication and authorization system.

---

## Objective

Implement a secure, JWT-based authentication system with role-based access control.

## Tasks

1. User registration with email verification
2. Login with JWT access token + refresh token
3. Token refresh endpoint
4. Password reset flow
5. Role-based authorization middleware (SuperAdmin, ClinicAdmin, Doctor, Patient)
6. Account lockout after failed attempts

## Constraints

- Access tokens expire in 15 minutes
- Refresh tokens stored as HTTP-only secure cookies
- Passwords hashed with bcrypt/Argon2
- All auth events logged to audit trail

---

> **Status:** Placeholder — To be expanded with detailed auth requirements.
