"use client"

import { useEffect, useState } from "react"
import { useForm, FormProvider, SubmitHandler } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import * as z from "zod"
import { toast } from "sonner"
import { AxiosError } from "axios"

import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { NameField } from "@/app/(dashboard)/gemini-keys/form/name-field"
import { KeyValueField } from "@/app/(dashboard)/gemini-keys/form/key-value-field"
import { UserSelectField } from "@/app/(dashboard)/gemini-keys/form/user-select-field"
import axiosClientInstance from "@/api/axiosClientInstance"
import mainApi from "@/api"
import type { GeminiKeyListItem } from "@/api/gemini-keys/types"

const editGeminiKeySchema = z.object({
  name: z.string().min(1, "Tên là bắt buộc").max(200, "Tên không được vượt quá 200 ký tự"),
  keyValue: z.string().min(1, "Giá trị Key là bắt buộc").max(500, "Giá trị Key không được vượt quá 500 ký tự"),
  userId: z.string().uuid("Vui lòng chọn người dùng"),
  replaceExisting: z.boolean(),
})

type EditGeminiKeyValues = z.infer<typeof editGeminiKeySchema>

interface EditGeminiKeyDialogProps {
  geminiKey: GeminiKeyListItem | null
  open: boolean
  onOpenChange: (open: boolean) => void
  onSuccess: () => void
}

export function EditGeminiKeyDialog({ geminiKey, open, onOpenChange, onSuccess }: EditGeminiKeyDialogProps) {
  const [loading, setLoading] = useState(false)
  const [showReplaceWarning, setShowReplaceWarning] = useState(false)

  const methods = useForm<EditGeminiKeyValues>({
    resolver: zodResolver(editGeminiKeySchema),
    defaultValues: {
      name: "",
      keyValue: "",
      userId: "",
      replaceExisting: false,
    },
  })

  const { handleSubmit, reset, setValue } = methods

  useEffect(() => {
    if (geminiKey && open) {
      reset({
        name: geminiKey.name,
        keyValue: geminiKey.maskedKey,
        userId: geminiKey.assignedUserId,
        replaceExisting: false,
      })
      setShowReplaceWarning(false)
    }
  }, [geminiKey, open, reset])

  const onSubmit: SubmitHandler<EditGeminiKeyValues> = async (values) => {
    if (!geminiKey) return

    setLoading(true)
    try {
      await mainApi(axiosClientInstance).geminiKeys.updateGeminiKey(geminiKey.id, values)
      toast.success("Cập nhật Gemini Key thành công")
      onOpenChange(false)
      setShowReplaceWarning(false)
      onSuccess()
    } catch (error) {
      if (error instanceof AxiosError && error.response?.status === 409) {
        setShowReplaceWarning(true)
        setValue("replaceExisting", true)
        toast.warning("Người dùng đã chọn đã có Gemini Key KHÁC. Xác nhận để thay thế.")
      } else if (error instanceof AxiosError && error.response?.data?.title === "GeminiKey.NameAlreadyExists") {
        methods.setError("name", { message: "Tên đã tồn tại" })
      } else {
        toast.error("Cập nhật Gemini Key thất bại")
        console.error(error)
      }
    } finally {
      setLoading(false)
    }
  }

  const handleOpenChange = (newOpen: boolean) => {
    if (!newOpen) {
      setShowReplaceWarning(false)
    }
    onOpenChange(newOpen)
  }

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Cập nhật Gemini Key</DialogTitle>
          <DialogDescription>
            Cập nhật chi tiết Gemini API Key hoặc thay đổi người dùng được gán.
          </DialogDescription>
        </DialogHeader>
        <FormProvider {...methods}>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
            <div className="grid gap-4 py-4">
              <NameField />
              <KeyValueField />
              <UserSelectField />
            </div>

            {showReplaceWarning && (
              <div className="p-3 bg-amber-50 border border-amber-500 rounded-md text-sm text-amber-700">
                <p className="font-semibold">⚠️ Cảnh báo</p>
                <p>Người dùng mới được chọn đã có Gemini key. Lưu bây giờ sẽ thay thế vĩnh viễn key cũ của họ bằng key này.</p>
              </div>
            )}

            <DialogFooter>
              <Button
                type="button"
                variant="outline"
                onClick={() => handleOpenChange(false)}
                disabled={loading}
              >
                Hủy
              </Button>
              <Button type="submit" disabled={loading} variant={showReplaceWarning ? "destructive" : "default"}>
                {loading ? "Đang lưu..." : showReplaceWarning ? "Thay thế Key hiện tại" : "Cập nhật Key"}
              </Button>
            </DialogFooter>
          </form>
        </FormProvider>
      </DialogContent>
    </Dialog>
  )
}
