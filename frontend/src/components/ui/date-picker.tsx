"use client"

import * as React from "react"
import { format, parseISO } from "date-fns"
import { vi } from "date-fns/locale"

import { Button } from "@/components/ui/button"
import { Calendar } from "@/components/ui/calendar"
import { Field, FieldLabel } from "@/components/ui/field"
import {
    Popover,
    PopoverContent,
    PopoverTrigger,
} from "@/components/ui/popover"

export function DatePickerSimple() {
    const [date, setDate] = React.useState<Date>()

    return (
        <Field className="mx-auto w-44">
            <FieldLabel htmlFor="date-picker-simple">Date</FieldLabel>
            <Popover>
                <PopoverTrigger asChild>
                    <Button
                        variant="outline"
                        id="date-picker-simple"
                        className="justify-start font-normal"
                    >
                        {date ? format(date, "PPP") : <span>Pick a date</span>}
                    </Button>
                </PopoverTrigger>
                <PopoverContent className="w-auto p-0" align="start">
                    <Calendar
                        mode="single"
                        selected={date}
                        onSelect={setDate}
                        defaultMonth={date}
                    />
                </PopoverContent>
            </Popover>
        </Field>
    )
}

interface DatePickerProps {
    value?: string
    onChange?: (value: string | undefined) => void
    placeholder?: string
    id?: string
    "aria-invalid"?: true | undefined
    "aria-describedby"?: string | undefined
}

export function DatePicker({
    value,
    onChange,
    placeholder = "Chọn ngày",
    id,
    "aria-invalid": ariaInvalid,
    "aria-describedby": ariaDescribedBy,
}: DatePickerProps) {
    const date = value ? parseISO(value) : undefined

    function handleSelect(selected: Date | undefined) {
        onChange?.(selected ? format(selected, "yyyy-MM-dd") : undefined)
    }

    return (
        <Popover>
            <PopoverTrigger asChild>
                <Button
                    variant="outline"
                    id={id}
                    aria-invalid={ariaInvalid}
                    aria-describedby={ariaDescribedBy}
                    className="w-full justify-start font-normal"
                >
                    {date ? format(date, "dd/MM/yyyy", { locale: vi }) : <span className="text-muted-foreground">{placeholder}</span>}
                </Button>
            </PopoverTrigger>
            <PopoverContent className="w-auto p-0" align="start">
                <Calendar
                    mode="single"
                    selected={date}
                    onSelect={handleSelect}
                    defaultMonth={date}
                    captionLayout="dropdown"
                    locale={vi}
                />
            </PopoverContent>
        </Popover>
    )
}
