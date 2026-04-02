# Tasks: Quản lý Người dùng

**Input**: Design documents from `specs/001-user-management/`  
**Prerequisites**: plan.md ✅ · spec.md ✅ · research.md ✅ · data-model.md ✅ · contracts/users-api.md ✅ · quickstart.md ✅

**Tests**: Không yêu cầu trong spec — test tasks không được tạo.

**Organization**: Tasks nhóm theo user story để mỗi story có thể implement và test độc lập.

## Format: `[ID] [P?] [Story?] Description — file path`

- **[P]**: Có thể chạy song song (file khác nhau, không có dependency chưa hoàn thành)
- **[Story]**: User story tương ứng (US1–US6)
- Paths dùng `backend/src/` và `frontend/src/` theo cấu trúc trong plan.md

---

## Phase 1: Setup

**Purpose**: Khởi tạo error codes dùng chung cho toàn module Users

- [X] T001 Add `UserErrors` static class with `NotFound`, `UsernameAlreadyExists`, `CannotDeleteSelf` error codes in `backend/src/AutomationPortal.Domain/Errors/UserErrors.cs`

**Checkpoint**: Error codes sẵn sàng cho tất cả handlers

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Shared infrastructure mà TẤT CẢ user stories đều phụ thuộc vào

**⚠️ CRITICAL**: Không có user story nào có thể bắt đầu cho đến khi phase này hoàn thành

- [X] T002 [P] Add `ResetPassword(string newPasswordHash)` method to User entity — calls `RevokeRefreshToken()` internally — in `backend/src/AutomationPortal.Domain/Entities/User.cs`
- [X] T003 [P] Create generic `PagedResponse<T>` sealed record (Items, TotalItems, TotalPages, Page, PageSize) in `backend/src/AutomationPortal.Application/Shared/Dtos/PagedResponse.cs`
- [X] T004 [P] Create reusable `PasswordValidator` FluentValidation rule (MinimumLength 8, Matches `[A-Z]`, Matches `[0-9]`) in `backend/src/AutomationPortal.Application/Shared/RuleValidator/PasswordValidator.cs`
- [X] T005 [P] Create TypeScript types (`UserListItem`, `UserDetail`, `PagedResponse<T>`, `CreateUserRequest`, `UpdateUserRequest`, `ResetPasswordRequest`, `GetUsersParams`) in `frontend/src/api/users/types.ts`

**Checkpoint**: Foundation sẵn sàng — tất cả user stories có thể bắt đầu

---

## Phase 3: User Story 1 — Danh sách và tìm kiếm người dùng (Priority: P1) 🎯 MVP

**Goal**: Người dùng đã đăng nhập xem được danh sách users dạng bảng có tìm kiếm, phân trang, sắp xếp

**Independent Test**: Truy cập `/users` → danh sách hiển thị → gõ từ khóa → kết quả lọc đúng → nhấn tiêu đề cột → sắp xếp đúng → điều hướng trang

### Backend — User Story 1

- [X] T006 [P] [US1] Create `GetUsersQuery` (search, sortColumn, sortDirection, page, pageSize), `GetUsersQueryValidator` (page ≥ 1, pageSize 1–100), and `UserListItemResponse` record DTO in `backend/src/AutomationPortal.Application/Features/Users/GetUsers/`
- [X] T007 [US1] Implement `GetUsersQueryHandler` using Dapper + `ISqlConnectionFactory`: `AllowedSortColumns` whitelist, 2 sequential queries (COUNT + SELECT LIMIT/OFFSET with ILIKE search), returns `PagedResponse<UserListItemResponse>` in `backend/src/AutomationPortal.Application/Features/Users/GetUsers/GetUsersQueryHandler.cs`
- [X] T008 [US1] Implement `GetUsersEndpoint`: `GET /api/users`, `[Authorize]`, `WithName("GetUsers")`, `WithTags("Users")`, maps query params to `GetUsersQuery`, returns 200 with `PagedResponse` or 422 in `backend/src/AutomationPortal.API/Endpoints/Users/GetUsersEndpoint.cs`

### Frontend — User Story 1

- [X] T009 [P] [US1] Implement `getListUser(axios: AxiosInstance, params: GetUsersParams): Promise<PagedResponse<UserListItem>>` in `frontend/src/api/users/getListUser.ts`
- [X] T010 [P] [US1] Create `users-columns.tsx` with `ColumnDef<UserListItem>[]`: columns for FirstName+LastName (combined), Username, Email, Phone, Birthday — column `id` values must be snake_case DB names (`first_name`, `last_name`, `username`, `email`, `phone`, `birthday`) for sort mapping in `frontend/src/app/(dashboard)/users/users-columns.tsx`
- [X] T011 [P] [US1] Create `page.tsx` Server Component as entry point for `/users` route — renders `UsersTableShell` client component in `frontend/src/app/(dashboard)/users/page.tsx`
- [X] T012 [US1] Create `users-table-shell.tsx` Client Component: `useState` for `page`, `search`, `sorting`; `useEffect` to fetch on state change; renders `DataTable` with `manualSorting + manualFiltering`, search `<Input>`, `DataPagination`; empty state message "Không tìm thấy người dùng nào phù hợp" in `frontend/src/app/(dashboard)/users/users-table-shell.tsx`

**Checkpoint**: US1 hoàn toàn có thể test độc lập — danh sách, tìm kiếm, phân trang, sắp xếp hoạt động

---

## Phase 4: User Story 2 — Xem chi tiết người dùng (Priority: P2)

**Goal**: Nhấn vào một user trong danh sách để xem đầy đủ thông tin (không hiển thị password/token)

**Independent Test**: Nhấn vào row trong bảng → dialog chi tiết hiển thị đúng thông tin → nhấn "Đóng" → trở về danh sách với trạng thái tìm kiếm được giữ nguyên (state trong dialog không làm mất list state)

### Backend — User Story 2

- [X] T013 [P] [US2] Create `GetUserByIdQuery` (UserId Guid), `GetUserByIdQueryValidator` (NotEmpty), and `UserDetailResponse` record DTO in `backend/src/AutomationPortal.Application/Features/Users/GetUserById/`
- [X] T014 [US2] Implement `GetUserByIdQueryHandler` using Dapper + `ISqlConnectionFactory`: single-row query by id with `is_deleted = false`, returns `UserDetailResponse` or `Result.Failure(UserErrors.NotFound)` in `backend/src/AutomationPortal.Application/Features/Users/GetUserById/GetUserByIdQueryHandler.cs`
- [X] T015 [US2] Implement `GetUserByIdEndpoint`: `GET /api/users/{id}`, `[Authorize]`, `WithName("GetUserById")`, `WithTags("Users")`, returns 200 with `UserDetailResponse` or 404 in `backend/src/AutomationPortal.API/Endpoints/Users/GetUserByIdEndpoint.cs`

### Frontend — User Story 2

- [X] T016 [P] [US2] Implement `getUserById(axios: AxiosInstance, id: string): Promise<UserDetail>` in `frontend/src/api/users/getUserById.ts`
- [X] T017 [US2] Create `view-user-dialog.tsx`: read-only fields (Họ, Tên, Username, Email, SĐT, Ngày sinh) loaded via `getUserById`, no sensitive fields — triggered by row click in table in `frontend/src/app/(dashboard)/users/view-user-dialog.tsx`
- [X] T018 [US2] Add row-click handler and `viewUserId` state to `users-table-shell.tsx` to open `ViewUserDialog`; closing dialog returns to list with existing `page/search/sorting` state preserved in `frontend/src/app/(dashboard)/users/users-table-shell.tsx`

**Checkpoint**: US1 + US2 hoạt động độc lập

---

## Phase 5: User Story 3 — Tạo người dùng mới (Priority: P2)

**Goal**: Mở form tạo mới → điền thông tin → lưu → user xuất hiện trong danh sách

**Independent Test**: Nhấn "Thêm người dùng" → form mở → điền fields hợp lệ → lưu → user xuất hiện trong danh sách. Nhập username trùng → hiển thị lỗi 409. Bỏ trống required fields → hiển thị validation error.

### Backend — User Story 3

- [X] T019 [P] [US3] Create `CreateUserCommand` (FirstName, LastName, Username, Password, Email?, Phone?, Birthday?), `CreateUserCommandValidator` (uses `PasswordValidator`, `UsernameValidator` pattern for username), and `CreateUserResponse` record (Id, Username) in `backend/src/AutomationPortal.Application/Features/Users/CreateUser/`
- [X] T020 [US3] Implement `CreateUserCommandHandler`: check `await IUserRepository.GetByUsernameAsync(command.Username, ct) != null` → `UserErrors.UsernameAlreadyExists` (409), hash password via `IPasswordHasher`, call `User.Create(...)`, `IUserRepository.Add`, `IUnitOfWork.SaveChangesAsync` in `backend/src/AutomationPortal.Application/Features/Users/CreateUser/CreateUserCommandHandler.cs`
- [X] T021 [US3] Implement `CreateUserEndpoint`: `POST /api/users`, `[Authorize]`, `WithName("CreateUser")`, `WithTags("Users")`, returns 201 Created with `CreateUserResponse`, 409 on duplicate username, 422 on validation error in `backend/src/AutomationPortal.API/Endpoints/Users/CreateUserEndpoint.cs`

### Frontend — User Story 3

- [X] T022 [P] [US3] Implement `createUser(axios: AxiosInstance, data: CreateUserRequest): Promise<CreateUserResponse>` in `frontend/src/api/users/createUser.ts`
- [X] T023 [P] [US3] Create 7 co-located FormField components (`FirstNameField`, `LastNameField`, `UsernameField`, `PasswordField`, `EmailField`, `PhoneField`, `BirthdayField`) each accepting `control: Control<T>` prop in `frontend/src/app/(dashboard)/users/form/` (one file per field)
- [X] T024 [US3] Create `create-user-dialog.tsx`: `react-hook-form` + `zod` schema (required: firstName, lastName, username, password; optional: email, phone, birthday), uses form field components, calls `createUser`, shows `sonner` toast on success/error in `frontend/src/app/(dashboard)/users/create-user-dialog.tsx`
- [X] T025 [US3] Add "Thêm người dùng" `<Button>` and `createDialogOpen` state to `users-table-shell.tsx`, wire `CreateUserDialog`; on success: close dialog, reset to page 1, re-fetch list in `frontend/src/app/(dashboard)/users/users-table-shell.tsx`

**Checkpoint**: US1 + US2 + US3 hoạt động độc lập

---

## Phase 6: User Story 4 — Chỉnh sửa thông tin người dùng (Priority: P2)

**Goal**: Nhấn "Chỉnh sửa" → form pre-filled → thay đổi → lưu → cập nhật hiển thị trong danh sách/chi tiết

**Independent Test**: Chỉnh sửa email/phone → lưu → danh sách/chi tiết cập nhật. Bỏ trống FirstName → validation error. Nhấn Hủy → không có thay đổi.

### Backend — User Story 4

- [X] T026 [P] [US4] Create `UpdateUserCommand` (UserId, FirstName, LastName, Email?, Phone?, Birthday?) and `UpdateUserCommandValidator` (FirstName/LastName NotEmpty) in `backend/src/AutomationPortal.Application/Features/Users/UpdateUser/`
- [X] T027 [US4] Implement `UpdateUserCommandHandler`: load user by id → `UserErrors.NotFound` if null, call `user.UpdateProfile(firstName, lastName, email, phone, birthday)`, `IUnitOfWork.SaveChangesAsync` in `backend/src/AutomationPortal.Application/Features/Users/UpdateUser/UpdateUserCommandHandler.cs`
- [X] T028 [US4] Implement `UpdateUserEndpoint`: `PUT /api/users/{id}`, `[Authorize]`, `WithName("UpdateUser")`, `WithTags("Users")`, returns 204 No Content or 404 in `backend/src/AutomationPortal.API/Endpoints/Users/UpdateUserEndpoint.cs`

### Frontend — User Story 4

- [X] T029 [P] [US4] Implement `updateUser(axios: AxiosInstance, id: string, data: UpdateUserRequest): Promise<void>` in `frontend/src/api/users/updateUser.ts`
- [X] T030 [US4] Create `edit-user-dialog.tsx`: loads user via `getUserById` on open, pre-fills form (Username shown as read-only text), uses existing form field components (no `UsernameField`, no `PasswordField`), calls `updateUser`, shows sonner toast in `frontend/src/app/(dashboard)/users/edit-user-dialog.tsx`
- [X] T031 [US4] Add "Chỉnh sửa" action button to `users-columns.tsx` actions column and `editUserId` state to `users-table-shell.tsx`, wire `EditUserDialog`; on success: close dialog, re-fetch list in `frontend/src/app/(dashboard)/users/users-table-shell.tsx`

**Checkpoint**: US1–US4 hoạt động độc lập; CRUD read + write hoàn chỉnh

---

## Phase 7: User Story 5 — Xóa người dùng (Priority: P3)

**Goal**: Nhấn "Xóa" → AlertDialog xác nhận → xác nhận → user biến mất khỏi danh sách; tự xóa chính mình → bị từ chối

**Independent Test**: Xóa user → không còn trong danh sách. Thử xóa chính mình → thông báo lỗi "Không thể xóa tài khoản đang đăng nhập". Nhấn Hủy → không có thay đổi.

### Backend — User Story 5

- [X] T032 [P] [US5] Create `DeleteUserCommand` (UserId) and `DeleteUserCommandValidator` (NotEmpty) in `backend/src/AutomationPortal.Application/Features/Users/DeleteUser/`
- [X] T033 [US5] Implement `DeleteUserCommandHandler`: inject `IUserContext`, check `command.UserId == userContext.UserId` → `UserErrors.CannotDeleteSelf` (409); load user → `UserErrors.NotFound` if null; `IUserRepository.Remove(user)`, `IUnitOfWork.SaveChangesAsync` (hard delete) in `backend/src/AutomationPortal.Application/Features/Users/DeleteUser/DeleteUserCommandHandler.cs`
- [X] T034 [US5] Implement `DeleteUserEndpoint`: `DELETE /api/users/{id}`, `[Authorize]`, `WithName("DeleteUser")`, `WithTags("Users")`, returns 204 No Content, 404, or 409 in `backend/src/AutomationPortal.API/Endpoints/Users/DeleteUserEndpoint.cs`

### Frontend — User Story 5

- [X] T035 [P] [US5] Implement `deleteUser(axios: AxiosInstance, id: string): Promise<void>` in `frontend/src/api/users/deleteUser.ts`
- [X] T036 [US5] Create `delete-user-dialog.tsx`: `AlertDialog` showing "Xóa người dùng [username]?" with Xác nhận/Hủy buttons, calls `deleteUser`, shows sonner toast on success or error (including 409 CannotDeleteSelf message) in `frontend/src/app/(dashboard)/users/delete-user-dialog.tsx`
- [X] T037 [US5] Add "Xóa" action button to `users-columns.tsx` actions column and `deleteUserId` state to `users-table-shell.tsx`, wire `DeleteUserDialog`; on success: re-fetch list in `frontend/src/app/(dashboard)/users/users-table-shell.tsx`

**Checkpoint**: US1–US5 hoạt động độc lập; full CRUD hoàn chỉnh

---

## Phase 8: User Story 6 — Đặt lại mật khẩu người dùng (Priority: P3)

**Goal**: Nhấn "Đặt lại mật khẩu" → form nhập mật khẩu mới + xác nhận → lưu → tất cả phiên của user đó bị vô hiệu hóa

**Independent Test**: Reset mật khẩu với mật khẩu hợp lệ → 204 No Content → user phải đăng nhập lại bằng mật khẩu mới. Mật khẩu không khớp → validation error.

### Backend — User Story 6

- [X] T038 [P] [US6] Create `ResetUserPasswordCommand` (UserId, NewPassword, ConfirmPassword) and `ResetUserPasswordCommandValidator` (uses `PasswordValidator` for NewPassword, `Equal(x => x.NewPassword)` for ConfirmPassword) in `backend/src/AutomationPortal.Application/Features/Users/ResetUserPassword/`
- [X] T039 [US6] Implement `ResetUserPasswordCommandHandler`: load user → `UserErrors.NotFound` if null; hash `NewPassword` via `IPasswordHasher`; call `user.ResetPassword(hash)` (invalidates refresh token); `IUnitOfWork.SaveChangesAsync` in `backend/src/AutomationPortal.Application/Features/Users/ResetUserPassword/ResetUserPasswordCommandHandler.cs`
- [X] T040 [US6] Implement `ResetUserPasswordEndpoint`: `POST /api/users/{id}/reset-password`, `[Authorize]`, `WithName("ResetUserPassword")`, `WithTags("Users")`, returns 204 No Content, 404, or 422 in `backend/src/AutomationPortal.API/Endpoints/Users/ResetUserPasswordEndpoint.cs`

### Frontend — User Story 6

- [X] T041 [P] [US6] Implement `resetUserPassword(axios: AxiosInstance, id: string, data: ResetPasswordRequest): Promise<void>` in `frontend/src/api/users/resetUserPassword.ts`
- [X] T042 [US6] Create `reset-password-dialog.tsx`: zod schema with `newPassword` (min 8, ≥1 uppercase, ≥1 digit) + `confirmPassword` (`refine` equal check), uses `PasswordField`, calls `resetUserPassword`, shows sonner toast in `frontend/src/app/(dashboard)/users/reset-password-dialog.tsx`
- [X] T043 [US6] Add "Đặt lại mật khẩu" action button to `users-columns.tsx` actions column and `resetPasswordUserId` state to `users-table-shell.tsx`, wire `ResetPasswordDialog` in `frontend/src/app/(dashboard)/users/users-table-shell.tsx`

**Checkpoint**: Tất cả 6 user stories hoạt động độc lập — module quản lý người dùng hoàn chỉnh

---

## Phase 9: Polish & Cross-Cutting Concerns

**Purpose**: Logging, exports, validation cuối

- [X] T044 [P] Add Serilog structured logging to all 6 handlers: `Log.Information` on success (with relevant entity IDs), `Log.Warning` on not-found; follow existing handler logging pattern in each `*Handler.cs` under `backend/src/AutomationPortal.Application/Features/Users/`
- [X] T045 [P] Create `usersApi(axios: AxiosInstance)` factory in `frontend/src/api/users/index.ts` wrapping all 6 functions; export TypeScript types from `types.ts`; register `users: usersApi(axios)` in `frontend/src/api/index.ts`
- [ ] T046 Run end-to-end validation per `quickstart.md`: start backend + frontend, verify all 6 endpoints in Scalar UI, execute all dialog flows (view, create, edit, delete, reset password) on the `/users` page

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — bắt đầu ngay
- **Foundational (Phase 2)**: Depends on Phase 1 — **BLOCKS tất cả user stories**
- **User Stories (Phase 3–8)**: Tất cả phụ thuộc vào Phase 2; sau đó có thể song song theo priority hoặc tuần tự P1 → P2 → P3
- **Polish (Phase 9)**: Depends on tất cả user stories đã chọn hoàn thành

### User Story Dependencies

| Story | Priority | Depends On | Notes |
|---|---|---|---|
| US1 — Danh sách | P1 | Phase 2 | Không phụ thuộc story khác |
| US2 — Chi tiết | P2 | Phase 2 | Tái sử dụng form fields từ US3 nếu US3 được làm trước |
| US3 — Tạo mới | P2 | Phase 2 | Tạo form/ fields dùng lại cho US4 |
| US4 — Chỉnh sửa | P2 | Phase 2 + form fields từ US3 | Reuse form components |
| US5 — Xóa | P3 | Phase 2 | Không phụ thuộc story P2 |
| US6 — Reset MK | P3 | Phase 2 | Sử dụng `ResetPassword()` từ Phase 2 |

### Parallel Opportunities Within Each Story

- `[P]` backend tasks (Query/Command/Validator/Response) chạy song song với `[P]` frontend tasks (API function)
- Handlers chỉ bắt đầu sau khi Query/Command tương ứng hoàn thành
- US3 form field components (T023) hoàn toàn song song — 7 files độc lập nhau

---

## Parallel Example: User Story 1

```bash
# Chạy song song (khác files, không dependency):
T006: GetUsersQuery + Validator + Response DTO  (backend)
T009: getListUser API function                  (frontend)
T010: users-columns.tsx                        (frontend)
T011: page.tsx                                 (frontend)

# Tuần tự sau T006:
T007: GetUsersQueryHandler                     (cần Query đã định nghĩa)
T008: GetUsersEndpoint                         (cần Handler)

# Tuần tự sau T009, T010, T011:
T012: users-table-shell.tsx                    (cần API + columns + page)
```

## Parallel Example: User Story 3

```bash
# Chạy song song:
T019: CreateUserCommand + Validator + Response  (backend)
T022: createUser API function                   (frontend)
T023: 7 form field components                  (frontend — 7 files song song nhau)

# Tuần tự sau T019:
T020: CreateUserCommandHandler

# Tuần tự sau T020:
T021: CreateUserEndpoint

# Tuần tự sau T022, T023:
T024: create-user-dialog.tsx

# Tuần tự sau T021 + T024:
T025: Wire dialog vào users-table-shell.tsx
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Phase 1: Setup (T001)
2. Phase 2: Foundational (T002–T005)
3. Phase 3: User Story 1 (T006–T012)
4. **STOP và VALIDATE**: Test danh sách/tìm kiếm/phân trang/sắp xếp độc lập
5. Demo nếu sẵn sàng

### Incremental Delivery

1. Phase 1 + 2 → Foundation sẵn sàng
2. Phase 3 (US1) → **MVP Demo**: danh sách người dùng hoạt động
3. Phase 4 (US2) → Thêm: xem chi tiết
4. Phase 5 (US3) → Thêm: tạo người dùng
5. Phase 6 (US4) → Thêm: chỉnh sửa
6. Phase 7 (US5) → Thêm: xóa
7. Phase 8 (US6) → Thêm: reset mật khẩu
8. Phase 9: Polish → Sẵn sàng production

### Parallel Team Strategy

Sau khi Phase 1 + 2 hoàn thành:
- **Developer A**: US1 (T006–T012) → US2 (T013–T018)
- **Developer B**: US3 (T019–T025) → US4 (T026–T031)
- **Developer C**: US5 (T032–T037) → US6 (T038–T043)

---

## Notes

- `[P]` = files khác nhau, không có dependency chưa hoàn thành — chạy song song an toàn
- `[USn]` = task thuộc user story cụ thể — dùng để trace traceability
- Mỗi user story có thể complete và test độc lập trước khi chuyển sang story tiếp theo
- Không cần migration database mới — schema `users` đã sẵn có
- Dapper alias (`AS ColumnName`) bắt buộc trong tất cả Dapper queries để map sang C# record properties
- Column whitelist trong `GetUsersQueryHandler` phải được validate trước khi interpolate vào SQL string
- Commit sau mỗi task hoặc nhóm logic nhỏ để dễ rollback nếu cần
