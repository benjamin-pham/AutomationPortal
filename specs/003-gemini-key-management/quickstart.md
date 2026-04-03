# Hướng dẫn Nhanh: Quản lý Gemini Key

**Tính năng**: `003-gemini-key-management`  
**Ngày**: 2026-04-03

---

## Chạy môi trường phát triển

### Backend

```bash
cd backend
dotnet build
dotnet run --project src/AutomationPortal.API/AutomationPortal.API.csproj
```

API mặc định chạy tại `https://localhost:5001`. Scalar UI: `https://localhost:5001/scalar`.

### Frontend

```bash
cd frontend
npm run dev
```

Frontend chạy tại `http://localhost:3000`. Trang Gemini Keys: `http://localhost:3000/gemini-keys`.

---

## Migration cơ sở dữ liệu

Sau khi thêm `GeminiKeyConfiguration` và cập nhật `AppDbContext`:

```bash
cd backend
dotnet ef migrations add AddGeminiKeys \
  --project src/AutomationPortal.Infrastructure \
  --startup-project src/AutomationPortal.API
dotnet ef database update \
  --project src/AutomationPortal.Infrastructure \
  --startup-project src/AutomationPortal.API
```

---

## Chạy kiểm thử

```bash
cd backend

# Unit tests
dotnet test tests/AutomationPortal.Domain.UnitTests/
dotnet test tests/AutomationPortal.Application.UnitTests/

# Integration tests (cần Docker để chạy Testcontainers)
dotnet test tests/AutomationPortal.Infrastructure.IntegrationTests/
dotnet test tests/AutomationPortal.API.IntegrationTests/

# Architecture tests
dotnet test tests/AutomationPortal.ArchitectureTests/
```

---

## Các điểm tích hợp chính

### Backend — Đăng ký phụ thuộc

1. **`AppDbContext`**: Thêm `DbSet<GeminiKey> GeminiKeys` vào `AppDbContext.cs`
2. **`DependencyInjection.cs` (Infrastructure)**: Đăng ký `IGeminiKeyRepository` → `GeminiKeyRepository`
3. **`EndpointExtensions.cs` (API)**: `MapGeminiKeyEndpoints()` được tự động quét qua `IEndpoint` pattern

### Frontend — Đăng ký API

`src/api/index.ts` — thêm `geminiKeysApi`:

```typescript
import geminiKeysApi from "./gemini-keys";

const api = (axios: AxiosInstance) => ({
  // ...existing...
  geminiKeys: geminiKeysApi(axios),
});
```

### Frontend — Navigation

Thêm mục "Gemini Keys" vào sidebar trong `src/app/(dashboard)/layout.tsx`.

---

## Luồng dữ liệu: Tạo Key với cảnh báo "user đã có key"

```
User nhấn Lưu
    ↓
POST /api/gemini-keys { replaceExisting: false }
    ↓ (409 nếu user đã có key)
Frontend hiển thị cảnh báo xác nhận
    ↓ (User xác nhận)
POST /api/gemini-keys { replaceExisting: true }
    ↓
Handler: xóa key cũ + tạo key mới trong transaction
    ↓
201 Created → danh sách refresh
```

---

## Luồng dữ liệu: Xóa Key với xác nhận tên

```
User nhấn Xóa
    ↓
Hiển thị dialog — yêu cầu gõ tên key
    ↓
Frontend: trim() + toLowerCase() cả hai phía, so sánh
    ↓ (khớp)
Nút "Xác nhận xóa" được kích hoạt
    ↓ (User nhấn)
DELETE /api/gemini-keys/{id}
    ↓
204 No Content → danh sách refresh
```

---

## Che giá trị Key

- **Backend (Application layer)**: `string maskedKey = keyValue.Length >= 4 ? "****" + keyValue[^4..] : "****";`
- `key_value` **không bao giờ** xuất hiện trong response DTO ở bất kỳ endpoint nào
- Frontend chỉ nhận `maskedKey` — không cần xử lý che ký tự
