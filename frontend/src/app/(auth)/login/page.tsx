"use client"

import { useState } from "react"
import { useRouter } from "next/navigation"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { z } from "zod"
import { toast } from "sonner"

import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { FormField } from "@/components/form/form-field"

const loginSchema = z.object({
  username: z.string().min(1, "Tên đăng nhập không được để trống"),
  password: z.string().min(1, "Mật khẩu không được để trống"),
})

type LoginFormValues = z.infer<typeof loginSchema>

export default function LoginPage() {
  const router = useRouter()
  const [isLoading, setIsLoading] = useState(false)

  const form = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: { username: "", password: "" },
  })

  async function onSubmit(values: LoginFormValues) {
    console.log('Submitting form with values:', values);
    setIsLoading(true)
    try {
      const res = await fetch("/api/auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(values),
      })

      if (!res.ok) {
        const data = await res.json().catch(() => ({}))
        toast.error(data?.detail ?? data?.message ?? "Đăng nhập thất bại")
        return
      }

      router.push("/")
      router.refresh()
    } catch {
      toast.error("Đã xảy ra lỗi, vui lòng thử lại")
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <Card className="w-full max-w-sm">
      <CardHeader>
        <CardTitle className="text-2xl">Đăng nhập</CardTitle>
        <CardDescription>Nhập thông tin tài khoản để tiếp tục</CardDescription>
      </CardHeader>
      <CardContent>
        <form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col gap-4">
          <FormField
            control={form.control}
            name="username"
            label="Tên đăng nhập"
            required
            render={({ field, inputProps }) => (
              <Input
                {...field}
                {...inputProps}
                type="text"
                placeholder="username"
                autoComplete="username"
              />
            )}
          />
          <FormField
            control={form.control}
            name="password"
            label="Mật khẩu"
            required
            render={({ field, inputProps }) => (
              <Input
                {...field}
                {...inputProps}
                type="password"
                placeholder="••••••••"
                autoComplete="current-password"
              />
            )}
          />
          <Button type="submit" className="w-full mt-2" disabled={isLoading}>
            {isLoading ? "Đang đăng nhập..." : "Đăng nhập"}
          </Button>
        </form>
      </CardContent>
    </Card>
  )
}
