"use client"
import { useState, useEffect, useCallback, useMemo } from "react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { DataTable, type DataSorting } from "@/components/table/data-table";
import DataPagination from "@/components/table/data-pagination";
import { Card, CardContent } from "@/components/ui/card";
import axiosClientInstance from "@/api/axiosClientInstance";
import mainApi from "@/api";
import type { UserListItem } from "@/api/users/types";
import { getUsersColumns } from "./users-columns";
import ViewUserDialog from "./view-user-dialog";
import CreateUserDialog from "./create-user-dialog";
import EditUserDialog from "./edit-user-dialog";
import DeleteUserDialog from "./delete-user-dialog"
import ResetPasswordDialog from "./reset-password-dialog";
import { PagedList } from "@/types/PagedList";

export default function UsersTableShell() {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [sorting, setSorting] = useState<DataSorting>({ column: "username", direction: "asc" });
  const [isLoading, setIsLoading] = useState(false);
  const [data, setData] = useState<PagedList<UserListItem> | null>(null);
  const [viewUserId, setViewUserId] = useState<string | null>(null);
  const [createDialogOpen, setCreateDialogOpen] = useState(false);
  const [editUserId, setEditUserId] = useState<string | null>(null);
  const [deleteUser, setDeleteUser] = useState<{ id: string; username: string } | null>(null);
  const [resetPasswordUserId, setResetPasswordUserId] = useState<string | null>(null);

  const fetchUsers = useCallback(async () => {
    setIsLoading(true);
    try {
      const result = await mainApi(axiosClientInstance).users.getListUser({
        page,
        pageSize: 20,
        search: search || undefined,
        sortColumn: sorting.column || undefined,
        sortDirection: sorting.direction,
      });
      setData(result);
    } catch {
      // keep existing data on error
    } finally {
      setIsLoading(false);
    }
  }, [page, search, sorting]);

  useEffect(() => {
    fetchUsers();
  }, [fetchUsers]);

  const handleSortChange = (newSorting: DataSorting) => {
    setSorting(newSorting);
    setPage(1);
  };

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearch(e.target.value);
    setPage(1);
  };

  const columns = useMemo(
    () => getUsersColumns(setViewUserId, setEditUserId, (id, username) => setDeleteUser({ id, username }), setResetPasswordUserId),
    []
  );

  return (
    <div className="p-4 flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Quản lý người dùng</h1>
        <Button onClick={() => setCreateDialogOpen(true)}>Thêm người dùng</Button>
      </div>
      <Card>
        <CardContent className="flex flex-col gap-4 pt-4">
          <Input
            placeholder="Tìm kiếm theo tên, tên đăng nhập, email..."
            value={search}
            onChange={handleSearchChange}
            className="max-w-sm"
          />
          <DataTable
            columns={columns}
            data={data?.items ?? []}
            isLoading={isLoading}
            onSortChange={handleSortChange}
            stripedPattern="odd"
          />
          {data && data.totalPages > 0 && (
            <DataPagination
              pageIndex={page}
              data={{ totalPages: data.totalPages, totalItems: data.totalItems }}
              isLoading={isLoading}
              onPageChange={setPage}
            />
          )}
          {!isLoading && data?.items.length === 0 && (
            <p className="text-center text-sm text-muted-foreground py-4">
              Không tìm thấy người dùng nào phù hợp
            </p>
          )}
        </CardContent>
      </Card>

      <ViewUserDialog
        userId={viewUserId}
        onClose={() => setViewUserId(null)}
      />

      <CreateUserDialog
        open={createDialogOpen}
        onClose={() => setCreateDialogOpen(false)}
        onSuccess={() => {
          setCreateDialogOpen(false);
          setPage(1);
          fetchUsers();
        }}
      />

      <EditUserDialog
        userId={editUserId}
        onClose={() => setEditUserId(null)}
        onSuccess={() => {
          setEditUserId(null);
          fetchUsers();
        }}
      />

      <DeleteUserDialog
        userId={deleteUser?.id ?? null}
        username={deleteUser?.username ?? null}
        onClose={() => setDeleteUser(null)}
        onSuccess={() => {
          setDeleteUser(null);
          fetchUsers();
        }}
      />

      <ResetPasswordDialog
        userId={resetPasswordUserId}
        onClose={() => setResetPasswordUserId(null)}
      />
    </div>
  );
}
