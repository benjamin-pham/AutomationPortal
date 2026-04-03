"use client"

import { useFormContext } from "react-hook-form"
import { Field, FieldLabel, FieldError } from "@/components/ui/field"
import { Input } from "@/components/ui/input"

export const KeyValueField = () => {
  const {
    register,
    formState: { errors },
  } = useFormContext()

  return (
    <Field>
      <FieldLabel htmlFor="keyValue">Giá trị Key</FieldLabel>
      <Input
        id="keyValue"
        type="password"
        placeholder="Nhập giá trị Gemini API Key của bạn"
        {...register("keyValue")}
        aria-invalid={!!errors.keyValue}
      />
      <FieldError errors={[errors.keyValue as any]} />
    </Field>
  )
}
