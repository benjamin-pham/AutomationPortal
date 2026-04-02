# Data Model: Quản lý Người dùng

**Branch**: `001-user-management` | **Ngày**: 2026-04-02

## Entity hiện có: User

Entity `User` **không thay đổi schema** — tái sử dụng hoàn toàn. Chỉ bổ sung method `ResetPassword()`.

```csharp
// backend/src/AutomationPortal.Domain/Entities/User.cs
public sealed class User : BaseEntity
{
    public string FirstName { get; set; }         // bắt buộc
    public string LastName { get; set; }          // bắt buộc
    public string Username { get; set; }          // bắt buộc, duy nhất, không thay đổi
    public string PasswordHash { get; set; }      // không bao giờ hiển thị
    public string? Email { get; set; }            // tùy chọn, không unique
    public string? Phone { get; set; }            // tùy chọn
    public DateOnly? Birthday { get; set; }       // tùy chọn
    public string? HashedRefreshToken { get; set; }      // không hiển thị
    public DateTime? RefreshTokenExpiresAt { get; set; } // không hiển thị

    // Các method hiện có:
    // static Create(...) — tạo mới
    // UpdateProfile(...) — cập nhật hồ sơ (không có Username)
    // SetRefreshToken(...) — đặt refresh token
    // RevokeRefreshToken() — thu hồi refresh token

    // Method mới cần thêm:
    public void ResetPassword(string newPasswordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newPasswordHash);
        PasswordHash = newPasswordHash;
        RevokeRefreshToken(); // vô hiệu hóa phiên ngay
    }
}
```

**Lý do thêm `ResetPassword()`**: Business logic (gắn với invalidation phiên) thuộc về entity theo nguyên tắc Rich Domain Model của constitution.

## Quy tắc validation

| Trường | Bắt buộc | Quy tắc |
|---|---|---|
| FirstName | ✅ | NotEmpty |
| LastName | ✅ | NotEmpty |
| Username | ✅ (chỉ Create) | `[a-zA-Z0-9]`, 6–50 ký tự; phải duy nhất |
| Password | ✅ (Create + Reset) | ≥ 8 ký tự, có ≥ 1 chữ hoa, ≥ 1 chữ số |
| ConfirmPassword | ✅ (Reset) | Phải khớp Password |
| Email | ❌ | Nếu có: đúng định dạng email |
| Phone | ❌ | Nếu có: không rỗng (định dạng cơ bản) |
| Birthday | ❌ | Nếu có: không trong tương lai |

## Response DTOs (records)

### UserListItemResponse

```csharp
public record UserListItemResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Username,
    string? Email,
    string? Phone,
    DateOnly? Birthday
);
```

### PagedResponse\<T\>

```csharp
// Application/Shared/Dtos/PagedResponse.cs
public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int TotalItems,
    int TotalPages,
    int Page,
    int PageSize);
```

> `TotalPages` được tính sẵn ở backend (`(int)Math.Ceiling((double)totalItems / pageSize)`) để frontend `DataPagination` component không phải tính lại.

### UserDetailResponse

```csharp
public record UserDetailResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Username,
    string? Email,
    string? Phone,
    DateOnly? Birthday
);
```

### CreateUserResponse

```csharp
public record CreateUserResponse(Guid Id, string Username);
```

## IUserRepository — không thay đổi cho phân trang

`IUserRepository` **không** nhận thêm method `GetPagedAsync`. Việc phân trang được thực hiện trực tiếp trong `GetUsersQueryHandler` qua `ISqlConnectionFactory` + Dapper, nhất quán với pattern `GetProfileQueryHandler`. Repository chỉ xử lý write-side và lookup đơn (theo id, username, refresh token).

## TypeScript types (frontend)

```typescript
// Dùng trong api/users/*.ts
export interface UserListItem {
  id: string;
  firstName: string;
  lastName: string;
  username: string;
  email?: string;
  phone?: string;
  birthday?: string; // ISO date string "YYYY-MM-DD"
}

export interface PagedResponse<T> {
  items: T[];
  totalItems: number;
  totalPages: number;
  page: number;
  pageSize: number;
}

export interface UserDetail {
  id: string;
  firstName: string;
  lastName: string;
  username: string;
  email?: string;
  phone?: string;
  birthday?: string;
}

export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  username: string;
  password: string;
  email?: string;
  phone?: string;
  birthday?: string;
}

export interface UpdateUserRequest {
  firstName: string;
  lastName: string;
  email?: string;
  phone?: string;
  birthday?: string;
}

export interface ResetPasswordRequest {
  newPassword: string;
  confirmPassword: string;
}

export interface GetUsersParams {
  search?: string;
  sortColumn?: string;      // snake_case: "first_name" | "last_name" | "username" | "email" | "phone" | "birthday"
  sortDirection?: "asc" | "desc";
  page?: number;
  pageSize?: number;
}
```

## State transitions

```
User tồn tại → ResetPassword() → HashedRefreshToken = null, RefreshTokenExpiresAt = null
                                → PasswordHash = newHash
```

Không có soft delete — xóa là hard delete trực tiếp từ bảng `users`.
