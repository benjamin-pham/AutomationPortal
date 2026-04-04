"use client"

import { useEffect, useState } from "react"
import { useFormContext, Controller } from "react-hook-form"
import { Field, FieldLabel, FieldError } from "@/components/ui/field"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { getListUser, UserListItem } from "@/api/users/getListUser"
import axiosClientInstance from "@/api/axiosClientInstance"

export const UserSelectField = () => {
  const {
    control,
    formState: { errors },
  } = useFormContext()

  const [users, setUsers] = useState<UserListItem[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const data = await getListUser(axiosClientInstance, { page: 1, pageSize: 100 })
        setUsers(data.items)
      } catch (error) {
        console.error("Failed to fetch users", error)
      } finally {
        setLoading(false)
      }
    }
    fetchUsers()
  }, [])

  return (
    <Field>
      <FieldLabel htmlFor="userId">Gán cho người dùng</FieldLabel>
      <Controller
        control={control}
        name="userId"
        render={({ field }) => (
          <Select
            onValueChange={field.onChange}
            value={field.value}
            disabled={loading}
          >
            <SelectTrigger id="userId" aria-invalid={!!errors.userId}>
              <SelectValue placeholder={loading ? "Đang tải danh sách người dùng..." : "Chọn người dùng"} />
            </SelectTrigger>
            <SelectContent>
              {users.map((user) => (
                <SelectItem key={user.id} value={user.id}>
                  {user.username} ({user.email})
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        )}
      />
      <FieldError errors={[errors.userId as any]} />
    </Field>
  )
}
