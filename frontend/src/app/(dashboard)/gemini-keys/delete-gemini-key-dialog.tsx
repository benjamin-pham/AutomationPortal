"use client"

import { useState } from "react"
import { toast } from "sonner"

import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import axiosClientInstance from "@/api/axiosClientInstance"
import mainApi from "@/api"

interface DeleteGeminiKeyDialogProps {
  geminiKeyId: string | null
  geminiKeyName: string | null
  open: boolean
  onOpenChange: (open: boolean) => void
  onSuccess: () => void
}

export function DeleteGeminiKeyDialog({
  geminiKeyId,
  geminiKeyName,
  open,
  onOpenChange,
  onSuccess,
}: DeleteGeminiKeyDialogProps) {
  const [confirmName, setConfirmName] = useState("")
  const [loading, setLoading] = useState(false)

  const isConfirmed = 
    geminiKeyName && 
    confirmName.trim().toLowerCase() === geminiKeyName.trim().toLowerCase()

  const handleDelete = async () => {
    if (!geminiKeyId || !isConfirmed) return

    setLoading(true)
    try {
      await mainApi(axiosClientInstance).geminiKeys.deleteGeminiKey(geminiKeyId)
      toast.success("Xóa Gemini Key thành công")
      onOpenChange(false)
      setConfirmName("")
      onSuccess()
    } catch (error) {
      toast.error("Xóa Gemini Key thất bại")
      console.error(error)
    } finally {
      setLoading(false)
    }
  }

  const handleOpenChange = (newOpen: boolean) => {
    if (!newOpen) {
      setConfirmName("")
    }
    onOpenChange(newOpen)
  }

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle className="text-destructive">Xóa Gemini Key</DialogTitle>
          <DialogDescription>
            Hành động này là vĩnh viễn và không thể hoàn tác. Tất cả dữ liệu liên quan đến key này sẽ bị xóa.
          </DialogDescription>
        </DialogHeader>
        <div className="py-4 space-y-4">
          <p className="text-sm">
            Để xác nhận, vui lòng nhập <span className="font-bold select-none">"{geminiKeyName}"</span> vào ô bên dưới:
          </p>
          <div className="space-y-2">
            <Label htmlFor="confirmName">Tên Gemini Key</Label>
            <Input
              id="confirmName"
              placeholder="Nhập tên để xác nhận"
              value={confirmName}
              onChange={(e) => setConfirmName(e.target.value)}
              autoComplete="off"
            />
          </div>
        </div>
        <DialogFooter>
          <Button
            type="button"
            variant="outline"
            onClick={() => handleOpenChange(false)}
            disabled={loading}
          >
            Hủy
          </Button>
          <Button
            type="button"
            variant="destructive"
            onClick={handleDelete}
            disabled={!isConfirmed || loading}
          >
            {loading ? "Đang xóa..." : "Xóa vĩnh viễn"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
