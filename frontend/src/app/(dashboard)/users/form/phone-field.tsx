"use client"
import { Control, FieldValues, Path } from "react-hook-form"
import { FormField } from "@/components/form/form-field"
import { Input } from "@/components/ui/input"

interface PhoneFieldProps<T extends FieldValues> {
  control: Control<T>
  name: Path<T>
}

export function PhoneField<T extends FieldValues>({ control, name }: PhoneFieldProps<T>) {
  return (
    <FormField
      control={control}
      name={name}
      label="Số điện thoại"
      render={({ field, inputProps }) => (
        <Input {...field} {...inputProps} type="tel" placeholder="0912345678" />
      )}
    />
  )
}
