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
├── users-create-dialog.tsx
```

### Component library

UI primitives live in `src/components/ui/` — these are shadcn-style components built on `radix-ui` and styled with Tailwind CSS v4. Use these rather than raw Radix primitives.

Key custom composites:
- **`DataTable` / `DataTableColumnHeader` / `DataPagination`** (`src/components/table/data-table.tsx`) — wraps TanStack Table with server-side sorting, loading state, and empty state. Sorting/filtering is `manual` — pass `onSortChange` to handle server-side sorting.
- **`FormField`** (`src/components/form/form-field.tsx`) — wraps `react-hook-form` `Controller` with `Field`/`FieldLabel`/`FieldError` layout and accessibility wiring. Use its `render` prop and spread `inputProps` onto the input element.

## DataTable Usage

The `DataTable` component supports row-level actions with icons and tooltips. Actions are defined via the `actions` prop, which is an array of action objects.

### Action Structure

Each action object has the following properties:
- `icon`: React node (e.g., Lucide icon component)
- `tooltip`: String for tooltip text
- `onClick`: Function that receives the row data as parameter
- `label`: String for dropdown menu item text

### Action Rendering Logic

- **Less than 3 actions**: Actions are rendered as individual icon buttons with tooltips.
- **3 or more actions**: Actions are grouped in a dropdown menu triggered by a "More" icon (MoreVertical). The dropdown items display the action labels instead of icons.

### API calls — axios instances

All API functions follow the **dependency-injection pattern**: they accept an `AxiosInstance` and return a function, then are grouped under `mainApi(axios)`.

**Important:** Only use `mainApi(axios)` with the injected `axiosServerInstance` or `axiosClientInstance`. Do not call endpoint functions directly.

Key files:
- `src/api/axiosServerInstance.ts` — async factory, Server Components only
- `src/api/axiosClientInstance.ts` — plain instance, Client Components only
- `src/api/index.ts` — `mainApi(axios)` aggregates all domain groups

#### `axiosServerInstance` — Server Components only

Async factory (uses `cookies()` from `next/headers`) — reads `accessToken` cookie and attaches it as a Bearer token. Must be `await`ed. **Never** use in Client Components.

#### `axiosClientInstance` — Client Components only

Plain pre-configured `axios` instance — reads `authToken` from `localStorage`, handles 401. Import directly (not async). **Never** use in Server Components.

| | `axiosServerInstance` | `axiosClientInstance` |
|---|---|---|
| Where | Server Components, Route Handlers | Client Components (`"use client"`) |
| Usage | `const axios = await axiosServerInstance()` | import directly (not async) |
| Auth source | `accessToken` cookie (via `next/headers`) | `authToken` in `localStorage` |

#### Adding a new API

1. Create `src/api/{entity}/` folder (e.g. `src/api/users/`)
2. One file per endpoint inside that folder.
3. In each endpoint file, define Request and Response DTOs alongside the API function, using the `{EndpointName}Request` and `{EndpointName}Response` naming pattern.
4. `src/api/{entity}/index.ts` — aggregate functions into `{entity}Api(axios)`, following the pattern in `src/api/auth/index.ts`
5. Register in `src/api/index.ts` — add a new key to the `mainApi` return object

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
