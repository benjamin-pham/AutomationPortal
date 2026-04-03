"use client"
import type { ColumnDef } from "@tanstack/react-table";
import { DataTableColumnHeader } from "@/components/table/data-table";
import { RowActionsCell, type RowAction } from "@/components/table/row-actions-cell";
import type { GeminiKeyListItem } from "@/api/gemini-keys/types";

export function getGeminiKeysColumns(
  onEdit: (row: GeminiKeyListItem) => void,
  onDelete: (row: GeminiKeyListItem) => void
): ColumnDef<GeminiKeyListItem>[] {
  return [
    {
      id: "name",
      header: ({ column }) => <DataTableColumnHeader column={column} title="Tên" />,
      cell: ({ row }) => row.original.name,
    },
    {
      id: "assignedUsername",
      header: ({ column }) => <DataTableColumnHeader column={column} title="Người dùng" />,
      cell: ({ row }) => row.original.assignedUsername || "—",
    },
    {
      id: "maskedKey",
      header: ({ column }) => <DataTableColumnHeader column={column} title="Giá trị Key" />,
      cell: ({ row }) => (
        <span className="font-mono text-sm">{row.original.maskedKey}</span>
      ),
    },
    {
      id: "createdAt",
      header: ({ column }) => <DataTableColumnHeader column={column} title="Ngày tạo" />,
      cell: ({ row }) => new Date(row.original.createdAt).toLocaleDateString("vi-VN"),
    },
    {
      id: "actions",
      header: () => null,
      cell: ({ row }) => {
        const actions: RowAction[] = [
          {
            label: "Chỉnh sửa",
            onClick: () => onEdit(row.original),
          },
          {
            label: "Xóa",
            onClick: () => onDelete(row.original),
            variant: "destructive",
            divider: true,
          },
        ];

        return <RowActionsCell actions={actions} />;
      },
    },
  ];
}
