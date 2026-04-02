# Research: Quản lý Người dùng

**Branch**: `001-user-management` | **Ngày**: 2026-04-02

## 1. Phân trang server-side với Raw SQL + Dapper (ĐÃ CẬP NHẬT)

**Quyết định**: Sử dụng Dapper + raw SQL (`LIMIT`/`OFFSET`) thay vì EF Core `Skip`/`Take`. Handler `GetUsersQueryHandler` inject `ISqlConnectionFactory`, chạy **hai truy vấn** tuần tự: `COUNT(*)` và `SELECT ... LIMIT/OFFSET`. Trả về `PagedResponse<T>` gồm `Items`, `TotalItems`, `TotalPages`, `Page`, `PageSize`.

**Lý do chuyển sang Dapper**:
- Nhất quán với pattern read-side hiện có của project: `GetProfileQueryHandler` đã dùng Dapper + `ISqlConnectionFactory`.
- Read-only query không cần EF Core change tracking; Dapper nhẹ hơn và query rõ ràng hơn.
- Kiểm soát SQL hoàn toàn: dễ thêm index hint, explain analyze khi cần tối ưu sau.
- `ISqlConnectionFactory` và `SqlConnectionFactory` (Npgsql) đã sẵn có, Dapper đã có trong `Directory.Packages.props`.

**Chiến lược hai truy vấn** (thay vì `COUNT(*) OVER()`):
```sql
-- Query 1: đếm tổng
SELECT COUNT(*)
FROM users
WHERE is_deleted = false
  AND (@Search IS NULL OR
       first_name ILIKE '%' || @Search || '%' OR
       last_name  ILIKE '%' || @Search || '%' OR
       username   ILIKE '%' || @Search || '%' OR
       email      ILIKE '%' || @Search || '%');

-- Query 2: lấy dữ liệu phân trang
SELECT
    id          AS UserId,
    first_name  AS FirstName,
    last_name   AS LastName,
    username    AS Username,
    email       AS Email,
    phone       AS Phone,
    birthday    AS Birthday
FROM users
WHERE is_deleted = false
  AND (@Search IS NULL OR
       first_name ILIKE '%' || @Search || '%' OR
       last_name  ILIKE '%' || @Search || '%' OR
       username   ILIKE '%' || @Search || '%' OR
       email      ILIKE '%' || @Search || '%')
ORDER BY {safe_column} {ASC|DESC}
LIMIT @PageSize OFFSET @Offset;
```

**Phương án thay thế đã xét**:
- EF Core Skip/Take: bị loại — không nhất quán với read-side pattern hiện tại.
- `COUNT(*) OVER()` window function trong một query: bị loại — tính tổng cho từng hàng, overhead lớn hơn khi dataset lớn.
- Cursor-based pagination: bị loại — phức tạp không cần thiết ở quy mô ≤ 10k bản ghi.

---

## 2. Tìm kiếm và sắp xếp server-side (ĐÃ CẬP NHẬT)

**Quyết định**: Query nhận `string? search`, `string? sortColumn`, `string? sortDirection`. Handler áp dụng parameterized `ILIKE` cho search và **column whitelist** cho sort.

**Lý do**: Tìm kiếm case-insensitive với PostgreSQL `ILIKE`. Sắp xếp động qua tên cột DB (snake_case) — whitelist ngăn SQL injection vì Dapper không parameterize tên cột.

**Các trường tìm kiếm**: `first_name`, `last_name`, `username`, `email` — `ILIKE '%@Search%'`.

**Column whitelist cho sort**:
```csharp
private static readonly HashSet<string> AllowedSortColumns =
[
    "first_name", "last_name", "username", "email", "phone", "birthday"
];
var safeColumn    = AllowedSortColumns.Contains(sortColumn) ? sortColumn : "username";
var safeDirection = sortDirection?.ToUpperInvariant() == "DESC" ? "DESC" : "ASC";
```

**Phương án thay thế đã xét**:
- Full-text search PostgreSQL (`tsvector`): quá mức cần thiết cho substring search đơn giản.
- Dictionary mapping UI → DB column: thêm indirection không cần thiết khi tên đã nhất quán (camelCase → snake_case mapping do Dapper alias xử lý).

---

## 3. Reset mật khẩu và vô hiệu hóa phiên

**Quyết định**: `ResetUserPassword` command: hash mật khẩu mới → gọi `User.ResetPassword(newPasswordHash)` (method mới trên entity) → `User.RevokeRefreshToken()` → lưu → các phiên refresh token bị vô hiệu hóa ngay lập tức.

**Lý do**: `RevokeRefreshToken()` đã có sẵn trên entity. Vô hiệu hóa phiên theo cơ chế: refresh token bị xóa → mọi request refresh token mới sẽ thất bại → access token hiện tại hết hiệu lực sau TTL (thường 15 phút). Đây là cách đơn giản nhất phù hợp với hệ thống JWT hiện tại mà không cần token blacklist.

**Phương án thay thế đã xét**: Lưu `SecurityStamp` + blocklist — phức tạp, không cần thiết ở quy mô này.

---

## 4. Ngăn người dùng tự xóa tài khoản (FR-009)

**Quyết định**: Handler `DeleteUser` inject `IUserContext`, so sánh `command.UserId == userContext.UserId` → trả về `Result.Failure(UserErrors.CannotDeleteSelf)` nếu trùng.

**Lý do**: `IUserContext.UserId` đã được inject từ JWT claim, không cần query thêm.

---

## 5. Chính sách mật khẩu (FR-005a)

**Quyết định**: Dùng FluentValidation rule tái sử dụng được — tạo `PasswordValidator` tương tự `UsernameValidator`. Rule: `MinimumLength(8)`, `Matches(@"[A-Z]")`, `Matches(@"[0-9]")`.

**Lý do**: Tái sử dụng pattern `UsernameValidator` hiện có. Co-locate validator với Command tương ứng, nhưng các rule password có thể đặt trong `Shared/RuleValidator/PasswordValidator.cs` để dùng lại ở cả `CreateUser` và `ResetUserPassword`.

---

## 6. Frontend — quản lý trạng thái tìm kiếm khi quay lại (User Story 2 AC-3)

**Quyết định**: Lưu `search`, `page`, `sortBy`, `sortDesc` vào `searchParams` của URL. Server Component đọc `searchParams`, truyền xuống. Khi bấm "Quay lại" từ trang chi tiết → browser back → `searchParams` được giữ nguyên trong URL history.

**Lý do**: Không cần state management phức tạp (Redux/Zustand). URL là nguồn sự thật duy nhất cho bộ lọc, có thể bookmark và chia sẻ.

---

## 7. Username không thể thay đổi sau khi tạo

**Quyết định**: `UpdateUser` command chỉ nhận `FirstName`, `LastName`, `Email`, `Phone`, `Birthday` — không có trường `Username`. Validator của `UpdateUser` không bao gồm username. Frontend form chỉnh sửa hiển thị username dưới dạng read-only.

**Lý do**: Assumption đã xác nhận trong spec. Đơn giản hóa implementation, không cần kiểm tra uniqueness khi update.

---

## 8. Kết luận — không có NEEDS CLARIFICATION còn lại

Tất cả unknowns đã được giải quyết từ spec đã clarified và codebase hiện có. Không cần nghiên cứu thêm.
