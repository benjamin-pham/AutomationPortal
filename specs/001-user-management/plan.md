# Kế hoạch triển khai: Quản lý Người dùng

**Branch**: `001-user-management` | **Ngày**: 2026-04-02 | **Spec**: [spec.md](spec.md)  
**Input**: Feature spec từ `specs/001-user-management/spec.md`  
**Cập nhật**: Phân trang dùng Raw SQL + Dapper (không dùng EF Core cho read side)

## Tóm tắt

Xây dựng module Quản lý Người dùng đầy đủ CRUD + reset mật khẩu trên entity `User` hiện có. Backend dùng MediatR CQRS trên .NET 10 Clean Architecture; **phân trang/tìm kiếm/sắp xếp dùng Raw SQL + Dapper** (2 query: COUNT + SELECT LIMIT/OFFSET) thay vì EF Core để nhất quán với pattern read-side hiện tại (`GetProfileQueryHandler`). Frontend dùng Next.js 16 App Router với TanStack Table (`manualSorting: true`, `manualFiltering: true` đã sẵn có) và component `DataPagination` hiện có.

## Bối cảnh kỹ thuật

**Ngôn ngữ/Version**: C# / .NET 10 (backend) · TypeScript / Next.js 16 App Router (frontend)  
**Phụ thuộc chính**: MediatR 12 · FluentValidation 12 · Dapper 2.1 · EF Core 10 (write-side) · Serilog · TanStack Table · react-hook-form + zod · Shadcn/Radix UI · Axios · sonner  
**Storage**: PostgreSQL — snake_case Fluent API config, Dapper alias (AS ColumnName)  
**Testing**: xUnit + NSubstitute + FluentAssertions · Testcontainers (PostgreSQL) + Respawn · NetArchTest  
**Target Platform**: Web service (ASP.NET Core Minimal API) + Web app (Next.js SPA)  
**Mục tiêu hiệu năng**: Danh sách 10.000+ users tải < 2 giây (SC-004)  
**Ràng buộc bảo mật**: Column whitelist cho ORDER BY động; search parameterized — không có SQL injection  
**Quy mô**: ≤ 10.000 người dùng, LIMIT/OFFSET đủ dùng (không cần cursor-based)

## Constitution Check

### Nguyên tắc I — Clean Architecture ✅

- Domain entity `User` chứa business logic (`ResetPassword`, `UpdateProfile`, `RevokeRefreshToken`).
- Application layer: mỗi operation là một Command/Query + Handler riêng trong `Features/Users/{Operation}/`.
- FluentValidation co-located với Command/Query; `ValidationBehavior` tự chạy.
- `IDateTimeProvider` thay `DateTime.UtcNow`.
- Dependencies chỉ chảy vào trong: Handler → ISqlConnectionFactory (Application abstraction).

### Nguyên tắc II — Frontend Best Practices ✅

- API calls: client component → `axiosClientInstance` (localStorage token).
- `src/api/users/` đã tồn tại, bổ sung các hàm còn thiếu.
- Page component co-located với `page.tsx`.
- Mỗi `FormField render` extract ra file riêng trong `form/` subdirectory.

### Nguyên tắc III — Code Quality & Simplicity ✅

- Không thêm repository method cho read side; Dapper handler trực tiếp là đủ.
- `PagedResponse<T>` là shared DTO hợp lý (sẽ dùng lại), không phải over-engineering.
- Column whitelist inline trong handler (không cần abstraction riêng).

### Nguyên tắc IV — Testing Discipline ✅

- `GetUsersQueryHandler` test bằng Application.UnitTests (mock `ISqlConnectionFactory`).
- API.IntegrationTests kiểm thử end-to-end với Testcontainers PostgreSQL.
- Không dùng in-memory EF cho integration tests.

### Nguyên tắc V — Observability ✅

- Mỗi handler log ít nhất 1 entry (Information khi thành công, Warning khi không tìm thấy).
- Endpoint đặt tên `WithName()` và có tag `"Users"`.

**Kết luận**: Không có vi phạm. Không cần Complexity Tracking.

## Cấu trúc dự án

### Tài liệu (feature này)

```text
specs/001-user-management/
├── plan.md              # File này
├── research.md          # Phase 0 — chiến lược Dapper, whitelist, response shape
├── data-model.md        # Phase 1 — entities, DTOs, TypeScript types
├── quickstart.md        # Phase 1 — hướng dẫn khởi động
├── contracts/
│   └── users-api.md     # Phase 1 — HTTP API contracts
└── tasks.md             # Phase 2 (output của /speckit.tasks — chưa tạo)
```

### Source Code (backend)

```text
backend/src/
├── AutomationPortal.Domain/
│   └── Entities/
│       └── User.cs                          # Thêm method ResetPassword()
│
├── AutomationPortal.Application/
│   ├── Shared/Dtos/
│   │   └── PagedResponse.cs                 # NEW — generic paged wrapper
│   ├── Shared/RuleValidator/
│   │   └── PasswordValidator.cs             # NEW — tái sử dụng cho Create + Reset
│   └── Features/Users/
│       ├── GetUsers/
│       │   ├── GetUsersQuery.cs             # NEW
│       │   ├── GetUsersQueryHandler.cs      # NEW — Dapper, 2 queries
│       │   ├── GetUsersQueryValidator.cs    # NEW
│       │   └── UserListItemResponse.cs      # NEW — record DTO
│       ├── GetUserById/
│       │   ├── GetUserByIdQuery.cs          # NEW
│       │   ├── GetUserByIdQueryHandler.cs   # NEW — Dapper
│       │   ├── GetUserByIdQueryValidator.cs # NEW
│       │   └── UserDetailResponse.cs        # NEW — record DTO
│       ├── CreateUser/
│       │   ├── CreateUserCommand.cs         # NEW
│       │   ├── CreateUserCommandHandler.cs  # NEW — IUserRepository + IUnitOfWork
│       │   ├── CreateUserCommandValidator.cs # NEW
│       │   └── CreateUserResponse.cs        # NEW — record DTO
│       ├── UpdateUser/
│       │   ├── UpdateUserCommand.cs         # NEW
│       │   ├── UpdateUserCommandHandler.cs  # NEW
│       │   └── UpdateUserCommandValidator.cs # NEW
│       ├── DeleteUser/
│       │   ├── DeleteUserCommand.cs         # NEW
│       │   ├── DeleteUserCommandHandler.cs  # NEW — kiểm tra self-delete
│       │   └── DeleteUserCommandValidator.cs # NEW
│       └── ResetUserPassword/
│           ├── ResetUserPasswordCommand.cs  # NEW
│           ├── ResetUserPasswordCommandHandler.cs # NEW
│           └── ResetUserPasswordCommandValidator.cs # NEW
│
├── AutomationPortal.Infrastructure/
│   └── Repositories/
│       └── UserRepository.cs               # Không thay đổi cho phân trang
│
└── AutomationPortal.API/
    └── Endpoints/
        └── Users/
            ├── GetUsersEndpoint.cs          # NEW
            ├── GetUserByIdEndpoint.cs       # NEW
            ├── CreateUserEndpoint.cs        # NEW
            ├── UpdateUserEndpoint.cs        # NEW
            ├── DeleteUserEndpoint.cs        # NEW
            └── ResetUserPasswordEndpoint.cs # NEW
```

### Source Code (frontend)

```text
frontend/src/
├── api/
│   └── users/
│       ├── getListUser.ts       # Implement (hiện rỗng) — axiosClientInstance
│       ├── getUserById.ts       # Implement (hiện rỗng)
│       ├── createUser.ts        # Implement (hiện rỗng)
│       ├── updateUser.ts        # Implement (hiện rỗng)
│       ├── deleteUser.ts        # NEW
│       ├── resetUserPassword.ts # NEW
│       └── index.ts             # Bổ sung exports
│
└── app/(dashboard)/
    └── users/
        ├── page.tsx                          # NEW — Server Component (entry)
        ├── users-table-shell.tsx             # NEW — Client Component quản lý state
        ├── users-columns.tsx                 # NEW — ColumnDef[] cho DataTable
        ├── create-user-dialog.tsx            # NEW — Dialog tạo mới
        ├── delete-user-dialog.tsx            # NEW — AlertDialog xác nhận xóa
        ├── reset-password-dialog.tsx         # NEW — Dialog reset mật khẩu
        ├── edit-user-dialog.tsx              # NEW — Dialog chỉnh sửa
        └── form/
            ├── first-name-field.tsx          # NEW — FormField extract
            ├── last-name-field.tsx           # NEW
            ├── username-field.tsx            # NEW
            ├── password-field.tsx            # NEW
            ├── email-field.tsx               # NEW
            ├── phone-field.tsx               # NEW
            └── birthday-field.tsx            # NEW
```

**Quyết định cấu trúc**: Web application (Option 2) với backend/frontend tách biệt. Frontend dùng client-side state management (`useState`) cho page/search/sorting — URL searchParams không áp dụng (YAGNI, spec không yêu cầu shareable URL).

## Thiết kế chi tiết — GetUsersQueryHandler (Dapper)

```csharp
// Hai truy vấn trong cùng một connection:
internal sealed class GetUsersQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetUsersQuery, PagedResponse<UserListItemResponse>>
{
    private static readonly HashSet<string> AllowedSortColumns =
    [
        "first_name", "last_name", "username", "email", "phone", "birthday"
    ];

    public async Task<Result<PagedResponse<UserListItemResponse>>> Handle(
        GetUsersQuery request, CancellationToken cancellationToken)
    {
        var safeColumn    = AllowedSortColumns.Contains(request.SortColumn ?? "")
                            ? request.SortColumn!
                            : "username";
        var safeDirection = request.SortDirection?.ToUpperInvariant() == "DESC" ? "DESC" : "ASC";
        var offset        = (request.Page - 1) * request.PageSize;
        var searchParam   = string.IsNullOrWhiteSpace(request.Search) ? null : request.Search;

        using var connection = sqlConnectionFactory.CreateConnection();

        const string countSql = """
            SELECT COUNT(*)
            FROM users
            WHERE is_deleted = false
              AND (@Search IS NULL OR
                   first_name ILIKE '%' || @Search || '%' OR
                   last_name  ILIKE '%' || @Search || '%' OR
                   username   ILIKE '%' || @Search || '%' OR
                   email      ILIKE '%' || @Search || '%')
            """;

        var totalItems = await connection.ExecuteScalarAsync<int>(
            countSql, new { Search = searchParam });

        var dataSql = $"""
            SELECT
                id         AS UserId,
                first_name AS FirstName,
                last_name  AS LastName,
                username   AS Username,
                email      AS Email,
                phone      AS Phone,
                birthday   AS Birthday
            FROM users
            WHERE is_deleted = false
              AND (@Search IS NULL OR
                   first_name ILIKE '%' || @Search || '%' OR
                   last_name  ILIKE '%' || @Search || '%' OR
                   username   ILIKE '%' || @Search || '%' OR
                   email      ILIKE '%' || @Search || '%')
            ORDER BY {safeColumn} {safeDirection}
            LIMIT @PageSize OFFSET @Offset
            """;

        var items = await connection.QueryAsync<UserListItemResponse>(
            dataSql, new { Search = searchParam, PageSize = request.PageSize, Offset = offset });

        var totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

        return new PagedResponse<UserListItemResponse>(
            items.ToList(), totalItems, totalPages, request.Page, request.PageSize);
    }
}
```

> **Bảo mật**: `safeColumn` và `safeDirection` được nội suy vào SQL string **sau khi** đã validate qua whitelist/so sánh cố định. Tất cả giá trị user input (`@Search`, `@PageSize`, `@Offset`) đều được parameterize. Không có SQL injection.

## Thiết kế chi tiết — Frontend State Management

```typescript
// users-table-shell.tsx — Client Component
"use client"

const [page, setPage] = useState(1);
const [search, setSearch] = useState("");
const [sorting, setSorting] = useState<DataSorting>({ column: "username", direction: "asc" });
const [isLoading, setIsLoading] = useState(false);
const [data, setData] = useState<PagedResponse<UserListItem> | null>(null);

// Fetch khi page/search/sorting thay đổi
useEffect(() => {
  fetchUsers({ page, pageSize: 20, search, sortColumn: sorting.column, sortDirection: sorting.direction });
}, [page, search, sorting]);

// DataTable nhận onSortChange → setSorting → reset page về 1
// DataPagination nhận data.totalPages + data.totalItems, onPageChange → setPage
```

**Mapping DataSorting → API**: Tên cột từ TanStack Table (`id` của ColumnDef) phải đặt bằng snake_case DB column để pass thẳng vào `sortColumn` query param mà không cần mapping.

## Phụ thuộc

- `Dapper` 2.1.72 — **đã có** trong `Directory.Packages.props`
- `ISqlConnectionFactory` — **đã có** trong Application layer
- `SqlConnectionFactory` (Npgsql) — **đã có** trong Infrastructure
- `DataTable`, `DataPagination`, `DataTableColumnHeader` — **đã có** trong frontend components
- `axiosClientInstance` — **đã có**, dùng cho Users page (client component)

## Không thay đổi

- Schema database / Migrations — không cần migration mới
- `IUserRepository` interface — không thêm method cho read side
- `AppDbContext` — không thêm configuration
- `DataTable` / `DataPagination` components — dùng nguyên, không sửa
