# 🌐 API Specification — MedicHp Digital Healthcare Ecosystem

> **Document Type:** API Specification (Authoritative)
> **Version:** 2.0
> **Last Revised:** 2026-08-06
> **Status:** Active
> **Audience:** Backend engineers, frontend developers, mobile developers, AI coding assistants
> **Scope:** Phase 1 — Independent Doctors + Patients
> **Base URL:** `https://api.medichp.com/api/v1`

---

## Table of Contents

- [1. API Design Philosophy](#1-api-design-philosophy)
- [2. General Standards](#2-general-standards)
- [3. Authentication & Security](#3-authentication--security)
- [4. Global Conventions](#4-global-conventions)
- [5. Authentication Endpoints](#5-authentication-endpoints)
- [6. Patient Endpoints](#6-patient-endpoints)
- [7. Doctor Endpoints](#7-doctor-endpoints)
- [8. Appointment Endpoints](#8-appointment-endpoints)
- [9. Consultation Endpoints](#9-consultation-endpoints)
- [10. Prescription Endpoints](#10-prescription-endpoints)
- [11. Chat Endpoints](#11-chat-endpoints)
- [12. Notification Endpoints](#12-notification-endpoints)
- [13. Search Endpoints](#13-search-endpoints)
- [14. File Upload Endpoints](#14-file-upload-endpoints)
- [15. Super Admin Endpoints](#15-super-admin-endpoints)
- [16. API Flow Diagrams](#16-api-flow-diagrams)
- [17. Versioning & Deprecation](#17-versioning--deprecation)
- [18. Version History](#18-version-history)

---

## 1. API Design Philosophy

### 1.1 Contract-First Design

This API specification serves as the **contract** between all consumers:

| Consumer                     | Technology           | Usage                                  |
|------------------------------|----------------------|----------------------------------------|
| Public Website               | Next.js (SSR + CSR)  | Doctor search, public profiles, SEO    |
| Admin Dashboard              | React + Vite (SPA)   | Platform management, analytics         |
| Mobile App                   | React Native + Expo  | Patient & doctor mobile experience     |
| Backend API                  | ASP.NET Core 9       | RESTful API server                     |
| Third-Party Integrations     | HTTP/REST            | Future partner APIs                    |

### 1.2 Design Principles

| Principle                  | Implementation                                                          |
|----------------------------|-------------------------------------------------------------------------|
| RESTful Resources          | Nouns for resources, HTTP verbs for actions                             |
| Consistent Response Shape  | All responses follow a unified envelope structure                       |
| Stateless Authentication   | JWT bearer tokens; no server-side session state                         |
| Idempotent Operations      | PUT and DELETE are idempotent; POST creates new resources               |
| HATEOAS-Ready              | Response structure supports `_links` expansion in future versions       |
| Versioned                  | URL-based versioning (`/api/v1/`, `/api/v2/`)                          |

---

## 2. General Standards

### 2.1 HTTP Methods

| Method   | Usage                          | Idempotent | Safe |
|----------|--------------------------------|------------|------|
| `GET`    | Retrieve resource(s)           | Yes        | Yes  |
| `POST`   | Create resource or trigger action | No      | No   |
| `PUT`    | Full resource replacement      | Yes        | No   |
| `PATCH`  | Partial resource update        | No         | No   |
| `DELETE` | Soft-delete resource           | Yes        | No   |

### 2.2 Content Types

| Header           | Value                              |
|------------------|------------------------------------|
| `Content-Type`   | `application/json; charset=utf-8`  |
| `Accept`         | `application/json`                 |

### 2.3 URL Structure

```
https://api.medichp.com/api/v1/{resource}/{id?}/{sub-resource?}
```

| Convention                      | Example                                      |
|---------------------------------|----------------------------------------------|
| Resource names are **plural**   | `/api/v1/patients`, `/api/v1/appointments`   |
| Path parameters for IDs         | `/api/v1/doctors/{doctorId}`                 |
| Query parameters for filtering  | `/api/v1/doctors?cityId=...&minFee=200`      |
| Nested resources for ownership  | `/api/v1/appointments/{id}/consultation`     |
| Actions as sub-paths            | `/api/v1/appointments/{id}/cancel`           |

### 2.4 Standard HTTP Status Codes

| Code  | Meaning                      | When Used                                             |
|-------|------------------------------|-------------------------------------------------------|
| `200` | OK                           | Successful GET, PUT, PATCH                            |
| `201` | Created                      | Successful POST creating a resource                   |
| `204` | No Content                   | Successful DELETE                                     |
| `400` | Bad Request                  | Malformed request body or parameters                  |
| `401` | Unauthorized                 | Missing, invalid, or expired JWT                      |
| `403` | Forbidden                    | Valid JWT but insufficient role/permissions            |
| `404` | Not Found                    | Resource does not exist or is soft-deleted             |
| `409` | Conflict                     | Duplicate resource, booking conflict                  |
| `422` | Unprocessable Entity         | Valid JSON but fails business rule validation          |
| `423` | Locked                       | Account is locked (failed login attempts)             |
| `429` | Too Many Requests            | Rate limit exceeded                                   |
| `500` | Internal Server Error        | Unhandled server exception                            |

---

## 3. Authentication & Security

### 3.1 JWT Bearer Token

All authenticated endpoints require a JWT access token in the `Authorization` header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

**JWT Claims:**

| Claim        | Description                           | Example                                  |
|--------------|---------------------------------------|------------------------------------------|
| `sub`        | User ID (UUID)                        | `a1b2c3d4-e5f6-7890-abcd-ef1234567890`  |
| `email`      | User email                            | `dr.ayesha@example.com`                  |
| `role`       | User role(s)                          | `Doctor`                                 |
| `iat`        | Issued at (Unix timestamp)            | `1691308800`                             |
| `exp`        | Expiry (Unix timestamp)               | `1691309700` (15 min default)            |
| `jti`        | Unique token ID                       | UUID                                     |

> **Reference:** AUTH-R04 (15-min JWT expiry, configurable), AUTH-R05 (single-use refresh token rotation).

### 3.2 Refresh Token Flow

```mermaid
sequenceDiagram
    participant C as Client
    participant A as API Server
    participant D as Database

    C->>A: POST /api/v1/auth/login {email, password}
    A->>D: Validate credentials
    D-->>A: User record
    A-->>C: 200 {accessToken, refreshToken, expiresIn}

    Note over C: Access token expires after 15 min

    C->>A: POST /api/v1/auth/refresh {refreshToken}
    A->>D: Validate & rotate refresh token
    D-->>A: New token pair
    A-->>C: 200 {accessToken, refreshToken, expiresIn}
```

### 3.3 Role-Based Access Control (RBAC)

| Role          | Access Scope                                                      |
|---------------|-------------------------------------------------------------------|
| `Patient`     | Own profile, own appointments, own consultations, own prescriptions, own chat |
| `Doctor`      | Own profile, own appointments, consented patient data, own consultations |
| `SuperAdmin`  | All user management, platform analytics, audit logs, system settings |

### 3.4 Rate Limiting

| Endpoint Category        | Rate Limit              | Window   |
|--------------------------|-------------------------|----------|
| Authentication (login)   | 10 requests             | 1 minute |
| Authentication (register)| 5 requests              | 1 minute |
| Password reset           | 3 requests              | 15 minutes |
| General API              | 100 requests            | 1 minute |
| Search (public)          | 30 requests             | 1 minute |
| File upload              | 10 requests             | 1 minute |

**Rate Limit Response Headers:**

```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 42
X-RateLimit-Reset: 1691309760
```

### 3.5 CORS Policy

| Setting              | Value                                                    |
|----------------------|----------------------------------------------------------|
| Allowed Origins      | `https://medichp.com`, `https://admin.medichp.com`, `http://localhost:3000` (dev) |
| Allowed Methods      | `GET, POST, PUT, PATCH, DELETE, OPTIONS`                 |
| Allowed Headers      | `Authorization, Content-Type, X-Request-Id`              |
| Exposed Headers      | `X-RateLimit-Limit, X-RateLimit-Remaining, X-Correlation-Id` |
| Max Age              | `86400` (24 hours)                                       |

---

## 4. Global Conventions

### 4.1 Unified Response Envelope

Every API response uses this envelope structure:

**Success Response:**

```json
{
  "success": true,
  "message": "Resource retrieved successfully",
  "data": { },
  "meta": {
    "timestamp": "2026-08-06T10:15:00Z",
    "requestId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
  }
}
```

**Paginated Response:**

```json
{
  "success": true,
  "message": "Results retrieved successfully",
  "data": [ ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalRecords": 150,
    "totalPages": 8,
    "hasNext": true,
    "hasPrevious": false
  },
  "meta": {
    "timestamp": "2026-08-06T10:15:00Z",
    "requestId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
  }
}
```

**Error Response (RFC 7807 Problem Details):**

```json
{
  "success": false,
  "error": {
    "type": "https://medichp.com/errors/validation-error",
    "title": "Validation Error",
    "status": 422,
    "detail": "One or more validation errors occurred.",
    "instance": "/api/v1/appointments",
    "errors": {
      "scheduledAt": ["Appointment must be booked at least 1 hour in advance."],
      "doctorId": ["Doctor does not have available slots at the requested time."]
    }
  },
  "meta": {
    "timestamp": "2026-08-06T10:15:00Z",
    "requestId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
  }
}
```

### 4.2 Pagination

All list endpoints support pagination via query parameters:

| Parameter   | Type   | Default | Max  | Description                   |
|-------------|--------|---------|------|-------------------------------|
| `page`      | `int`  | `1`     | —    | Page number (1-indexed)       |
| `pageSize`  | `int`  | `20`    | `100`| Items per page                |

**Example:** `GET /api/v1/patients?page=2&pageSize=25`

### 4.3 Filtering

Filters are passed as query parameters using the field name:

| Pattern                   | Example                                       |
|---------------------------|-----------------------------------------------|
| Exact match               | `?status=Confirmed`                           |
| Range (min/max)           | `?minFee=200&maxFee=500`                      |
| Date range                | `?fromDate=2026-08-01&toDate=2026-08-31`      |
| Multiple values (OR)      | `?status=Pending,Confirmed`                   |
| Boolean                   | `?isActive=true`                              |
| UUID reference            | `?cityId=a1b2c3d4-...`                        |

### 4.4 Sorting

| Parameter   | Format                  | Example                       | Default              |
|-------------|-------------------------|-------------------------------|----------------------|
| `sortBy`    | `{field}`               | `?sortBy=consultationFee`     | Resource-specific    |
| `sortOrder` | `asc` or `desc`         | `?sortOrder=desc`             | `asc`                |

**Multiple sort fields:** `?sortBy=experience&sortOrder=desc&thenBy=fee&thenOrder=asc`

### 4.5 Searching

Full-text search uses the `q` parameter:

```
GET /api/v1/search/doctors?q=headache&cityId=...&minFee=200
```

### 4.6 Standard Request Headers

| Header              | Required | Description                          |
|---------------------|----------|--------------------------------------|
| `Authorization`     | Auth endpoints | `Bearer {accessToken}`          |
| `Content-Type`      | POST/PUT/PATCH | `application/json`             |
| `Accept`            | All      | `application/json`                   |
| `X-Request-Id`      | Optional | Client-generated correlation ID      |
| `Accept-Language`   | Optional | Locale (future i18n), e.g., `en-US`  |

### 4.7 Standard Response Headers

| Header                | Description                              |
|-----------------------|------------------------------------------|
| `X-Correlation-Id`    | Server-assigned or echoed request ID     |
| `X-RateLimit-Limit`   | Rate limit ceiling                       |
| `X-RateLimit-Remaining` | Remaining requests in window          |
| `X-RateLimit-Reset`   | Unix timestamp when limit resets         |

---

## 5. Authentication Endpoints

### 5.1 Register Patient

**Purpose:** Create a new patient account (Stage 1 of progressive registration).

| Attribute          | Value                                      |
|--------------------|--------------------------------------------|
| **Method**         | `POST`                                     |
| **URL**            | `/api/v1/auth/register/patient`            |
| **Authentication** | None                                       |
| **Roles**          | Public                                     |

**Request Body:**

```json
{
  "firstName": "Ahmed",
  "lastName": "Khan",
  "email": "ahmed@example.com",
  "phoneNumber": "+923001234567",
  "password": "SecureP@ss1",
  "confirmPassword": "SecureP@ss1",
  "acceptTerms": true
}
```

**Validation Rules:**

| Field             | Rules                                                            | Business Rule |
|-------------------|------------------------------------------------------------------|---------------|
| `firstName`       | Required, 2–100 characters, alphabetic                           | —             |
| `lastName`        | Required, 2–100 characters, alphabetic                           | —             |
| `email`           | Required, valid email format, unique                             | REG-R01       |
| `phoneNumber`     | Required, valid international format, unique                     | REG-R02       |
| `password`        | Required, min 8 chars, 1 upper, 1 number, 1 special             | AUTH-R02      |
| `confirmPassword` | Required, must match `password`                                  | —             |
| `acceptTerms`     | Required, must be `true`                                         | REG-R03       |

**Response — 201 Created:**

```json
{
  "success": true,
  "message": "Registration successful. Please verify your email.",
  "data": {
    "userId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "email": "ahmed@example.com",
    "emailVerificationRequired": true
  }
}
```

**Error Responses:**

| Status | Condition                  | Error Type                                |
|--------|----------------------------|-------------------------------------------|
| `400`  | Malformed request body     | `validation-error`                        |
| `409`  | Email already exists       | `duplicate-email`                         |
| `409`  | Phone already exists       | `duplicate-phone`                         |
| `422`  | Terms not accepted         | `terms-not-accepted`                      |
| `429`  | Rate limit exceeded        | `rate-limit-exceeded`                     |

---

### 5.2 Register Doctor

**Purpose:** Create a new doctor account with professional information.

| Attribute          | Value                                      |
|--------------------|--------------------------------------------|
| **Method**         | `POST`                                     |
| **URL**            | `/api/v1/auth/register/doctor`             |
| **Authentication** | None                                       |
| **Roles**          | Public                                     |

**Request Body:**

```json
{
  "firstName": "Ayesha",
  "lastName": "Malik",
  "email": "dr.ayesha@example.com",
  "phoneNumber": "+923009876543",
  "password": "SecureP@ss1",
  "confirmPassword": "SecureP@ss1",
  "specializationIds": ["uuid-dermatology", "uuid-cosmetology"],
  "yearsOfExperience": 10,
  "consultationFee": 1500.00,
  "licenseNumber": "PMC-12345",
  "licenseAuthority": "Pakistan Medical Commission",
  "acceptTerms": true
}
```

**Validation Rules:**

| Field                | Rules                                                        | Business Rule |
|----------------------|--------------------------------------------------------------|---------------|
| `firstName`          | Required, 2–100 characters                                   | —             |
| `lastName`           | Required, 2–100 characters                                   | —             |
| `email`              | Required, valid email, unique                                | REG-R01       |
| `phoneNumber`        | Required, valid format, unique                               | REG-R02       |
| `password`           | Required, min 8, 1 upper, 1 number, 1 special               | AUTH-R02      |
| `specializationIds`  | Required, at least 1, valid UUID(s)                          | REG-R21       |
| `yearsOfExperience`  | Required, integer ≥ 0                                        | REG-R21       |
| `consultationFee`    | Required, decimal > 0                                        | REG-R21       |
| `licenseNumber`      | Required, non-empty string                                   | REG-R22       |
| `licenseAuthority`   | Required, non-empty string                                   | REG-R22       |
| `acceptTerms`        | Required, must be `true`                                     | REG-R03       |

**Response — 201 Created:**

```json
{
  "success": true,
  "message": "Registration successful. Please verify your email.",
  "data": {
    "userId": "b2c3d4e5-f6a7-8901-bcde-f23456789012",
    "email": "dr.ayesha@example.com",
    "verificationStatus": "Unverified",
    "emailVerificationRequired": true
  }
}
```

**Business Notes:**
- License data is stored but **not verified** in Phase 1 (REG-R23).
- Doctor is active upon email verification — no admin approval gate (REG-R24).

---

### 5.3 Verify Email

**Purpose:** Verify user email via OTP code sent during registration.

| Attribute          | Value                                      |
|--------------------|--------------------------------------------|
| **Method**         | `POST`                                     |
| **URL**            | `/api/v1/auth/verify-email`                |
| **Authentication** | None                                       |
| **Roles**          | Public                                     |

**Request Body:**

```json
{
  "email": "ahmed@example.com",
  "otpCode": "584723"
}
```

**Response — 200 OK:**

```json
{
  "success": true,
  "message": "Email verified successfully.",
  "data": {
    "emailConfirmed": true
  }
}
```

**Error Responses:**

| Status | Condition               | Error Type                 |
|--------|--------------------------|----------------------------|
| `400`  | Invalid OTP format       | `validation-error`         |
| `404`  | Email not found          | `user-not-found`           |
| `410`  | OTP expired              | `otp-expired`              |
| `422`  | Incorrect OTP            | `invalid-otp`              |
| `429`  | Too many attempts        | `rate-limit-exceeded`      |

---

### 5.4 Resend Verification OTP

**Purpose:** Request a new OTP for email verification.

| Attribute          | Value                                      |
|--------------------|--------------------------------------------|
| **Method**         | `POST`                                     |
| **URL**            | `/api/v1/auth/resend-verification`         |
| **Authentication** | None                                       |
| **Roles**          | Public                                     |

**Request Body:**

```json
{
  "email": "ahmed@example.com"
}
```

**Response — 200 OK:**

```json
{
  "success": true,
  "message": "Verification code sent to your email."
}
```

---

### 5.5 Login

**Purpose:** Authenticate a user and issue JWT + refresh token.

| Attribute          | Value                                      |
|--------------------|--------------------------------------------|
| **Method**         | `POST`                                     |
| **URL**            | `/api/v1/auth/login`                       |
| **Authentication** | None                                       |
| **Roles**          | Public                                     |

**Request Body:**

```json
{
  "email": "ahmed@example.com",
  "password": "SecureP@ss1",
  "deviceInfo": "Mozilla/5.0 Chrome/120 Windows"
}
```

**Response — 200 OK:**

```json
{
  "success": true,
  "message": "Login successful.",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2g...",
    "tokenType": "Bearer",
    "expiresIn": 900,
    "user": {
      "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "firstName": "Ahmed",
      "lastName": "Khan",
      "email": "ahmed@example.com",
      "role": "Patient",
      "profilePhotoUrl": null,
      "emailConfirmed": true
    }
  }
}
```

**Error Responses:**

| Status | Condition                     | Error Type                  |
|--------|-------------------------------|-----------------------------|
| `401`  | Invalid credentials           | `invalid-credentials`       |
| `401`  | Email not verified            | `email-not-verified`        |
| `401`  | Account suspended             | `account-suspended`         |
| `423`  | Account locked (5 failures)   | `account-locked`            |
| `429`  | Rate limit exceeded           | `rate-limit-exceeded`       |

**Business Notes:**
- `expiresIn` is in seconds (default 900 = 15 minutes; AUTH-R04).
- Failed login increments `FailedLoginAttempts`; 5 failures locks the account for 15 minutes (AUTH-R07, AUTH-R08).
- `deviceInfo` is stored with the refresh token for per-device session management (AUTH-R13).

---

### 5.6 Refresh Token

**Purpose:** Exchange a valid refresh token for a new token pair. Single-use rotation.

| Attribute          | Value                                      |
|--------------------|--------------------------------------------|
| **Method**         | `POST`                                     |
| **URL**            | `/api/v1/auth/refresh`                     |
| **Authentication** | None                                       |
| **Roles**          | Public                                     |

**Request Body:**

```json
{
  "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2g..."
}
```

**Response — 200 OK:**

```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "bmV3IHJlZnJlc2ggdG9rZW4...",
    "tokenType": "Bearer",
    "expiresIn": 900
  }
}
```

**Error Responses:**

| Status | Condition               | Error Type                    |
|--------|--------------------------|-------------------------------|
| `401`  | Token revoked or expired | `invalid-refresh-token`       |
| `401`  | Token already used       | `refresh-token-reused`        |

**Business Notes:**
- Old refresh token is immediately invalidated upon use (AUTH-R05).
- Reuse of a previously-used token triggers a security alert and invalidates all tokens for that user.

---

### 5.7 Logout

**Purpose:** Invalidate the current refresh token.

| Attribute          | Value                                      |
|--------------------|--------------------------------------------|
| **Method**         | `POST`                                     |
| **URL**            | `/api/v1/auth/logout`                      |
| **Authentication** | Required                                   |
| **Roles**          | All authenticated users                    |

**Request Body:**

```json
{
  "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2g..."
}
```

**Response — 200 OK:**

```json
{
  "success": true,
  "message": "Logged out successfully."
}
```

**Business Notes:**
- Logout invalidates the refresh token only. The JWT remains valid until natural expiry (AUTH-R12).
- Only the session on the current device is terminated (AUTH-R13).

---

### 5.8 Forgot Password

**Purpose:** Initiate a password reset by sending a reset link/token to the user's email.

| Attribute          | Value                                      |
|--------------------|--------------------------------------------|
| **Method**         | `POST`                                     |
| **URL**            | `/api/v1/auth/forgot-password`             |
| **Authentication** | None                                       |
| **Roles**          | Public                                     |

**Request Body:**

```json
{
  "email": "ahmed@example.com"
}
```

**Response — 200 OK:**

```json
{
  "success": true,
  "message": "If an account with that email exists, a password reset link has been sent."
}
```

**Business Notes:**
- Response is always 200 to prevent email enumeration attacks.
- Reset token expires after 1 hour (AUTH-R11).

---

### 5.9 Reset Password

**Purpose:** Set a new password using the reset token received via email.

| Attribute          | Value                                      |
|--------------------|--------------------------------------------|
| **Method**         | `POST`                                     |
| **URL**            | `/api/v1/auth/reset-password`              |
| **Authentication** | None                                       |
| **Roles**          | Public                                     |

**Request Body:**

```json
{
  "email": "ahmed@example.com",
  "token": "reset-token-from-email",
  "newPassword": "NewSecureP@ss1",
  "confirmNewPassword": "NewSecureP@ss1"
}
```

**Response — 200 OK:**

```json
{
  "success": true,
  "message": "Password has been reset successfully. Please log in."
}
```

**Error Responses:**

| Status | Condition                    | Error Type                |
|--------|------------------------------|---------------------------|
| `400`  | Token invalid or expired     | `invalid-reset-token`     |
| `422`  | Password doesn't meet policy | `password-policy-violation`|

---

### 5.10 Change Password

**Purpose:** Change password for an already authenticated user.

| Attribute          | Value                                      |
|--------------------|--------------------------------------------|
| **Method**         | `POST`                                     |
| **URL**            | `/api/v1/auth/change-password`             |
| **Authentication** | Required                                   |
| **Roles**          | All authenticated users                    |

**Request Body:**

```json
{
  "currentPassword": "OldSecureP@ss1",
  "newPassword": "NewSecureP@ss2",
  "confirmNewPassword": "NewSecureP@ss2"
}
```

**Response — 200 OK:**

```json
{
  "success": true,
  "message": "Password changed successfully."
}
```

---

## 6. Patient Endpoints

### 6.1 Get Patient Profile

**Purpose:** Retrieve the authenticated patient's full profile.

| Attribute          | Value                                      |
|--------------------|--------------------------------------------|
| **Method**         | `GET`                                      |
| **URL**            | `/api/v1/patients/profile`                 |
| **Authentication** | Required                                   |
| **Roles**          | `Patient`                                  |

**Response — 200 OK:**

```json
{
  "success": true,
  "data": {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "firstName": "Ahmed",
    "lastName": "Khan",
    "email": "ahmed@example.com",
    "phoneNumber": "+923001234567",
    "profilePhotoUrl": "https://cdn.medichp.com/photos/a1b2c3d4.jpg",
    "dateOfBirth": "1998-05-15",
    "gender": "Male",
    "bloodType": "A+",
    "city": {
      "id": "city-uuid",
      "name": "Lahore"
    },
    "address": "123 Main Street, Gulberg III",
    "dataSharingConsent": true,
    "profileCompletionPct": 75,
    "emergencyContacts": [
      {
        "id": "ec-uuid-1",
        "fullName": "Fatima Khan",
        "relationship": "Spouse",
        "phoneNumber": "+923001111111",
        "isPrimary": true
      }
    ],
    "allergies": [
      {
        "id": "allergy-uuid-1",
        "allergyName": "Penicillin",
        "severity": "Severe",
        "notes": "Causes rash and swelling"
      }
    ],
    "chronicConditions": [
      {
        "id": "condition-uuid-1",
        "conditionName": "Asthma",
        "diagnosedDate": "2015-03-01",
        "notes": "Well controlled with inhaler"
      }
    ],
    "currentMedications": [
      {
        "id": "med-uuid-1",
        "medicationName": "Ventolin Inhaler",
        "dosage": "100mcg",
        "frequency": "As needed"
      }
    ],
    "createdAt": "2026-08-01T10:00:00Z"
  }
}
```

**Business Notes:**
- Returns all data for all progressive registration stages (REG-R11–R14).
- `profileCompletionPct` is calculated based on filled fields (REG-R16).

---

### 6.2 Update Patient Profile

**Purpose:** Update the patient's personal and health information.

| Attribute          | Value                                      |
|--------------------|--------------------------------------------|
| **Method**         | `PATCH`                                    |
| **URL**            | `/api/v1/patients/profile`                 |
| **Authentication** | Required                                   |
| **Roles**          | `Patient`                                  |

**Request Body (partial update):**

```json
{
  "dateOfBirth": "1998-05-15",
  "gender": "Male",
  "bloodType": "A+",
  "cityId": "city-uuid",
  "address": "123 Main Street, Gulberg III",
  "dataSharingConsent": true
}
```

**Response — 200 OK:**

```json
{
  "success": true,
  "message": "Profile updated successfully.",
  "data": { "profileCompletionPct": 85 }
}
```

---

### 6.3 Manage Emergency Contacts

| Endpoint                                           | Method   | Purpose                 |
|----------------------------------------------------|----------|-------------------------|
| `/api/v1/patients/emergency-contacts`              | `GET`    | List all contacts       |
| `/api/v1/patients/emergency-contacts`              | `POST`   | Add a contact           |
| `/api/v1/patients/emergency-contacts/{id}`         | `PUT`    | Update a contact        |
| `/api/v1/patients/emergency-contacts/{id}`         | `DELETE` | Remove a contact        |

### 6.4 Manage Allergies

| Endpoint                                           | Method   | Purpose                 |
|----------------------------------------------------|----------|-------------------------|
| `/api/v1/patients/allergies`                       | `GET`    | List all allergies      |
| `/api/v1/patients/allergies`                       | `POST`   | Add an allergy          |
| `/api/v1/patients/allergies/{id}`                  | `PUT`    | Update an allergy       |
| `/api/v1/patients/allergies/{id}`                  | `DELETE` | Remove an allergy       |

### 6.5 Manage Chronic Conditions

| Endpoint                                           | Method   | Purpose                 |
|----------------------------------------------------|----------|-------------------------|
| `/api/v1/patients/chronic-conditions`              | `GET`    | List all conditions     |
| `/api/v1/patients/chronic-conditions`              | `POST`   | Add a condition         |
| `/api/v1/patients/chronic-conditions/{id}`         | `PUT`    | Update a condition      |
| `/api/v1/patients/chronic-conditions/{id}`         | `DELETE` | Remove a condition      |

### 6.6 Manage Current Medications

| Endpoint                                           | Method   | Purpose                 |
|----------------------------------------------------|----------|-------------------------|
| `/api/v1/patients/medications`                     | `GET`    | List all medications    |
| `/api/v1/patients/medications`                     | `POST`   | Add a medication        |
| `/api/v1/patients/medications/{id}`                | `PUT`    | Update a medication     |
| `/api/v1/patients/medications/{id}`                | `DELETE` | Remove a medication     |

### 6.7 Patient Dashboard

**Purpose:** Retrieve aggregated dashboard data for the patient.

| Attribute          | Value                                      |
|--------------------|--------------------------------------------|
| **Method**         | `GET`                                      |
| **URL**            | `/api/v1/patients/dashboard`               |
| **Authentication** | Required                                   |
| **Roles**          | `Patient`                                  |

**Response — 200 OK:**

```json
{
  "success": true,
  "data": {
    "upcomingAppointments": [
      {
        "id": "appt-uuid-1",
        "doctorName": "Dr. Ayesha Malik",
        "doctorSpecialization": "Dermatology",
        "scheduledAt": "2026-08-10T14:00:00Z",
        "status": "Confirmed"
      }
    ],
    "recentPrescriptions": [
      {
        "id": "rx-uuid-1",
        "doctorName": "Dr. Ayesha Malik",
        "issuedAt": "2026-08-05T10:00:00Z",
        "medicationCount": 3
      }
    ],
    "unreadMessages": 2,
    "unreadNotifications": 5,
    "profileCompletionPct": 75
  }
}
```

---

## 7. Doctor Endpoints

### 7.1 Get Doctor Profile (Own)

| Attribute          | Value                                      |
|--------------------|--------------------------------------------|
| **Method**         | `GET`                                      |
| **URL**            | `/api/v1/doctors/profile`                  |
| **Authentication** | Required                                   |
| **Roles**          | `Doctor`                                   |

**Response — 200 OK:**

```json
{
  "success": true,
  "data": {
    "id": "doc-profile-uuid",
    "userId": "user-uuid",
    "firstName": "Ayesha",
    "lastName": "Malik",
    "email": "dr.ayesha@example.com",
    "phoneNumber": "+923009876543",
    "profilePhotoUrl": "https://cdn.medichp.com/photos/doc.jpg",
    "bio": "Board-certified dermatologist with 10 years of experience...",
    "specializations": [
      { "id": "spec-uuid", "name": "Dermatology", "isPrimary": true }
    ],
    "yearsOfExperience": 10,
    "consultationFee": 1500.00,
    "feeCurrency": "PKR",
    "licenseNumber": "PMC-12345",
    "licenseAuthority": "Pakistan Medical Commission",
    "verificationStatus": "Unverified",
    "slotDurationMinutes": 30,
    "city": { "id": "city-uuid", "name": "Lahore" },
    "address": "456 Medical Plaza, DHA Phase 5",
    "averageRating": 4.5,
    "totalReviews": 28,
    "createdAt": "2026-07-15T08:00:00Z"
  }
}
```

### 7.2 Update Doctor Profile

| Attribute          | Value                                      |
|--------------------|--------------------------------------------|
| **Method**         | `PATCH`                                    |
| **URL**            | `/api/v1/doctors/profile`                  |
| **Authentication** | Required                                   |
| **Roles**          | `Doctor`                                   |

**Request Body (partial update):**

```json
{
  "bio": "Updated bio text...",
  "consultationFee": 2000.00,
  "cityId": "city-uuid",
  "address": "New clinic address",
  "slotDurationMinutes": 20,
  "specializationIds": ["spec-uuid-1", "spec-uuid-2"]
}
```

### 7.3 Doctor Availability

#### Set Recurring Availability

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `PUT`                                        |
| **URL**            | `/api/v1/doctors/availability`               |
| **Authentication** | Required                                     |
| **Roles**          | `Doctor`                                     |

**Request Body:**

```json
{
  "schedule": [
    { "dayOfWeek": 1, "startTime": "09:00", "endTime": "13:00", "isActive": true },
    { "dayOfWeek": 1, "startTime": "15:00", "endTime": "18:00", "isActive": true },
    { "dayOfWeek": 2, "startTime": "09:00", "endTime": "17:00", "isActive": true },
    { "dayOfWeek": 3, "startTime": "09:00", "endTime": "13:00", "isActive": true },
    { "dayOfWeek": 5, "startTime": "10:00", "endTime": "14:00", "isActive": true }
  ]
}
```

> **Business Note:** `dayOfWeek` uses ISO 8601: 0 = Sunday, 6 = Saturday (APT-R30).

#### Get Recurring Availability

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/doctors/availability`               |
| **Authentication** | Required                                     |
| **Roles**          | `Doctor`                                     |

#### Set Unavailability (Holiday/Leave)

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `POST`                                       |
| **URL**            | `/api/v1/doctors/unavailability`             |
| **Authentication** | Required                                     |
| **Roles**          | `Doctor`                                     |

**Request Body:**

```json
{
  "unavailableDate": "2026-08-14",
  "reason": "National Holiday",
  "isFullDay": true
}
```

#### Get Unavailability List

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/doctors/unavailability`             |
| **Authentication** | Required                                     |
| **Roles**          | `Doctor`                                     |

**Query Parameters:** `?fromDate=2026-08-01&toDate=2026-08-31`

#### Delete Unavailability

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `DELETE`                                     |
| **URL**            | `/api/v1/doctors/unavailability/{id}`        |
| **Authentication** | Required                                     |
| **Roles**          | `Doctor`                                     |

### 7.4 Get Available Slots (Public)

**Purpose:** Retrieve bookable time slots for a specific doctor on a date range. Used by patients during booking.

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/doctors/{doctorId}/slots`           |
| **Authentication** | None                                         |
| **Roles**          | Public                                       |

**Query Parameters:**

| Parameter | Type   | Required | Description              |
|-----------|--------|----------|--------------------------|
| `date`    | `date` | Yes      | Date to check (YYYY-MM-DD) |

**Response — 200 OK:**

```json
{
  "success": true,
  "data": {
    "doctorId": "doctor-user-uuid",
    "date": "2026-08-10",
    "slotDurationMinutes": 30,
    "slots": [
      { "startTime": "09:00", "endTime": "09:30", "isAvailable": true },
      { "startTime": "09:30", "endTime": "10:00", "isAvailable": false },
      { "startTime": "10:00", "endTime": "10:30", "isAvailable": true },
      { "startTime": "10:30", "endTime": "11:00", "isAvailable": true }
    ]
  }
}
```

**Business Notes:**
- Slots are auto-generated from `DoctorAvailabilities` minus `DoctorUnavailabilities` minus booked `Appointments` (APT-R32, APT-R34).
- `isAvailable = false` when a confirmed/pending appointment occupies the slot (APT-R04).

### 7.5 Doctor's Patient List

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/doctors/patients`                   |
| **Authentication** | Required                                     |
| **Roles**          | `Doctor`                                     |

**Query Parameters:** `?page=1&pageSize=20&q=Ahmed&sortBy=lastVisit&sortOrder=desc`

**Business Notes:**
- Only returns patients who have had at least one appointment with this doctor (DOC-R04).
- Excludes patients who have revoked data sharing consent (DOC-R05).

### 7.6 Doctor-Initiated Patient Creation

**Purpose:** Doctor adds a walk-in patient who doesn't have a MedicHp account.

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `POST`                                       |
| **URL**            | `/api/v1/doctors/patients`                   |
| **Authentication** | Required                                     |
| **Roles**          | `Doctor`                                     |

**Request Body:**

```json
{
  "firstName": "Walk-in",
  "lastName": "Patient",
  "email": "walkin@example.com",
  "phoneNumber": "+923001112222"
}
```

**Response — 201 Created:**

```json
{
  "success": true,
  "message": "Patient created. Invitation email sent.",
  "data": {
    "patientUserId": "new-patient-uuid",
    "invitationSent": true
  }
}
```

**Business Notes:**
- Creates a provisional account (DOC-R11). Patient can record consultations immediately (DOC-R13).
- Invitation email sent with activation link (DOC-R12).

### 7.7 Doctor Dashboard

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/doctors/dashboard`                  |
| **Authentication** | Required                                     |
| **Roles**          | `Doctor`                                     |

**Response — 200 OK:**

```json
{
  "success": true,
  "data": {
    "todaysAppointments": [
      {
        "id": "appt-uuid",
        "patientName": "Ahmed Khan",
        "scheduledAt": "2026-08-06T14:00:00Z",
        "status": "Confirmed",
        "bookingNote": "Skin rash on hands"
      }
    ],
    "pendingRequests": 3,
    "totalPatients": 142,
    "recentConsultations": [
      {
        "id": "consult-uuid",
        "patientName": "Sara Ali",
        "diagnosis": "Allergic Contact Dermatitis",
        "createdAt": "2026-08-05T11:00:00Z"
      }
    ],
    "unreadMessages": 5,
    "unreadNotifications": 8
  }
}
```

---

## 8. Appointment Endpoints

### 8.1 Book Appointment

**Purpose:** Patient books an appointment with a doctor.

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `POST`                                       |
| **URL**            | `/api/v1/appointments`                       |
| **Authentication** | Required                                     |
| **Roles**          | `Patient`                                    |

**Request Body:**

```json
{
  "doctorId": "doctor-user-uuid",
  "scheduledAt": "2026-08-10T14:00:00Z",
  "bookingNote": "Persistent skin rash on both hands for 2 weeks"
}
```

**Validation Rules:**

| Field          | Rules                                                               | Business Rule |
|----------------|---------------------------------------------------------------------|---------------|
| `doctorId`     | Required, valid UUID, doctor must exist and be active               | —             |
| `scheduledAt`  | Required, future date, at least 1 hour from now                    | APT-R02       |
| `scheduledAt`  | Must fall within doctor's available slot                            | APT-R01       |
| `scheduledAt`  | Must not conflict with an existing appointment for this doctor     | APT-R04       |
| `bookingNote`  | Optional, max 500 characters                                       | APT-R06       |

**Response — 201 Created:**

```json
{
  "success": true,
  "message": "Appointment booked successfully.",
  "data": {
    "id": "appt-uuid",
    "doctorId": "doctor-user-uuid",
    "doctorName": "Dr. Ayesha Malik",
    "scheduledAt": "2026-08-10T14:00:00Z",
    "durationMinutes": 30,
    "status": "Pending",
    "bookingNote": "Persistent skin rash on both hands for 2 weeks",
    "createdAt": "2026-08-06T10:15:00Z"
  }
}
```

**Error Responses:**

| Status | Condition                          | Error Type                    | Business Rule |
|--------|------------------------------------|-------------------------------|---------------|
| `409`  | Time slot already booked           | `slot-conflict`               | APT-R04       |
| `409`  | Active appointment already exists  | `active-appointment-exists`   | APT-R03       |
| `422`  | Less than 1 hour advance           | `insufficient-advance-time`   | APT-R02       |
| `422`  | Slot not in doctor's availability  | `slot-not-available`          | APT-R01       |

### 8.2 Get Appointment Details

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/appointments/{id}`                  |
| **Authentication** | Required                                     |
| **Roles**          | `Patient` (own), `Doctor` (own)              |

### 8.3 List My Appointments

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/appointments`                       |
| **Authentication** | Required                                     |
| **Roles**          | `Patient`, `Doctor`                          |

**Query Parameters:**

| Parameter    | Type     | Description                              |
|--------------|----------|------------------------------------------|
| `status`     | `string` | Filter: `Pending,Confirmed,Completed,Cancelled` |
| `fromDate`   | `date`   | Start of date range                      |
| `toDate`     | `date`   | End of date range                        |
| `upcoming`   | `bool`   | Only future appointments                 |
| `sortBy`     | `string` | `scheduledAt` (default)                  |
| `sortOrder`  | `string` | `asc` (default for upcoming), `desc`     |
| `page`       | `int`    | Page number                              |
| `pageSize`   | `int`    | Items per page                           |

### 8.4 Accept/Reject Appointment (Doctor)

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `PATCH`                                      |
| **URL**            | `/api/v1/appointments/{id}/status`           |
| **Authentication** | Required                                     |
| **Roles**          | `Doctor`                                     |

**Request Body:**

```json
{
  "status": "Confirmed",
  "reason": null
}
```

**Valid Status Transitions (APT-R20):**

| Current Status | Allowed Transitions     | Who            |
|----------------|-------------------------|----------------|
| `Pending`      | `Confirmed`, `Cancelled`| Doctor         |
| `Confirmed`    | `InProgress`, `Cancelled`| Doctor (InProgress), Both (Cancel) |
| `InProgress`   | `Completed`             | Doctor         |

**Error Responses:**

| Status | Condition                     | Error Type                     |
|--------|-------------------------------|--------------------------------|
| `403`  | Not the assigned doctor       | `not-assigned-doctor`          |
| `422`  | Invalid status transition     | `invalid-status-transition`    |

### 8.5 Cancel Appointment

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `POST`                                       |
| **URL**            | `/api/v1/appointments/{id}/cancel`           |
| **Authentication** | Required                                     |
| **Roles**          | `Patient`, `Doctor`                          |

**Request Body:**

```json
{
  "reason": "Schedule conflict — need to reschedule"
}
```

**Validation Rules:**

| Role      | Rule                                                        | Business Rule |
|-----------|-------------------------------------------------------------|---------------|
| `Patient` | Must be at least 4 hours before scheduled time              | APT-R10       |
| `Doctor`  | Can cancel at any time; reason is mandatory                 | APT-R12       |

**Error Responses:**

| Status | Condition                            | Error Type                           |
|--------|--------------------------------------|--------------------------------------|
| `422`  | Patient cancelling < 4 hours before  | `cancellation-window-exceeded`       |
| `422`  | Already cancelled or completed       | `invalid-status-transition`          |

### 8.6 Reschedule Appointment

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `POST`                                       |
| **URL**            | `/api/v1/appointments/{id}/reschedule`       |
| **Authentication** | Required                                     |
| **Roles**          | `Patient`                                    |

**Request Body:**

```json
{
  "newScheduledAt": "2026-08-12T10:00:00Z"
}
```

**Business Notes:**
- Creates a new appointment and cancels the old one (APT-R13).
- Original booking note is preserved (APT-R14).
- Subject to advance booking policy (APT-R02).

### 8.7 Doctor's Daily Schedule

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/doctors/schedule`                   |
| **Authentication** | Required                                     |
| **Roles**          | `Doctor`                                     |

**Query Parameters:** `?date=2026-08-10`

**Response — 200 OK:**

```json
{
  "success": true,
  "data": {
    "date": "2026-08-10",
    "appointments": [
      {
        "id": "appt-uuid-1",
        "patientName": "Ahmed Khan",
        "scheduledAt": "2026-08-10T09:00:00Z",
        "durationMinutes": 30,
        "status": "Confirmed",
        "bookingNote": "Skin rash"
      },
      {
        "id": "appt-uuid-2",
        "patientName": "Sara Ali",
        "scheduledAt": "2026-08-10T09:30:00Z",
        "durationMinutes": 30,
        "status": "Pending",
        "bookingNote": null
      }
    ],
    "availableSlots": [
      { "startTime": "10:00", "endTime": "10:30" },
      { "startTime": "10:30", "endTime": "11:00" }
    ]
  }
}
```

---

## 9. Consultation Endpoints

### 9.1 Create Consultation

**Purpose:** Doctor creates a clinical record for a completed appointment.

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `POST`                                       |
| **URL**            | `/api/v1/consultations`                      |
| **Authentication** | Required                                     |
| **Roles**          | `Doctor`                                     |

**Request Body:**

```json
{
  "appointmentId": "appt-uuid",
  "chiefComplaint": "Persistent skin rash on both hands",
  "symptoms": "Itching, redness, scaling, dry patches on dorsum of both hands",
  "diagnosis": "Allergic Contact Dermatitis",
  "treatmentPlan": "Avoid irritants, topical corticosteroids, moisturizer",
  "clinicalNotes": "Patient reports onset 2 weeks ago after using new detergent.",
  "vitals": {
    "bloodPressureSystolic": 120,
    "bloodPressureDiastolic": 80,
    "temperatureCelsius": 37.0,
    "weightKg": 72.5,
    "heartRateBpm": 76,
    "notes": "All vitals within normal range"
  }
}
```

**Validation Rules:**

| Field              | Rules                                                      | Business Rule |
|--------------------|-------------------------------------------------------------|---------------|
| `appointmentId`    | Required, must reference a `Completed` appointment          | CON-R01       |
| `appointmentId`    | Must not already have a consultation                        | CON-R03       |
| `chiefComplaint`   | Required, max 500 characters                                | CON-R04       |
| `symptoms`         | Required                                                    | CON-R04       |
| `diagnosis`        | Required                                                    | CON-R04       |
| `treatmentPlan`    | Required                                                    | CON-R04       |
| `clinicalNotes`    | Optional                                                    | CON-R04       |
| `vitals`           | Optional object                                             | CON-R10       |

**Response — 201 Created:**

```json
{
  "success": true,
  "message": "Consultation record created.",
  "data": {
    "id": "consult-uuid",
    "appointmentId": "appt-uuid",
    "isFinalized": false,
    "createdAt": "2026-08-06T15:00:00Z"
  }
}
```

**Error Responses:**

| Status | Condition                         | Error Type                         | Business Rule |
|--------|-----------------------------------|------------------------------------|---------------|
| `403`  | Not the assigned doctor           | `not-assigned-doctor`              | CON-R02       |
| `409`  | Consultation already exists       | `consultation-already-exists`      | CON-R03       |
| `422`  | Appointment not in Completed state| `appointment-not-completed`        | CON-R01       |

### 9.2 Update Consultation (Before Finalization)

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `PATCH`                                      |
| **URL**            | `/api/v1/consultations/{id}`                 |
| **Authentication** | Required                                     |
| **Roles**          | `Doctor`                                     |

**Error — 403 if `IsFinalized = true`** (CON-R06).

### 9.3 Finalize Consultation

**Purpose:** Mark a consultation as finalized and immutable.

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `POST`                                       |
| **URL**            | `/api/v1/consultations/{id}/finalize`        |
| **Authentication** | Required                                     |
| **Roles**          | `Doctor`                                     |

**Response — 200 OK:**

```json
{
  "success": true,
  "message": "Consultation finalized. No further edits are permitted.",
  "data": {
    "id": "consult-uuid",
    "isFinalized": true,
    "finalizedAt": "2026-08-06T15:30:00Z"
  }
}
```

**Business Notes:**
- After finalization, the record is immutable (CON-R06). Any corrections must go through the addendum endpoint (CON-R07).

### 9.4 Add Consultation Addendum

**Purpose:** Append a correction or additional note to a finalized consultation.

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `POST`                                       |
| **URL**            | `/api/v1/consultations/{id}/addenda`         |
| **Authentication** | Required                                     |
| **Roles**          | `Doctor`                                     |

**Request Body:**

```json
{
  "content": "Updated diagnosis: Nummular Eczema based on follow-up test results.",
  "reason": "Lab results received after initial consultation"
}
```

### 9.5 Get Consultation Details

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/consultations/{id}`                 |
| **Authentication** | Required                                     |
| **Roles**          | `Patient` (own), `Doctor` (own)              |

**Response includes:** Consultation fields, vitals, addenda list, linked prescriptions.

### 9.6 List Consultations

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/consultations`                      |
| **Authentication** | Required                                     |
| **Roles**          | `Patient` (own), `Doctor` (own)              |

**Query Parameters:** `?patientId={uuid}&fromDate=...&toDate=...&page=1&pageSize=20`

---

## 10. Prescription Endpoints

### 10.1 Create Prescription

**Purpose:** Doctor issues a digital prescription linked to a consultation.

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `POST`                                       |
| **URL**            | `/api/v1/prescriptions`                      |
| **Authentication** | Required                                     |
| **Roles**          | `Doctor`                                     |

**Request Body:**

```json
{
  "consultationId": "consult-uuid",
  "items": [
    {
      "medicationName": "Betamethasone Cream 0.1%",
      "dosage": "Apply thin layer",
      "frequency": "Twice daily",
      "duration": "14 days",
      "instructions": "Apply to affected areas after washing hands"
    },
    {
      "medicationName": "Cetirizine 10mg",
      "dosage": "1 tablet",
      "frequency": "Once daily at bedtime",
      "duration": "7 days",
      "instructions": "May cause drowsiness"
    }
  ],
  "notes": "Follow up in 2 weeks if no improvement."
}
```

**Validation Rules:**

| Field              | Rules                                                      | Business Rule |
|--------------------|-------------------------------------------------------------|---------------|
| `consultationId`   | Required, valid UUID, must exist                            | RX-R02        |
| `items`            | Required, at least 1 item                                   | RX-R03        |
| `items[].medicationName` | Required, max 200 chars                              | RX-R04        |
| `items[].dosage`   | Required                                                    | RX-R04        |
| `items[].frequency`| Required                                                    | RX-R04        |
| `items[].duration` | Required                                                    | RX-R04        |

**Response — 201 Created:**

```json
{
  "success": true,
  "message": "Prescription issued successfully.",
  "data": {
    "id": "rx-uuid",
    "consultationId": "consult-uuid",
    "issuedAt": "2026-08-06T15:35:00Z",
    "itemCount": 2
  }
}
```

### 10.2 Get Prescription Details

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/prescriptions/{id}`                 |
| **Authentication** | Required                                     |
| **Roles**          | `Patient` (own), `Doctor` (own)              |

**Response — 200 OK:**

```json
{
  "success": true,
  "data": {
    "id": "rx-uuid",
    "consultationId": "consult-uuid",
    "doctor": {
      "name": "Dr. Ayesha Malik",
      "specialization": "Dermatology",
      "licenseNumber": "PMC-12345"
    },
    "patient": {
      "name": "Ahmed Khan"
    },
    "issuedAt": "2026-08-06T15:35:00Z",
    "isSuperseded": false,
    "items": [
      {
        "id": "item-uuid-1",
        "medicationName": "Betamethasone Cream 0.1%",
        "dosage": "Apply thin layer",
        "frequency": "Twice daily",
        "duration": "14 days",
        "instructions": "Apply to affected areas after washing hands",
        "sortOrder": 1
      },
      {
        "id": "item-uuid-2",
        "medicationName": "Cetirizine 10mg",
        "dosage": "1 tablet",
        "frequency": "Once daily at bedtime",
        "duration": "7 days",
        "instructions": "May cause drowsiness",
        "sortOrder": 2
      }
    ],
    "notes": "Follow up in 2 weeks if no improvement."
  }
}
```

### 10.3 List Prescriptions

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/prescriptions`                      |
| **Authentication** | Required                                     |
| **Roles**          | `Patient` (own), `Doctor` (own)              |

**Query Parameters:** `?patientId={uuid}&fromDate=...&toDate=...&page=1&pageSize=20&sortBy=issuedAt&sortOrder=desc`

### 10.4 Supersede Prescription (Correction)

**Purpose:** Issue a corrected prescription, marking the original as superseded.

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `POST`                                       |
| **URL**            | `/api/v1/prescriptions/{id}/supersede`       |
| **Authentication** | Required                                     |
| **Roles**          | `Doctor`                                     |

**Request Body:** Same as Create Prescription (§10.1). The original prescription is automatically marked `isSuperseded = true`.

> **Business Note:** Prescriptions are immutable (RX-R05). Corrections create a new prescription and mark the old one as superseded (RX-R06).

### 10.5 Download Prescription PDF (Future Ready)

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/prescriptions/{id}/pdf`             |
| **Authentication** | Required                                     |
| **Roles**          | `Patient` (own), `Doctor` (own)              |

**Response:** `Content-Type: application/pdf`

> **Status:** Endpoint reserved. Implementation in Phase 1 returns the PDF. Full template customization in Phase 2.

---

## 11. Chat Endpoints

### 11.1 List Conversations

**Purpose:** Retrieve the authenticated user's conversation list.

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/conversations`                      |
| **Authentication** | Required                                     |
| **Roles**          | `Patient`, `Doctor`                          |

**Response — 200 OK:**

```json
{
  "success": true,
  "data": [
    {
      "id": "conv-uuid-1",
      "participant": {
        "id": "user-uuid",
        "name": "Dr. Ayesha Malik",
        "role": "Doctor",
        "profilePhotoUrl": "https://cdn.medichp.com/photos/doc.jpg"
      },
      "lastMessage": {
        "content": "Please share photos of the affected area.",
        "sentAt": "2026-08-06T14:30:00Z",
        "isFromMe": false
      },
      "unreadCount": 1,
      "lastMessageAt": "2026-08-06T14:30:00Z"
    }
  ],
  "pagination": { "page": 1, "pageSize": 20, "totalRecords": 3 }
}
```

### 11.2 Get Conversation Messages

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/conversations/{conversationId}/messages` |
| **Authentication** | Required                                     |
| **Roles**          | `Patient` (own), `Doctor` (own)              |

**Query Parameters:** `?page=1&pageSize=50&before={messageId}`

**Response — 200 OK:**

```json
{
  "success": true,
  "data": [
    {
      "id": "msg-uuid-1",
      "senderId": "doctor-user-uuid",
      "senderName": "Dr. Ayesha Malik",
      "content": "How are you feeling today?",
      "sentAt": "2026-08-06T14:00:00Z",
      "isRead": true,
      "readAt": "2026-08-06T14:05:00Z"
    },
    {
      "id": "msg-uuid-2",
      "senderId": "patient-user-uuid",
      "senderName": "Ahmed Khan",
      "content": "Much better, thank you! The rash is subsiding.",
      "sentAt": "2026-08-06T14:10:00Z",
      "isRead": true,
      "readAt": "2026-08-06T14:12:00Z"
    }
  ]
}
```

### 11.3 Send Message

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `POST`                                       |
| **URL**            | `/api/v1/conversations/{conversationId}/messages` |
| **Authentication** | Required                                     |
| **Roles**          | `Patient` (own), `Doctor` (own)              |

**Request Body:**

```json
{
  "content": "Thank you for the update. Continue the medication for 3 more days."
}
```

**Validation Rules:**

| Field     | Rules                                                                | Business Rule |
|-----------|----------------------------------------------------------------------|---------------|
| `content` | Required, non-empty, max 5000 characters                            | CHT-R02       |

**Error Responses:**

| Status | Condition                                | Error Type                          |
|--------|------------------------------------------|-------------------------------------|
| `403`  | No appointment relationship exists       | `no-appointment-relationship`       |
| `403`  | User not a participant in conversation   | `not-a-participant`                 |

> **Business Note (CHT-R01):** Chat is only available between patient-doctor pairs who have at least one past or active appointment. The API verifies this relationship before allowing the first message or creating a conversation.

### 11.4 Mark Messages as Read

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `POST`                                       |
| **URL**            | `/api/v1/conversations/{conversationId}/read`|
| **Authentication** | Required                                     |
| **Roles**          | `Patient` (own), `Doctor` (own)              |

**Request Body:**

```json
{
  "upToMessageId": "msg-uuid-2"
}
```

---

## 12. Notification Endpoints

### 12.1 List Notifications

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/notifications`                      |
| **Authentication** | Required                                     |
| **Roles**          | All authenticated users                      |

**Query Parameters:** `?isRead=false&page=1&pageSize=20&sortBy=sentAt&sortOrder=desc`

**Response — 200 OK:**

```json
{
  "success": true,
  "data": [
    {
      "id": "notif-uuid-1",
      "type": "AppointmentConfirmed",
      "channel": "InApp",
      "title": "Appointment Confirmed",
      "body": "Your appointment with Dr. Ayesha Malik on Aug 10 has been confirmed.",
      "referenceType": "Appointment",
      "referenceId": "appt-uuid",
      "isRead": false,
      "isDismissed": false,
      "sentAt": "2026-08-06T14:00:00Z"
    }
  ],
  "pagination": { "page": 1, "pageSize": 20, "totalRecords": 12 }
}
```

### 12.2 Get Unread Count

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/notifications/unread-count`         |
| **Authentication** | Required                                     |
| **Roles**          | All authenticated users                      |

**Response — 200 OK:**

```json
{
  "success": true,
  "data": { "unreadCount": 5 }
}
```

### 12.3 Mark as Read

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `POST`                                       |
| **URL**            | `/api/v1/notifications/mark-read`            |
| **Authentication** | Required                                     |
| **Roles**          | All authenticated users                      |

**Request Body:**

```json
{
  "notificationIds": ["notif-uuid-1", "notif-uuid-2"]
}
```

### 12.4 Mark All as Read

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `POST`                                       |
| **URL**            | `/api/v1/notifications/mark-all-read`        |
| **Authentication** | Required                                     |
| **Roles**          | All authenticated users                      |

### 12.5 Dismiss Notification

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `DELETE`                                     |
| **URL**            | `/api/v1/notifications/{id}`                 |
| **Authentication** | Required                                     |
| **Roles**          | All authenticated users                      |

---

## 13. Search Endpoints

### 13.1 Search Doctors

**Purpose:** Intelligent doctor discovery — patients search by symptoms, health concerns, or specializations. The platform maps inputs to the most relevant specialties and returns matching doctors.

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/search/doctors`                     |
| **Authentication** | None (Public endpoint)                       |
| **Roles**          | Public                                       |

**Query Parameters:**

| Parameter        | Type       | Required | Description                                       |
|------------------|------------|----------|---------------------------------------------------|
| `q`              | `string`   | No       | Free-text search (symptom, concern, or specialty)  |
| `specializationId`| `uuid`    | No       | Filter by specific specialization                  |
| `cityId`         | `uuid`     | No       | Filter by city                                     |
| `minFee`         | `decimal`  | No       | Minimum consultation fee                           |
| `maxFee`         | `decimal`  | No       | Maximum consultation fee                           |
| `minExperience`  | `int`      | No       | Minimum years of experience                        |
| `availableOn`    | `date`     | No       | Filter by availability on a specific date          |
| `sortBy`         | `string`   | No       | `relevance` (default), `fee`, `experience`, `rating` |
| `sortOrder`      | `string`   | No       | `asc` or `desc`                                    |
| `page`           | `int`      | No       | Page number (default: 1)                           |
| `pageSize`       | `int`      | No       | Items per page (default: 20, max: 50)              |

**Response — 200 OK:**

```json
{
  "success": true,
  "data": [
    {
      "doctorId": "doctor-user-uuid",
      "firstName": "Ayesha",
      "lastName": "Malik",
      "profilePhotoUrl": "https://cdn.medichp.com/photos/doc.jpg",
      "specializations": [
        { "id": "spec-uuid", "name": "Dermatology" }
      ],
      "yearsOfExperience": 10,
      "consultationFee": 1500.00,
      "feeCurrency": "PKR",
      "averageRating": 4.5,
      "totalReviews": 28,
      "city": { "id": "city-uuid", "name": "Lahore" },
      "nextAvailableSlot": "2026-08-07T09:00:00Z",
      "relevanceScore": 0.95
    }
  ],
  "searchContext": {
    "query": "skin rash",
    "mappedSpecializations": [
      { "id": "spec-uuid", "name": "Dermatology", "relevanceScore": 9 },
      { "id": "spec-uuid-2", "name": "General Medicine", "relevanceScore": 5 }
    ]
  },
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalRecords": 47,
    "totalPages": 3,
    "hasNext": true,
    "hasPrevious": false
  }
}
```

**Business Notes:**
- Implements PAT-R10 through PAT-R15.
- `searchContext.mappedSpecializations` shows which specializations the search query was mapped to, enabling transparency for the user (PAT-R11).
- When `q` is provided, the system queries `SymptomSpecializations` to map symptoms to specialties ranked by `RelevanceScore`.
- When `q` matches a specialization name directly, it's treated as a direct specialization filter.
- This endpoint does **not** require authentication (SRC-08, PAT-R14).

### 13.2 Get Doctor Public Profile

**Purpose:** View a doctor's full public profile for the search results detail page.

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/search/doctors/{doctorId}`          |
| **Authentication** | None (Public)                                |
| **Roles**          | Public                                       |

**Response includes:** Full bio, specializations, experience, fee, city, availability summary, average rating, review count.

### 13.3 Autocomplete Suggestions

**Purpose:** Provide typeahead suggestions for the search bar.

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/search/autocomplete`                |
| **Authentication** | None (Public)                                |
| **Roles**          | Public                                       |

**Query Parameters:** `?q=head&limit=10`

**Response — 200 OK:**

```json
{
  "success": true,
  "data": {
    "symptoms": [
      { "id": "sym-uuid-1", "name": "Headache" },
      { "id": "sym-uuid-2", "name": "Head Injury" }
    ],
    "specializations": [
      { "id": "spec-uuid-1", "name": "Neurology" }
    ]
  }
}
```

### 13.4 List Specializations

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/lookup/specializations`             |
| **Authentication** | None (Public)                                |
| **Roles**          | Public                                       |

### 13.5 List Cities

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/lookup/cities`                      |
| **Authentication** | None (Public)                                |
| **Roles**          | Public                                       |

**Query Parameters:** `?q=Lah&country=Pakistan`

---

## 14. File Upload Endpoints

> **Status:** Architecture defined. Implementation begins in Phase 1 for profile photos and license documents. Medical report uploads are Phase 2.

### 14.1 Upload File

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `POST`                                       |
| **URL**            | `/api/v1/files`                              |
| **Authentication** | Required                                     |
| **Roles**          | All authenticated users                      |
| **Content-Type**   | `multipart/form-data`                        |

**Form Fields:**

| Field     | Type   | Required | Description                                    |
|-----------|--------|----------|------------------------------------------------|
| `file`    | `file` | Yes      | The file to upload                             |
| `purpose` | `string`| Yes     | `ProfilePhoto`, `LicenseDocument`, `Other`     |

**Validation Rules:**

| Rule                          | Value                                       |
|-------------------------------|---------------------------------------------|
| Max file size                 | 5 MB (profile photos), 10 MB (documents)    |
| Allowed image types           | `image/jpeg`, `image/png`, `image/webp`     |
| Allowed document types        | `application/pdf`, `image/jpeg`, `image/png` |

**Response — 201 Created:**

```json
{
  "success": true,
  "data": {
    "fileId": "file-uuid",
    "url": "https://cdn.medichp.com/files/file-uuid.jpg",
    "contentType": "image/jpeg",
    "fileSizeBytes": 245000
  }
}
```

### 14.2 Get File

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/files/{fileId}`                     |
| **Authentication** | Required                                     |
| **Roles**          | Owner, `SuperAdmin`                          |

### 14.3 Delete File

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `DELETE`                                     |
| **URL**            | `/api/v1/files/{fileId}`                     |
| **Authentication** | Required                                     |
| **Roles**          | Owner only                                   |

---

## 15. Super Admin Endpoints

> All Super Admin endpoints require `SuperAdmin` role. All actions are logged to the audit trail (ADM-R23).

### 15.1 Admin Dashboard

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/admin/dashboard`                    |
| **Authentication** | Required                                     |
| **Roles**          | `SuperAdmin`                                 |

**Response — 200 OK:**

```json
{
  "success": true,
  "data": {
    "totalUsers": 15420,
    "totalPatients": 12300,
    "totalDoctors": 3100,
    "totalAdmins": 20,
    "newRegistrations": {
      "today": 45,
      "thisWeek": 312,
      "thisMonth": 1250
    },
    "appointments": {
      "total": 85000,
      "todayTotal": 320,
      "completionRate": 0.87,
      "cancellationRate": 0.08
    },
    "activeConversations": 2450,
    "systemHealth": {
      "apiStatus": "Healthy",
      "databaseStatus": "Healthy",
      "lastBackupAt": "2026-08-06T02:00:00Z"
    }
  }
}
```

### 15.2 User Management

#### List All Users

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/admin/users`                        |
| **Authentication** | Required                                     |
| **Roles**          | `SuperAdmin`                                 |

**Query Parameters:**

| Parameter    | Type     | Description                              |
|--------------|----------|------------------------------------------|
| `q`          | `string` | Search by name, email, phone             |
| `role`       | `string` | Filter: `Patient`, `Doctor`, `SuperAdmin`|
| `isActive`   | `bool`   | Filter by active/suspended status        |
| `fromDate`   | `date`   | Registration date range start            |
| `toDate`     | `date`   | Registration date range end              |
| `sortBy`     | `string` | `createdAt`, `name`, `lastLoginAt`       |
| `sortOrder`  | `string` | `asc`, `desc`                            |
| `page`       | `int`    | —                                        |
| `pageSize`   | `int`    | —                                        |

#### Get User Details

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/admin/users/{userId}`               |
| **Authentication** | Required                                     |
| **Roles**          | `SuperAdmin`                                 |

**Business Notes:**
- Returns user profile, role, registration date, last login, account status.
- Does **not** return medical records (consultations, prescriptions, chat) — DAT-R12.

### 15.3 Suspend Account

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `POST`                                       |
| **URL**            | `/api/v1/admin/users/{userId}/suspend`       |
| **Authentication** | Required                                     |
| **Roles**          | `SuperAdmin`                                 |

**Request Body:**

```json
{
  "reason": "Violation of platform terms of service — fake profile"
}
```

**Validation Rules:**

| Field    | Rules                    | Business Rule |
|----------|--------------------------|---------------|
| `reason` | Required, max 500 chars  | ADM-R15       |

**Error Responses:**

| Status | Condition                      | Error Type                      |
|--------|--------------------------------|---------------------------------|
| `404`  | User not found                 | `user-not-found`                |
| `409`  | User already suspended         | `user-already-suspended`        |
| `422`  | Cannot suspend last SuperAdmin | `cannot-suspend-last-admin`     |

### 15.4 Reactivate Account

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `POST`                                       |
| **URL**            | `/api/v1/admin/users/{userId}/reactivate`    |
| **Authentication** | Required                                     |
| **Roles**          | `SuperAdmin`                                 |

**Request Body:**

```json
{
  "reason": "User completed identity verification"
}
```

### 15.5 Create Admin Account

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `POST`                                       |
| **URL**            | `/api/v1/admin/users/admins`                 |
| **Authentication** | Required                                     |
| **Roles**          | `SuperAdmin`                                 |

**Request Body:**

```json
{
  "firstName": "Haris",
  "lastName": "Ahmed",
  "email": "haris@medichp.com",
  "phoneNumber": "+923001234568",
  "password": "AdminP@ss1"
}
```

### 15.6 Audit Logs

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/admin/audit-logs`                   |
| **Authentication** | Required                                     |
| **Roles**          | `SuperAdmin`                                 |

**Query Parameters:**

| Parameter      | Type     | Description                              |
|----------------|----------|------------------------------------------|
| `userId`       | `uuid`   | Filter by acting user                    |
| `action`       | `string` | Filter by action type                    |
| `entityType`   | `string` | Filter by entity (User, Appointment, etc.)|
| `fromDate`     | `date`   | Start of date range                      |
| `toDate`       | `date`   | End of date range                        |
| `page`         | `int`    | —                                        |
| `pageSize`     | `int`    | —                                        |

**Response — 200 OK:**

```json
{
  "success": true,
  "data": [
    {
      "id": "audit-uuid",
      "userId": "admin-user-uuid",
      "userName": "Haris Ahmed",
      "action": "User.Suspend",
      "entityType": "User",
      "entityId": "target-user-uuid",
      "oldValues": { "isActive": true },
      "newValues": { "isActive": false, "suspensionReason": "Terms violation" },
      "ipAddress": "192.168.1.100",
      "timestamp": "2026-08-06T14:00:00Z"
    }
  ],
  "pagination": { "page": 1, "pageSize": 20, "totalRecords": 5432 }
}
```

### 15.7 Platform Settings

#### Get All Settings

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/admin/settings`                     |
| **Authentication** | Required                                     |
| **Roles**          | `SuperAdmin`                                 |

#### Update Setting

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `PUT`                                        |
| **URL**            | `/api/v1/admin/settings/{key}`               |
| **Authentication** | Required                                     |
| **Roles**          | `SuperAdmin`                                 |

**Request Body:**

```json
{
  "value": "30"
}
```

**Business Notes:**
- Changes take effect immediately for new actions (ADM-R31).
- All changes are logged to audit trail with old and new values (ADM-R32).

### 15.8 Platform Statistics

| Attribute          | Value                                        |
|--------------------|----------------------------------------------|
| **Method**         | `GET`                                        |
| **URL**            | `/api/v1/admin/statistics`                   |
| **Authentication** | Required                                     |
| **Roles**          | `SuperAdmin`                                 |

**Query Parameters:** `?period=monthly&fromDate=2026-01-01&toDate=2026-08-06`

**Response — 200 OK:**

```json
{
  "success": true,
  "data": {
    "registrations": [
      { "period": "2026-07", "patients": 1200, "doctors": 85 },
      { "period": "2026-08", "patients": 450, "doctors": 32 }
    ],
    "appointments": [
      { "period": "2026-07", "total": 8500, "completed": 7200, "cancelled": 680 },
      { "period": "2026-08", "total": 3200, "completed": 2800, "cancelled": 150 }
    ]
  }
}
```

---

## 16. API Flow Diagrams

### 16.1 Complete Patient Journey

```mermaid
sequenceDiagram
    participant P as Patient
    participant API as MedicHp API
    participant DB as Database
    participant E as Email Service

    Note over P,E: Registration & Verification
    P->>API: POST /auth/register/patient
    API->>DB: Create User + PatientProfile
    API->>E: Send verification OTP
    API-->>P: 201 {userId, emailVerificationRequired}
    P->>API: POST /auth/verify-email {otp}
    API->>DB: Confirm email
    API-->>P: 200 {emailConfirmed: true}

    Note over P,E: Login
    P->>API: POST /auth/login
    API-->>P: 200 {accessToken, refreshToken}

    Note over P,E: Doctor Search & Booking
    P->>API: GET /search/doctors?q=skin+rash
    API->>DB: Map symptoms → specializations → doctors
    API-->>P: 200 {doctors[], searchContext}
    P->>API: GET /doctors/{id}/slots?date=2026-08-10
    API-->>P: 200 {slots[]}
    P->>API: POST /appointments
    API->>DB: Create appointment (Pending)
    API->>E: Notify doctor
    API-->>P: 201 {appointment}

    Note over P,E: Consultation & Prescription
    P->>API: GET /appointments (upcoming)
    API-->>P: 200 {appointments[]}
    P->>API: GET /consultations/{id}
    API-->>P: 200 {consultation, vitals}
    P->>API: GET /prescriptions/{id}
    API-->>P: 200 {prescription, items[]}
```

### 16.2 Doctor Consultation Flow

```mermaid
sequenceDiagram
    participant D as Doctor
    participant API as MedicHp API
    participant DB as Database
    participant N as Notification Service

    D->>API: PATCH /appointments/{id}/status {Confirmed}
    API->>DB: Update status → Confirmed
    API->>N: Notify patient
    API-->>D: 200

    D->>API: PATCH /appointments/{id}/status {InProgress}
    API-->>D: 200

    D->>API: PATCH /appointments/{id}/status {Completed}
    API-->>D: 200

    D->>API: POST /consultations {appointmentId, diagnosis...}
    API->>DB: Create consultation record
    API-->>D: 201 {consultation}

    D->>API: POST /prescriptions {consultationId, items[]}
    API->>DB: Create prescription + items
    API->>N: Notify patient of new prescription
    API-->>D: 201 {prescription}

    D->>API: POST /consultations/{id}/finalize
    API->>DB: Set isFinalized = true
    API-->>D: 200 {isFinalized: true}
```

### 16.3 Token Lifecycle

```mermaid
sequenceDiagram
    participant C as Client
    participant API as MedicHp API

    C->>API: POST /auth/login
    API-->>C: 200 {accessToken (15m), refreshToken}

    loop Every API Request
        C->>API: GET /patients/dashboard [Bearer accessToken]
        alt Token Valid
            API-->>C: 200 {data}
        else Token Expired
            API-->>C: 401 {expired-token}
            C->>API: POST /auth/refresh {refreshToken}
            alt Refresh Valid
                API-->>C: 200 {new accessToken, new refreshToken}
            else Refresh Invalid
                API-->>C: 401 {invalid-refresh-token}
                Note over C: Redirect to login
            end
        end
    end

    C->>API: POST /auth/logout {refreshToken}
    API-->>C: 200 {logged out}
```

---

## 17. Versioning & Deprecation

### 17.1 Versioning Strategy

| Aspect                  | Implementation                                          |
|-------------------------|---------------------------------------------------------|
| Versioning method       | URL path: `/api/v1/`, `/api/v2/`                        |
| Current version         | `v1`                                                    |
| Header support (future) | `Accept: application/vnd.medichp.v2+json`               |

### 17.2 Version Lifecycle

| Phase          | Duration       | Behavior                                          |
|----------------|----------------|---------------------------------------------------|
| **Active**     | Current        | Full support, new features added                   |
| **Deprecated** | 6 months       | Functional but marked for removal; deprecation header returned |
| **Sunset**     | After 6 months | Returns `410 Gone`                                 |

### 17.3 Deprecation Headers

When an endpoint or version is deprecated, responses include:

```
Deprecation: true
Sunset: Sat, 06 Feb 2027 00:00:00 GMT
Link: <https://api.medichp.com/api/v2/doctors>; rel="successor-version"
```

### 17.4 Breaking vs. Non-Breaking Changes

| Change Type                     | Classification | Versioning Required |
|---------------------------------|----------------|---------------------|
| Adding a new optional field     | Non-breaking    | No                  |
| Adding a new endpoint           | Non-breaking    | No                  |
| Adding a new enum value         | Non-breaking    | No                  |
| Removing a field                | **Breaking**    | Yes (new version)   |
| Renaming a field                | **Breaking**    | Yes (new version)   |
| Changing a field type           | **Breaking**    | Yes (new version)   |
| Changing URL structure          | **Breaking**    | Yes (new version)   |
| Making optional field required  | **Breaking**    | Yes (new version)   |

---

## 18. Version History

| Version | Date       | Author                    | Changes                                                      |
|---------|------------|---------------------------|--------------------------------------------------------------|
| 1.0     | 2026-08-04 | MedicHp Architecture Team | Initial placeholder API structure                            |
| 2.0     | 2026-08-06 | MedicHp API Architecture Team | Complete Phase 1 API specification: 70+ endpoints, request/response contracts, validation rules, error handling, flow diagrams |

### Complete Endpoint Index

| #  | Method   | Endpoint                                           | Auth   | Roles            |
|----|----------|----------------------------------------------------|--------|------------------|
| 1  | `POST`   | `/api/v1/auth/register/patient`                    | No     | Public           |
| 2  | `POST`   | `/api/v1/auth/register/doctor`                     | No     | Public           |
| 3  | `POST`   | `/api/v1/auth/verify-email`                        | No     | Public           |
| 4  | `POST`   | `/api/v1/auth/resend-verification`                 | No     | Public           |
| 5  | `POST`   | `/api/v1/auth/login`                               | No     | Public           |
| 6  | `POST`   | `/api/v1/auth/refresh`                             | No     | Public           |
| 7  | `POST`   | `/api/v1/auth/logout`                              | Yes    | All              |
| 8  | `POST`   | `/api/v1/auth/forgot-password`                     | No     | Public           |
| 9  | `POST`   | `/api/v1/auth/reset-password`                      | No     | Public           |
| 10 | `POST`   | `/api/v1/auth/change-password`                     | Yes    | All              |
| 11 | `GET`    | `/api/v1/patients/profile`                         | Yes    | Patient          |
| 12 | `PATCH`  | `/api/v1/patients/profile`                         | Yes    | Patient          |
| 13 | `GET`    | `/api/v1/patients/emergency-contacts`              | Yes    | Patient          |
| 14 | `POST`   | `/api/v1/patients/emergency-contacts`              | Yes    | Patient          |
| 15 | `PUT`    | `/api/v1/patients/emergency-contacts/{id}`         | Yes    | Patient          |
| 16 | `DELETE` | `/api/v1/patients/emergency-contacts/{id}`         | Yes    | Patient          |
| 17 | `GET`    | `/api/v1/patients/allergies`                       | Yes    | Patient          |
| 18 | `POST`   | `/api/v1/patients/allergies`                       | Yes    | Patient          |
| 19 | `PUT`    | `/api/v1/patients/allergies/{id}`                  | Yes    | Patient          |
| 20 | `DELETE` | `/api/v1/patients/allergies/{id}`                  | Yes    | Patient          |
| 21 | `GET`    | `/api/v1/patients/chronic-conditions`              | Yes    | Patient          |
| 22 | `POST`   | `/api/v1/patients/chronic-conditions`              | Yes    | Patient          |
| 23 | `PUT`    | `/api/v1/patients/chronic-conditions/{id}`         | Yes    | Patient          |
| 24 | `DELETE` | `/api/v1/patients/chronic-conditions/{id}`         | Yes    | Patient          |
| 25 | `GET`    | `/api/v1/patients/medications`                     | Yes    | Patient          |
| 26 | `POST`   | `/api/v1/patients/medications`                     | Yes    | Patient          |
| 27 | `PUT`    | `/api/v1/patients/medications/{id}`                | Yes    | Patient          |
| 28 | `DELETE` | `/api/v1/patients/medications/{id}`                | Yes    | Patient          |
| 29 | `GET`    | `/api/v1/patients/dashboard`                       | Yes    | Patient          |
| 30 | `GET`    | `/api/v1/doctors/profile`                          | Yes    | Doctor           |
| 31 | `PATCH`  | `/api/v1/doctors/profile`                          | Yes    | Doctor           |
| 32 | `GET`    | `/api/v1/doctors/availability`                     | Yes    | Doctor           |
| 33 | `PUT`    | `/api/v1/doctors/availability`                     | Yes    | Doctor           |
| 34 | `POST`   | `/api/v1/doctors/unavailability`                   | Yes    | Doctor           |
| 35 | `GET`    | `/api/v1/doctors/unavailability`                   | Yes    | Doctor           |
| 36 | `DELETE` | `/api/v1/doctors/unavailability/{id}`              | Yes    | Doctor           |
| 37 | `GET`    | `/api/v1/doctors/{doctorId}/slots`                 | No     | Public           |
| 38 | `GET`    | `/api/v1/doctors/patients`                         | Yes    | Doctor           |
| 39 | `POST`   | `/api/v1/doctors/patients`                         | Yes    | Doctor           |
| 40 | `GET`    | `/api/v1/doctors/dashboard`                        | Yes    | Doctor           |
| 41 | `GET`    | `/api/v1/doctors/schedule`                         | Yes    | Doctor           |
| 42 | `POST`   | `/api/v1/appointments`                             | Yes    | Patient          |
| 43 | `GET`    | `/api/v1/appointments`                             | Yes    | Patient, Doctor  |
| 44 | `GET`    | `/api/v1/appointments/{id}`                        | Yes    | Patient, Doctor  |
| 45 | `PATCH`  | `/api/v1/appointments/{id}/status`                 | Yes    | Doctor           |
| 46 | `POST`   | `/api/v1/appointments/{id}/cancel`                 | Yes    | Patient, Doctor  |
| 47 | `POST`   | `/api/v1/appointments/{id}/reschedule`             | Yes    | Patient          |
| 48 | `POST`   | `/api/v1/consultations`                            | Yes    | Doctor           |
| 49 | `GET`    | `/api/v1/consultations`                            | Yes    | Patient, Doctor  |
| 50 | `GET`    | `/api/v1/consultations/{id}`                       | Yes    | Patient, Doctor  |
| 51 | `PATCH`  | `/api/v1/consultations/{id}`                       | Yes    | Doctor           |
| 52 | `POST`   | `/api/v1/consultations/{id}/finalize`              | Yes    | Doctor           |
| 53 | `POST`   | `/api/v1/consultations/{id}/addenda`               | Yes    | Doctor           |
| 54 | `POST`   | `/api/v1/prescriptions`                            | Yes    | Doctor           |
| 55 | `GET`    | `/api/v1/prescriptions`                            | Yes    | Patient, Doctor  |
| 56 | `GET`    | `/api/v1/prescriptions/{id}`                       | Yes    | Patient, Doctor  |
| 57 | `POST`   | `/api/v1/prescriptions/{id}/supersede`             | Yes    | Doctor           |
| 58 | `GET`    | `/api/v1/prescriptions/{id}/pdf`                   | Yes    | Patient, Doctor  |
| 59 | `GET`    | `/api/v1/conversations`                            | Yes    | Patient, Doctor  |
| 60 | `GET`    | `/api/v1/conversations/{id}/messages`              | Yes    | Patient, Doctor  |
| 61 | `POST`   | `/api/v1/conversations/{id}/messages`              | Yes    | Patient, Doctor  |
| 62 | `POST`   | `/api/v1/conversations/{id}/read`                  | Yes    | Patient, Doctor  |
| 63 | `GET`    | `/api/v1/notifications`                            | Yes    | All              |
| 64 | `GET`    | `/api/v1/notifications/unread-count`               | Yes    | All              |
| 65 | `POST`   | `/api/v1/notifications/mark-read`                  | Yes    | All              |
| 66 | `POST`   | `/api/v1/notifications/mark-all-read`              | Yes    | All              |
| 67 | `DELETE` | `/api/v1/notifications/{id}`                       | Yes    | All              |
| 68 | `GET`    | `/api/v1/search/doctors`                           | No     | Public           |
| 69 | `GET`    | `/api/v1/search/doctors/{doctorId}`                | No     | Public           |
| 70 | `GET`    | `/api/v1/search/autocomplete`                      | No     | Public           |
| 71 | `GET`    | `/api/v1/lookup/specializations`                   | No     | Public           |
| 72 | `GET`    | `/api/v1/lookup/cities`                            | No     | Public           |
| 73 | `POST`   | `/api/v1/files`                                    | Yes    | All              |
| 74 | `GET`    | `/api/v1/files/{fileId}`                           | Yes    | Owner, Admin     |
| 75 | `DELETE` | `/api/v1/files/{fileId}`                           | Yes    | Owner            |
| 76 | `GET`    | `/api/v1/admin/dashboard`                          | Yes    | SuperAdmin       |
| 77 | `GET`    | `/api/v1/admin/users`                              | Yes    | SuperAdmin       |
| 78 | `GET`    | `/api/v1/admin/users/{userId}`                     | Yes    | SuperAdmin       |
| 79 | `POST`   | `/api/v1/admin/users/{userId}/suspend`             | Yes    | SuperAdmin       |
| 80 | `POST`   | `/api/v1/admin/users/{userId}/reactivate`          | Yes    | SuperAdmin       |
| 81 | `POST`   | `/api/v1/admin/users/admins`                       | Yes    | SuperAdmin       |
| 82 | `GET`    | `/api/v1/admin/audit-logs`                         | Yes    | SuperAdmin       |
| 83 | `GET`    | `/api/v1/admin/settings`                           | Yes    | SuperAdmin       |
| 84 | `PUT`    | `/api/v1/admin/settings/{key}`                     | Yes    | SuperAdmin       |
| 85 | `GET`    | `/api/v1/admin/statistics`                         | Yes    | SuperAdmin       |

### Future Revisions

| Planned Revision                            | Trigger                                      |
|---------------------------------------------|----------------------------------------------|
| Add WebSocket specification for real-time chat | Phase 2 real-time features                 |
| Add payment/billing endpoints               | Phase 3 planning                             |
| Add telemedicine/video session endpoints    | Phase 3 planning                             |
| Add OpenAPI 3.1 YAML export                 | API gateway deployment                       |
| Add rate limiting per-endpoint customization | Load testing results                        |
| Add webhook specification for integrations  | Phase 2 partner API program                  |

---

> **Cross-References:**
> - Technical architecture and philosophy → [PROJECT_SPECIFICATION.md](PROJECT_SPECIFICATION.md)
> - Feature requirements and user stories → [PRODUCT_REQUIREMENTS.md](PRODUCT_REQUIREMENTS.md)
> - Domain-specific business rules → [BUSINESS_RULES.md](BUSINESS_RULES.md)
> - Database schema and entities → [DATABASE_ARCHITECTURE.md](DATABASE_ARCHITECTURE.md)
> - Security policies and compliance → [SECURITY_GUIDELINES.md](SECURITY_GUIDELINES.md)
> - AI development guide → [README_FOR_AI.md](../README_FOR_AI.md)
