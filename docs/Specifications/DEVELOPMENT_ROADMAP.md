# 🚀 Development Roadmap — MedCore Digital Healthcare Ecosystem

> **Document Type:** Engineering Roadmap
> **Version:** 1.0
> **Last Revised:** 2026-08-06
> **Status:** Active
> **Audience:** Project Managers, Engineering Leads, Software Developers

---

## Table of Contents
- [Roadmap Overview](#roadmap-overview)
- [Phase 1: Foundation (Current)](#phase-1-foundation)
  - [Sprint 1: Infrastructure & Scaffolding](#sprint-1-infrastructure--scaffolding)
  - [Sprint 2: Database & Backend Core](#sprint-2-database--backend-core)
  - [Sprint 3: Authentication & Security](#sprint-3-authentication--security)
  - [Sprint 4: Profile Management (Patient & Doctor)](#sprint-4-profile-management)
  - [Sprint 5: Intelligent Search & Discovery](#sprint-5-intelligent-search--discovery)
  - [Sprint 6: Appointment Engine](#sprint-6-appointment-engine)
  - [Sprint 7: Consultations & Clinical Records](#sprint-7-consultations--clinical-records)
  - [Sprint 8: Prescriptions & File Management](#sprint-8-prescriptions--file-management)
  - [Sprint 9: Chat & Notifications](#sprint-9-chat--notifications)
  - [Sprint 10: Super Admin & Analytics](#sprint-10-super-admin--analytics)
  - [Sprint 11: End-to-End Testing & QA](#sprint-11-end-to-end-testing--qa)
  - [Sprint 12: Staging & Production Deployment](#sprint-12-staging--production-deployment)
- [Future Phases (Outlook)](#future-phases-outlook)

---

## Roadmap Overview
This document breaks down **Phase 1 (Independent Doctors + Patients)** into 12 actionable sprints. Assuming 2-week sprints, this represents a ~6-month timeline to Minimum Viable Product (MVP) commercial launch.

---

## Phase 1: Foundation

### Sprint 1: Infrastructure & Scaffolding
- **Objectives:** Initialize the monorepo, set up CI/CD, scaffold application shells.
- **Deliverables:**
  - Turborepo setup with `web`, `admin`, `api` apps.
  - ASP.NET Core 9 Clean Architecture scaffolding.
  - Next.js App Router scaffolding with Tailwind CSS.
  - Shared UI package initialization.
  - GitHub Actions pipelines (Build, Lint, Test).
- **Dependencies:** None.
- **Complexity:** Medium.

### Sprint 2: Database & Backend Core
- **Objectives:** Implement the EF Core models, DB migrations, and core infrastructure services.
- **Deliverables:**
  - All 33 tables mapped in Entity Framework Core.
  - Initial PostgreSQL database migration.
  - Base Repository and Unit of Work patterns implemented.
  - Global Exception Handling and Serilog integration.
- **Dependencies:** Sprint 1.
- **Complexity:** High.

### Sprint 3: Authentication & Security
- **Objectives:** Implement robust identity and access management.
- **Deliverables:**
  - JWT generation and single-use Refresh Token rotation.
  - Password hashing (Argon2id).
  - Patient & Doctor Registration endpoints.
  - Login, Logout, Forgot/Reset Password flows.
  - Role-based authorization middleware.
- **Dependencies:** Sprint 2.
- **Complexity:** High.

### Sprint 4: Profile Management
- **Objectives:** Build APIs and UI for managing user profiles.
- **Deliverables:**
  - Patient progressive profile completion (Vitals, Allergies, History).
  - Doctor profile (Specializations, Experience, Bio).
  - Doctor Availability configuration (schedule definition).
  - Profile UIs in Next.js.
- **Dependencies:** Sprint 3.
- **Complexity:** Medium.

### Sprint 5: Intelligent Search & Discovery
- **Objectives:** Implement the core patient-to-doctor discovery engine.
- **Deliverables:**
  - Symptom-to-Specialization mapping algorithm (`SymptomSpecializations` table logic).
  - Doctor Search API with filtering (City, Fee, Rating) and pagination.
  - Public Website Search Interface and Doctor Profile pages.
- **Dependencies:** Sprint 4.
- **Complexity:** High.

### Sprint 6: Appointment Engine
- **Objectives:** Build the scheduling system avoiding slot conflicts.
- **Deliverables:**
  - Slot generation logic based on Doctor Availability.
  - Booking, Rescheduling, and Cancellation APIs.
  - Status transition state machine (Pending -> Confirmed -> Completed).
  - Patient and Doctor Appointment Dashboards UI.
- **Dependencies:** Sprint 5.
- **Complexity:** Very High (concurrency and timezone handling).

### Sprint 7: Consultations & Clinical Records
- **Objectives:** Enable doctors to record medical outcomes.
- **Deliverables:**
  - Create/Update Consultation APIs.
  - Finalization logic (Immutability lock).
  - Addendum logic for finalized records.
  - Consultation UI form for doctors.
- **Dependencies:** Sprint 6.
- **Complexity:** Medium.

### Sprint 8: Prescriptions & File Management
- **Objectives:** Digital prescriptions and document uploads.
- **Deliverables:**
  - Create Prescription APIs (linked to consultation).
  - Supersession logic (correction chain).
  - File upload endpoints (Profile photos, base logic for PDF generation).
- **Dependencies:** Sprint 7.
- **Complexity:** Medium.

### Sprint 9: Chat & Notifications
- **Objectives:** Asynchronous communication and system alerts.
- **Deliverables:**
  - RESTful chat endpoints (Conversation list, Messages).
  - Verification of patient-doctor appointment relationship.
  - In-app notification creation and read-state management.
  - *Optional Phase 1.5:* SignalR/WebSocket integration for real-time.
- **Dependencies:** Sprint 6.
- **Complexity:** High.

### Sprint 10: Super Admin & Analytics
- **Objectives:** Platform governance tools.
- **Deliverables:**
  - Admin Dashboard UI (React + Vite).
  - User management (Suspend/Reactivate).
  - System statistics APIs.
  - Audit Log viewer.
- **Dependencies:** Sprints 3-9.
- **Complexity:** Low to Medium.

### Sprint 11: End-to-End Testing & QA
- **Objectives:** Ensure production readiness and compliance.
- **Deliverables:**
  - 80%+ Unit Test coverage on Domain and Application layers.
  - Integration tests for critical flows (Booking, Search, Auth).
  - UI testing (Playwright/Cypress).
  - Security vulnerability scan and penetration testing remediation.
- **Dependencies:** All previous sprints.
- **Complexity:** High.

### Sprint 12: Staging & Production Deployment
- **Objectives:** Cloud provisioning and go-live.
- **Deliverables:**
  - Docker containers and cloud infrastructure (AWS/Azure) provisioning.
  - PostgreSQL managed database configuration.
  - SSL/TLS certification, Domain routing.
  - CI/CD deployment pipelines active.
  - Final UAT (User Acceptance Testing) sign-off.
- **Dependencies:** Sprint 11.
- **Complexity:** High.

---

## Future Phases (Outlook)
- **Phase 2:** Clinic/Hospital Management (Organizations, Roles, Multi-doctor clinics).
- **Phase 3:** Telemedicine & Billing (Video calls, Stripe integration, Invoicing).
- **Phase 4:** AI Integration (Diagnosis assist, automated transcription).
