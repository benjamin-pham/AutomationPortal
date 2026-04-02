## Repository Layout

Monorepo with two independently built parts:
- `backend/` — .NET 10 Clean Architecture API
- `frontend/` — Next.js 16 App Router SPA

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

**Layer dependency rule**: `API → Application → Domain`; `Infrastructure → Domain`

### Endpoints (API layer)

Implement `IEndpoint` in `src/AutomationPortal.API/Endpoints/`. Auto-discovered at startup — no manual registration.

```csharp
public sealed class CreateOrderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/orders", HandleAsync).WithName("CreateOrder");

    private static async Task<IResult> HandleAsync(CreateOrderCommand cmd, IMediator mediator)
    {
        var result = await mediator.Send(cmd);
        return result.IsFailure ? Results.BadRequest(result.Error) : Results.Created();
    }
}
```

### Commands & Queries (Application layer)

Feature folders live in `src/AutomationPortal.Application/Features/{Feature}/{Operation}/`. Each operation folder contains:
- `{Op}Command.cs` / `{Op}Query.cs` — implements `IRequest<Result<T>>`
- `{Op}CommandHandler.cs` / `{Op}QueryHandler.cs`
- `{Op}CommandValidator.cs` — `AbstractValidator<T>`; auto-run by `ValidationBehavior`
- `{Op}Response.cs` — always its own file

**Result pattern**: return `Result.Success(value)` / `Result.Failure(error)` — never throw for domain errors.

### Database (Infrastructure layer)

- PostgreSQL via EF Core; entity configs in `Data/Configurations/` using Fluent API + snake_case naming
- All entities extend `BaseEntity` — `AppDbContext.SaveChangesAsync` auto-sets audit fields and soft-deletes
- Never use data annotations on entities
- NuGet versions go in `Directory.Packages.props` — no `Version` on individual `<PackageReference>` tags

### Code style rules

- Async all the way — never `.Result` or `.Wait()`
- Inject `IDateTimeProvider` instead of `DateTime.Now`
- `IOptions<T>` for config — never `config["Key"]`
- Nullable reference types enabled throughout; use record types for DTOs

### Testing

| Project | Scope | Key libs |
|---|---|---|
| `Domain.UnitTests` | Entity/domain logic | xunit, FluentAssertions |
| `Application.UnitTests` | Handlers, validators | NSubstitute |
| `Infrastructure.IntegrationTests` | Repositories, EF Core | Testcontainers (PostgreSQL), Respawn |
| `API.IntegrationTests` | End-to-end HTTP + auth | WebApplicationFactory, Testcontainers, Respawn |
| `ArchitectureTests` | Layer dependency enforcement | NetArchTest.Rules |

Integration tests spin up a real PostgreSQL container (Docker required). Respawn resets DB state between tests.

## Frontend Architecture

### Next.js 16 specifics

Turbopack is the default. Request APIs are async — `params`, `searchParams`, `cookies()`, `headers()` must all be `await`ed:

```tsx
export default async function Page({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
}
```

### API layer (dependency injection pattern)

All API functions accept an `AxiosInstance` and are grouped under `domainApi(axios)` in `src/api/index.ts`.

- **Server Components** → `await axiosServerInstance()` from `src/api/axiosServerInstance.ts` (reads `accessToken` cookie)
- **Client Components** → `axiosClientInstance` from `src/api/axiosClientInstance.ts` (reads `authToken` from localStorage)

Adding a new domain: create `src/api/{entity}/` with one function per file, aggregate as `{entity}Api(axios)` in an `index.ts`, register in `src/api/index.ts`.

### Component & file structure

Page-specific components are co-located with `page.tsx`. Form fields **must** be extracted to individual files in a `form/` subdirectory — never inlined in the page:

```
src/app/(dashboard)/users/
├── page.tsx
├── users-table.tsx
└── form/
    ├── name-field.tsx
    └── email-field.tsx
```

### Key shared components

- `src/components/table/data-table.tsx` — TanStack Table wrapper with server-side sorting/pagination; pass `onSortChange` for server logic
- `src/components/form/form-field.tsx` — react-hook-form + zod wrapper; use `render` prop and spread `inputProps`
- `src/components/ui/` — Shadcn/Radix primitives
- `src/lib/utils.ts` — `cn()` utility (clsx + tailwind-merge)

### Stack

Tailwind CSS v4, lucide-react icons, @tanstack/react-table, react-hook-form + zod, next-themes, sonner (toasts), recharts, axios.

## Language of speckit Outputs

All output produced by speckit commands (`/speckit.specify`, `/speckit.clarify`,
`/speckit.plan`, `/speckit.tasks`, `/speckit.analyze`, `/speckit.implement`)
MUST be written in **Vietnamese**. Code, file paths, identifiers, and
code comments remain in English. Only prose, headings, descriptions,
rationale, and task descriptions are in Vietnamese.