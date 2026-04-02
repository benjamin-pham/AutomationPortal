## Repository Layout

Monorepo with two independently built parts:
- `backend/` — .NET 10 Clean Architecture API ([backend/README.md](backend/README.md))
- `frontend/` — Next.js 16 App Router SPA ([frontend/README.md](frontend/README.md))

Commands below assume you `cd` into the relevant directory first.

## Commands

### Backend (`cd backend`)

```bash
dotnet build
dotnet run --project src/AutomationPortal.API/AutomationPortal.API.csproj
dotnet test
dotnet test tests/AutomationPortal.Application.UnitTests/          # single project
dotnet test --filter "FullyQualifiedName~TestMethodName"           # single test
```

### Frontend (`cd frontend`)

```bash
npm run dev      # Turbopack dev server
npm run build
npm run start
npm run lint
```

## Backend Architecture

Clean Architecture with 4 layers (strict unidirectional dependency: API → Application → Domain; Infrastructure → Domain).

See [backend/README.md](backend/README.md) for detailed architecture, folder structure, and patterns.

## Frontend Architecture

Next.js 16 App Router application with collapsible sidebar layout.

See [frontend/README.md](frontend/README.md) for detailed architecture, components, and API patterns.

## Language of speckit Outputs

All output produced by speckit commands (`/speckit.specify`, `/speckit.clarify`,
`/speckit.plan`, `/speckit.tasks`, `/speckit.analyze`, `/speckit.implement`)
MUST be written in **Vietnamese**. Code, file paths, identifiers, and
code comments remain in English. Only prose, headings, descriptions,
rationale, and task descriptions are in Vietnamese.