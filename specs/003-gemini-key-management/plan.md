# Kế hoạch Triển khai: Quản lý Gemini Key

**Nhánh**: `003-gemini-key-management` | **Ngày**: 2026-04-03 | **Spec**: [spec.md](spec.md)  
**Đầu vào**: Đặc tả tính năng từ `/specs/003-gemini-key-management/spec.md`

## Tóm tắt

Xây dựng tính năng CRUD quản lý Gemini Key, trong đó mỗi người dùng chỉ được gắn tối đa một key (quan hệ một-một). Backend theo chuẩn Clean Architecture với MediatR CQRS; frontend theo chuẩn Next.js 16 App Router. Key được lưu nguyên văn trong DB và chỉ hiển thị dạng che một phần (`****xyz`). Thao tác xóa yêu cầu xác nhận bằng cách gõ lại tên key (so sánh case-insensitive).

## Bối cảnh Kỹ thuật

**Ngôn ngữ/Phiên bản**: C# / .NET 10 (backend) · TypeScript / Node 20 (frontend)  
**Phụ thuộc chính**:
- Backend: ASP.NET Core Minimal API · MediatR 12 · FluentValidation 12 · EF Core 10 + Dapper · PostgreSQL · Serilog · Scalar
- Frontend: Next.js 16 App Router · Tailwind CSS v4 · Shadcn/Radix UI · react-hook-form + zod · TanStack Table · Axios (server + client instances)

**Lưu trữ**: PostgreSQL — bảng `gemini_keys`, snake_case, EF Core Fluent API  
**Kiểm thử**: xUnit · FluentAssertions · NSubstitute · Testcontainers (PostgreSQL) · Respawn · NetArchTest.Rules  
**Nền tảng đích**: Web (desktop) — Linux server (backend) + Vercel/static (frontend)  
**Loại dự án**: Web application (backend API + frontend SPA)  
**Mục tiêu hiệu năng**: Thao tác tạo/cập nhật/xóa key trong < 60 giây  
**Ràng buộc**: Giá trị key KHÔNG được lộ dưới dạng văn bản thô trong bất kỳ response nào; tên key phải duy nhất toàn hệ thống; mỗi user tối đa một key  
**Quy mô**: Dự án nội bộ — lượng user nhỏ, không yêu cầu tối ưu mobile

## Kiểm tra Hiến pháp

*GATE: Phải vượt qua trước Phase 0 nghiên cứu. Kiểm tra lại sau Phase 1 thiết kế.*

| # | Nguyên tắc | Trạng thái | Ghi chú |
|---|-----------|-----------|---------|
| I | Clean Architecture — 4 lớp, phụ thuộc hướng vào trong | ✅ PASS | Theo đúng Domain → Application → Infrastructure → API |
| I | Rich Domain Model — business logic trong entity | ✅ PASS | `GeminiKey.Create()` / `GeminiKey.Update()` mang logic |
| I | MediatR CQRS — một Command/Query + Handler mỗi operation | ✅ PASS | 4 operations → 4 Command/Query riêng biệt |
| I | `Result<T>` cho lỗi domain — không throw exception | ✅ PASS | Mọi lỗi trả về `Result.Failure(error)` |
| I | FluentValidation co-located, chạy qua `ValidationBehavior` | ✅ PASS | Validator đặt cùng thư mục command |
| I | `IDateTimeProvider` thay `DateTime.UtcNow` | ✅ PASS | Inject qua constructor theo pattern hiện có |
| I | `IOptions<T>` cho config | ✅ PASS | Không có config mới cần thêm trong tính năng này |
| I | Centralized package versioning qua `Directory.Packages.props` | ✅ PASS | Không thêm package mới |
| I | Nullable reference types | ✅ PASS | Enabled project-wide |
| II | Next.js 16 async APIs `await`ed | ✅ PASS | Tuân thủ pattern trang `users` hiện có |
| II | Axios DI pattern — `axiosServerInstance` / `axiosClientInstance` | ✅ PASS | Thêm `src/api/gemini-keys/` theo pattern `users` |
| II | FormField mỗi trường một file riêng trong `form/` | ✅ PASS | 3 trường form → 3 file component |
| II | UI primitives từ `src/components/ui/` | ✅ PASS | Tái sử dụng Dialog, Input, Button, Table, Badge hiện có |
| III | Async end-to-end — không `.Result`, `.Wait()` | ✅ PASS | |
| III | Record types cho DTOs | ✅ PASS | Tất cả response DTOs dùng `record` |
| III | YAGNI — không abstraction suy đoán | ✅ PASS | Không thêm repository pattern mới ngoài yêu cầu |
| IV | Tests đúng scope — integration test dùng PostgreSQL thực | ✅ PASS | Testcontainers cho infra/API integration tests |
| V | Structured logging trong mọi handler | ✅ PASS | `ILogger<T>` + Serilog properties `{Property}` / `{@Property}` |
| V | Scalar API docs — `WithName()` trên mọi endpoint | ✅ PASS | |

**Kết quả GATE: PASS** — Không có vi phạm, không cần bảng Complexity Tracking.

## Cấu trúc Dự án

### Tài liệu (tính năng này)

```text
specs/003-gemini-key-management/
├── plan.md              # File này (/speckit.plan)
├── research.md          # Phase 0 (/speckit.plan)
├── data-model.md        # Phase 1 (/speckit.plan)
├── quickstart.md        # Phase 1 (/speckit.plan)
├── contracts/           # Phase 1 (/speckit.plan)
│   └── gemini-keys-api.md
└── tasks.md             # Phase 2 (/speckit.tasks — chưa tạo)
```

### Source Code (gốc repo)

```text
backend/
├── src/
│   ├── AutomationPortal.Domain/
│   │   ├── Entities/
│   │   │   └── GeminiKey.cs                          # Entity mới
│   │   ├── Errors/
│   │   │   └── GeminiKeyErrors.cs                    # Lỗi domain mới
│   │   └── Repositories/
│   │       └── IGeminiKeyRepository.cs               # Interface mới
│   ├── AutomationPortal.Application/
│   │   └── Features/
│   │       └── GeminiKeys/
│   │           ├── GetGeminiKeys/                    # Query + Handler + Response
│   │           ├── CreateGeminiKey/                  # Command + Handler + Validator + Response
│   │           ├── UpdateGeminiKey/                  # Command + Handler + Validator
│   │           └── DeleteGeminiKey/                  # Command + Handler + Validator
│   ├── AutomationPortal.Infrastructure/
│   │   ├── Data/
│   │   │   └── Configurations/
│   │   │       └── GeminiKeyConfiguration.cs         # EF Core Fluent API
│   │   └── Repositories/
│   │       └── GeminiKeyRepository.cs                # Implementation
│   └── AutomationPortal.API/
│       └── Endpoints/
│           └── GeminiKeys/
│               ├── GetGeminiKeysEndpoint.cs
│               ├── CreateGeminiKeyEndpoint.cs
│               ├── UpdateGeminiKeyEndpoint.cs
│               └── DeleteGeminiKeyEndpoint.cs
└── tests/
    ├── AutomationPortal.Domain.UnitTests/
    │   └── GeminiKeys/
    │       └── GeminiKeyTests.cs
    ├── AutomationPortal.Application.UnitTests/
    │   └── GeminiKeys/
    │       ├── CreateGeminiKeyCommandHandlerTests.cs
    │       ├── UpdateGeminiKeyCommandHandlerTests.cs
    │       └── DeleteGeminiKeyCommandHandlerTests.cs
    ├── AutomationPortal.Infrastructure.IntegrationTests/
    │   └── GeminiKeys/
    │       └── GeminiKeyRepositoryTests.cs
    └── AutomationPortal.API.IntegrationTests/
        └── GeminiKeys/
            └── GeminiKeysEndpointTests.cs

frontend/
└── src/
    ├── api/
    │   ├── gemini-keys/
    │   │   ├── getGeminiKeys.ts
    │   │   ├── createGeminiKey.ts
    │   │   ├── updateGeminiKey.ts
    │   │   ├── deleteGeminiKey.ts
    │   │   ├── types.ts
    │   │   └── index.ts
    │   └── index.ts                                   # Đăng ký geminiKeysApi
    └── app/
        └── (dashboard)/
            └── gemini-keys/
                ├── page.tsx
                ├── gemini-keys-table-shell.tsx
                ├── gemini-keys-columns.tsx
                ├── create-gemini-key-dialog.tsx
                ├── edit-gemini-key-dialog.tsx
                ├── delete-gemini-key-dialog.tsx
                └── form/
                    ├── name-field.tsx
                    ├── key-value-field.tsx
                    └── user-select-field.tsx
```

**Quyết định cấu trúc**: Web application (Option 2) — backend API + frontend Next.js. Theo đúng cấu trúc hiện có của dự án (`users` feature) để đảm bảo nhất quán.
