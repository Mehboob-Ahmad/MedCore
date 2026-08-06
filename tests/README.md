# 🧪 Tests

This directory contains all test suites for the MedCore platform, organized by scope.

## Architecture Rationale

A dedicated top-level `tests/` directory provides:

- **Clear test boundaries** — Backend, frontend, and E2E tests are cleanly separated
- **CI/CD targeting** — Workflows can run specific test suites independently
- **Shared fixtures** — Common test data and utilities are co-located
- **Coverage visibility** — Test health is immediately visible at the repo root

## Structure

```
tests/
├── backend/     → Unit and integration tests for the ASP.NET Core API
├── frontend/    → Component and snapshot tests for React/Next.js apps
└── e2e/         → End-to-end tests simulating real user workflows
```

## Test Categories

### `backend/`
- **Unit tests** — Domain logic, services, validators
- **Integration tests** — API endpoints, database operations, caching
- **Framework** — xUnit / NUnit with Moq, FluentAssertions

### `frontend/`
- **Component tests** — Isolated React component rendering and interaction
- **Snapshot tests** — Visual regression detection
- **Framework** — Vitest, React Testing Library

### `e2e/`
- **User flow tests** — Complete workflows: registration, login, booking, etc.
- **Cross-browser testing** — Chromium, Firefox, WebKit
- **Framework** — Playwright
