"use client"
import { Control, FieldValues, Path } from "react-hook-form"
import { FormField } from "@/components/form/form-field"
import { Input } from "@/components/ui/input"

interface UsernameFieldProps<T extends FieldValues> {
  control: Control<T>
  name: Path<T>
}

export function UsernameField<T extends FieldValues>({ control, name }: UsernameFieldProps<T>) {
  return (
    <FormField
      control={control}
      name={name}
      label="Tên đăng nhập"
      required
      render={({ field, inputProps }) => (
        <Input {...field} {...inputProps} type="text" placeholder="nguyen_van_a" autoComplete="username" />
      )}
    />
  )
}
