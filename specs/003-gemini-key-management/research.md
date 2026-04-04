# Nghiên cứu: Quản lý Gemini Key

**Tính năng**: `003-gemini-key-management`  
**Ngày**: 2026-04-03  
**Trạng thái**: Hoàn thành — mọi NEEDS CLARIFICATION đã giải quyết

---

## 1. Lưu trữ giá trị API Key

**Quyết định**: Lưu giá trị Gemini Key dưới dạng **plaintext** trong cột `key_value` (VARCHAR).

**Lý do**:
- Key của người dùng là credential của họ với Google Gemini API — ứng dụng này là consumer, không phải issuer. Để có thể tái sử dụng key (gọi API Gemini sau này), hệ thống phải có khả năng lấy lại giá trị gốc → không thể hash một chiều.
- Mã hóa symmetric (AES-256) tại application layer là phương án tốt hơn cho production, nhưng vượt quá scope MVP này. DB-level encryption (PostgreSQL `pgcrypto` hoặc Transparent Data Encryption) xử lý ở tầng infrastructure, không cần thay đổi application code.
- Pattern hiện tại của dự án (password được hash bằng `IPasswordHasher`) không áp dụng được ở đây vì cần khôi phục giá trị thực.

**Thay thế đã xem xét**:
- AES-256 symmetric encryption tại application layer: bảo mật hơn nhưng cần quản lý master key và `IEncryptionService` mới — quá phức tạp cho MVP.
- Lưu hash: không thể phục hồi giá trị gốc → không khả thi.

**Che giá trị trong UI**: Hiển thị dạng `****{last4}` — lấy 4 ký tự cuối của `key_value`, che phần còn lại bằng `****`. Logic này thực hiện ở **tầng Application** khi map sang DTO (không bao giờ trả về `key_value` đầy đủ trong response).

---

## 2. Quan hệ một-một User — GeminiKey

**Quyết định**: `GeminiKey` giữ `UserId` (FK → `users.id`) với **unique index** trên cột `user_id` (filter `is_deleted = false`).

**Lý do**:
- Quan hệ owner nằm ở `GeminiKey` (side "có FK") phù hợp với ngữ nghĩa: key gắn với user, không phải ngược lại.
- `User` entity hiện có KHÔNG thêm navigation property sang `GeminiKey` — tránh coupling và vi phạm Clean Architecture (Domain layer chỉ biết `GeminiKey` depend on `User`, không cần User biết GeminiKey).
- Unique index đảm bảo ràng buộc một-một ở tầng DB — defense-in-depth bên cạnh validation ở Application layer.

**Thay thế đã xem xét**:
- Thêm `GeminiKeyId?` vào `User`: tạo circular dependency — bác bỏ.
- Join table: overkill cho quan hệ 1-1 — bác bỏ.

---

## 3. Xử lý "replace key cũ khi user đã có key"

**Quyết định**: Khi tạo/cập nhật GeminiKey mà `UserId` đã có key khác → trả về `Result.Failure(GeminiKeyErrors.UserAlreadyHasKey)` từ handler nếu không có confirmation; frontend hiển thị cảnh báo rồi gọi lại với flag `replaceExisting = true`.

**Lý do**:
- Cách đơn giản nhất: dùng một trường boolean trong command để biểu thị người dùng đã xác nhận thay thế.
- Handler kiểm tra: nếu user đã có key và `replaceExisting = false` → trả lỗi có HTTP 409. Frontend nhận 409 → hiển thị cảnh báo → người dùng xác nhận → gọi lại với `replaceExisting = true` → handler xóa key cũ và tạo key mới.
- Tránh cần thêm endpoint riêng cho "replace".

**Thay thế đã xem xét**:
- Upsert bằng `ON CONFLICT DO UPDATE` ở SQL: bypass domain logic và business rule — bác bỏ.
- Endpoint riêng `/api/gemini-keys/replace`: thêm surface API không cần thiết — bác bỏ.

---

## 4. Phân trang danh sách Gemini Key

**Quyết định**: Sử dụng **Dapper** (server-side pagination) với `PagedList<T>` — theo đúng pattern `GetUsersQueryHandler`.

**Lý do**:
- `PagedList<T>` đã có sẵn trong `Application.Shared`; TanStack Table server-side đã có sẵn trên frontend.
- Page size mặc định 20 — phù hợp với FR-002.
- Không cần tìm kiếm/lọc theo clarification của người dùng.

---

## 5. Xóa key — so sánh tên case-insensitive

**Quyết định**: Logic so sánh nằm hoàn toàn ở **frontend** — JavaScript `trim()` + `toLowerCase()` cả hai phía trước khi so sánh. Backend không cần xác thực lại chuỗi xác nhận (backend chỉ nhận `DELETE /api/gemini-keys/{id}`).

**Lý do**:
- Backend đã authorize request — việc ép người dùng gõ lại tên là UX guard, không phải security control. Nếu attacker có token hợp lệ, họ có thể gọi DELETE trực tiếp mà không cần qua UI.
- Đơn giản, không cần thêm trường `confirmationName` vào DELETE request.

---

## 6. Hiển thị danh sách — cột "Key (ẩn một phần)"

**Quyết định**: Column hiển thị `masked_key` — được tính tại Application layer khi query, trả về trong DTO. Frontend không cần xử lý che ký tự.

**Lý do**: Giữ logic che key tập trung tại một nơi — Application layer. Frontend chỉ hiển thị chuỗi đã nhận.

**Format**: Nếu `key_value` có ≥ 4 ký tự → `"****" + key_value[^4..]`. Nếu < 4 ký tự → `"****"`.

---

## 7. Navigation trong frontend

**Quyết định**: Thêm mục "Gemini Keys" vào sidebar của `(dashboard)/layout.tsx` dẫn tới `/gemini-keys`.

**Lý do**: Nhất quán với cách `users` đã được thêm vào dashboard navigation.
