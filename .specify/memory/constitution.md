<!--
SYNC IMPACT REPORT
==================
Version change: 1.0.0 → 1.0.1
Modified principles: N/A (wording only — translated body to English)
Added sections: none
Removed sections: none
Templates requiring updates:
  ✅ .specify/memory/constitution.md — this file
  ⚠ .specify/templates/plan-template.md — Constitution Check gates should reference these principles
  ⚠ .specify/templates/spec-template.md — no structural changes required
  ⚠ .specify/templates/tasks-template.md — task categories align with principles
Follow-up TODOs: none
-->

# AutomationPortal Constitution

> **Speckit output language** (`/specify`, `/clarify`, `/plan`, `/analyze`, `/task`):
> All output produced by speckit commands (spec.md, plan.md, tasks.md, research.md, data-model.md,
> contracts/, clarification questions, analysis reports) MUST be written in **Vietnamese**.
> File names, paths, and code identifiers (variables, functions, classes) remain in English.

## Core Principles

### I. Clean Architecture & Domain-Driven Design (.NET 10)

The backend MUST follow Clean Architecture with a strict unidirectional dependency order:
`API → Application → Domain; Infrastructure → Domain`.

- **Domain**: Rich Domain Model — business logic lives in entities, not services.
  Entities MUST expose a static factory method `Create(...)`. Use `Result<T>` / `Error` for
  domain errors; do not use exceptions for business flow control.
- **Application**: CQRS via MediatR — each use case is an `ICommand`/`IQuery` + dedicated handler.
  FluentValidation validators go in the same folder as the handler; the pipeline runs them automatically.
  Never inject `DbContext` directly into handlers — use repository interfaces or `ISqlConnectionFactory`.
- **Infrastructure**: EF Core with Fluent API, snake_case naming convention.
  Repository implementations and `IEntityTypeConfiguration<T>` belong in `Infrastructure`.
- **API**: Minimal API class-per-endpoint implementing `IEndpoint`. Auto-registration — no manual wiring.
- **Non-negotiable rules**: Nullable reference types enabled. Async all the way — `.Result` and `.Wait()`
  are forbidden. Use `IDateTimeProvider` instead of `DateTime.Now/UtcNow`. Use `IOptions<T>` instead
  of raw config keys. Use record types for DTOs.
- **Package management**: All NuGet versions are centrally managed in `Directory.Packages.props`.
  Do not set `Version` on individual `.csproj` files; use `VersionOverride` only when necessary.

### II. Next.js 16 Frontend Best Practices

The frontend MUST use Next.js 16 App Router with React 19 and TypeScript strict mode.

- **Server Components by default**: Components MUST be Server Components unless interactivity is
  required (event handlers, hooks, browser APIs) — only then add `"use client"` at the top.
- **Data fetching**: Fetch data in Server Components or Server Actions. Avoid unnecessary client-side
  API calls. Use `axios` only in client context or via a dedicated service layer.
- **Routing**: Use App Router (`app/` directory). File conventions: `page.tsx`, `layout.tsx`,
  `loading.tsx`, `error.tsx`, `not-found.tsx`.
- **Styling**: Tailwind CSS with `clsx`/`cva` for conditional classes. No inline styles.
- **Form handling**: `react-hook-form` + `@hookform/resolvers` with Zod validation.
- **State management**: Prefer React built-ins (useState, useReducer, Context) before reaching for
  external libraries. Do not add Redux/Zustand without clear justification.
- **Components**: shadcn/ui (Radix UI) as the base. Do not build primitive components from scratch
  when a suitable one already exists.
- **Type safety**: No `any`. Export explicit types for API response shapes.

### III. Test-First (NON-NEGOTIABLE)

Tests MUST be written before implementation. Tests MUST fail first; only then implement to make them
pass. Never bypass or comment-out tests to force CI green.

| Project | Scope | Key Tools |
|---------|-------|-----------|
| `Domain.UnitTests` | Entity logic, domain rules | xUnit, FluentAssertions |
| `Application.UnitTests` | Handlers, validators, behaviors | + NSubstitute |
| `Infrastructure.IntegrationTests` | Repositories, EF Core config | + Testcontainers (PostgreSQL), Respawn |
| `API.IntegrationTests` | End-to-end HTTP | + WebApplicationFactory, Testcontainers, Respawn |
| `ArchitectureTests` | Layer dependency enforcement | NetArchTest.Rules |

Integration tests MUST run against a real PostgreSQL container (Testcontainers).
Do not mock the database in integration tests — mock/prod divergence has caused real production failures.

### IV. Code Quality & Simplicity

Write the simplest code that solves the problem. Avoid over-engineering and premature abstraction.

- **YAGNI**: Do not add features, configuration, or abstractions for hypothetical future requirements.
- **No single-use helpers**: Three similar lines of code is better than a premature abstraction.
- **No speculative error handling**: Only validate at system boundaries (user input, external APIs).
  Do not add guards for scenarios that cannot happen given internal guarantees.
- **No commented-out code**: If something is unused, delete it — do not leave `// removed` comments.
- **No feature flags or backwards-compatibility shims** when you can just change the code directly.
- **No docstrings or comments** added to code that was not changed in the current task.
- **No surrounding refactors** when only a bug fix or small feature was requested.

### V. Observability & Error Handling

- **Logging**: Use `ILogger<T>` with Serilog backend. Structured logging with `{Property}` templates.
  Choose the correct level: Debug (dev detail), Information (business event), Warning (recoverable),
  Error (failures requiring attention).
- **Correlation ID**: Every request MUST carry a correlation ID propagated through all log entries.
- **Global exception handler**: Use `GlobalExceptionHandler` — no unhandled exception may bubble out
  of the API without a proper error response.
- **Problem Details**: Return RFC 9457 Problem Details format for all error responses.
- **Result pattern**: Domain errors MUST use `Result<T>` — do not throw exceptions for business logic failures.

## Technology Stack

### Backend

- **Runtime**: .NET 10
- **Framework**: ASP.NET Core Minimal API
- **ORM**: EF Core with PostgreSQL
- **CQRS**: MediatR
- **Validation**: FluentValidation
- **Logging**: Serilog
- **Auth**: JWT Bearer + API Key (dual scheme)
- **API Docs**: Scalar (OpenAPI)
- **Testing**: xUnit, NSubstitute, FluentAssertions, Testcontainers, Respawn, NetArchTest

### Frontend

- **Framework**: Next.js 16 (App Router)
- **Runtime**: React 19
- **Language**: TypeScript (strict mode)
- **Styling**: Tailwind CSS v4
- **Components**: shadcn/ui (Radix UI)
- **Forms**: react-hook-form + Zod
- **HTTP client**: axios
- **Tables**: TanStack Table v8
- **Charts**: Recharts

## Development Workflow

- **Git**: Feature branches from `main`. Branch name: `###-feature-name` (ticket number + slug).
- **Commits**: Conventional Commits (`feat:`, `fix:`, `docs:`, `refactor:`, `test:`, `chore:`).
- **PRs**: MUST have test coverage for new code. CI MUST be green before merge.
- **Code review**: At least 1 approval. Reviewer checks compliance with this constitution.
- **No force push** to `main` or any branch with an open PR.
- **Database migrations**: Every schema change MUST have a migration file. Never edit an already-applied migration.
- **Speckit output**: All documents generated by speckit commands (`/specify`, `/clarify`,
  `/plan`, `/analyze`, `/task`) MUST be written in Vietnamese.

## Governance

This constitution supersedes all individual conventions and practices.
All architectural decisions MUST align with the principles above.

**Amendment procedure**:
1. Propose changes via a PR titled `docs: amend constitution to vX.Y.Z (...)`.
2. Describe the reason for the change and its impact on existing templates and artifacts.
3. Update the Sync Impact Report at the top of this file.
4. Requires at least 1 approval from the tech lead or project owner.

**Versioning policy**:
- MAJOR: Removal or redefinition of a core principle (breaking governance change).
- MINOR: New principle or section added, or materially expanded guidance.
- PATCH: Clarifications, typo fixes, wording improvements with no semantic change.

**Compliance review**: Every PR review MUST verify the Constitution Check gates in plan.md.
Complexity violations MUST be documented in the Complexity Tracking table in plan.md.

**Version**: 1.0.1 | **Ratified**: 2026-04-01 | **Last Amended**: 2026-04-01
