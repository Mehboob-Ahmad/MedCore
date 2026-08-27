# 🤖 README FOR AI — MedicHp Project Constitution

> **⚠️ MANDATORY — Read this document in full before generating, modifying, or reviewing any code in the MedicHp codebase.**
>
> This is the authoritative guide for every AI coding assistant — including Google Antigravity, Gemini, Codex, Claude, Cursor, GitHub Copilot, and any future tool — working on the MedicHp project. Treat it as the project's **constitution**. Violations of these rules will result in rejected code.

---

## Table of Contents

- [1. Project Identity](#1-project-identity)
- [2. AI Responsibilities](#2-ai-responsibilities)
- [3. Mandatory Reading Order](#3-mandatory-reading-order)
- [4. Technology Stack](#4-technology-stack)
- [5. Architecture Overview](#5-architecture-overview)
- [6. Development Principles](#6-development-principles)
- [7. Coding Rules — TypeScript / JavaScript](#7-coding-rules--typescript--javascript)
- [8. Coding Rules — C# / .NET Backend](#8-coding-rules--c--net-backend)
- [9. Coding Rules — React Components](#9-coding-rules--react-components)
- [10. Coding Rules — Database](#10-coding-rules--database)
- [11. Security Rules](#11-security-rules)
- [12. Naming Conventions](#12-naming-conventions)
- [13. Git & Workflow Rules](#13-git--workflow-rules)
- [14. AI Restrictions](#14-ai-restrictions)
- [15. Quality Checklist](#15-quality-checklist)
- [16. Examples — Good vs. Bad Practices](#16-examples--good-vs-bad-practices)
- [17. Phase-Aware Development](#17-phase-aware-development)
- [18. Notes for Future Contributors](#18-notes-for-future-contributors)

---

## 1. Project Identity

| Attribute        | Value                                                        |
|------------------|--------------------------------------------------------------|
| **Name**         | MedicHp                                                      |
| **Type**         | Digital Healthcare Ecosystem (SaaS)                          |
| **Architecture** | Monorepo with microservice-ready backend                     |
| **License**      | MIT                                                          |
| **Stage**        | Active development — Phase 1 (Foundation)                    |

### What MedicHp Is

MedicHp is a **production-ready, scalable SaaS platform** that connects patients, doctors, clinics, and administrators through a unified, secure digital healthcare ecosystem. It is designed for:

- **Long-term commercial deployment** — not a prototype, demo, or throwaway project.
- **Regulatory compliance** — HIPAA-aware and GDPR-aware by design.
- **Multi-tenant operation** — supporting multiple clinics and organizations.
- **Multi-platform delivery** — web (Next.js), admin dashboard (React + Vite), and mobile (React Native + Expo).

### What MedicHp Is NOT

- ❌ A university or academic project.
- ❌ A quick proof-of-concept or hackathon app.
- ❌ A single-developer hobby project with loose standards.
- ❌ A playground for experimental or unproven patterns.

> **Every line of code must be written as if it will serve real patients and handle real medical data in production.**

---

## 2. AI Responsibilities

Every AI agent working on this codebase **MUST** adhere to the following contract:

### Before Generating Code

| #  | Responsibility                                                 |
|----|----------------------------------------------------------------|
| 1  | Read this document (`README_FOR_AI.md`) in its entirety.       |
| 2  | Read the relevant specification documents (see §3).            |
| 3  | Read any relevant ADRs in `docs/Decisions/`.                   |
| 4  | Understand the current development phase and sprint scope.     |

### While Generating Code

| #  | Responsibility                                                                |
|----|-------------------------------------------------------------------------------|
| 5  | Follow the existing architecture **exactly** — do not redesign.               |
| 6  | Generate code **only** for the requested module or feature.                   |
| 7  | Keep future phases in mind — design for extensibility.                        |
| 8  | Prioritize **maintainability** over speed of delivery.                        |
| 9  | Avoid duplicate logic — check existing packages and utilities first.          |
| 10 | Produce **production-ready** code with proper error handling and validation.  |
| 11 | Follow all naming conventions, coding standards, and security guidelines.     |
| 12 | Add comments only where they genuinely improve clarity — no noise.            |

### After Generating Code

| #  | Responsibility                                                     |
|----|--------------------------------------------------------------------|
| 13 | Run through the Quality Checklist (§15) before marking done.       |
| 14 | Update relevant documentation if behavior or APIs have changed.    |
| 15 | Flag any deviations from the spec for human review.                |

---

## 3. Mandatory Reading Order

Before implementing **any** feature, every AI must read the following documents **in this exact order**. Each document builds on the previous one.

| Order | Document                                                                 | Purpose                                      |
|-------|--------------------------------------------------------------------------|----------------------------------------------|
| 1     | `docs/README_FOR_AI.md` *(this file)*                                   | Project constitution & AI rules              |
| 2     | `docs/Specifications/PROJECT_SPECIFICATION.md`                           | System goals, technical requirements         |
| 3     | `docs/Specifications/PRODUCT_REQUIREMENTS.md`                            | User roles, features, MVP scope              |
| 4     | `docs/Specifications/BUSINESS_RULES.md`                                  | Domain constraints and validation rules      |
| 5     | `docs/Specifications/DATABASE_ARCHITECTURE.md`                           | Schema design, entities, data principles     |
| 6     | `docs/Specifications/API_SPECIFICATION.md`                               | REST API contracts and response format       |
| 7     | `docs/Specifications/CODING_STANDARDS.md`                                | Code style, commit conventions, review rules |
| 8     | `docs/Specifications/FOLDER_STRUCTURE.md`                                | Monorepo layout and naming conventions       |
| 9     | `docs/Specifications/SECURITY_GUIDELINES.md`                             | Authentication, encryption, compliance       |
| 10    | `docs/Specifications/UI_UX_GUIDELINES.md`                                | Design system, accessibility, responsiveness |
| 11    | Relevant ADRs in `docs/Decisions/`                                       | Architectural decisions and their rationale  |

### Currently Available ADRs

| ADR                                       | Decision                                    |
|-------------------------------------------|---------------------------------------------|
| `ADR-001-Monorepo.md`                     | Monorepo structure decision                 |
| `ADR-002-TechStack.md`                    | Technology stack selection                  |
| `ADR-003-Authentication.md`               | Authentication strategy (JWT + refresh)     |
| `ADR-004-PatientRegistration.md`          | Patient registration flow                   |
| `ADR-005-DoctorRegistration.md`           | Doctor registration flow                    |

> **Rule:** If an ADR exists for the area you are working on, you **must** read it and follow the decisions documented in it. Do not contradict an ADR without explicit human approval.

---

## 4. Technology Stack

| Layer             | Technology                     | Notes                                        |
|-------------------|--------------------------------|----------------------------------------------|
| **Website**       | Next.js                        | Public-facing marketing + patient portal     |
| **Dashboard**     | React + Vite                   | Admin / clinic management panel              |
| **Mobile**        | React Native + Expo            | Patient and doctor mobile app                |
| **Backend API**   | ASP.NET Core 9 Web API         | Clean Architecture, RESTful                  |
| **ORM**           | Entity Framework Core          | Code-first migrations                        |
| **Database**      | PostgreSQL 16+                 | Primary data store                           |
| **Cache**         | Redis 7+                       | Caching, sessions, rate limiting             |
| **Shared Code**   | TypeScript                     | Shared packages in `packages/`               |
| **CI/CD**         | GitHub Actions                 | Automated build, test, lint, deploy          |
| **Containers**    | Docker + Docker Compose        | Local dev and production deployment          |

> **Do not introduce additional frameworks, languages, or databases without human approval and an ADR.**

---

## 5. Architecture Overview

### 5.1 Monorepo Layout

```
MedicHp/
│
├── apps/                    → Frontend applications
│   ├── website/             → Next.js public website
│   ├── dashboard/           → React + Vite admin dashboard
│   └── mobile/              → React Native + Expo mobile app
│
├── backend/                 → ASP.NET Core 9 Web API (Clean Architecture)
│
├── packages/                → Shared TypeScript packages
│   ├── ui/                  → Shared React component library
│   ├── api-client/          → Typed API client (all HTTP calls go through here)
│   ├── types/               → Shared TypeScript type definitions
│   ├── config/              → Shared tooling configuration
│   ├── utils/               → Common utility functions
│   └── constants/           → Shared constants and enums
│
├── database/                → Database assets
│   ├── migrations/          → Versioned migration scripts
│   ├── seeds/               → Data seeding scripts
│   ├── schema/              → Schema definitions and ERDs
│   └── backups/             → Backup procedures
│
├── docker/                  → Dockerfiles and container configurations
├── scripts/                 → Build, deploy, and maintenance scripts
│
├── tests/                   → Test suites
│   ├── backend/             → API and service tests
│   ├── frontend/            → Component and UI tests
│   └── e2e/                 → End-to-end tests
│
├── docs/                    → All project documentation
│   ├── AI/                  → AI prompts and context files
│   ├── Brand/               → Brand assets, colors, typography
│   ├── Decisions/           → Architecture Decision Records (ADRs)
│   ├── Diagrams/            → System and database diagrams
│   ├── Releases/            → Release notes and changelogs
│   ├── Sources/             → Research, ideas, roadmap
│   └── Specifications/      → All specification documents
│
├── .github/                 → GitHub Actions CI/CD workflows
└── .vscode/                 → VS Code workspace settings
```

### 5.2 Backend — Clean Architecture Layers

The backend follows **Clean Architecture** with the following dependency flow:

```
Domain (innermost) → Application → Infrastructure → API (outermost)
```

| Layer              | Responsibility                                                 | Contains                                     |
|--------------------|----------------------------------------------------------------|----------------------------------------------|
| **Domain**         | Core business entities, value objects, domain events           | Entities, enums, exceptions, interfaces      |
| **Application**    | Use cases, DTOs, validation, business orchestration            | Services, DTOs, validators, mappers          |
| **Infrastructure** | External concerns: database, email, file storage, caching      | EF Core contexts, repositories, integrations |
| **API**            | HTTP layer: controllers, middleware, DI configuration           | Controllers, filters, startup config         |

> **Critical Rule:** Dependencies flow **inward only**. The Domain layer has **zero** external dependencies. The API layer references everything but is referenced by nothing.

### 5.3 Frontend — API-First Consumer Pattern

All frontend applications are **API consumers**. They:

- **Never** contain business logic — that belongs in the backend.
- **Always** use the shared `packages/api-client/` for all HTTP communication.
- **Always** use the shared `packages/types/` for type definitions.
- **Always** use the shared `packages/ui/` for reusable React components.
- **Never** make raw HTTP calls (`fetch`, `axios`) directly — use the typed API client.

---

## 6. Development Principles

These principles are **non-negotiable**. Every AI agent must internalize and follow them.

### Core Principles

| #  | Principle                                     | Explanation                                                                                      |
|----|-----------------------------------------------|--------------------------------------------------------------------------------------------------|
| 1  | **Documentation first, code second**          | Understand the spec, ADRs, and business rules before writing a single line of code.              |
| 2  | **Build module-by-module**                    | Complete one module fully (code + tests + docs) before starting the next.                        |
| 3  | **Never implement out-of-phase features**     | If it's not in the current phase/sprint scope, don't build it — even if you "know" it's needed.  |
| 4  | **Respect separation of concerns**            | Each layer, module, and component has a single, well-defined responsibility.                     |
| 5  | **Follow Clean Architecture**                 | Dependencies flow inward. Domain has no external dependencies.                                   |
| 6  | **Follow SOLID principles**                   | Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion. |
| 7  | **Prefer composition over inheritance**       | Use interfaces and dependency injection. Avoid deep inheritance hierarchies.                     |
| 8  | **Keep modules loosely coupled**              | Modules communicate through well-defined interfaces, not direct references.                      |
| 9  | **Write reusable and testable code**          | Every service, utility, and component must be independently testable.                            |
| 10 | **No premature optimization**                 | Write correct, readable code first. Optimize only when profiling reveals a bottleneck.           |

### The "Check Before You Create" Rule

Before creating any new file, utility, type, or component:

1. **Check `packages/`** — Does a shared utility already exist?
2. **Check the target directory** — Is there a similar file you should extend?
3. **Check `packages/types/`** — Does the type already exist?
4. **Check `packages/ui/`** — Does the UI component already exist?

> If it exists, **use it**. If it almost exists, **extend it**. Only create new if nothing suitable exists.

---

## 7. Coding Rules — TypeScript / JavaScript

| Rule                                        | Details                                                                                 |
|---------------------------------------------|-----------------------------------------------------------------------------------------|
| **Strict mode**                             | `"strict": true` in all `tsconfig.json` — no exceptions.                                |
| **No `any` type**                           | Use `unknown` when the type is genuinely uncertain. Never use `any` as a shortcut.       |
| **No `var`**                                | Use `const` by default. Use `let` only when reassignment is needed.                      |
| **Arrow functions for callbacks**           | Use arrow functions for inline callbacks and anonymous functions.                        |
| **Async/await over raw promises**           | Use `async/await` for asynchronous code. Avoid `.then()/.catch()` chains.                |
| **No hardcoded values**                     | Use environment variables for URLs, secrets, API keys. Use `packages/constants/` for app constants. |
| **Meaningful names**                        | Variables, functions, and types must have descriptive, self-documenting names.            |
| **Comments for clarity, not narration**     | Comment the *why*, not the *what*. If the code needs a comment to explain *what* it does, refactor it. |
| **Exports**                                 | Use named exports. Avoid default exports except for Next.js pages/layouts.               |
| **Error handling**                          | Always handle errors explicitly. Never swallow errors silently.                          |

---

## 8. Coding Rules — C# / .NET Backend

| Rule                                        | Details                                                                                 |
|---------------------------------------------|-----------------------------------------------------------------------------------------|
| **Clean Architecture**                      | Strict layer separation: Domain → Application → Infrastructure → API.                   |
| **Dependency injection**                    | Use constructor injection. No static classes for services. Register in DI container.     |
| **Async I/O**                               | All I/O operations (database, HTTP, file) must be `async/await`.                         |
| **Nullable reference types**                | Enable `<Nullable>enable</Nullable>` in all projects.                                    |
| **IActionResult returns**                   | Controllers return `IActionResult` or `ActionResult<T>`.                                 |
| **No business logic in controllers**        | Controllers are thin — they delegate to Application-layer services.                      |
| **Validation**                              | Use FluentValidation or Data Annotations. Validate at the Application layer.             |
| **Repository pattern**                      | Data access goes through repository interfaces defined in Domain, implemented in Infrastructure. |
| **EF Core migrations**                      | All schema changes via code-first migrations. No manual SQL in production.               |
| **Naming**                                  | `PascalCase` for public members, `_camelCase` for private fields, `I` prefix for interfaces. |

---

## 9. Coding Rules — React Components

| Rule                                        | Details                                                                                 |
|---------------------------------------------|-----------------------------------------------------------------------------------------|
| **Functional components only**              | No class components. Use hooks for state and side effects.                               |
| **Reusability**                             | Design components to be reusable. Shared components go in `packages/ui/`.                |
| **Single responsibility**                   | Each component does one thing well. Decompose large components.                          |
| **Props typing**                            | All props must be explicitly typed with TypeScript interfaces.                            |
| **No inline styles**                        | Use CSS Modules or the project's design system. Never use inline `style={}`.             |
| **Accessibility**                           | WCAG 2.1 AA minimum. Proper ARIA attributes, keyboard navigation, semantic HTML.         |
| **Dark/light mode**                         | All shared components must support both themes.                                          |
| **No direct API calls**                     | Use `packages/api-client/` for all backend communication.                                |
| **State management**                        | Prefer local state and React Context. Introduce state libraries only with ADR approval.  |
| **Error boundaries**                        | Wrap feature areas with error boundaries for graceful failure.                           |

---

## 10. Coding Rules — Database

| Rule                                        | Details                                                                                 |
|---------------------------------------------|-----------------------------------------------------------------------------------------|
| **Migrations only**                         | All schema changes via EF Core migrations. No ad-hoc SQL in production.                  |
| **Soft deletes**                            | All entities use `IsDeleted` flag. Never hard-delete records.                            |
| **Audit columns**                           | Every table includes `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`.                 |
| **UUID primary keys**                       | Use GUIDs for primary keys — never auto-increment integers.                              |
| **UTC timestamps**                          | All `DateTime` values stored and transmitted in UTC. No local time zones.                |
| **Normalization**                           | Follow 3NF for transactional tables. Denormalize only with ADR justification.            |
| **Row-Level Security**                      | Use PostgreSQL RLS for multi-tenant data isolation.                                      |
| **Medical records**                         | Medical records are **append-only** — never update or delete finalized records.           |

---

## 11. Security Rules

Healthcare data demands the highest security standards. These rules are **absolute**.

| Rule                                        | Details                                                                                 |
|---------------------------------------------|-----------------------------------------------------------------------------------------|
| **No hardcoded secrets**                    | Secrets, API keys, connection strings → environment variables only. Never in code.       |
| **JWT with short expiry**                   | Access tokens expire in 15 minutes. Refresh tokens in HTTP-only secure cookies.          |
| **RBAC everywhere**                         | Role-Based Access Control enforced at API **and** database levels.                       |
| **Input validation**                        | Validate and sanitize all user input. Never trust client-side validation alone.           |
| **Encryption**                              | TLS 1.3 in transit. AES-256 at rest for PHI/PII data.                                   |
| **Audit logging**                           | Log every read/write of patient data to immutable audit trail.                           |
| **Account lockout**                         | Lock account after 5 failed login attempts (15-minute cooldown).                         |
| **Rate limiting**                           | Rate limit all public endpoints. Stricter limits on auth endpoints.                      |
| **CORS**                                    | Restrict to known origins only. No wildcard `*` in production.                           |
| **Dependency scanning**                     | Run `npm audit` and `dotnet audit` regularly. No known-vulnerable dependencies.          |

> **If you are unsure whether data is sensitive, treat it as sensitive.**

---

## 12. Naming Conventions

Consistent naming across the entire monorepo is critical. Follow these conventions **exactly**.

### File and Directory Naming

| Type                  | Convention            | Example                     |
|-----------------------|-----------------------|-----------------------------|
| Directories           | `kebab-case`          | `api-client/`               |
| React components      | `PascalCase`          | `PatientCard.tsx`            |
| TypeScript files      | `camelCase`           | `dateUtils.ts`               |
| CSS modules           | `camelCase`           | `patientCard.module.css`     |
| C# files              | `PascalCase`          | `PatientService.cs`          |
| Test files             | Match source + suffix | `dateUtils.test.ts`, `PatientService.Tests.cs` |

### Code Naming

| Type                  | Convention                | Example                      |
|-----------------------|---------------------------|------------------------------|
| Variables / functions | `camelCase`               | `getPatientById`             |
| Constants             | `SCREAMING_SNAKE_CASE`    | `MAX_RETRY_COUNT`            |
| Types / Interfaces    | `PascalCase`              | `PatientProfile`             |
| React components      | `PascalCase`              | `AppointmentCard`            |
| C# public members     | `PascalCase`              | `GetPatientAsync()`          |
| C# private fields     | `_camelCase`              | `_patientRepository`         |
| C# interfaces         | `IPascalCase`             | `IPatientRepository`         |
| Database tables       | `PascalCase` (plural)     | `Patients`, `AuditLogs`      |
| Enums                 | `PascalCase`              | `AppointmentStatus`          |
| Enum values           | `PascalCase`              | `Confirmed`, `Cancelled`     |

---

## 13. Git & Workflow Rules

### Branching Strategy

```
main            ← Production-ready code (protected, no direct pushes)
├── develop     ← Integration branch for all feature work
│   ├── feature/MC-42-patient-profile
│   ├── bugfix/MC-108-login-redirect
│   └── hotfix/MC-200-auth-crash
```

| Branch Type  | Branches From | Merges Into           | Naming Pattern                   |
|-------------|---------------|-----------------------|----------------------------------|
| `feature/`  | `develop`     | `develop`             | `feature/TICKET-short-desc`      |
| `bugfix/`   | `develop`     | `develop`             | `bugfix/TICKET-short-desc`       |
| `hotfix/`   | `main`        | `main` **and** `develop` | `hotfix/TICKET-short-desc`    |
| `chore/`    | `develop`     | `develop`             | `chore/short-desc`               |
| `docs/`     | `develop`     | `develop`             | `docs/short-desc`                |

### Commit Rules

Follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <short description>

[optional body]

[optional footer(s)]
```

**Types:** `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`, `revert`

**Scopes:** `backend`, `website`, `dashboard`, `mobile`, `db`, `docker`, `ci`, `docs`, `packages`

**Examples:**
```
feat(backend): add patient registration endpoint
fix(dashboard): resolve date picker timezone offset
docs(specs): update API specification for appointments
refactor(packages): extract date formatting to shared utils
test(backend): add unit tests for appointment validation
```

### Workflow Rules

| #  | Rule                                                                 |
|----|----------------------------------------------------------------------|
| 1  | **One feature per branch.** Never mix unrelated changes.             |
| 2  | **Small, focused commits.** Each commit should be atomic and purposeful. |
| 3  | **No force pushes** to `main` or `develop`.                         |
| 4  | **All changes via pull request.** Minimum 1 approval required.      |
| 5  | **CI must pass** before merge. Failing builds block merging.        |
| 6  | **No breaking changes** without updating the relevant ADR first.    |
| 7  | **Rebase on latest `develop`** before submitting a PR.              |

---

## 14. AI Restrictions

The following actions are **explicitly forbidden** without prior human approval:

### 🚫 Structural Changes

| Restriction                                                   | Reason                                               |
|---------------------------------------------------------------|------------------------------------------------------|
| Do NOT change the folder structure                             | The monorepo layout is an architectural decision (ADR-001). |
| Do NOT rename existing directories or files                    | Breaks imports, CI, and other developers' work.       |
| Do NOT move files between layers or packages                   | Violates Clean Architecture boundaries.               |

### 🚫 Dependency Changes

| Restriction                                                   | Reason                                               |
|---------------------------------------------------------------|------------------------------------------------------|
| Do NOT add new npm packages without justification              | Each dependency is a liability (security, size, maintenance). |
| Do NOT add new NuGet packages without justification            | Must align with architectural decisions.              |
| Do NOT upgrade major versions of existing dependencies         | Major versions may contain breaking changes.          |

### 🚫 Code Changes

| Restriction                                                   | Reason                                               |
|---------------------------------------------------------------|------------------------------------------------------|
| Do NOT delete existing code without clear explanation          | Code was written with intent; removal needs rationale. |
| Do NOT ignore or override coding standards                     | Standards ensure consistency across the team.         |
| Do NOT invent features not present in the specifications       | Scope creep is the enemy of production software.      |
| Do NOT generate code outside the requested module              | Stay within the boundary of the current task.         |
| Do NOT redesign existing architecture without ADR approval     | Architecture decisions are deliberate and documented. |
| Do NOT create "temporary" workarounds as permanent code        | Every line ships. Write it right the first time.      |

### 🚫 Data & Security

| Restriction                                                   | Reason                                               |
|---------------------------------------------------------------|------------------------------------------------------|
| Do NOT hardcode any secrets, keys, or connection strings       | Security violation. Use environment variables.        |
| Do NOT disable authentication or authorization for convenience | Security is not optional, even in dev.                |
| Do NOT log PHI/PII data to console or file logs                | HIPAA violation.                                      |
| Do NOT weaken input validation                                 | Opens attack vectors.                                 |

---

## 15. Quality Checklist

Before submitting **any** piece of work, verify **every** item on this checklist:

### ✅ Code Quality

- [ ] Code compiles without errors.
- [ ] No TypeScript `any` types used.
- [ ] No lint warnings or errors (`eslint`, `dotnet format`).
- [ ] All public methods and interfaces are documented.
- [ ] No hardcoded values — environment variables and constants used.
- [ ] Error handling is comprehensive — no swallowed errors.

### ✅ Architecture Compliance

- [ ] Code is in the correct layer (Domain / Application / Infrastructure / API).
- [ ] No circular dependencies between layers or modules.
- [ ] Dependencies flow inward only (Clean Architecture).
- [ ] Shared code is in the appropriate `packages/` directory.
- [ ] No duplicate logic — reuses existing utilities and services.

### ✅ Testing

- [ ] Unit tests written for all business logic (target: > 80% coverage).
- [ ] Tests follow AAA pattern (Arrange, Act, Assert).
- [ ] Tests are independent — no shared mutable state.
- [ ] Edge cases and error scenarios are covered.

### ✅ Security

- [ ] No secrets or credentials in code or config files.
- [ ] Input validation applied to all user-facing endpoints.
- [ ] Authorization checks in place for protected resources.
- [ ] Sensitive data (PHI/PII) is encrypted and audit-logged.

### ✅ Database

- [ ] Schema changes are in a migration file — no manual SQL.
- [ ] Audit columns (`CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`) present.
- [ ] Soft delete implemented (`IsDeleted` flag) — no hard deletes.
- [ ] Primary keys are UUIDs.
- [ ] All timestamps are UTC.

### ✅ Documentation

- [ ] Updated `API_SPECIFICATION.md` if new endpoints were added.
- [ ] Updated `DATABASE_ARCHITECTURE.md` if schema changed.
- [ ] Created or updated ADR if an architectural decision was made.
- [ ] Code comments explain *why*, not *what*.

### ✅ Git

- [ ] Commit message follows Conventional Commits format.
- [ ] One logical change per commit.
- [ ] Branch follows naming convention.
- [ ] No unrelated changes included.

---

## 16. Examples — Good vs. Bad Practices

### Environment Variables

```typescript
// ❌ BAD — Hardcoded URL
const API_URL = "https://api.medichp.com/v1";

// ✅ GOOD — Environment variable
const API_URL = process.env.NEXT_PUBLIC_API_URL;
```

### Type Safety

```typescript
// ❌ BAD — Using `any`
function getPatient(id: any): any {
  return fetch(`/api/patients/${id}`);
}

// ✅ GOOD — Explicit types
async function getPatient(id: string): Promise<PatientProfile> {
  return apiClient.get<PatientProfile>(`/patients/${id}`);
}
```

### API Calls

```typescript
// ❌ BAD — Direct fetch in a component
useEffect(() => {
  fetch("/api/v1/patients").then(res => res.json()).then(setPatients);
}, []);

// ✅ GOOD — Shared API client from packages/api-client
import { patientApi } from "@medichp/api-client";

useEffect(() => {
  patientApi.getAll().then(setPatients).catch(handleError);
}, []);
```

### Controller Thickness (C#)

```csharp
// ❌ BAD — Business logic in the controller
[HttpPost]
public async Task<IActionResult> Register(RegisterDto dto)
{
    if (dto.Age < 18) return BadRequest("Must be 18+");
    var user = new User { Name = dto.Name, Age = dto.Age };
    _context.Users.Add(user);
    await _context.SaveChangesAsync();
    return Ok(user);
}

// ✅ GOOD — Thin controller, logic in Application layer
[HttpPost]
public async Task<IActionResult> Register(RegisterPatientCommand command)
{
    var result = await _patientService.RegisterAsync(command);
    return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
}
```

### Comments

```typescript
// ❌ BAD — States the obvious
// Increment counter by 1
counter += 1;

// ✅ GOOD — Explains the business reason
// Retry count resets after successful token refresh per ADR-003
retryCount = 0;
```

---

## 17. Phase-Aware Development

MedicHp is built in **phases**. Each phase has defined scope. AI agents must respect phase boundaries.

| Phase   | Scope                                            | Status           |
|---------|--------------------------------------------------|------------------|
| Phase 1 | Foundation — Project setup, auth, core entities  | 🟡 In Progress   |
| Phase 2 | Core Features — Appointments, records, dashboard | ⚪ Not Started   |
| Phase 3 | Advanced Features — Telemedicine, analytics, etc | ⚪ Not Started   |

### Rules for Phase-Aware Development

1. **Implement only what is in the current phase.** Do not build Phase 2 features during Phase 1.
2. **Design for the future, build for the present.** Interfaces and abstractions should anticipate future needs, but implementations should not.
3. **Check `docs/Specifications/DEVELOPMENT_ROADMAP.md`** for the current sprint scope before starting any work.
4. **If asked to implement something out of phase**, flag it and ask for confirmation rather than proceeding.

---

## 18. Notes for Future Contributors

### For AI Assistants Joining Mid-Project

1. Start by reading this document completely.
2. Read the documents listed in §3 in order.
3. Explore the existing codebase to understand established patterns.
4. Ask clarifying questions rather than making assumptions.
5. When in doubt, follow the existing pattern in the codebase — consistency trumps personal preference.

### For Human Developers

1. This document governs AI-assisted development. Human developers should also follow these standards.
2. If you disagree with a rule, propose a change via an ADR — don't just ignore it.
3. Keep this document updated when architectural decisions change.
4. Review AI-generated code with extra scrutiny for:
   - Hallucinated APIs or packages
   - Subtle security issues
   - Over-engineering / unnecessary abstractions
   - Deviation from established patterns

### Document Maintenance

| Trigger                                  | Action Required                                              |
|------------------------------------------|--------------------------------------------------------------|
| New ADR created                          | Add to the ADR table in §3                                   |
| New specification document added         | Add to the mandatory reading order in §3                     |
| Architecture changes                     | Update §5 and related sections                               |
| New phase begins                         | Update the phase table in §17                                |
| Coding standards updated                 | Reflect changes in §7–§10                                    |
| Technology stack changes                 | Update §4 and create a new ADR                               |

---

> **This document is a living artifact.** It will evolve as MedicHp grows. Every change to this document must be deliberate, reviewed, and committed with a `docs(ai): ...` commit message.
>
> *Last reviewed: August 2026 — Phase 1 (Foundation)*
