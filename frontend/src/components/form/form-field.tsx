"use client"

import React from "react"
import {
  Controller,
  type ControllerProps,
  type ControllerFieldState,
  type ControllerRenderProps,
  type FieldPath,
  type FieldValues,
  type UseFormStateReturn,
} from "react-hook-form"

import {
  Field,
  FieldDescription,
  FieldError,
  FieldLabel,
} from "@/components/ui/field"

type Orientation = "vertical" | "horizontal" | "responsive"

interface FormFieldInputProps {
  id: string
  "aria-invalid": true | undefined
  "aria-describedby": string | undefined
}

interface FormFieldRenderProps<
  TFieldValues extends FieldValues,
  TName extends FieldPath<TFieldValues>,
> {
  field: ControllerRenderProps<TFieldValues, TName>
  fieldState: ControllerFieldState
  formState: UseFormStateReturn<TFieldValues>
  /** Spread onto your input element for proper a11y wiring. */
  inputProps: FormFieldInputProps
}

interface FormFieldProps<
  TFieldValues extends FieldValues = FieldValues,
  TName extends FieldPath<TFieldValues> = FieldPath<TFieldValues>,
> extends Omit<ControllerProps<TFieldValues, TName>, "render"> {
  label?: React.ReactNode
  description?: React.ReactNode
  required?: boolean
  orientation?: Orientation
  className?: string
  render: (props: FormFieldRenderProps<TFieldValues, TName>) => React.ReactElement
}

export default function FormField<
  TFieldValues extends FieldValues = FieldValues,
  TName extends FieldPath<TFieldValues> = FieldPath<TFieldValues>,
>({
  label,
  description,
  required,
  orientation = "vertical",
  className,
  render,
  ...controllerProps
}: FormFieldProps<TFieldValues, TName>) {
  const descriptionId = `${controllerProps.name}-description`

  return (
    <Controller
      {...controllerProps}
      render={({ field, fieldState, formState }) => {
        const isInvalid = !!fieldState.error

        const inputProps: FormFieldInputProps = {
          id: field.name,
          "aria-invalid": isInvalid || undefined,
          "aria-describedby": description ? descriptionId : undefined,
        }

        return (
          <Field
            orientation={orientation}
            data-invalid={isInvalid || undefined}
            className={className}
          >
            {label && (
              <FieldLabel htmlFor={field.name}>
                {label}
                {required && (
                  <span aria-hidden="true" className="ml-0.5 text-destructive">
                    *
                  </span>
                )}
              </FieldLabel>
            )}
            {render({ field, fieldState, formState, inputProps })}
            {description && (
              <FieldDescription id={descriptionId}>{description}</FieldDescription>
            )}
            <FieldError errors={fieldState.error ? [fieldState.error] : []} />
          </Field>
        )
      }}
    />
  )
}

export { FormField }
export type { FormFieldProps, FormFieldRenderProps, FormFieldInputProps }
