# API Contracts: Users

**Branch**: `001-user-management` | **Ngày**: 2026-04-02  
**Base URL**: `/api/users`  
**Authentication**: Bearer JWT (tất cả endpoint yêu cầu `[Authorize]`)  
**Tags**: `Users`

---

## GET /api/users

Lấy danh sách người dùng có phân trang, tìm kiếm và sắp xếp.

**WithName**: `GetUsers`

**Triển khai**: Raw SQL + Dapper qua `ISqlConnectionFactory` — không dùng EF Core cho endpoint này.

### Query Parameters

| Tham số | Kiểu | Mặc định | Mô tả |
|---|---|---|---|
| `search` | `string?` | null | Tìm kiếm theo first_name, last_name, username, email (ILIKE, case-insensitive) |
| `sortColumn` | `string?` | `"username"` | Cột sắp xếp: `first_name`, `last_name`, `username`, `email`, `phone`, `birthday` (whitelist, giá trị khác → default) |
| `sortDirection` | `string?` | `"asc"` | Hướng sắp xếp: `"asc"` hoặc `"desc"` |
| `page` | `int` | `1` | Số trang (≥ 1) |
| `pageSize` | `int` | `20` | Số bản ghi mỗi trang (1–100) |

### Response 200 OK

```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "firstName": "Nguyễn",
      "lastName": "An",
      "username": "nguyen_an",
      "email": "an@example.com",
      "phone": "0901234567",
      "birthday": "1990-05-15"
    }
  ],
  "totalItems": 150,
  "totalPages": 8,
  "page": 1,
  "pageSize": 20
}
```

### Response 422 Validation Error

```json
{
  "type": "https://tools.ietf.org/html/rfc4918#section-11.2",
  "title": "Validation.Error",
  "detail": "page phải ≥ 1"
}
```

---

## GET /api/users/{id}

Lấy chi tiết một người dùng theo ID.

**WithName**: `GetUserById`

### Path Parameters

| Tham số | Kiểu | Mô tả |
|---|---|---|
| `id` | `Guid` | ID người dùng |

### Response 200 OK

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "Nguyễn",
  "lastName": "An",
  "username": "nguyen_an",
  "email": "an@example.com",
  "phone": "0901234567",
  "birthday": "1990-05-15"
}
```

### Response 404 Not Found

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "User.NotFound",
  "detail": "Người dùng không tồn tại."
}
```

---

## POST /api/users

Tạo người dùng mới.

**WithName**: `CreateUser`

### Request Body

```json
{
  "firstName": "Nguyễn",
  "lastName": "An",
  "username": "nguyenan01",
  "password": "Password1",
  "email": "an@example.com",
  "phone": "0901234567",
  "birthday": "1990-05-15"
}
```

| Trường | Bắt buộc | Quy tắc |
|---|---|---|
| `firstName` | ✅ | NotEmpty |
| `lastName` | ✅ | NotEmpty |
| `username` | ✅ | `[a-zA-Z0-9]`, 6–50 ký tự, duy nhất |
| `password` | ✅ | ≥ 8 ký tự, ≥ 1 chữ hoa, ≥ 1 chữ số |
| `email` | ❌ | Đúng định dạng email nếu có |
| `phone` | ❌ | Không rỗng nếu có |
| `birthday` | ❌ | `YYYY-MM-DD`, không trong tương lai |

### Response 201 Created

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "username": "nguyenan01"
}
```

### Response 409 Conflict

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.10",
  "title": "User.UsernameAlreadyExists",
  "detail": "Username đã được sử dụng."
}
```

### Response 422 Validation Error

```json
{
  "type": "https://tools.ietf.org/html/rfc4918#section-11.2",
  "title": "Validation.Error",
  "detail": "password phải có ít nhất 8 ký tự."
}
```

---

## PUT /api/users/{id}

Cập nhật thông tin hồ sơ người dùng (không bao gồm Username).

**WithName**: `UpdateUser`

### Path Parameters

| Tham số | Kiểu | Mô tả |
|---|---|---|
| `id` | `Guid` | ID người dùng |

### Request Body

```json
{
  "firstName": "Nguyễn",
  "lastName": "Bình",
  "email": "binh@example.com",
  "phone": "0909999999",
  "birthday": "1990-05-15"
}
```

| Trường | Bắt buộc | Quy tắc |
|---|---|---|
| `firstName` | ✅ | NotEmpty |
| `lastName` | ✅ | NotEmpty |
| `email` | ❌ | Đúng định dạng email nếu có |
| `phone` | ❌ | Không rỗng nếu có |
| `birthday` | ❌ | `YYYY-MM-DD` nếu có |

### Response 204 No Content

*(Không có body)*

### Response 404 Not Found

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "User.NotFound",
  "detail": "Người dùng không tồn tại."
}
```

---

## DELETE /api/users/{id}

Xóa vĩnh viễn người dùng (hard delete).

**WithName**: `DeleteUser`

### Path Parameters

| Tham số | Kiểu | Mô tả |
|---|---|---|
| `id` | `Guid` | ID người dùng |

### Response 204 No Content

*(Không có body)*

### Response 404 Not Found

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "User.NotFound",
  "detail": "Người dùng không tồn tại."
}
```

### Response 409 Conflict — tự xóa chính mình

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.10",
  "title": "User.CannotDeleteSelf",
  "detail": "Không thể xóa tài khoản đang đăng nhập."
}
```

---

## POST /api/users/{id}/reset-password

Đặt lại mật khẩu người dùng và vô hiệu hóa tất cả phiên đăng nhập.

**WithName**: `ResetUserPassword`

### Path Parameters

| Tham số | Kiểu | Mô tả |
|---|---|---|
| `id` | `Guid` | ID người dùng |

### Request Body

```json
{
  "newPassword": "NewPassword1",
  "confirmPassword": "NewPassword1"
}
```

| Trường | Bắt buộc | Quy tắc |
|---|---|---|
| `newPassword` | ✅ | ≥ 8 ký tự, ≥ 1 chữ hoa, ≥ 1 chữ số |
| `confirmPassword` | ✅ | Phải khớp `newPassword` |

### Response 204 No Content

*(Không có body — phiên refresh token bị thu hồi)*

### Response 404 Not Found

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "User.NotFound",
  "detail": "Người dùng không tồn tại."
}
```

### Response 422 Validation Error

```json
{
  "type": "https://tools.ietf.org/html/rfc4918#section-11.2",
  "title": "Validation.Error",
  "detail": "confirmPassword không khớp với newPassword."
}
```

---

## Error Codes tổng hợp

| Error Code | HTTP Status | Mô tả |
|---|---|---|
| `User.NotFound` | 404 | Không tìm thấy người dùng với ID đã cho |
| `User.UsernameAlreadyExists` | 409 | Username đã tồn tại trong hệ thống |
| `User.CannotDeleteSelf` | 409 | Người dùng cố xóa tài khoản của chính mình |
| `Validation.Error` | 422 | Lỗi validation input |
| Unauthorized | 401 | JWT không hợp lệ hoặc hết hạn |
