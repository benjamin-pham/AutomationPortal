# Row Actions Cell Component

The `RowActionsCell` component provides a standardized way to add a dropdown menu of actions to table rows.

## Usage

### Basic Example

```typescript
import { RowActionsCell, type RowAction } from "@/components/table/row-actions-cell"
import type { ColumnDef } from "@tanstack/react-table"

interface MyData {
  id: string
  name: string
}

const columns: ColumnDef<MyData>[] = [
  {
    id: "actions",
    header: () => null,
    cell: ({ row }) => {
      const actions: RowAction[] = [
        {
          label: "Edit",
          onClick: () => console.log("Edit:", row.original.id),
        },
        {
          label: "Delete",
          onClick: () => console.log("Delete:", row.original.id),
          variant: "destructive",
          divider: true, // Add separator before this action
        },
      ]

      return <RowActionsCell actions={actions} />
    },
  },
  // ... other columns
]
```

## RowAction Type

```typescript
type RowAction = {
  label: string                          // Display text for the action
  onClick: () => void                    // Callback when action is clicked
  variant?: "default" | "destructive"    // Optional: style (red color for destructive)
  divider?: boolean                      // Optional: add separator before this action
}
```

## Props

- **actions** (required): Array of `RowAction` objects
- **align** (optional): Dropdown menu alignment - `"start" | "center" | "end"` (default: `"end"`)

## Features

✅ Consistent styling across all tables
✅ Support for destructive actions with red highlighting
✅ Visual separators to group related actions
✅ Customizable alignment
✅ Accessible (uses semantic HTML and ARIA labels)
✅ Mobile-friendly dropdown menu

## Advanced Example

With callbacks and state management:

```typescript
import { useState } from "react"
import { RowActionsCell, type RowAction } from "@/components/table/row-actions-cell"

export function MyColumnsWithActions(
  onEdit: (id: string) => void,
  onDelete: (id: string) => void
) {
  return [
    {
      id: "actions",
      header: () => null,
      cell: ({ row }) => {
        const actions: RowAction[] = [
          {
            label: "View details",
            onClick: () => onEdit(row.original.id),
          },
          {
            label: "Duplicate",
            onClick: () => console.log("Duplicate:", row.original.id),
          },
          {
            label: "Archive",
            onClick: () => console.log("Archive:", row.original.id),
          },
          {
            label: "Delete",
            onClick: () => onDelete(row.original.id),
            variant: "destructive",
            divider: true,
          },
        ]

        return <RowActionsCell actions={actions} />
      },
    },
  ]
}
```
