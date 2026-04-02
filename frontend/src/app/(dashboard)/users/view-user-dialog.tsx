"use client"
import { useEffect, useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
  DialogClose,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import axiosClientInstance from "@/api/axiosClientInstance";
import { getUserById } from "@/api/users/getUserById";
import type { UserDetail } from "@/api/users/types";

interface ViewUserDialogProps {
  userId: string | null;
  onClose: () => void;
}

function LabeledField({ label, value }: { label: string; value?: string }) {
  return (
    <div className="flex flex-col gap-1">
      <span className="text-xs text-muted-foreground">{label}</span>
      <span className="text-sm font-medium">{value || "—"}</span>
    </div>
  );
}

export default function ViewUserDialog({ userId, onClose }: ViewUserDialogProps) {
  const [user, setUser] = useState<UserDetail | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (!userId) {
      setUser(null);
      return;
    }
    setIsLoading(true);
    getUserById(axiosClientInstance, userId)
      .then(setUser)
      .catch(() => setUser(null))
      .finally(() => setIsLoading(false));
  }, [userId]);

  return (
    <Dialog open={!!userId} onOpenChange={(open) => { if (!open) onClose(); }}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Chi tiết người dùng</DialogTitle>
        </DialogHeader>

        {isLoading ? (
          <div className="py-6 text-center text-sm text-muted-foreground">Đang tải...</div>
        ) : user ? (
          <div className="grid grid-cols-2 gap-4 py-2">
            <LabeledField label="Họ" value={user.firstName} />
            <LabeledField label="Tên" value={user.lastName} />
            <LabeledField label="Tên đăng nhập" value={user.username} />
            <LabeledField label="Email" value={user.email} />
            <LabeledField label="Số điện thoại" value={user.phone} />
            <LabeledField label="Ngày sinh" value={user.birthday} />
          </div>
        ) : (
          <div className="py-6 text-center text-sm text-muted-foreground">Không tìm thấy người dùng.</div>
        )}

        <DialogFooter>
          <DialogClose asChild>
            <Button variant="outline">Đóng</Button>
          </DialogClose>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
