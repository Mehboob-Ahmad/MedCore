# 💻 Coding Standards — MedicHp Digital Healthcare Ecosystem

> **Document Type:** Engineering Standards
> **Version:** 1.0
> **Last Revised:** 2026-08-06
> **Status:** Active
> **Audience:** All Software Engineers, AI Coding Assistants

---

## Table of Contents
- [1. Architecture Principles](#1-architecture-principles)
- [2. Naming Conventions](#2-naming-conventions)
- [3. Code Formatting & Linting](#3-code-formatting--linting)
- [4. TypeScript & React Standards](#4-typescript--react-standards)
- [5. C# & ASP.NET Core Standards](#5-c--aspnet-core-standards)
- [6. Error Handling & Logging](#6-error-handling--logging)
- [7. Testing Standards](#7-testing-standards)
- [8. Git & Version Control](#8-git--version-control)

---

## 1. Architecture Principles

MedicHp strictly follows these software engineering paradigms:

- **Clean Architecture:** Separation of concerns into layers (Domain, Application, Infrastructure, Presentation/API). Inner layers never depend on outer layers.
- **SOLID Principles:** Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion.
- **DRY (Don't Repeat Yourself):** Extract common logic into shared utility functions or base classes.
- **KISS (Keep It Simple, Stupid):** Avoid premature optimization and complex design patterns where a simple functional approach works.
- **YAGNI (You Aren't Gonna Need It):** Build only what is specified in the PRD for the current phase. Do not build speculative features.

---

## 2. Naming Conventions

### 2.1 General Naming
- **Variables / Functions (TS/JS):** `camelCase` (e.g., `getUserProfile`)
- **Variables / Fields (C#):** `camelCase` for private fields (e.g., `_userRepository`), `PascalCase` for properties (e.g., `UserProfile`)
- **Classes / Interfaces / Types (All):** `PascalCase` (e.g., `AppointmentService`, `IPatientRepository`)
- **Constants (All):** `UPPER_SNAKE_CASE` (e.g., `MAX_RETRY_ATTEMPTS`)
- **Booleans:** Prefix with `is`, `has`, `should`, or `can` (e.g., `isFinalized`, `hasConsent`).

### 2.2 File and Folder Naming
| Tech Stack | Rule | Example |
|------------|------|---------|
| React/Next.js (Components) | PascalCase | `DoctorProfileCard.tsx` |
| React/Next.js (Hooks/Utils) | camelCase | `useAuth.ts`, `formatDate.ts` |
| Next.js App Router | kebab-case | `app/doctor-dashboard/page.tsx` |
| C# (.NET) | PascalCase | `AppointmentController.cs` |
| Folders (General) | kebab-case | `components/ui/`, `features/auth/` |

---

## 3. Code Formatting & Linting

Automated tools enforce these rules. Code failing these checks will fail the CI/CD pipeline.

- **Frontend:** Prettier + ESLint. 
  - `printWidth: 100`, `semi: true`, `singleQuote: true`, `trailingComma: es5`.
- **Backend:** EditorConfig + `dotnet format`. 
  - All C# code uses implicit usings and file-scoped namespaces.
- **Line Endings:** LF (`\n`) for all files.

---

## 4. TypeScript & React Standards

### 4.1 Strict Typing
- `any` is strictly prohibited. Use `unknown` if the type is truly dynamic, followed by type narrowing.
- Enable `strict: true` in `tsconfig.json`.

### 4.2 React Components
- Use Functional Components with Hooks. Class components are banned.
- Use explicit return types or `React.FC` interface.
- Destructure props in the function signature.
- Keep components small (< 150 lines). Extract logic to custom hooks.

```tsx
// ✅ GOOD
interface Props {
  doctorId: string;
  onSelect: (id: string) => void;
}

export const DoctorCard = ({ doctorId, onSelect }: Props) => {
  // logic
  return <div>...</div>;
};
```

### 4.3 Data Fetching
- Use **React Query** (TanStack Query) for data fetching, caching, and state synchronization on the client.
- Use Next.js Server Components (RSC) for initial data loads where SEO or performance dictates.

---

## 5. C# & ASP.NET Core Standards

### 5.1 Project Structure
- Use the **Repository Pattern** and **Unit of Work** for database access.
- Controllers should be extremely thin. All business logic lives in the `Application` layer (Services or CQRS Handlers).

### 5.2 Dependency Injection
- Never instantiate services using `new`. Inject everything via constructor injection.

### 5.3 Data Transfer Objects (DTOs)
- Never return Domain Entities or Database Models directly from an API.
- Always map Domain Models to DTOs using Mapper (e.g., AutoMapper or Mapster).
- Separate Request DTOs and Response DTOs (e.g., `CreatePatientRequest`, `PatientResponse`).

### 5.4 Validation
- Use **FluentValidation** in the Application layer. Do not rely solely on data annotations.
- Controllers automatically validate incoming requests and return `422 Unprocessable Entity` on failure.

---

## 6. Error Handling & Logging

### 6.1 Backend Exceptions
- Never throw raw `Exception`. Throw custom domain exceptions (e.g., `NotFoundException`, `ValidationException`, `ConflictException`).
- A global exception middleware catches these and formats them into **RFC 7807 Problem Details** JSON responses.

### 6.2 Logging
- Use **Serilog** for structured logging.
- Log Levels:
  - `Error`: System failure, unhandled exception (triggers alerts).
  - `Warning`: Handled business errors, rate limits hit.
  - `Information`: State changes, audit trails (e.g., "User {UserId} logged in").
  - `Debug`: Tracing details (development only).
- Never log sensitive data (Passwords, Tokens, PHI).

---

## 7. Testing Standards

- **Unit Tests:** High coverage required for Domain logic, Application Services, and complex UI hooks.
  - *Backend:* xUnit + Moq + FluentAssertions.
  - *Frontend:* Jest + React Testing Library.
- **Integration Tests:** Test API endpoints against an in-memory or test database (Testcontainers).
- **Naming Convention (Tests):** `MethodName_StateUnderTest_ExpectedBehavior` (e.g., `CancelAppointment_WindowExceeded_ThrowsValidationException`).
- **Arrange, Act, Assert (AAA):** Structure all tests using the AAA pattern.

---

## 8. Git & Version Control

### 8.1 Branching Strategy
- `main`: Production-ready code. Protected branch.
- `develop`: Integration branch.
- `feature/*`: New features (e.g., `feature/patient-registration`).
- `bugfix/*`: Bug fixes (e.g., `bugfix/timezone-issue`).

### 8.2 Commit Messages (Conventional Commits)
Format: `type(scope): description`
- `feat(auth): add email verification OTP`
- `fix(appointments): resolve timezone overlap bug`
- `docs(api): update consultation endpoint spec`
- `refactor(db): optimize audit log indexes`

### 8.3 Pull Request (PR) Checklist
- Code compiles without warnings.
- Linters and formatters pass.
- Unit and integration tests pass.
- No hardcoded secrets.
- Documentation (Swagger, Markdown) is updated.
- Requires at least 1 approval from a senior engineer.
