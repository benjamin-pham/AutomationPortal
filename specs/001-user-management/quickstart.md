# Quickstart: Quản lý Người dùng

**Branch**: `001-user-management` | **Ngày**: 2026-04-02

## Yêu cầu trước

- .NET 10 SDK (`dotnet --version` → `10.x`)
- Node.js 20+ (`node --version`)
- PostgreSQL chạy local hoặc Docker
- Database đã chạy migrations (`20260327031101_AddUsersTable` đã áp dụng)

## 1. Khởi động backend

```bash
cd backend
dotnet build
dotnet run --project src/AutomationPortal.API
# API chạy tại https://localhost:7xxx hoặc http://localhost:5xxx
# Scalar UI: https://localhost:7xxx/scalar
```

## 2. Khởi động frontend

```bash
cd frontend
npm install
npm run dev
# Frontend chạy tại http://localhost:3000
```

## 3. Kiểm tra nhanh API Users

```bash
# Đăng nhập lấy token (thay username/password phù hợp)
TOKEN=$(curl -s -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin123"}' | jq -r '.accessToken')

# Lấy danh sách người dùng
curl -H "Authorization: Bearer $TOKEN" \
  "http://localhost:5000/api/users?page=1&pageSize=10"

# Tạo người dùng mới
curl -X POST http://localhost:5000/api/users \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"firstName":"Test","lastName":"User","username":"testuser1","password":"Password1"}'
```

## 4. Truy cập giao diện

1. Mở `http://localhost:3000` → đăng nhập
2. Điều hướng sidebar → **Người dùng** (`/users`)
3. Thực hiện các thao tác: tìm kiếm, tạo mới, chỉnh sửa, xóa, reset mật khẩu

## 5. Chạy tests

```bash
cd backend

# Unit tests (không cần DB)
dotnet test tests/AutomationPortal.Application.UnitTests
dotnet test tests/AutomationPortal.Domain.UnitTests

# Integration tests (cần Docker cho Testcontainers)
dotnet test tests/AutomationPortal.Infrastructure.IntegrationTests
dotnet test tests/AutomationPortal.API.IntegrationTests
```

## 6. Kiểm tra API docs

Truy cập Scalar UI tại `https://localhost:7xxx/scalar` → section **Users** để xem và thử tất cả 6 endpoint.

## Các endpoint mới

| Method | Path | Chức năng |
|---|---|---|
| GET | `/api/users` | Danh sách + tìm kiếm + phân trang + sắp xếp |
| GET | `/api/users/{id}` | Chi tiết người dùng |
| POST | `/api/users` | Tạo người dùng mới |
| PUT | `/api/users/{id}` | Cập nhật hồ sơ |
| DELETE | `/api/users/{id}` | Xóa vĩnh viễn |
| POST | `/api/users/{id}/reset-password` | Đặt lại mật khẩu + vô hiệu hóa phiên |

## Cấu trúc frontend mới

| Route | Mô tả |
|---|---|
| `/users` | Trang danh sách người dùng |
| *(modal)* | Tạo mới, chỉnh sửa, xóa, reset mật khẩu — không có route riêng |
