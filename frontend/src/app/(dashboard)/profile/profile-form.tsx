"use client"

import { useState } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { z } from "zod"
import { toast } from "sonner"
import { AxiosError } from "axios"

import { ProfileResponse } from "@/api/auth/getProfile"
import axiosClientInstance from "@/api/axiosClientInstance"
import domainApi from "@/api"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { FormField } from "@/components/form/form-field"
import { DatePicker } from "@/components/ui/date-picker"

const schema = z.object({
  firstName: z.string().min(1, "Không được để trống"),
  lastName: z.string().min(1, "Không được để trống"),
  email: z.string().email("Email không hợp lệ").optional().or(z.literal("")),
  phone: z.string().optional(),
  birthday: z.string().optional(),
})

type FormValues = z.infer<typeof schema>

export default function ProfileForm({ profile }: { profile: ProfileResponse }) {
  const [isLoading, setIsLoading] = useState(false)

  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      firstName: profile.firstName,
      lastName: profile.lastName,
      email: profile.email ?? "",
      phone: profile.phone ?? "",
      birthday: profile.birthday ? profile.birthday.split("T")[0] : "",
    },
  })

  async function onSubmit(values: FormValues) {
    setIsLoading(true)
    try {
      await domainApi(axiosClientInstance).auth.updateProfile({
        ...values,
        email: values.email || null,
        phone: values.phone || null,
        birthday: values.birthday || null,
      })()
      toast.success("Cập nhật thông tin thành công")
    } catch (error) {
      if (error instanceof AxiosError) {
        const data = error.response?.data
        toast.error(data?.detail ?? data?.message ?? "Cập nhật thất bại")
      } else {
        toast.error("Đã xảy ra lỗi, vui lòng thử lại")
      }
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col gap-4">
      <div className="grid grid-cols-2 gap-4">
        <FormField
          control={form.control}
          name="lastName"
          label="Họ"
          required
          render={({ field, inputProps }) => (
            <Input {...field} {...inputProps} placeholder="Nguyễn" />
          )}
        />
        <FormField
          control={form.control}
          name="firstName"
          label="Tên"
          required
          render={({ field, inputProps }) => (
            <Input {...field} {...inputProps} placeholder="Văn A" />
          )}
        />
      </div>
      <FormField
        control={form.control}
        name="email"
        label="Email"
        render={({ field, inputProps }) => (
          <Input {...field} {...inputProps} type="email" placeholder="example@email.com" />
        )}
      />
      <FormField
        control={form.control}
        name="phone"
        label="Số điện thoại"
        render={({ field, inputProps }) => (
          <Input {...field} {...inputProps} type="tel" placeholder="0912345678" />
        )}
      />
      <FormField
        control={form.control}
        name="birthday"
        label="Ngày sinh"
        render={({ field, inputProps }) => (
          <DatePicker
            value={field.value}
            onChange={field.onChange}
            {...inputProps}
          />
        )}
      />
      <Button type="submit" className="w-fit" disabled={isLoading}>
        {isLoading ? "Đang lưu..." : "Lưu thay đổi"}
      </Button>
    </form>
  )
}
