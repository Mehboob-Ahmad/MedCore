# 📦 Packages

This directory contains shared TypeScript packages consumed by all frontend applications (`apps/`).

## Architecture Rationale

Shared packages are the backbone of monorepo efficiency. By extracting common functionality into discrete packages, we achieve:

- **DRY principle** — Write once, use across website, dashboard, and mobile
- **Consistency** — Unified types, API contracts, and UI components across all apps
- **Independent versioning** — Each package can be versioned and published independently
- **Faster development** — Developers don't reinvent common utilities

## Structure

```
packages/
├── ui/           → Shared React component library (buttons, forms, modals, etc.)
├── api-client/   → Typed HTTP client for the MedicHp API
├── types/        → Shared TypeScript interfaces and type definitions
├── config/       → Shared configuration (ESLint, Prettier, TSConfig, etc.)
├── utils/        → Common utility functions (date formatting, validation, etc.)
└── constants/    → Shared constants (roles, status enums, error codes, etc.)
```

## Package Descriptions

### `ui/`
A shared React component library providing a consistent design system across all frontend apps. Components are built with accessibility (a11y) and responsiveness in mind.

### `api-client/`
A typed HTTP client wrapping all MedicHp API endpoints. Provides type-safe request/response handling, automatic token refresh, and error normalization.

### `types/`
Shared TypeScript type definitions and interfaces that mirror backend DTOs. Ensures frontend–backend contract alignment and enables compile-time safety.

### `config/`
Shared tooling configuration: ESLint rules, Prettier settings, TypeScript base configs, and other development toolchain settings used across all apps.

### `utils/`
Common utility functions such as date/time formatting, input validation helpers, string manipulation, and other reusable logic.

### `constants/`
Application-wide constants: user roles, appointment statuses, HTTP status codes, error message keys, and other domain-specific enumerations.
