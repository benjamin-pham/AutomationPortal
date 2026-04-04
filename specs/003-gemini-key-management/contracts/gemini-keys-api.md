# Hợp đồng API: Gemini Keys

**Tính năng**: `003-gemini-key-management`  
**Ngày**: 2026-04-03  
**Xác thực**: JWT Bearer (RequireAuthorization — tất cả endpoint)  
**Tag Scalar**: `GeminiKeys`

---

## GET /api/gemini-keys

Lấy danh sách Gemini Key dạng phân trang.

### Query Parameters

| Tham số | Kiểu | Bắt buộc | Mặc định | Mô tả |
|---------|------|----------|---------|-------|
| `pageNumber` | `int` | Không | `1` | Số trang (≥ 1) |
| `pageSize` | `int` | Không | `20` | Kích thước trang (1–100) |

### Response 200 OK

```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Key Production",
      "maskedKey": "****Xyz9",
      "assignedUsername": "john.doe",
      "createdAt": "2026-04-03T10:00:00Z"
    }
  ],
  "pageNumber": 1,
  "pageSize": 20,
  "totalItems": 1,
  "totalPages": 1
}
```

**Kiểu**: `PagedList<GeminiKeyListItemResponse>`

---

## POST /api/gemini-keys

Tạo Gemini Key mới. Nếu user đã có key, cần truyền `replaceExisting: true` để thay thế.

### Request Body

```json
{
  "name": "Key Production",
  "keyValue": "AIzaSyABC123xyz",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "replaceExisting": false
}
```

| Trường | Kiểu | Bắt buộc | Mô tả |
|--------|------|----------|-------|
| `name` | `string` | Có | Tên key — duy nhất toàn hệ thống |
| `keyValue` | `string` | Có | Giá trị Gemini API key (sẽ được trim trước khi lưu) |
| `userId` | `Guid` | Có | ID người dùng được gán |
| `replaceExisting` | `bool` | Không | `false` mặc định — nếu user đã có key và muốn thay thế, set `true` |

### Response 201 Created

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Key Production"
}
```

### Response 409 Conflict (`GeminiKey.UserAlreadyHasKey`)

Trả về khi `userId` đã có key và `replaceExisting = false`. Frontend hiển thị cảnh báo, người dùng xác nhận → gọi lại với `replaceExisting: true`.

```json
{
  "title": "GeminiKey.UserAlreadyHasKey",
  "detail": "Người dùng này đã có Gemini Key.",
  "status": 422
}
```

### Response 422 Unprocessable Entity

- `GeminiKey.NameAlreadyExists` — Tên Key đã được sử dụng
- `User.NotFound` — Người dùng không tồn tại
- Lỗi validation FluentValidation (trường bắt buộc trống)

---

## PUT /api/gemini-keys/{id}

Cập nhật Gemini Key theo ID.

### Path Parameter

| Tham số | Kiểu | Mô tả |
|---------|------|-------|
| `id` | `Guid` | ID của GeminiKey cần cập nhật |

### Request Body

```json
{
  "name": "Key Production Updated",
  "keyValue": "AIzaSyXYZ789new",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "replaceExisting": false
}
```

Cùng schema với POST body.

### Response 204 No Content

Cập nhật thành công.

### Response 404 Not Found

GeminiKey không tồn tại.

### Response 409 / 422

Cùng logic với POST.

---

## DELETE /api/gemini-keys/{id}

Xóa vĩnh viễn (hard delete) Gemini Key theo ID.

### Path Parameter

| Tham số | Kiểu | Mô tả |
|---------|------|-------|
| `id` | `Guid` | ID của GeminiKey cần xóa |

### Response 204 No Content

Xóa thành công.

### Response 404 Not Found

```json
{
  "title": "GeminiKey.NotFound",
  "detail": "Gemini Key không tồn tại.",
  "status": 422
}
```

---

## Endpoint Registration (Scalar)

| Endpoint | `WithName()` | Tag |
|----------|-------------|-----|
| GET /api/gemini-keys | `"GetGeminiKeys"` | GeminiKeys |
| POST /api/gemini-keys | `"CreateGeminiKey"` | GeminiKeys |
| PUT /api/gemini-keys/{id} | `"UpdateGeminiKey"` | GeminiKeys |
| DELETE /api/gemini-keys/{id} | `"DeleteGeminiKey"` | GeminiKeys |
