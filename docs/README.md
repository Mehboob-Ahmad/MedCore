# 📚 Documentation

This directory contains all project documentation for the MedCore Digital Healthcare Ecosystem.

## Architecture Rationale

Comprehensive documentation is a first-class citizen in MedCore. This directory ensures:

- **Single source of truth** — All decisions, specs, and guidelines live alongside the code
- **Onboarding velocity** — New team members can self-serve without tribal knowledge
- **AI-assisted development** — Structured prompts and specs enable consistent AI code generation
- **Audit trail** — Architecture Decision Records (ADRs) capture the _why_ behind every major choice

## Structure

```
docs/
├── README_FOR_AI.md       → Instructions for AI assistants working on this codebase
├── Sources/               → Roadmap, competitor analysis, references, future ideas
├── Specifications/        → Technical specs, business rules, coding standards
├── AI/                    → Structured prompts for AI-assisted development
├── Decisions/             → Architecture Decision Records (ADRs)
├── Diagrams/              → Visual documentation (architecture, DB, flows, UI)
├── Brand/                 → Brand identity assets (logos, colors, typography)
└── Releases/              → Release notes and changelogs
```

## Quick Links

| Document | Purpose |
|----------|---------|
| [Project Specification](Specifications/PROJECT_SPECIFICATION.md) | Full system requirements |
| [Product Requirements](Specifications/PRODUCT_REQUIREMENTS.md) | Feature-level requirements |
| [Database Architecture](Specifications/DATABASE_ARCHITECTURE.md) | Schema and data model |
| [API Specification](Specifications/API_SPECIFICATION.md) | REST API contracts |
| [Security Guidelines](Specifications/SECURITY_GUIDELINES.md) | Security policies and practices |
| [Roadmap](Sources/MedCore_Roadmap.md) | Feature timeline |
| [Master Prompt](AI/MASTER_PROMPT.md) | AI development context |
