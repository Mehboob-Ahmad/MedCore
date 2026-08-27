# 🛡️ Security Guidelines — MedicHp Digital Healthcare Ecosystem

> **Document Type:** Security Architecture & Guidelines
> **Version:** 1.0
> **Last Revised:** 2026-08-06
> **Status:** Active
> **Audience:** All Engineers, DevOps, Security Architects

---

## Table of Contents
- [1. Core Security Principles](#1-core-security-principles)
- [2. Authentication & Authorization](#2-authentication--authorization)
- [3. Data Protection & Encryption](#3-data-protection--encryption)
- [4. Web Application Security (OWASP)](#4-web-application-security-owasp)
- [5. Secrets Management](#5-secrets-management)
- [6. Audit Logging & Monitoring](#6-audit-logging--monitoring)
- [7. Healthcare Compliance (HIPAA/GDPR)](#7-healthcare-compliance-hipaagdpr)
- [8. Infrastructure Security & BCDR](#8-infrastructure-security--bcdr)

---

## 1. Core Security Principles

MedicHp handles Protected Health Information (PHI) and Personally Identifiable Information (PII). A breach is catastrophic.

- **Zero Trust:** Do not trust the network, do not trust the client, do not trust the input. Verify everything.
- **Defense in Depth:** Multiple layers of security (WAF -> Network ACL -> App Auth -> DB Row-Level Security).
- **Least Privilege:** Users, services, and databases only have access to what they explicitly need.

---

## 2. Authentication & Authorization

### 2.1 Passwords
- Passwords are never stored in plaintext.
- Use **Argon2id** (or BCrypt work factor 12+) for password hashing.
- Password Policy: Minimum 8 characters, 1 uppercase, 1 number, 1 special character.

### 2.2 JWT & Refresh Tokens
- **Access Tokens (JWT):** Short-lived (15 minutes). Signed using asymmetric keys (RS256) or strong symmetric keys (HS256). Do not store sensitive PII inside the JWT payload.
- **Refresh Tokens:** Long-lived, stored securely in the database as hashed values (like passwords). **Single-use only** (Rotation strategy). If a reused refresh token is detected, all sessions for that user are immediately revoked.
- **Storage:** Web clients store JWT in memory and Refresh Tokens in `HttpOnly`, `Secure`, `SameSite=Strict` cookies to prevent XSS exfiltration.

### 2.3 Role-Based Access Control (RBAC)
- Enforce authorization at the controller level using `[Authorize(Roles = "Doctor")]`.
- Enforce resource-level authorization in the Application layer (e.g., verifying a Patient can only access their *own* `AppointmentId`).

---

## 3. Data Protection & Encryption

### 3.1 Encryption in Transit
- All communication must use **TLS 1.3** (or 1.2 minimum).
- HTTP traffic is strictly redirected to HTTPS via HSTS (HTTP Strict Transport Security).

### 3.2 Encryption at Rest
- The PostgreSQL database must have transparent data encryption (TDE) enabled at the volume/storage layer (AES-256).
- Highly sensitive fields (e.g., Social Security Numbers, exact payment tokens in future phases) must undergo Application-Level Encryption before hitting the database.

---

## 4. Web Application Security (OWASP)

### 4.1 SQL Injection
- Blocked by using **Entity Framework Core** and parameterized queries exclusively. Raw SQL is heavily scrutinized in PRs and must use parameters.

### 4.2 Cross-Site Scripting (XSS)
- React inherently escapes JSX variables.
- Strictly prohibit the use of `dangerouslySetInnerHTML`.
- Enforce a strict **Content Security Policy (CSP)** via HTTP headers.

### 4.3 Cross-Site Request Forgery (CSRF)
- Mitigated by using stateless JWTs (if stored in memory/local storage) or by using `SameSite=Strict` for cookies.

### 4.4 Rate Limiting & DoS Protection
- Apply global rate limiting to the API.
- Apply aggressive rate limiting to sensitive endpoints (`/login`, `/register`, `/forgot-password`) to prevent brute-force and enumeration attacks.

### 4.5 CORS
- Explicitly define allowed origins (e.g., `https://medichp.com`).
- Never use `*` for CORS in production.

---

## 5. Secrets Management

- **NO SECRETS IN CODE:** API keys, database connection strings, and JWT secrets must never be committed to Git.
- Use `.env` files for local development (ignored by `.gitignore`).
- In production, inject secrets using **Azure Key Vault**, **AWS Secrets Manager**, or environment variables managed by CI/CD securely.

---

## 6. Audit Logging & Monitoring

- Every mutation (Create, Update, Delete) of sensitive data MUST be logged to the `AuditLogs` table.
- Logs must include: `UserId`, `Action`, `TableName`, `RecordId`, `OldValues`, `NewValues`, `IpAddress`, `Timestamp`.
- Logs must be append-only and immutable.
- Failed login attempts must be logged to detect brute-force attacks.

---

## 7. Healthcare Compliance (HIPAA/GDPR)

While MedicHp Phase 1 is a foundation, it is built to be compliant:
- **Right to Erasure (GDPR):** Handled via Soft Deletes (`IsDeleted`), with a future automated script for hard redaction of PII after regulatory retention periods.
- **Data Access Auditing (HIPAA):** Covered by the `AuditLogs` and `ActivityLogs`.
- **BAA Readiness:** Infrastructure is prepared to be hosted on HIPAA-eligible cloud segments.

---

## 8. Infrastructure Security & BCDR

- **Backups:** Database backups taken daily, with Write-Ahead Logging (WAL) archiving for point-in-time recovery (PITR) up to 7 days.
- **Disaster Recovery (BCDR):** Backups must be replicated to a secondary geographic region.
- **Least Privilege Infrastructure:** Database cannot be accessed directly from the public internet; it sits in a private subnet accessible only by the API servers.
