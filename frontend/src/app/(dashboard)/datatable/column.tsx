'use client'
import type { IProductTable } from "./IProductTable"
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuLabel,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { MoreHorizontal } from "lucide-react"

import { Button } from "@/components/ui/button"
import { DataTableColumnHeader } from "@/components/table/data-table"
import type { ColumnDef } from "@tanstack/react-table"

export const columns: ColumnDef<IProductTable>[] = [
    {
        accessorKey: "name",
        header: "name",
    },
    {
        accessorKey: "price",
        header: ({ column }) => {
            return (
                <DataTableColumnHeader column={column} title="Price" />
            )
        },
        cell: ({ row }) => {
            const amount = parseFloat(row.getValue("price"))
            const formatted = new Intl.NumberFormat("en-US", {
                style: "currency",
                currency: "USD",
            }).format(amount)

            return <div className="text-right font-medium" > {formatted} </div>
        }
    },
    {
        accessorKey: "categoryId",
        header: ({ column }) => {
            return (
                <DataTableColumnHeader column={column} title="Category" />
            )
        }
    },
    {
        accessorKey: "stock",
        header: ({ column }) => {
            return (
                <DataTableColumnHeader column={column} title="Stock" />
            )
        }

    },
    {
        accessorKey: "description",
        header: ({ column }) => {
            return (
                <DataTableColumnHeader column={column} title="Description" />
            )
        }
    },
    {
        id: "actions",
        header: "Actions",
        meta: { sticky: "right", offset: 0 },
        cell: ({ row }) => {
            const product = row.original

            return (
                <DropdownMenu modal={false} >
                    <DropdownMenuTrigger asChild >
                        <Button variant="ghost" className="h-8 w-8 p-0" >
                            <span className="sr-only" > Open menu </span>
                            < MoreHorizontal className="h-4 w-4" />
                        </Button>
                    </DropdownMenuTrigger>
                    < DropdownMenuContent align="end" >
                        <DropdownMenuLabel>Actions </DropdownMenuLabel>
                        < DropdownMenuItem
                            onClick={() => navigator.clipboard.writeText(product.id.toString())
                            }
                        >
                            Copy product ID
                        </DropdownMenuItem>
                        < DropdownMenuSeparator />
                        <DropdownMenuItem>View customer </DropdownMenuItem>
                        < DropdownMenuItem > View payment details </DropdownMenuItem>
                    </DropdownMenuContent>
                </DropdownMenu>
            )
        },
    }
]