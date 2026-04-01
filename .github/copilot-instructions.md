# Copilot Instructions for AutomationPortal

This is a **monorepo** with three main parts: a .NET backend (Clean Architecture), a Next.js 16 frontend, and supporting utilities.

## Quick Start Commands

### Backend (.NET 10)

```bash
# Build the entire backend
dotnet build

# Run the API server
dotnet run --project src/AutomationPortal.API/AutomationPortal.API.csproj

# Run all tests
dotnet test

# Run a single test project
dotnet test tests/AutomationPortal.Application.UnitTests/

# Run a specific test by name
dotnet test --filter "FullyQualifiedName~TestMethodName"
```

### Frontend (Next.js 16)

```bash
# Start development server (Turbopack by default)
npm run dev

# Production build
npm run build

# Start production server
npm run start

# Lint code
npm run lint
```

## Architecture Overview

### Backend: Clean Architecture (4-Layer)

**Dependency Flow**: API → Application → Domain; Infrastructure → Domain

```
src/
├── AutomationPortal.Domain/
│   ├── Abstractions/          IRepository<T>, IUnitOfWork, IUserContext, IDateTimeProvider, Result<T>, Error
│   ├── Entities/              Business entities with encapsulated behavior
│   ├── Enums/                 OrderStatus, PaymentMethod, etc.
│   └── Repositories/          IOrderRepository, IProductRepository (domain interfaces)
│
├── AutomationPortal.Application/
│   ├── Abstractions/          ISqlConnectionFactory, ICommand, IQuery, ICommandHandler, IQueryHandler
│   ├── Behaviors/            ValidationBehavior (MediatR pipeline)
│   ├── Exceptions/           ValidationException with errors collection
│   ├── Shared/               Common DTOs, RuleValidators
│   └── Features/             MediatR Commands/Queries organized by entity
│       ├── Users/
│       │   ├── Register/     RegisterCommand, RegisterCommandHandler, RegisterCommandValidator, RegisterResponse
│       │   └── Login/        LoginCommand, LoginCommandHandler, etc.
│       └── Orders/
│           ├── Create/       CreateCommand, CreateCommandHandler, CreateCommandValidator, CreateResponse
│           └── Get/          GetQuery, GetQueryHandler, GetResponse
│
├── AutomationPortal.Infrastructure/
│   ├── Data/
│   │   ├── AppDbContext.cs   Entity configurations, SaveChangesAsync audit logic
│   │   └── Configurations/   OrderConfiguration : IEntityTypeConfiguration<Order> (Fluent API, snake_case)
│   └── Repositories/         OrderRepository : IOrderRepository
│
└── AutomationPortal.API/
    ├── Endpoints/            IEndpoint implementations (auto-discovered, no manual registration)
    │   ├── Users/            CreateUserEndpoint, GetUserEndpoint, etc.
    │   └── Orders/
    └── Extensions/           GlobalExceptionHandler, CorrelationIdMiddleware, SerilogExtensions
```

### Frontend: Next.js 16 App Router

Single SPA with collapsible sidebar layout. Uses Server and Client Components (async/await for request APIs).

**Key structure**:
- `src/app/` — Route handlers and page components
- `src/components/ui/` — Shadcn UI primitives (Radix + Tailwind)
- `src/components/layout/` — App shell (sidebar, header, theme provider)
- `src/components/table/` — DataTable wrapper (TanStack Table, server-side operations)
- `src/components/form/` — FormField wrapper (react-hook-form + zod)
- `src/api/` — Axios instances and domain-specific API functions

## Key Patterns

### Backend

#### Endpoints

Implement `IEndpoint` in `src/AutomationPortal.API/Endpoints/`. Auto-discovered; no manual registration.

```csharp
public sealed class CreateOrderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/orders", HandleAsync)
           .WithName("CreateOrder");

    private static async Task<IResult> HandleAsync(CreateOrderCommand cmd, IMediator mediator)
    {
        var result = await mediator.Send(cmd);
        return result.IsFailure ? Results.BadRequest(result.Error) : Results.Created();
    }
}
```

#### Commands & Queries

Add MediatR `IRequest<Result<T>>` + handler in `src/AutomationPortal.Application/Features/{Feature}/{Operation}/`. Include a `AbstractValidator<TRequest>` in the same folder; ValidationBehavior runs it automatically.

- **Command**: `CreateOrderCommand.cs` → `CreateOrderCommandHandler.cs` → `CreateOrderCommandValidator.cs`
- **Query**: `GetOrderQuery.cs` → `GetOrderQueryHandler.cs`
- **Response**: Always its own file (e.g., `CreateOrderResponse.cs`) in the operation folder

#### Result Pattern

Use `Result<T>` (not exceptions) for domain errors:
- `Result.Success(value)` or `Result.Failure(error)`
- Check `result.IsFailure` in handlers/endpoints

#### Database

- All entities extending `BaseEntity` auto-get: `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`, `IsDeleted`
- `AppDbContext.SaveChangesAsync` sets audit fields and handles soft deletes
- Entity configurations: Fluent API in `src/AutomationPortal.Infrastructure/Data/Configurations/` using snake_case naming
- **Central package management**: Version all NuGet refs in `Directory.Packages.props`; no `Version` on `<PackageReference>` (use `VersionOverride` only if necessary)

#### Code Style

- Nullable reference types enabled throughout
- Async all the way — no `.Result` or `.Wait()`
- Record types for DTOs
- Always `IOptions<T>` for config (never raw `config["Key"]`)
- **Never use `DateTime.Now`** — inject `IDateTimeProvider`

### Frontend

#### Next.js 16 Breaking Changes

- **Turbopack** is the default for both `next dev` and `next build` (use `--webpack` flag to opt out)
- **Async request APIs** — `cookies()`, `headers()`, `params`, `searchParams` must all be `await`ed in async components:
  ```tsx
  export default async function Page({ params }: { params: Promise<{ id: string }> }) {
    const { id } = await params;
  }
  ```

#### Page & Component Structure

- **Page-specific components** must be co-located in the same directory as `page.tsx`
- **Form fields** must be extracted into separate files inside a `form/` subdirectory:
  ```
  src/app/(dashboard)/users/
  ├── page.tsx
  ├── users-table.tsx
  └── form/
      ├── name-field.tsx
      ├── email-field.tsx
      └── role-field.tsx
  ```
- Never inline multiple `<FormField render={...} />` blocks directly in a page or single form file

#### API Calls — Dependency Injection Pattern

All API functions accept an `AxiosInstance` and return a function, grouped under `domainApi(axios)`.

**Server Components** (`src/api/axiosServerInstance.ts`):
- Async factory; reads `accessToken` cookie; must be `await`ed
- Never use in Client Components

**Client Components** (`src/api/axiosClientInstance.ts`):
- Pre-configured instance; reads `authToken` from `localStorage`; import directly
- Never use in Server Components

**Adding a new API domain**:
1. Create `src/api/{entity}/` (e.g., `src/api/users/`)
2. One function per file in that folder
3. Aggregate in `src/api/{entity}/index.ts` as `{entity}Api(axios)`
4. Register in `src/api/index.ts` in the `domainApi` return object

#### UI Components

- **`DataTable` / `DataTableColumnHeader`** (`src/components/table/data-table.tsx`) — wraps TanStack Table with server-side sorting/loading/empty states. Pass `onSortChange` for server logic; sorting is `manual`.
- **`FormField`** (`src/components/form/form-field.tsx`) — wraps `react-hook-form` + `zod` with `Field`/`FieldLabel`/`FieldError` layout. Use its `render` prop and spread `inputProps` onto the input.
- All UI primitives from `src/components/ui/` (shadcn components)

#### Stack

| Concern | Library |
|---------|---------|
| Routing/SSR | Next.js 16 App Router |
| UI Primitives | Radix UI via shadcn components |
| Styling | Tailwind CSS v4 |
| Icons | lucide-react |
| Tables | @tanstack/react-table |
| Forms | react-hook-form + zod |
| Theming | next-themes |
| Toasts | sonner |
| Charts | recharts |
| HTTP | axios |

#### Utilities

- `cn(...inputs)` in `src/lib/utils.ts` — `clsx` + `tailwind-merge` for conditional class names
- `use-mobile` hook in `src/hooks/use-mobile.ts` — breakpoint detection

## Testing Strategy

| Project | Scope | Key Dependencies |
|---------|-------|------------------|
| `Domain.UnitTests` | Entity logic, domain rules | xunit, FluentAssertions |
| `Application.UnitTests` | Handlers, validators, behaviors | NSubstitute (mocking) |
| `Infrastructure.IntegrationTests` | Repositories, EF Core config | Testcontainers (PostgreSQL), Respawn |
| `API.IntegrationTests` | End-to-end HTTP + auth | WebApplicationFactory, Testcontainers, Respawn |
| `ArchitectureTests` | Layer dependency enforcement | NetArchTest.Rules |

**Integration tests** spin up a real PostgreSQL container via Testcontainers. **Respawn** resets DB state between tests.

Frontend has no test suite configured.

## File Organization Conventions

### Backend

- **Solution**: `AutomationPortal.slnx` (no traditional .sln; directory build props define project graph)
- **Shared versioning**: `Directory.Packages.props` (don't add Version to individual project files)
- **Build props**: `Directory.Build.props` (common compiler settings)

### Frontend

- **Config**: `next.config.ts`, `tailwind.config.ts`, `tsconfig.json`, `components.json` (shadcn)
- **ESLint**: `.eslintrc.json` (eslint v9 flat config expected)
- **No test runner** configured

## Common Gotchas

1. **Backend**: Never call `.Result` or `.Wait()` on async tasks — use `await`
2. **Backend**: Always use `IDateTimeProvider` instead of `DateTime.Now` for testability
3. **Backend**: Entity type configuration must use Fluent API (snake_case naming); never use data annotations
4. **Frontend**: Request APIs (`params`, `searchParams`, `cookies()`, `headers()`) are async in Next.js 16 — must be `await`ed
5. **Frontend**: Form fields must be extracted to individual files in a `form/` subdirectory, never inlined in the page
6. **Frontend**: Use `axiosServerInstance` only in Server Components, `axiosClientInstance` only in Client Components
7. **Monorepo**: Backend and frontend are separate; build/test independently; coordinate via API contracts
