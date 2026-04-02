"use client"
import { useState } from "react"
import { toast } from "sonner"
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog"
import axiosClientInstance from "@/api/axiosClientInstance"
import { deleteUser } from "@/api/users/deleteUser"

interface DeleteUserDialogProps {
  userId: string | null
  username: string | null
  onClose: () => void
  onSuccess: () => void
}

export default function DeleteUserDialog({ userId, username, onClose, onSuccess }: DeleteUserDialogProps) {
  const [isSubmitting, setIsSubmitting] = useState(false)

  async function handleConfirm() {
    if (!userId) return
    setIsSubmitting(true)
    try {
      await deleteUser(axiosClientInstance, userId)
      toast.success("Xóa người dùng thành công")
      onSuccess()
    } catch (err: unknown) {
      const detail = (err as { response?: { data?: { detail?: string } } })?.response?.data?.detail
      toast.error(detail ?? "Đã xảy ra lỗi, vui lòng thử lại")
      onClose()
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <AlertDialog open={!!userId} onOpenChange={(open) => { if (!open) onClose() }}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Xóa người dùng</AlertDialogTitle>
          <AlertDialogDescription>
            Bạn có chắc chắn muốn xóa người dùng <strong>{username}</strong>? Hành động này không thể hoàn tác.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel disabled={isSubmitting}>Hủy</AlertDialogCancel>
          <AlertDialogAction onClick={handleConfirm} disabled={isSubmitting}>
            {isSubmitting ? "Đang xóa..." : "Xác nhận"}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  )
}
