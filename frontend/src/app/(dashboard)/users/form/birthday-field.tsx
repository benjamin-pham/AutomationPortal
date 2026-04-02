"use client"
import { Control, FieldValues, Path } from "react-hook-form"
import { FormField } from "@/components/form/form-field"
import { DatePicker } from "@/components/ui/date-picker"

interface BirthdayFieldProps<T extends FieldValues> {
  control: Control<T>
  name: Path<T>
}

export function BirthdayField<T extends FieldValues>({ control, name }: BirthdayFieldProps<T>) {
  return (
    <FormField
      control={control}
      name={name}
      label="Ngày sinh"
      render={({ field, inputProps }) => (
        <DatePicker
          value={field.value}
          onChange={field.onChange}
          id={inputProps.id}
          aria-invalid={inputProps["aria-invalid"]}
          aria-describedby={inputProps["aria-describedby"]}
        />
      )}
    />
  )
}
