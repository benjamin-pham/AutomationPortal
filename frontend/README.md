## Commands

```bash
npm run dev      # Start development server (Turbopack by default in Next.js 16)
npm run build    # Production build (also Turbopack by default)
npm run start    # Start production server
npm run lint     # Run ESLint
```

No test suite is configured.

## Architecture

This is an **AutomationPortal** frontend — a Next.js 16 App Router application with a collapsible sidebar layout.

### Key Next.js 16 breaking changes

- **Turbopack is the default** for both `next dev` and `next build`. Use `--webpack` flag to opt out.
- **Async Request APIs are fully async** — `cookies()`, `headers()`, `draftMode()`, `params`, and `searchParams` must all be awaited. Synchronous access is removed.
  ```tsx
  // Correct in Next.js 16
  export default async function Page({ params }: { params: Promise<{ id: string }> }) {
    const { id } = await params
  }
  ```
- `turbopack` config is now a top-level `nextConfig` option (not under `experimental`).

### Layout structure

`src/app/layout.tsx` wraps every page with: `ThemeProvider` → `TooltipProvider` → `SidebarProvider` → `AppSidebar` + `{children}`.

The sidebar (`src/components/layout/`) renders differently based on collapse state:
- **Expanded**: collapsible sub-menus using `Collapsible`
- **Collapsed (icon-only)**: sub-menus appear as `Popover` flyouts

To add a new sidebar nav item, edit the `items` array in `src/components/layout/app-sidebar-content.tsx`.

### Page structure conventions

When creating a new page, all components used exclusively by that page must be co-located in the **same directory** as the `page.tsx` file.

```
src/app/(dashboard)/users/
├── page.tsx
├── users-table.tsx        # page-specific components live here
├── users-filter-bar.tsx
└── form/                  # form field renders split into per-field files
    ├── name-field.tsx
    ├── email-field.tsx
    └── role-field.tsx
```

**Form field rule**: each `<FormField render={...} />` block must be extracted into its own file inside a `form/` subdirectory at the same level as `page.tsx`. Never inline multiple `FormField` render props directly in the page or a single form component file.

### Component library

UI primitives live in `src/components/ui/` — these are shadcn-style components built on `radix-ui` and styled with Tailwind CSS v4. Use these rather than raw Radix primitives.

Key custom composites:
- **`DataTable` / `DataTableColumnHeader`** (`src/components/table/data-table.tsx`) — wraps TanStack Table with server-side sorting, loading state, and empty state. Sorting/filtering is `manual` — pass `onSortChange` to handle server-side sorting.
- **`FormField`** (`src/components/form/form-field.tsx`) — wraps `react-hook-form` `Controller` with `Field`/`FieldLabel`/`FieldError` layout and accessibility wiring. Use its `render` prop and spread `inputProps` onto the input element.

### API calls — axios instances

All API functions follow the **dependency-injection pattern**: they accept an `AxiosInstance` and return a function, then are grouped under `domainApi(axios)`.

Key files:
- `src/api/axiosServerInstance.ts` — async factory, Server Components only
- `src/api/axiosClientInstance.ts` — plain instance, Client Components only
- `src/api/index.ts` — `domainApi(axios)` aggregates all domain groups

#### `axiosServerInstance` — Server Components only

Async factory (uses `cookies()` from `next/headers`) — reads `accessToken` cookie and attaches it as a Bearer token. Must be `await`ed. **Never** use in Client Components.

#### `axiosClientInstance` — Client Components only

Plain pre-configured `axios` instance — reads `authToken` from `localStorage`, handles 401. Import directly (not async). **Never** use in Server Components.

| | `axiosServerInstance` | `axiosClientInstance` |
|---|---|---|
| Where | Server Components, Route Handlers | Client Components (`"use client"`) |
| Usage | `const axios = await axiosServerInstance()` | import directly (not async) |
| Auth source | `accessToken` cookie (via `next/headers`) | `authToken` in `localStorage` |

#### Adding a new API domain

1. Create `src/api/{entity}/` folder (e.g. `src/api/users/`)
2. One file per function inside that folder
3. `src/api/{entity}/index.ts` — aggregate functions into `{entity}Api(axios)`, following the pattern in `src/api/auth/index.ts`
4. Register in `src/api/index.ts` — add a new key to the `domainApi` return object

### Utilities

- `cn(...inputs)` in `src/lib/utils.ts` — `clsx` + `tailwind-merge` for conditional class names.
- `use-mobile` hook in `src/hooks/use-mobile.ts` — breakpoint detection.

### Stack

| Concern | Library |
|---|---|
| Routing/SSR | Next.js 16 App Router |
| UI primitives | `radix-ui` via shadcn components |
| Styling | Tailwind CSS v4 |
| Icons | `lucide-react` |
| Tables | `@tanstack/react-table` |
| Forms | `react-hook-form` + `zod` |
| Theming | `next-themes` |
| Toasts | `sonner` |
| Charts | `recharts` |
