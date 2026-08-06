# 🤝 Contributing to MedCore

Thank you for your interest in contributing to the MedCore Digital Healthcare Ecosystem. This document provides guidelines and conventions to ensure a smooth collaboration experience.

---

## 📋 Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Workflow](#development-workflow)
- [Branch Naming](#branch-naming)
- [Commit Conventions](#commit-conventions)
- [Pull Request Process](#pull-request-process)
- [Coding Standards](#coding-standards)
- [Reporting Issues](#reporting-issues)

---

## Code of Conduct

By participating in this project, you agree to maintain a respectful, inclusive, and harassment-free environment. We are committed to providing a welcoming experience for everyone.

---

## Getting Started

1. **Fork** the repository
2. **Clone** your fork locally
3. **Install** prerequisites (see [README.md](README.md#prerequisites))
4. **Create** a feature branch from `develop`
5. **Make** your changes
6. **Submit** a pull request

---

## Development Workflow

```
main          ← Production-ready code (protected)
├── develop   ← Integration branch for feature work
│   ├── feature/TICKET-description
│   ├── bugfix/TICKET-description
│   └── hotfix/TICKET-description
```

- All feature work branches off `develop`
- Hotfixes branch from and merge back into `main` (and `develop`)
- Release branches are created from `develop` when preparing a release

---

## Branch Naming

Use the following convention:

| Type      | Pattern                          | Example                           |
|-----------|----------------------------------|-----------------------------------|
| Feature   | `feature/TICKET-short-desc`      | `feature/MC-42-patient-profile`   |
| Bugfix    | `bugfix/TICKET-short-desc`       | `bugfix/MC-108-login-redirect`    |
| Hotfix    | `hotfix/TICKET-short-desc`       | `hotfix/MC-200-auth-crash`        |
| Chore     | `chore/short-desc`               | `chore/update-dependencies`       |
| Docs      | `docs/short-desc`                | `docs/api-specification`          |

---

## Commit Conventions

We follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <short description>

[optional body]

[optional footer(s)]
```

### Types

| Type       | Description                                      |
|------------|--------------------------------------------------|
| `feat`     | A new feature                                    |
| `fix`      | A bug fix                                        |
| `docs`     | Documentation only changes                       |
| `style`    | Formatting, missing semicolons, etc.             |
| `refactor` | Code change that neither fixes a bug nor adds a feature |
| `perf`     | Performance improvement                          |
| `test`     | Adding or correcting tests                       |
| `build`    | Changes to the build system or dependencies      |
| `ci`       | Changes to CI configuration                      |
| `chore`    | Other changes that don't modify src or test files|
| `revert`   | Reverts a previous commit                        |

### Scopes

Use the relevant project area: `backend`, `website`, `dashboard`, `mobile`, `db`, `docker`, `ci`, `docs`, `packages`

### Examples

```
feat(backend): add patient registration endpoint
fix(dashboard): resolve date picker timezone offset
docs(specs): update API specification for appointments
chore(ci): add backend build workflow
```

---

## Pull Request Process

1. **Ensure** your branch is up to date with `develop`
2. **Run** all tests locally before submitting
3. **Fill out** the PR template completely
4. **Request** review from at least one team member
5. **Address** all review comments
6. **Squash** commits if requested by the reviewer

### PR Checklist

- [ ] Code follows the project's coding standards
- [ ] Tests are included for new functionality
- [ ] Documentation is updated if applicable
- [ ] No unrelated changes are included
- [ ] Branch is rebased on latest `develop`

---

## Coding Standards

Refer to the detailed standards in [`docs/Specifications/CODING_STANDARDS.md`](docs/Specifications/CODING_STANDARDS.md).

### Quick Summary

| Area          | Convention                                          |
|---------------|-----------------------------------------------------|
| C# (Backend)  | PascalCase classes/methods, camelCase locals         |
| TypeScript     | camelCase functions/variables, PascalCase types      |
| React          | PascalCase components, camelCase props               |
| CSS            | kebab-case class names or CSS Modules                |
| SQL            | UPPER CASE keywords, snake_case identifiers          |
| File names     | kebab-case for TS/JS, PascalCase for C# classes      |

---

## Reporting Issues

When opening an issue, please include:

1. **Summary** — Brief description of the problem
2. **Steps to reproduce** — Exact steps to trigger the issue
3. **Expected behavior** — What should happen
4. **Actual behavior** — What actually happens
5. **Environment** — OS, browser, Node.js version, .NET version
6. **Screenshots** — If applicable

Use the appropriate issue template when available.

---

## 📄 License

By contributing to MedCore, you agree that your contributions will be licensed under the [MIT License](LICENSE).
