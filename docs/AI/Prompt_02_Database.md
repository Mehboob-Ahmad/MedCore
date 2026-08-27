# Prompt 02 — Database Design

> AI prompt for designing and implementing the MedicHp database schema.

---

## Objective

Design a normalized, scalable PostgreSQL database schema for the MedicHp platform using Entity Framework Core.

## Tasks

1. Design core entity models (Users, Patients, Doctors, Clinics, Appointments)
2. Define relationships and constraints
3. Create initial EF Core migrations
4. Implement seed data for development
5. Document the schema in `docs/Specifications/DATABASE_ARCHITECTURE.md`

## Constraints

- Use UUID primary keys
- Include audit columns on all tables (`CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`)
- Implement soft deletes (`IsDeleted`, `DeletedAt`)
- Follow 3NF normalization

---

> **Status:** Placeholder — To be expanded with detailed schema requirements.
