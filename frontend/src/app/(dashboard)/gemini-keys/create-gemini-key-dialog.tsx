"use client"

import { useState } from "react"
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

const createGeminiKeySchema = z.object({
  name: z.string().min(1, "Tên là bắt buộc").max(200, "Tên không được vượt quá 200 ký tự"),
  keyValue: z.string().min(1, "Giá trị Key là bắt buộc").max(500, "Giá trị Key không được vượt quá 500 ký tự"),
  userId: z.string().uuid("Vui lòng chọn người dùng"),
  replaceExisting: z.boolean(),
})

type CreateGeminiKeyValues = z.infer<typeof createGeminiKeySchema>

interface CreateGeminiKeyDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  onSuccess: () => void
}

export function CreateGeminiKeyDialog({ open, onOpenChange, onSuccess }: CreateGeminiKeyDialogProps) {
  const [loading, setLoading] = useState(false)
  const [showReplaceWarning, setShowReplaceWarning] = useState(false)

  const methods = useForm<CreateGeminiKeyValues>({
    resolver: zodResolver(createGeminiKeySchema),
    defaultValues: {
      name: "",
      keyValue: "",
      userId: "",
      replaceExisting: false,
    },
  })

  const { handleSubmit, reset, setValue } = methods

  const onSubmit: SubmitHandler<CreateGeminiKeyValues> = async (values) => {
    setLoading(true)
    try {
      await mainApi(axiosClientInstance).geminiKeys.createGeminiKey(values)
      toast.success("Tạo Gemini Key thành công")
      onOpenChange(false)
      reset()
      setShowReplaceWarning(false)
      onSuccess()
    } catch (error) {
      if (error instanceof AxiosError && error.response?.status === 409) {
        setShowReplaceWarning(true)
        setValue("replaceExisting", true)
        toast.warning("Người dùng đã chọn đã có key. Xác nhận để thay thế.")
      } else if (error instanceof AxiosError && error.response?.data?.title === "GeminiKey.NameAlreadyExists") {
        methods.setError("name", { message: "Tên đã tồn tại" })
      } else {
        toast.error("Tạo Gemini Key thất bại")
        console.error(error)
      }
    } finally {
      setLoading(false)
    }
  }

  const handleOpenChange = (newOpen: boolean) => {
    if (!newOpen) {
      reset()
      setShowReplaceWarning(false)
    }
    onOpenChange(newOpen)
  }

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Thêm Gemini Key</DialogTitle>
          <DialogDescription>
            Đăng ký một Gemini API Key mới và gán nó cho một người dùng tự động hóa.
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
                <p>Người dùng đã chọn đã có Gemini key. Lưu bây giờ sẽ thay thế vĩnh viễn key cũ bằng key này.</p>
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
                {loading ? "Đang lưu..." : showReplaceWarning ? "Thay thế Key hiện tại" : "Tạo Key"}
              </Button>
            </DialogFooter>
          </form>
        </FormProvider>
      </DialogContent>
    </Dialog>
  )
}
