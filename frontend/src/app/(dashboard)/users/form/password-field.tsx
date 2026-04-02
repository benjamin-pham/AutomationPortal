"use client"
import { Control, FieldValues, Path } from "react-hook-form"
import { FormField } from "@/components/form/form-field"
import { Input } from "@/components/ui/input"

interface PasswordFieldProps<T extends FieldValues> {
  control: Control<T>
  name: Path<T>
  label?: string
}

export function PasswordField<T extends FieldValues>({ control, name, label = "Mật khẩu" }: PasswordFieldProps<T>) {
  return (
    <FormField
      control={control}
      name={name}
      label={label}
      required
      render={({ field, inputProps }) => (
        <Input {...field} {...inputProps} type="password" placeholder="••••••••" autoComplete="new-password" />
      )}
    />
  )
}
