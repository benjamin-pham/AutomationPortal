"use client"
import { Control, FieldValues, Path } from "react-hook-form"
import { FormField } from "@/components/form/form-field"
import { Input } from "@/components/ui/input"

interface FirstNameFieldProps<T extends FieldValues> {
  control: Control<T>
  name: Path<T>
}

export function FirstNameField<T extends FieldValues>({ control, name }: FirstNameFieldProps<T>) {
  return (
    <FormField
      control={control}
      name={name}
      label="Họ"
      required
      render={({ field, inputProps }) => (
        <Input {...field} {...inputProps} type="text" placeholder="Nguyễn" />
      )}
    />
  )
}
