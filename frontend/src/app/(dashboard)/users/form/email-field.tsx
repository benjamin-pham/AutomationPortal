"use client"
import { Control, FieldValues, Path } from "react-hook-form"
import { FormField } from "@/components/form/form-field"
import { Input } from "@/components/ui/input"

interface EmailFieldProps<T extends FieldValues> {
  control: Control<T>
  name: Path<T>
}

export function EmailField<T extends FieldValues>({ control, name }: EmailFieldProps<T>) {
  return (
    <FormField
      control={control}
      name={name}
      label="Email"
      render={({ field, inputProps }) => (
        <Input {...field} {...inputProps} type="email" placeholder="example@email.com" />
      )}
    />
  )
}
