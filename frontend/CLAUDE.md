# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

@AGENTS.md

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

### Component library

UI primitives live in `src/components/ui/` — these are shadcn-style components built on `radix-ui` and styled with Tailwind CSS v4. Use these rather than raw Radix primitives.

Key custom composites:
- **`DataTable` / `DataTableColumnHeader`** (`src/components/table/data-table.tsx`) — wraps TanStack Table with server-side sorting, loading state, and empty state. Sorting/filtering is `manual` — pass `onSortChange` to handle server-side sorting.
- **`FormField`** (`src/components/form/form-field.tsx`) — wraps `react-hook-form` `Controller` with `Field`/`FieldLabel`/`FieldError` layout and accessibility wiring. Use its `render` prop and spread `inputProps` onto the input element.

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
