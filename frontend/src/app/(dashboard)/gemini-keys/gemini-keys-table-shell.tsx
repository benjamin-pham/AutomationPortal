"use client"
import { useState, useEffect, useCallback, useMemo } from "react";
import { Button } from "@/components/ui/button";
import { DataTable } from "@/components/table/data-table";
import DataPagination from "@/components/table/data-pagination";
import { Card, CardContent } from "@/components/ui/card";
import axiosClientInstance from "@/api/axiosClientInstance";
import mainApi from "@/api";
import type { GeminiKeyListItem, GeminiKeyPagedResponse } from "@/api/gemini-keys/types";
import { getGeminiKeysColumns } from "./gemini-keys-columns";
import { CreateGeminiKeyDialog } from "./create-gemini-key-dialog";
import { EditGeminiKeyDialog } from "./edit-gemini-key-dialog";
import { DeleteGeminiKeyDialog } from "./delete-gemini-key-dialog";

const PAGE_SIZE = 20;

export default function GeminiKeysTableShell() {
  const [page, setPage] = useState(1);
  const [isLoading, setIsLoading] = useState(false);
  const [data, setData] = useState<GeminiKeyPagedResponse | null>(null);
  const [editRow, setEditRow] = useState<GeminiKeyListItem | null>(null);
  const [deleteRow, setDeleteRow] = useState<GeminiKeyListItem | null>(null);
  const [createDialogOpen, setCreateDialogOpen] = useState(false);

  const fetchKeys = useCallback(async () => {
    setIsLoading(true);
    try {
      const result = await mainApi(axiosClientInstance).geminiKeys.getGeminiKeys(page, PAGE_SIZE);
      setData(result);
    } catch {
      // keep existing data on error
    } finally {
      setIsLoading(false);
    }
  }, [page]);

  useEffect(() => {
    fetchKeys();
  }, [fetchKeys]);

  const columns = useMemo(
    () => getGeminiKeysColumns(
      (row) => setEditRow(row),
      (row) => setDeleteRow(row)
    ),
    [fetchKeys]
  );

  return (
    <div className="p-4 flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Quản lý Gemini Keys</h1>
        <Button onClick={() => setCreateDialogOpen(true)}>Thêm Key</Button>
      </div>

      <CreateGeminiKeyDialog
        open={createDialogOpen}
        onOpenChange={setCreateDialogOpen}
        onSuccess={fetchKeys}
      />

      <EditGeminiKeyDialog
        geminiKey={editRow}
        open={!!editRow}
        onOpenChange={(open) => !open && setEditRow(null)}
        onSuccess={fetchKeys}
      />

      <DeleteGeminiKeyDialog
        geminiKeyId={deleteRow?.id ?? null}
        geminiKeyName={deleteRow?.name ?? null}
        open={!!deleteRow}
        onOpenChange={(open) => !open && setDeleteRow(null)}
        onSuccess={fetchKeys}
      />

      <Card>
        <CardContent className="flex flex-col gap-4 pt-4">
          <DataTable
            columns={columns}
            data={data?.items ?? []}
            isLoading={isLoading}
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
              Chưa có Gemini Key nào
            </p>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
