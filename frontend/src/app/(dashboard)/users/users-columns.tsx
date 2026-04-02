"use client"
import type { ColumnDef } from "@tanstack/react-table";
import { DataTableColumnHeader } from "@/components/table/data-table";
import { Button } from "@/components/ui/button";
import { MoreHorizontal } from "lucide-react";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import type { UserListItem } from "@/api/users/types";

export function getUsersColumns(
  onView: (id: string) => void,
  onEdit: (id: string) => void,
  onDelete: (id: string, username: string) => void,
  onResetPassword: (id: string) => void
): ColumnDef<UserListItem>[] {
  return [
    {
      id: "first_name",
      header: ({ column }) => <DataTableColumnHeader column={column} title="Họ" />,
      cell: ({ row }) => (
        <Button
          variant="link"
          className="p-0 h-auto font-normal text-foreground hover:text-primary"
          onClick={() => onView(row.original.id)}
        >
          {row.original.firstName}
        </Button>
      ),
    },
    {
      id: "last_name",
      header: ({ column }) => <DataTableColumnHeader column={column} title="Tên" />,
      cell: ({ row }) => row.original.lastName,
    },
    {
      id: "username",
      header: ({ column }) => <DataTableColumnHeader column={column} title="Tên đăng nhập" />,
      cell: ({ row }) => row.original.username,
    },
    {
      id: "email",
      header: ({ column }) => <DataTableColumnHeader column={column} title="Email" />,
      cell: ({ row }) => row.original.email ?? "—",
    },
    {
      id: "phone",
      header: ({ column }) => <DataTableColumnHeader column={column} title="Số điện thoại" />,
      cell: ({ row }) => row.original.phone ?? "—",
    },
    {
      id: "birthday",
      header: ({ column }) => <DataTableColumnHeader column={column} title="Ngày sinh" />,
      cell: ({ row }) => row.original.birthday ?? "—",
    },
    {
      id: "actions",
      header: () => null,
      cell: ({ row }) => (
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" className="h-8 w-8 p-0">
              <span className="sr-only">Open menu</span>
              <MoreHorizontal className="h-4 w-4" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <DropdownMenuLabel>Hành động</DropdownMenuLabel>
            <DropdownMenuItem onClick={() => onEdit(row.original.id)}>
              Chỉnh sửa
            </DropdownMenuItem>
            <DropdownMenuItem onClick={() => onResetPassword(row.original.id)}>
              Đặt lại mật khẩu
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            <DropdownMenuItem onClick={() => onDelete(row.original.id, row.original.username)}>
              Xóa
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      ),
    },
  ];
}
