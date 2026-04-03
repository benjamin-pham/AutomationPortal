# Mô hình Dữ liệu: Quản lý Gemini Key

**Tính năng**: `003-gemini-key-management`  
**Ngày**: 2026-04-03

---

## Entity: GeminiKey

### Lớp Domain (`AutomationPortal.Domain/Entities/GeminiKey.cs`)

| Thuộc tính | Kiểu | Mô tả |
|-----------|------|-------|
| `Id` | `Guid` | PK, tự sinh (kế thừa `BaseEntity`) |
| `Name` | `string` | Tên key — bắt buộc, duy nhất toàn hệ thống, max 200 ký tự |
| `KeyValue` | `string` | Giá trị API key — bắt buộc, max 500 ký tự, tự động trim khoảng trắng đầu/cuối |
| `UserId` | `Guid` | FK → `users.id` — bắt buộc, unique (một user tối đa một key) |
| `CreatedAt` | `DateTime` | Kế thừa `BaseEntity` |
| `CreatedBy` | `string?` | Kế thừa `BaseEntity` |
| `UpdatedAt` | `DateTime?` | Kế thừa `BaseEntity` |
| `UpdatedBy` | `string?` | Kế thừa `BaseEntity` |
| `IsDeleted` | `bool` | Kế thừa `BaseEntity` — soft delete flag (dùng nội bộ EF Core query filter) |

> **Lưu ý**: Xóa GeminiKey là **hard delete** (theo Assumptions trong spec). `SoftDelete()` từ `BaseEntity` KHÔNG được dùng — gọi `Remove()` qua repository.

### Business Logic (phương thức factory và mutation)

```csharp
// Tạo mới — validate và trim đầu vào
public static GeminiKey Create(string name, string keyValue, Guid userId)

// Cập nhật — validate và trim đầu vào
public void Update(string name, string keyValue, Guid userId)
```

**Quy tắc validation trong entity**:
- `name`: không rỗng/whitespace, max 200 ký tự
- `keyValue`: không rỗng/whitespace sau trim, max 500 ký tự — lưu giá trị đã trim
- `userId`: không phải `Guid.Empty`

---

## Bảng DB: `gemini_keys`

```sql
CREATE TABLE gemini_keys (
    id          UUID        PRIMARY KEY,
    name        VARCHAR(200) NOT NULL,
    key_value   VARCHAR(500) NOT NULL,
    user_id     UUID        NOT NULL REFERENCES users(id),
    created_at  TIMESTAMP   NOT NULL,
    created_by  VARCHAR(256),
    updated_at  TIMESTAMP,
    updated_by  VARCHAR(256),
    is_deleted  BOOLEAN     NOT NULL DEFAULT FALSE
);

-- Đảm bảo tên key duy nhất toàn hệ thống (chỉ trên các bản ghi chưa xóa)
CREATE UNIQUE INDEX ix_gemini_keys_name
    ON gemini_keys (name)
    WHERE is_deleted = false;

-- Đảm bảo mỗi user tối đa một key (chỉ trên các bản ghi chưa xóa)
CREATE UNIQUE INDEX ix_gemini_keys_user_id
    ON gemini_keys (user_id)
    WHERE is_deleted = false;
```

---

## EF Core Configuration (`GeminiKeyConfiguration.cs`)

- Kế thừa `BaseEntityConfiguration<GeminiKey>`
- Table: `gemini_keys`
- `Name` → `name`, max 200, required
- `KeyValue` → `key_value`, max 500, required
- `UserId` → `user_id`, required; **không** có navigation property sang `User`
- Unique index `ix_gemini_keys_name` trên `name` (filter `is_deleted = false`)
- Unique index `ix_gemini_keys_user_id` trên `user_id` (filter `is_deleted = false`)

---

## DTOs (Application Layer)

### `GeminiKeyListItemResponse` (dùng trong GetGeminiKeys)

| Trường | Kiểu | Nguồn |
|--------|------|-------|
| `Id` | `Guid` | `gemini_keys.id` |
| `Name` | `string` | `gemini_keys.name` |
| `MaskedKey` | `string` | Tính từ `key_value`: `"****" + last4chars` |
| `AssignedUsername` | `string` | JOIN với `users.username` |
| `CreatedAt` | `DateTime` | `gemini_keys.created_at` |

### `CreateGeminiKeyResponse`

| Trường | Kiểu |
|--------|------|
| `Id` | `Guid` |
| `Name` | `string` |

---

## Quan hệ Entity

```
User (1) ────────── (0..1) GeminiKey
     users.id  ←  gemini_keys.user_id
```

- `User` entity **không thay đổi** — không thêm navigation property.
- `GeminiKey` giữ `UserId` (FK không có navigation property trong EF model để giữ Clean Architecture).
- Constraint một-một được đảm bảo bởi unique index `ix_gemini_keys_user_id`.

---

## Lỗi Domain (`GeminiKeyErrors.cs`)

| Mã lỗi | Mô tả |
|--------|-------|
| `GeminiKey.NotFound` | Key không tồn tại |
| `GeminiKey.NameAlreadyExists` | Tên Key đã được sử dụng |
| `GeminiKey.UserAlreadyHasKey` | Người dùng này đã có Gemini Key |

---

## Repository Interface (`IGeminiKeyRepository`)

```csharp
public interface IGeminiKeyRepository : IRepository<GeminiKey>
{
    Task<GeminiKey?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<GeminiKey?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
}
```
