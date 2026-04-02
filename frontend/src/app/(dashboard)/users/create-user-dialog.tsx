"use client"
import { useState } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { z } from "zod"
import { toast } from "sonner"
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import axiosClientInstance from "@/api/axiosClientInstance"
import { createUser } from "@/api/users/createUser"
import { FirstNameField } from "./form/first-name-field"
import { LastNameField } from "./form/last-name-field"
import { UsernameField } from "./form/username-field"
import { PasswordField } from "./form/password-field"
import { EmailField } from "./form/email-field"
import { PhoneField } from "./form/phone-field"
import { BirthdayField } from "./form/birthday-field"

const createUserSchema = z.object({
  firstName: z.string().min(1, "Họ không được để trống"),
  lastName: z.string().min(1, "Tên không được để trống"),
  username: z
    .string()
    .min(3, "Tên đăng nhập phải có ít nhất 3 ký tự")
    .max(50, "Tên đăng nhập không vượt quá 50 ký tự")
    .regex(/^[a-zA-Z0-9_]+$/, "Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới"),
  password: z
    .string()
    .min(8, "Mật khẩu phải có ít nhất 8 ký tự")
    .regex(/[a-z]/, "Mật khẩu phải có ít nhất 1 chữ thường")
    .regex(/[A-Z]/, "Mật khẩu phải có ít nhất 1 chữ hoa")
    .regex(/[0-9]/, "Mật khẩu phải có ít nhất 1 chữ số"),
  email: z.string().email("Email không hợp lệ").optional().or(z.literal("")),
  phone: z.string().optional(),
  birthday: z.string().optional(),
})

type CreateUserFormValues = z.infer<typeof createUserSchema>

interface CreateUserDialogProps {
  open: boolean
  onClose: () => void
  onSuccess: () => void
}

export default function CreateUserDialog({ open, onClose, onSuccess }: CreateUserDialogProps) {
  const [isSubmitting, setIsSubmitting] = useState(false)

  const form = useForm<CreateUserFormValues>({
    resolver: zodResolver(createUserSchema),
    defaultValues: {
      firstName: "",
      lastName: "",
      username: "",
      password: "",
      email: "",
      phone: "",
      birthday: "",
    },
  })

  async function onSubmit(values: CreateUserFormValues) {
    setIsSubmitting(true)
    try {
      await createUser(axiosClientInstance, {
        firstName: values.firstName,
        lastName: values.lastName,
        username: values.username,
        password: values.password,
        email: values.email || undefined,
        phone: values.phone || undefined,
        birthday: values.birthday || undefined,
      })
      toast.success("Tạo người dùng thành công")
      form.reset()
      onSuccess()
    } catch (err: unknown) {
      const status = (err as { response?: { status?: number; data?: { detail?: string } } })?.response?.status
      const detail = (err as { response?: { data?: { detail?: string } } })?.response?.data?.detail
      if (status === 409) {
        toast.error(detail ?? "Tên đăng nhập đã được sử dụng")
      } else {
        toast.error(detail ?? "Đã xảy ra lỗi, vui lòng thử lại")
      }
    } finally {
      setIsSubmitting(false)
    }
  }

  function handleOpenChange(open: boolean) {
    if (!open) {
      form.reset()
      onClose()
    }
  }

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="sm:max-w-lg">
        <DialogHeader>
          <DialogTitle>Thêm người dùng</DialogTitle>
        </DialogHeader>
        <form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col gap-4 py-2">
          <div className="grid grid-cols-2 gap-4">
            <FirstNameField control={form.control} name="firstName" />
            <LastNameField control={form.control} name="lastName" />
          </div>
          <UsernameField control={form.control} name="username" />
          <PasswordField control={form.control} name="password" />
          <EmailField control={form.control} name="email" />
          <PhoneField control={form.control} name="phone" />
          <BirthdayField control={form.control} name="birthday" />
          <DialogFooter className="pt-2">
            <Button type="button" variant="outline" onClick={() => handleOpenChange(false)} disabled={isSubmitting}>
              Hủy
            </Button>
            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting ? "Đang lưu..." : "Lưu"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
