# 📂 Folder Structure — MedCore Digital Healthcare Ecosystem

> **Document Type:** Architecture & Repository Map
> **Version:** 1.0
> **Last Revised:** 2026-08-06
> **Status:** Active
> **Audience:** All Software Engineers, AI Coding Assistants

---

## Table of Contents
- [1. Monorepo Overview](#1-monorepo-overview)
- [2. Root Directory Structure](#2-root-directory-structure)
- [3. Backend Structure (.NET)](#3-backend-structure-net)
- [4. Frontend Structure (Next.js)](#4-frontend-structure-nextjs)
- [5. Shared Packages](#5-shared-packages)
- [6. Scalability Strategy](#6-scalability-strategy)

---

## 1. Monorepo Overview

MedCore utilizes a **Monorepo** architecture managed by **Turborepo** (or equivalent workspace tool). This allows us to share code, configurations, and typing between the frontend, mobile app, and backend while maintaining strict boundaries.

---

## 2. Root Directory Structure

```text
MedCore/
│
├── apps/
│   ├── web/                # Next.js (Marketing + Patient Portal)
│   ├── admin/              # React Admin Dashboard
│   └── mobile/             # React Native Expo
│
├── services/
│   ├── api/                # ASP.NET Core Web API
│   ├── realtime/           # Future SignalR/WebSockets
│   ├── notifications/      # Future Notification Service
│   └── ai/                 # Future AI Service
│
├── packages/               # Shared internal libraries
│   ├── ui/                 # React UI components
│   ├── types/              # TypeScript types / interfaces
│   ├── api-client/         # Axios/Fetch wrappers & API contracts
│   ├── config/             # ESLint, Prettier, TSConfig
│   ├── utils/              # Helper functions (Date formatting, etc.)
│   ├── constants/          # Shared enums and constants
│   ├── validation/         # Zod/Yup schemas
│   └── hooks/              # Custom React hooks
│
├── infrastructure/         # DevOps and deployment
│   ├── docker/             # Dockerfiles and compose files
│   ├── nginx/              # Reverse proxy configuration
│   ├── github/             # GitHub Actions workflows
│   └── scripts/            # Build and utility scripts
│
├── database/               # Migrations, seeders, and SQL scripts
├── docs/                   # Specifications, ADRs, and guides
├── tools/                  # Internal developer tools
├── tests/                  # End-to-end integration tests
└── package.json            # Workspace configuration
```

---

## 3. Backend Structure (.NET)

The `services/api/` folder follows **Clean Architecture**.

```text
services/api/
├── MedCore.sln                       # Solution File
├── src/
│   ├── MedCore.Domain/               # Layer 1: Core
│   │   ├── Entities/                 # Patient, Doctor, Appointment
│   │   ├── Enums/                    # Status enums
│   │   └── Exceptions/               # Domain exceptions
│   ├── MedCore.Application/          # Layer 2: Business Logic
│   │   ├── Interfaces/               # Repository & Service interfaces
│   │   ├── DTOs/                     # Data Transfer Objects
│   │   ├── Services/                 # Business logic implementation
│   │   └── Validators/               # FluentValidation rules
│   ├── MedCore.Infrastructure/       # Layer 3: External Concerns
│   │   ├── Data/                     # Entity Framework DbContext
│   │   ├── Repositories/             # EF Core implementation of Interfaces
│   │   └── Services/                 # Email, File Storage, Auth providers
│   └── MedCore.API/                  # Layer 4: Presentation
│       ├── Controllers/              # REST Endpoints
│       ├── Middlewares/              # Error handling, Logging
│       └── Program.cs                # DI setup, pipeline configuration
└── tests/
    ├── MedCore.UnitTests/            # Isolated logic tests
    └── MedCore.IntegrationTests/     # API & DB testing
```

**Import Rules (Backend):**
- `API` depends on `Application` and `Infrastructure`.
- `Infrastructure` depends on `Application`.
- `Application` depends on `Domain`.
- `Domain` depends on **nothing**.

---

## 4. Frontend Structure (Next.js)

The `apps/web/` folder uses the Next.js App Router and a feature-sliced design pattern.

```text
apps/web/
├── src/
│   ├── app/                          # Next.js App Router
│   │   ├── (auth)/                   # Login, Register pages
│   │   ├── (dashboard)/              # Protected patient/doctor routes
│   │   ├── api/                      # Next.js API Routes (BFF pattern if needed)
│   │   ├── layout.tsx                # Global layout
│   │   └── page.tsx                  # Landing page
│   ├── components/                   # Application-specific components
│   │   ├── layout/                   # Header, Footer, Sidebar
│   │   └── shared/                   # Specific shared elements (Cards, Forms)
│   ├── features/                     # Feature-sliced domain modules
│   │   ├── appointments/             # Appointment-specific hooks, components, api calls
│   │   ├── consultations/            # Consultation-specific logic
│   │   └── search/                   # Search-specific logic
│   ├── lib/                          # Utility wrappers
│   │   ├── apiClient.ts              # Axios/Fetch wrapper with interceptors
│   │   └── queryClient.ts            # React Query configuration
│   ├── hooks/                        # Global custom hooks (useAuth, useToast)
│   ├── stores/                       # Global state (Zustand or Context)
│   └── styles/                       # Global CSS, Tailwind config
├── public/                           # Static assets, fonts, icons
├── next.config.js
└── tailwind.config.js
```

**Import Rules (Frontend):**
- Features cannot import from other features directly (promotes decoupling). Shared logic goes to `components/shared` or `hooks`.
- Use absolute imports (e.g., `@/features/appointments/...`).

---

## 5. Shared Packages

To ensure consistency, shared code lives in `packages/`.
- `packages/ui`: A component library (e.g., Tailwind, shadcn/ui) imported by web and admin.
- `packages/types`: Type definitions ensuring the frontend uses correct API interfaces.
- `packages/api-client`: Shared data fetching logic and REST wrappers.
- `packages/validation`: Cross-platform validation schemas (e.g., Zod) shared across frontends.
- `packages/utils`, `constants`, `hooks`: Common cross-app business logic and constants.

---

## 6. Scalability Strategy

This structure supports infinite scalability:
1. **Team Scaling:** Backend, Web, and Mobile teams can work in their respective folders independently while utilizing shared contracts.
2. **Microservices Ready:** The `services/` directory explicitly supports breaking out new microservices (e.g., `services/realtime/`, `services/ai/`) alongside the main API.
3. **Deployment:** Turborepo analyzes dependency trees and only rebuilds and redeploys the apps that have changed.
