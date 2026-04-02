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
import { resetUserPassword } from "@/api/users/resetUserPassword"
import { PasswordField } from "./form/password-field"

const resetPasswordSchema = z
  .object({
    newPassword: z
      .string()
      .min(8, "Mật khẩu phải có ít nhất 8 ký tự")
      .regex(/[a-z]/, "Mật khẩu phải có ít nhất 1 chữ thường")
      .regex(/[A-Z]/, "Mật khẩu phải có ít nhất 1 chữ hoa")
      .regex(/[0-9]/, "Mật khẩu phải có ít nhất 1 chữ số"),
    confirmPassword: z.string().min(1, "Xác nhận mật khẩu không được để trống"),
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    message: "Xác nhận mật khẩu không khớp",
    path: ["confirmPassword"],
  })

type ResetPasswordFormValues = z.infer<typeof resetPasswordSchema>

interface ResetPasswordDialogProps {
  userId: string | null
  onClose: () => void
}

export default function ResetPasswordDialog({ userId, onClose }: ResetPasswordDialogProps) {
  const [isSubmitting, setIsSubmitting] = useState(false)

  const form = useForm<ResetPasswordFormValues>({
    resolver: zodResolver(resetPasswordSchema),
    defaultValues: {
      newPassword: "",
      confirmPassword: "",
    },
  })

  async function onSubmit(values: ResetPasswordFormValues) {
    if (!userId) return
    setIsSubmitting(true)
    try {
      await resetUserPassword(axiosClientInstance, userId, {
        newPassword: values.newPassword,
        confirmPassword: values.confirmPassword,
      })
      toast.success("Đặt lại mật khẩu thành công")
      form.reset()
      onClose()
    } catch (err: unknown) {
      const detail = (err as { response?: { data?: { detail?: string } } })?.response?.data?.detail
      toast.error(detail ?? "Đã xảy ra lỗi, vui lòng thử lại")
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
    <Dialog open={!!userId} onOpenChange={handleOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Đặt lại mật khẩu</DialogTitle>
        </DialogHeader>
        <form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col gap-4 py-2">
          <PasswordField control={form.control} name="newPassword" label="Mật khẩu mới" />
          <PasswordField control={form.control} name="confirmPassword" label="Xác nhận mật khẩu" />
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
