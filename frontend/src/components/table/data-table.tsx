"use client"
import {
  type ColumnDef,
  flexRender,
  getCoreRowModel,
  useReactTable,
  type SortingState,
  Column,
} from "@tanstack/react-table"

import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"

import { useState } from "react";
import { cn } from "@/lib/utils";
import { DropdownMenuTrigger, DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuSeparator } from "@/components/ui/dropdown-menu";
import { Button } from "@/components/ui/button";
import { ArrowDown, ArrowUp, ChevronsUpDown, EyeOff, InboxIcon } from "lucide-react";
import { Empty, EmptyContent, EmptyHeader, EmptyMedia } from "@/components/ui/empty";
import { Spinner } from "@/components/ui/spinner";

export type DataSorting = {
  column: string,
  direction: "asc" | "desc"
}

interface DataTableProps<TData, TValue> {
  columns: ColumnDef<TData, TValue>[]
  data: TData[]
  isLoading?: boolean
  onSortChange?: (sorting: DataSorting) => void
  stripedPattern?: "odd" | "even"
}

export function DataTable<TData, TValue>({
  columns,
  data,
  isLoading,
  onSortChange,
  stripedPattern,
}: DataTableProps<TData, TValue>) {
  const [sorting, setSorting] = useState<SortingState>([]);
  // eslint-disable-next-line react-hooks/incompatible-library
  const table = useReactTable({
    data,
    columns,
    state: {
      sorting,
    },
    manualSorting: true,
    manualFiltering: true,
    onSortingChange: (newSorting) => {
      const sortingValue =
        typeof newSorting === "function"
          ? newSorting(sorting)
          : newSorting;

      setSorting(sortingValue);

      if (sortingValue.length > 0) {
        onSortChange?.({
          column: sortingValue[0].id,
          direction: sortingValue[0].desc ? "desc" : "asc"
        });
      } else {
        onSortChange?.({
          column: "",
          direction: "asc"
        });
      }
    },
    getCoreRowModel: getCoreRowModel(),
  })

  return (
    <Table className="rounded-md border-collapse">
      <TableHeader className="sticky top-0 bg-background z-10">
        {table.getHeaderGroups().map((headerGroup) => (
          <TableRow key={headerGroup.id} className="border-b">
            {headerGroup.headers.map((header) => {
              return (
                <TableHead key={header.id}>
                  {header.isPlaceholder
                    ? null
                    : flexRender(
                      header.column.columnDef.header,
                      header.getContext()
                    )}
                </TableHead>
              )
            })}
          </TableRow>
        ))}
      </TableHeader>
      <TableBody className={cn(
        stripedPattern === "odd" && "[&_tr:nth-child(odd)]:bg-muted/50",
        stripedPattern === "even" && "[&_tr:nth-child(even)]:bg-muted/50",
      )}>
        {isLoading ? (<TableRow>
          <TableCell colSpan={columns.length} className="h-24 text-center">
            <div className="flex justify-center items-center gap-2">
              <Spinner className="size-5" />
              <span>Loading...</span>
            </div>
          </TableCell>
        </TableRow>) : table.getRowModel().rows?.length ? (
          table.getRowModel().rows.map((row) => (
            <TableRow
              key={row.id}
              data-state={row.getIsSelected() && "selected"}
            >
              {row.getVisibleCells().map((cell) => {
                return (
                  <TableCell key={cell.id}>
                    {flexRender(cell.column.columnDef.cell, cell.getContext())}
                  </TableCell>
                )
              })}
            </TableRow>
          ))
        ) : (
          <TableRow>
            <TableCell colSpan={columns.length} className="h-24 text-center">
              <Empty className="border">
                <EmptyHeader>
                  <EmptyMedia>
                    <InboxIcon />
                  </EmptyMedia>
                  <EmptyContent>Không có dữ liệu</EmptyContent>
                </EmptyHeader>
              </Empty>
            </TableCell>
          </TableRow>
        )}
      </TableBody>
    </Table >
  )
}


interface DataTableColumnHeaderProps<TData, TValue>
  extends React.HTMLAttributes<HTMLDivElement> {
  column: Column<TData, TValue>
  title: string
}

export function DataTableColumnHeader<TData, TValue>({
  column,
  title,
  className,
}: DataTableColumnHeaderProps<TData, TValue>) {
  if (!column.getCanSort()) {
    return <div className={cn(className)}>{title}</div>
  }

  return (
    <div className={cn("flex items-center gap-2", className)}>
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button
            variant="ghost"
            size="sm"
            className="data-[state=open]:bg-accent -ml-3 h-8"
          >
            <span>{title}</span>
            {column.getIsSorted() === "desc" ? (
              <ArrowDown />
            ) : column.getIsSorted() === "asc" ? (
              <ArrowUp />
            ) : (
              <ChevronsUpDown />
            )}
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align="start">
          <DropdownMenuItem onClick={() => column.toggleSorting(false)}>
            <ArrowUp />
            Asc
          </DropdownMenuItem>
          <DropdownMenuItem onClick={() => column.toggleSorting(true)}>
            <ArrowDown />
            Desc
          </DropdownMenuItem>
          <DropdownMenuSeparator />
          <DropdownMenuItem onClick={() => column.toggleVisibility(false)}>
            <EyeOff />
            Hide
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    </div>
  )
}
