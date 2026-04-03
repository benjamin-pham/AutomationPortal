"use client"

import { useFormContext } from "react-hook-form"
import { Field, FieldLabel, FieldError } from "@/components/ui/field"
import { Input } from "@/components/ui/input"

export const NameField = () => {
  const {
    register,
    formState: { errors },
  } = useFormContext()

  return (
    <Field>
      <FieldLabel htmlFor="name">Tên</FieldLabel>
      <Input
        id="name"
        placeholder="Nhập tên key (VD: API Key của tôi)"
        {...register("name")}
        aria-invalid={!!errors.name}
      />
      <FieldError errors={[errors.name as any]} />
    </Field>
  )
}
