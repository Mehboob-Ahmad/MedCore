# ADR-001: Monorepo Architecture

> **Status:** Accepted  
> **Date:** 2026-08-04  
> **Decision Makers:** MedicHp Architecture Team

---

## Context

MedicHp consists of multiple frontend applications (website, dashboard, mobile), a backend API, and shared packages. We need to decide between a monorepo and a polyrepo structure.

## Decision

We will use a **monorepo** architecture managed at the repository root level.

## Rationale

- **Code sharing** — Shared TypeScript packages (`packages/`) are consumed across all apps without publishing
- **Atomic changes** — Cross-cutting changes (e.g., API contract updates) can be made in a single PR
- **Unified CI/CD** — One pipeline to lint, test, and deploy all services
- **Developer experience** — Single clone, single setup, consistent tooling
- **Dependency alignment** — All apps use the same versions of shared dependencies

## Alternatives Considered

### Polyrepo (Rejected)
- Higher overhead for cross-project changes
- Dependency version drift
- Separate CI/CD pipelines to maintain
- More complex onboarding

## Consequences

- Need clear `CODEOWNERS` to manage folder-level permissions
- CI must be smart about running only affected tests
- Larger repository size over time (mitigated by sparse checkout)
