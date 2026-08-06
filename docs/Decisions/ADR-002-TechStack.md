# ADR-002: Technology Stack Selection

> **Status:** Accepted  
> **Date:** 2026-08-04  
> **Decision Makers:** MedCore Architecture Team

---

## Context

We need to select technologies for the frontend, backend, database, and infrastructure layers that support a scalable, maintainable, commercial healthcare platform.

## Decision

| Layer | Technology | Rationale |
|-------|-----------|-----------|
| Website | Next.js | SSR for SEO, React ecosystem, performance |
| Dashboard | React + Vite | Fast HMR, SPA for admin workflows, lightweight |
| Mobile | React Native + Expo | Cross-platform (iOS + Android), shared JS knowledge |
| Backend | ASP.NET Core 9 | Performance, type safety, enterprise-grade, healthcare compliance tooling |
| ORM | Entity Framework Core | First-party .NET ORM, migrations, LINQ queries |
| Database | PostgreSQL | Open source, JSONB support, RLS, mature ecosystem |
| Cache | Redis | In-memory speed, pub/sub, session management |
| CI/CD | GitHub Actions | Native GitHub integration, marketplace ecosystem |
| Containers | Docker | Environment parity, deployment consistency |

## Alternatives Considered

### Backend: Node.js + Express (Rejected)
- Weaker type safety compared to C#
- Less mature ORM options for complex healthcare schemas
- Performance concerns for CPU-intensive operations

### Database: MySQL (Rejected)
- Lacking JSONB, RLS, and advanced indexing compared to PostgreSQL
- Weaker support for complex queries and full-text search

### Mobile: Flutter (Rejected)
- Different language (Dart) increases team knowledge requirements
- Less code sharing with TypeScript frontend packages

## Consequences

- Team must be proficient in both TypeScript and C#
- Two build systems (npm/node and dotnet) to maintain
- Shared types require manual synchronization between TypeScript and C# (mitigated by code generation)
