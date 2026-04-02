"use client"
import { Control, FieldValues, Path } from "react-hook-form"
import { FormField } from "@/components/form/form-field"
import { Input } from "@/components/ui/input"

interface LastNameFieldProps<T extends FieldValues> {
  control: Control<T>
  name: Path<T>
}

export function LastNameField<T extends FieldValues>({ control, name }: LastNameFieldProps<T>) {
  return (
    <FormField
      control={control}
      name={name}
      label="Tên"
      required
      render={({ field, inputProps }) => (
        <Input {...field} {...inputProps} type="text" placeholder="Văn A" />
      )}
    />
  )
}
