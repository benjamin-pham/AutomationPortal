<!--
==============================================================================
SYNC IMPACT REPORT
==============================================================================
Version change: (empty) → 1.0.0
Bump rationale: MINOR — initial population of constitution from empty file;
                all principles and sections are new additions.

Modified principles: N/A (initial creation)

Added sections:
  - Core Principles (I–V)
  - Technology Stack
  - Development Workflow & Speckit Language
  - Governance

Removed sections: N/A

Templates reviewed:
  ✅ .specify/templates/plan-template.md   — "Constitution Check" gate present; aligns with principles
  ✅ .specify/templates/spec-template.md   — User-story/acceptance format; aligns with Principle III
  ✅ .specify/templates/tasks-template.md  — Phase structure; aligns with Principle III & IV
  ⚠  .specify/templates/commands/*.md     — No command files found; nothing to update

Deferred TODOs: None
==============================================================================
-->

# AutomationPortal Constitution

## Core Principles

### I. Clean Architecture (NON-NEGOTIABLE)

The backend MUST follow a strict four-layer Clean Architecture:
`Domain → Application → Infrastructure → API`.

- Dependencies MUST flow inward only; outer layers depend on inner layers, never the reverse.
- Domain entities MUST contain business logic (Rich Domain Model); anemic service classes are forbidden.
- Application layer MUST use MediatR CQRS: one `Command`/`Query` class + one `Handler` class per operation,
  each in its own feature folder `Features/{Feature}/{Operation}/`.
- All domain errors MUST be expressed as `Result<T>` / `Result.Failure(error)`; exceptions MUST NOT be used
  for expected domain outcomes.
- FluentValidation validators MUST be co-located with their command/query and run automatically via
  `ValidationBehavior`; inline validation is forbidden.
- `IDateTimeProvider` MUST replace all direct `DateTime.Now` / `DateTime.UtcNow` usages.
- `IOptions<T>` MUST be used for configuration; raw `config["Key"]` access is forbidden.
- NuGet package versions MUST be centrally managed in `Directory.Packages.props`; `Version=` attributes
  on individual `<PackageReference>` tags are forbidden.
- Nullable reference types MUST be enabled project-wide; all nullable paths MUST be handled.

### II. Frontend Best Practices (NON-NEGOTIABLE)

The frontend MUST follow Next.js 16 App Router conventions and project-specific patterns.

- All Next.js 16 async APIs (`params`, `searchParams`, `cookies()`, `headers()`) MUST be `await`ed in
  Server Components and Server Actions.
- API calls MUST use the project's Axios DI pattern:
  - Server Components → `axiosServerInstance` (reads `accessToken` cookie, async).
  - Client Components → `axiosClientInstance` (reads `authToken` from localStorage, sync).
  - New API domains MUST be added under `src/api/{entity}/` with one function per file, aggregated via
    `{entity}Api(axios)` in an `index.ts`, and registered in `src/api/index.ts`.
- Page-specific components MUST be co-located with their `page.tsx`.
- Each `<FormField render={...}>` block MUST be extracted to its own file inside a `form/` subdirectory;
  multiple inline FormField render props in a single file are forbidden.
- UI primitives MUST come from `src/components/ui/` (Shadcn/Radix); do not duplicate Radix primitives.
- The `cn()` utility (`clsx` + `tailwind-merge`) from `src/lib/utils.ts` MUST be used for conditional
  class name composition; string concatenation for class names is forbidden.

### III. Code Quality & Simplicity (NON-NEGOTIABLE)

All code MUST be clean, readable, and the simplest correct solution.

- Async MUST be used end-to-end in the backend; `.Result`, `.Wait()`, or blocking async calls are forbidden.
- Record types MUST be used for DTOs and response models in the backend.
- Abstractions MUST NOT be introduced speculatively; build only what the current task requires (YAGNI).
- Helpers and utilities MUST NOT be created for one-time operations; prefer inline code.
- Error handling and validation MUST NOT be added for impossible scenarios; trust framework guarantees.
- Dead code, unused variables, and orphaned files MUST be deleted; backwards-compatibility shims are
  forbidden unless explicitly required by an external contract.
- Comments MUST be reserved for non-obvious logic; self-documenting naming is preferred.
- Security MUST be considered at every change: no command injection, XSS, SQL injection, or other OWASP
  Top-10 vulnerabilities; inputs MUST be validated at system boundaries.

### IV. Testing Discipline

Tests MUST match scope; integration tests MUST use real infrastructure.

- **Domain.UnitTests** — entity and domain logic; xunit + FluentAssertions.
- **Application.UnitTests** — handlers and validators; NSubstitute for mocking.
- **Infrastructure.IntegrationTests** — repositories and EF Core; Testcontainers (PostgreSQL) + Respawn.
- **API.IntegrationTests** — end-to-end HTTP + auth; WebApplicationFactory + Testcontainers + Respawn.
- **ArchitectureTests** — layer dependency enforcement; NetArchTest.Rules.
- Database mocks (e.g., in-memory EF) are FORBIDDEN in integration tests; a real PostgreSQL container
  (via Testcontainers) MUST be used.
- Respawn MUST reset database state between tests.
- Frontend has no test suite configured; this is acceptable for the current project stage.

### V. Observability & Traceability

Structured logging MUST be present for all significant operations.

- Serilog is the logging backend; `ILogger<T>` MUST be injected and used throughout handlers and services.
- Structured log properties MUST use `{Property}` (scalar) or `{@Property}` (destructured object) notation.
- All commands and queries MUST produce at least one log entry at appropriate level
  (Information for success, Warning for expected failures, Error for unexpected exceptions).
- API documentation MUST be kept current via Scalar; endpoints MUST be named (`WithName()`).

## Technology Stack

| Concern | Technology |
|---|---|
| Backend runtime | .NET 10 |
| Backend framework | ASP.NET Core Minimal API |
| CQRS | MediatR |
| Validation | FluentValidation |
| ORM | EF Core (PostgreSQL, snake_case Fluent API) |
| Auth | JWT + API Key |
| Logging | Serilog |
| API docs | Scalar |
| Frontend framework | Next.js 16 App Router (Turbopack) |
| Styling | Tailwind CSS v4 |
| UI components | Shadcn/Radix UI |
| Forms | react-hook-form + zod |
| Tables | TanStack Table (server-side) |
| HTTP client | Axios (server + client instances) |
| Icons | lucide-react |
| Charts | recharts |
| Toasts | sonner |

## Development Workflow & Speckit Language

All speckit slash-command outputs (`/speckit.specify`, `/speckit.clarify`, `/speckit.plan`,
`/speckit.tasks`, `/speckit.analyze`, `/speckit.implement`) MUST be written in **Vietnamese**.

This applies to:
- Feature specifications (`spec.md`)
- Clarification questions and answers
- Implementation plans (`plan.md`)
- Task lists (`tasks.md`)
- Analysis reports
- Implementation commentary and progress output

Code, file paths, identifiers, and command syntax remain in English regardless of this rule.

Development commands:
- Backend: `cd backend && dotnet build` / `dotnet run --project src/AutomationPortal.API/...` / `dotnet test`
- Frontend: `cd frontend && npm run dev` / `npm run build` / `npm run lint`

## Governance

This constitution supersedes all other project-level practices and conventions. Any practice not explicitly
addressed here MUST default to the principles above.

**Amendment procedure**:
1. Propose the change with rationale in a PR description referencing this file.
2. Increment `CONSTITUTION_VERSION` following semantic versioning:
   - MAJOR: Removal or redefinition of an existing principle.
   - MINOR: New principle or materially expanded guidance.
   - PATCH: Clarification, wording, or typo fix.
3. Update `LAST_AMENDED_DATE` to the amendment date (ISO 8601).
4. Propagate changes to all templates in `.specify/templates/` and update this file's Sync Impact Report.

**Compliance review**: All PRs and code reviews MUST verify adherence to Principles I–V.
Complexity beyond what the task requires MUST be explicitly justified in the PR description.

**Runtime guidance**: See `README.md` and `CLAUDE.md` for agent-specific development guidance.

**Version**: 1.0.0 | **Ratified**: 2026-04-02 | **Last Amended**: 2026-04-02
