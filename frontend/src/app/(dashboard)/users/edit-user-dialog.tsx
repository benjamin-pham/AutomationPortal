"use client"
import { useEffect, useState } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from '@hookform/resolvers/zod';
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
import { getUserById } from "@/api/users/getUserById"
import { updateUser } from "@/api/users/updateUser"
import { FirstNameField } from "./form/first-name-field"
import { LastNameField } from "./form/last-name-field"
import { EmailField } from "./form/email-field"
import { PhoneField } from "./form/phone-field"
import { BirthdayField } from "./form/birthday-field"

const editUserSchema = z.object({
  firstName: z.string().min(1, "Họ không được để trống"),
  lastName: z.string().min(1, "Tên không được để trống"),
  email: z.string().email("Email không hợp lệ").optional().or(z.literal("")),
  phone: z.string().optional(),
  birthday: z.string().optional(),
})

type EditUserFormValues = z.infer<typeof editUserSchema>

interface EditUserDialogProps {
  userId: string | null
  onClose: () => void
  onSuccess: () => void
}

export default function EditUserDialog({ userId, onClose, onSuccess }: EditUserDialogProps) {
  const [isLoading, setIsLoading] = useState(false)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [username, setUsername] = useState<string>("")

  const form = useForm<EditUserFormValues>({
    resolver: zodResolver(editUserSchema),
    defaultValues: {
      firstName: "",
      lastName: "",
      email: "",
      phone: "",
      birthday: "",
    },
  })

  useEffect(() => {
    if (!userId) return

    setIsLoading(true)
    getUserById(axiosClientInstance, userId)
      .then((user) => {
        setUsername(user.username)
        form.reset({
          firstName: user.firstName,
          lastName: user.lastName,
          email: user.email ?? "",
          phone: user.phone ?? "",
          birthday: user.birthday ?? "",
        })
      })
      .catch(() => {
        toast.error("Không thể tải thông tin người dùng")
        onClose()
      })
      .finally(() => setIsLoading(false))
  }, [userId, form, onClose])

  async function onSubmit(values: EditUserFormValues) {
    if (!userId) return
    setIsSubmitting(true)
    try {
      await updateUser(axiosClientInstance, userId, {
        firstName: values.firstName,
        lastName: values.lastName,
        email: values.email || undefined,
        phone: values.phone || undefined,
        birthday: values.birthday || undefined,
      })
      toast.success("Cập nhật người dùng thành công")
      onSuccess()
    } catch (err: unknown) {
      const detail = (err as { response?: { data?: { detail?: string } } })?.response?.data?.detail
      toast.error(detail ?? "Đã xảy ra lỗi, vui lòng thử lại")
    } finally {
      setIsSubmitting(false)
    }
  }

  function handleOpenChange(open: boolean) {
    if (!open) onClose()
  }

  return (
    <Dialog open={!!userId} onOpenChange={handleOpenChange}>
      <DialogContent className="sm:max-w-lg">
        <DialogHeader>
          <DialogTitle>Chỉnh sửa người dùng</DialogTitle>
        </DialogHeader>

        {isLoading ? (
          <div className="py-6 text-center text-sm text-muted-foreground">Đang tải...</div>
        ) : (
          <form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col gap-4 py-2">
            <div className="flex flex-col gap-1">
              <span className="text-xs text-muted-foreground">Tên đăng nhập</span>
              <span className="text-sm font-medium">{username}</span>
            </div>
            <div className="grid grid-cols-2 gap-4">
              <FirstNameField control={form.control} name="firstName" />
              <LastNameField control={form.control} name="lastName" />
            </div>
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
        )}
      </DialogContent>
    </Dialog>
  )
}
