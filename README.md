# MedCore Digital Healthcare Ecosystem

![MedCore Architecture](https://via.placeholder.com/800x400?text=MedCore+Architecture)

MedCore is a production-ready, enterprise-grade Digital Healthcare Ecosystem built for scalability and long-term commercial use.

## Architecture

This project utilizes a **Turborepo Monorepo** architecture to enforce strict boundaries while seamlessly sharing configuration, types, and UI components across the stack.

### Directory Layout

- `apps/web`: Next.js App Router (Patient Portal & Marketing)
- `apps/admin`: Vite + React (Hospital Admin Dashboard)
- `apps/mobile`: React Native Expo (Mobile App)
- `services/api`: ASP.NET Core 9 Web API (Clean Architecture)
- `packages/*`: Shared utilities, types, config, and React components
- `infrastructure/`: Docker Compose, Nginx, and GitHub Actions configs

## Technology Stack

### Frontend
- Next.js / Vite / React Native (Expo)
- Tailwind CSS / Ant Design / shadcn/ui
- TanStack Query (React Query)
- Zustand (State Management)

### Backend
- .NET 9 (C# 13)
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL 16
- Redis

### Tooling
- pnpm Workspaces
- Turborepo
- ESLint / Prettier / Husky

## Development Commands

Ensure you have `pnpm`, `docker`, and `.NET 9 SDK` installed.

### Initial Setup
```bash
# Install dependencies across all workspaces
pnpm install

# Start the database and Redis using Docker Compose
docker-compose up -d
```

### Running the Frontend Apps
```bash
# Start all frontend apps simultaneously
pnpm dev

# Start a specific app (e.g., the web portal)
pnpm turbo run dev --filter=web
```

### Running the Backend API
```bash
cd services/api/MedCore.API
dotnet run
```

## Contribution Guide

1. Read the `docs/Specifications/CODING_STANDARDS.md` before making any changes.
2. Adhere to **Conventional Commits** for all commit messages.
3. Ensure all tests and linting pass via `pnpm lint` and `dotnet build` before opening a Pull Request.

---
> *Generated in accordance with the MedCore architectural specification.*
