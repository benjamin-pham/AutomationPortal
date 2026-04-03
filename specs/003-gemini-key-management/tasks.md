# Tasks: Gemini Key Management

**Input**: Design documents from `/specs/003-gemini-key-management/`
**Feature Branch**: `003-gemini-key-management`
**Date**: 2026-04-03

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.
**Tests**: Included — test file paths are explicitly defined in plan.md and required by the architecture constitution.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no unmet dependencies)
- **[Story]**: Which user story this task belongs to (US1–US4)
- Exact file paths included in every task description

---

## Phase 1: Setup

**Purpose**: Confirm baseline before adding new feature code

- [X] T001 Confirm solution builds and all existing tests pass on branch `003-gemini-key-management`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Domain entity, infrastructure, and frontend API scaffolding that ALL user stories depend on

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

### Domain Layer

- [X] T002 [P] Create `GeminiKey` entity with `Create()` factory and `Update()` mutation methods (validate name/keyValue not empty, max lengths, trim keyValue, userId not empty) in `backend/src/AutomationPortal.Domain/Entities/GeminiKey.cs`
- [X] T003 [P] Create `GeminiKeyErrors` with `NotFound`, `NameAlreadyExists`, and `UserAlreadyHasKey` static error codes in `backend/src/AutomationPortal.Domain/Errors/GeminiKeyErrors.cs`
- [X] T004 [P] Create `IGeminiKeyRepository` interface extending `IRepository<GeminiKey>` with `GetByNameAsync` and `GetByUserIdAsync` methods in `backend/src/AutomationPortal.Domain/Repositories/IGeminiKeyRepository.cs`

### Domain Unit Tests

- [X] T005 [P] Add `GeminiKey` entity unit tests covering `Create()` and `Update()` validation rules (empty name, empty keyValue, max length, trim whitespace, empty userId) in `backend/tests/AutomationPortal.Domain.UnitTests/GeminiKeys/GeminiKeyTests.cs`

### Infrastructure Layer

- [X] T006 Create `GeminiKeyConfiguration` extending `BaseEntityConfiguration<GeminiKey>` (table `gemini_keys`, column mapping, unique filtered indexes `ix_gemini_keys_name` and `ix_gemini_keys_user_id` with `is_deleted = false` filter) in `backend/src/AutomationPortal.Infrastructure/Data/Configurations/GeminiKeyConfiguration.cs`
- [X] T007 Add `DbSet<GeminiKey> GeminiKeys` property and apply `GeminiKeyConfiguration` in `backend/src/AutomationPortal.Infrastructure/Data/AppDbContext.cs`
- [X] T008 Implement `GeminiKeyRepository` extending base repository with `GetByNameAsync` and `GetByUserIdAsync` using EF Core in `backend/src/AutomationPortal.Infrastructure/Repositories/GeminiKeyRepository.cs`
- [X] T009 Register `IGeminiKeyRepository → GeminiKeyRepository` scoped dependency in `backend/src/AutomationPortal.Infrastructure/DependencyInjection.cs`
- [X] T010 Add EF Core migration `AddGeminiKeys` via `dotnet ef migrations add AddGeminiKeys --project src/AutomationPortal.Infrastructure --startup-project src/AutomationPortal.API` and update database

### Infrastructure Integration Tests

- [X] T011 Add `GeminiKeyRepository` integration tests (create/get by id, `GetByNameAsync`, `GetByUserIdAsync`, unique constraint enforcement) in `backend/tests/AutomationPortal.Infrastructure.IntegrationTests/GeminiKeys/GeminiKeyRepositoryTests.cs`

### Frontend API Scaffold

- [X] T012 [P] Create frontend API types (`GeminiKeyListItem`, `GeminiKeyPagedResponse`, `CreateGeminiKeyRequest`, `UpdateGeminiKeyRequest`, `CreateGeminiKeyResponse`) in `frontend/src/api/gemini-keys/types.ts`
- [X] T013 [P] Create gemini-keys API barrel export (re-export all functions) in `frontend/src/api/gemini-keys/index.ts`
- [X] T014 [P] Add Gemini Keys navigation link to dashboard sidebar in `frontend/src/components/layout/menu-item.tsx`
- [X] T015 Register `geminiKeysApi` in the global API factory following the `usersApi` pattern in `frontend/src/api/index.ts`

**Checkpoint**: Foundation ready — all user story phases can now begin

---

## Phase 3: User Story 1 — List Gemini Keys (Priority: P1) 🎯 MVP

**Goal**: Display a paginated table of all Gemini Keys with name, masked key value, assigned username, and created date

**Independent Test**: Navigate to `/gemini-keys` and verify the table renders with correct columns (Name, Assigned User, Masked Key, Created At, Actions) and pagination controls work

### Backend

- [X] T016 [P] [US1] Create `GetGeminiKeysQuery` record (with `PageNumber` and `PageSize`) and `GeminiKeyListItemResponse` DTO record (`Id`, `Name`, `MaskedKey`, `AssignedUsername`, `CreatedAt`) in `backend/src/AutomationPortal.Application/Features/GeminiKeys/GetGeminiKeys/GetGeminiKeysQuery.cs`
- [X] T017 [US1] Implement `GetGeminiKeysQueryHandler` using Dapper with server-side pagination (`PagedList<GeminiKeyListItemResponse>`), JOIN with `users` for `AssignedUsername`, and masked key calculation (`"****" + keyValue[^4..]` if length ≥ 4, else `"****"`) in `backend/src/AutomationPortal.Application/Features/GeminiKeys/GetGeminiKeys/GetGeminiKeysQueryHandler.cs`
- [X] T018 [US1] Implement `GetGeminiKeysEndpoint` (`GET /api/gemini-keys`, `WithName("GetGeminiKeys")`, tag `GeminiKeys`, `RequireAuthorization`) in `backend/src/AutomationPortal.API/Endpoints/GeminiKeys/GetGeminiKeysEndpoint.cs`

### Frontend

- [X] T019 [P] [US1] Create `getGeminiKeys` API function using `axiosServerInstance` with `pageNumber` and `pageSize` query params in `frontend/src/api/gemini-keys/getGeminiKeys.ts`
- [X] T020 [US1] Create Gemini Keys server page component that fetches initial page data and renders `GeminiKeysTableShell` in `frontend/src/app/(dashboard)/gemini-keys/page.tsx`
- [X] T021 [US1] Create TanStack Table column definitions (Name, Assigned User, Masked Key, Created At, Actions column with Edit/Delete buttons) in `frontend/src/app/(dashboard)/gemini-keys/gemini-keys-columns.tsx`
- [X] T022 [US1] Create `GeminiKeysTableShell` client component with TanStack Table, server-side pagination, and stubbed Edit/Delete action handlers in `frontend/src/app/(dashboard)/gemini-keys/gemini-keys-table-shell.tsx`

### Tests

- [X] T023 [P] [US1] Add `GET /api/gemini-keys` integration tests (200 paged response structure, `maskedKey` format `****xxxx`, `key_value` absent from response) in `backend/tests/AutomationPortal.API.IntegrationTests/GeminiKeys/GeminiKeysEndpointTests.cs`

**Checkpoint**: User Story 1 fully functional — list page shows paginated Gemini Keys with masked values

---

## Phase 4: User Story 2 — Create Gemini Key (Priority: P1)

**Goal**: Allow creating a new Gemini Key with name, key value, and assigned user; warn and offer replace if the selected user already has a key

**Independent Test**: Click "Add Key", fill in valid Name/Key Value/User, save, and confirm the new key appears in the list with the correct name and masked value

### Backend

- [X] T024 [P] [US2] Create `CreateGeminiKeyCommand` record (`Name`, `KeyValue`, `UserId`, `ReplaceExisting`) and `CreateGeminiKeyResponse` record (`Id`, `Name`) in `backend/src/AutomationPortal.Application/Features/GeminiKeys/CreateGeminiKey/CreateGeminiKeyCommand.cs`
- [X] T025 [P] [US2] Create `CreateGeminiKeyCommandValidator` (Name required/max 200, KeyValue required/max 500, UserId not empty) in `backend/src/AutomationPortal.Application/Features/GeminiKeys/CreateGeminiKey/CreateGeminiKeyCommandValidator.cs`
- [X] T026 [US2] Implement `CreateGeminiKeyCommandHandler` (check `NameAlreadyExists` via `GetByNameAsync`, check `UserAlreadyHasKey` via `GetByUserIdAsync` — return `Result.Failure` with HTTP 409 if `!ReplaceExisting`, else hard-delete old key + create new within transaction, structured logging) in `backend/src/AutomationPortal.Application/Features/GeminiKeys/CreateGeminiKey/CreateGeminiKeyCommandHandler.cs`
- [X] T027 [US2] Implement `CreateGeminiKeyEndpoint` (`POST /api/gemini-keys`, `WithName("CreateGeminiKey")`, 201/409/422 responses, `RequireAuthorization`) in `backend/src/AutomationPortal.API/Endpoints/GeminiKeys/CreateGeminiKeyEndpoint.cs`

### Frontend

- [X] T028 [P] [US2] Create `createGeminiKey` API function using `axiosClientInstance` (POST body with `replaceExisting` flag) in `frontend/src/api/gemini-keys/createGeminiKey.ts`
- [X] T029 [P] [US2] Create `NameField` form component (controlled Input with label and error message) in `frontend/src/app/(dashboard)/gemini-keys/form/name-field.tsx`
- [X] T030 [P] [US2] Create `KeyValueField` form component (controlled Input with label and error message) in `frontend/src/app/(dashboard)/gemini-keys/form/key-value-field.tsx`
- [X] T031 [P] [US2] Create `UserSelectField` form component (Select populated from `GET /api/users` with label and error message) in `frontend/src/app/(dashboard)/gemini-keys/form/user-select-field.tsx`
- [X] T032 [US2] Create `CreateGeminiKeyDialog` with react-hook-form + zod schema, renders NameField/KeyValueField/UserSelectField, handles 409 response by showing inline replace-existing warning and retrying with `replaceExisting: true`, invalidates list on 201 in `frontend/src/app/(dashboard)/gemini-keys/create-gemini-key-dialog.tsx`
- [X] T033 [US2] Wire "Add Key" button in `GeminiKeysTableShell` to open `CreateGeminiKeyDialog` in `frontend/src/app/(dashboard)/gemini-keys/gemini-keys-table-shell.tsx`

### Tests

- [X] T034 [P] [US2] Add `CreateGeminiKeyCommandHandler` unit tests (success, `NameAlreadyExists`, `UserAlreadyHasKey` without `ReplaceExisting` returns 409 error, `UserAlreadyHasKey` with `ReplaceExisting: true` deletes old and creates new) in `backend/tests/AutomationPortal.Application.UnitTests/GeminiKeys/CreateGeminiKeyCommandHandlerTests.cs`
- [X] T035 [P] [US2] Add `POST /api/gemini-keys` integration tests (201 created, 409 user-already-has-key, 409 then retry with `replaceExisting: true` succeeds, 422 name-already-exists, 422 validation errors) in `backend/tests/AutomationPortal.API.IntegrationTests/GeminiKeys/GeminiKeysEndpointTests.cs`

**Checkpoint**: User Story 2 fully functional — create dialog works including replace-existing confirmation flow

---

## Phase 5: User Story 3 — Edit Gemini Key (Priority: P2)

**Goal**: Allow updating an existing Gemini Key's name, key value, and assigned user; warn if the new user already has a different key

**Independent Test**: Click Edit on an existing key, change name/value/user, save, and confirm the change is reflected in the list

### Backend

- [X] T036 [P] [US3] Create `UpdateGeminiKeyCommand` record (`Id`, `Name`, `KeyValue`, `UserId`, `ReplaceExisting`) in `backend/src/AutomationPortal.Application/Features/GeminiKeys/UpdateGeminiKey/UpdateGeminiKeyCommand.cs`
- [X] T037 [P] [US3] Create `UpdateGeminiKeyCommandValidator` (Id not empty, Name required/max 200, KeyValue required/max 500, UserId not empty) in `backend/src/AutomationPortal.Application/Features/GeminiKeys/UpdateGeminiKey/UpdateGeminiKeyCommandValidator.cs`
- [X] T038 [US3] Implement `UpdateGeminiKeyCommandHandler` (NotFound check, NameAlreadyExists check for other records, UserAlreadyHasKey check for different key with `ReplaceExisting` flow, call `GeminiKey.Update()`, structured logging) in `backend/src/AutomationPortal.Application/Features/GeminiKeys/UpdateGeminiKey/UpdateGeminiKeyCommandHandler.cs`
- [X] T039 [US3] Implement `UpdateGeminiKeyEndpoint` (`PUT /api/gemini-keys/{id}`, `WithName("UpdateGeminiKey")`, 204/404/409/422 responses, `RequireAuthorization`) in `backend/src/AutomationPortal.API/Endpoints/GeminiKeys/UpdateGeminiKeyEndpoint.cs`

### Frontend

- [X] T040 [P] [US3] Create `updateGeminiKey` API function using `axiosClientInstance` (PUT `{id}` with body including `replaceExisting`) in `frontend/src/api/gemini-keys/updateGeminiKey.ts`
- [X] T041 [US3] Create `EditGeminiKeyDialog` with react-hook-form + zod schema, pre-filled from selected row (name and maskedKey displayed for key value field), replaceExisting warning flow on 409, invalidates list on 204 in `frontend/src/app/(dashboard)/gemini-keys/edit-gemini-key-dialog.tsx`
- [X] T042 [US3] Wire Edit action button in `GeminiKeysTableShell` to open `EditGeminiKeyDialog` with selected row data in `frontend/src/app/(dashboard)/gemini-keys/gemini-keys-table-shell.tsx`

### Tests

- [X] T043 [P] [US3] Add `UpdateGeminiKeyCommandHandler` unit tests (success, NotFound, NameAlreadyExists, UserAlreadyHasKey without/with ReplaceExisting) in `backend/tests/AutomationPortal.Application.UnitTests/GeminiKeys/UpdateGeminiKeyCommandHandlerTests.cs`
- [X] T044 [P] [US3] Add `PUT /api/gemini-keys/{id}` integration tests (204 success, 404 not found, 409/422 cases) in `backend/tests/AutomationPortal.API.IntegrationTests/GeminiKeys/GeminiKeysEndpointTests.cs`

**Checkpoint**: User Story 3 fully functional — edit dialog updates keys including replace-existing flow

---

## Phase 6: User Story 4 — Delete Gemini Key (Priority: P3)

**Goal**: Allow hard-deleting a Gemini Key after the user confirms by typing the key's name (case-insensitive, trimmed)

**Independent Test**: Click Delete on a key, type its name in different case, verify the confirm button enables, click confirm, and verify the key is absent from the list

### Backend

- [X] T045 [P] [US4] Create `DeleteGeminiKeyCommand` record with `Id` field in `backend/src/AutomationPortal.Application/Features/GeminiKeys/DeleteGeminiKey/DeleteGeminiKeyCommand.cs`
- [X] T046 [P] [US4] Create `DeleteGeminiKeyCommandValidator` (Id not empty) in `backend/src/AutomationPortal.Application/Features/GeminiKeys/DeleteGeminiKey/DeleteGeminiKeyCommandValidator.cs`
- [X] T047 [US4] Implement `DeleteGeminiKeyCommandHandler` (NotFound check, hard delete via repository `Remove()` — do NOT use `SoftDelete()`, structured logging) in `backend/src/AutomationPortal.Application/Features/GeminiKeys/DeleteGeminiKey/DeleteGeminiKeyCommandHandler.cs`
- [X] T048 [US4] Implement `DeleteGeminiKeyEndpoint` (`DELETE /api/gemini-keys/{id}`, `WithName("DeleteGeminiKey")`, 204/404 responses, `RequireAuthorization`) in `backend/src/AutomationPortal.API/Endpoints/GeminiKeys/DeleteGeminiKeyEndpoint.cs`

### Frontend

- [X] T049 [P] [US4] Create `deleteGeminiKey` API function using `axiosClientInstance` (DELETE `{id}`) in `frontend/src/api/gemini-keys/deleteGeminiKey.ts`
- [X] T050 [US4] Create `DeleteGeminiKeyDialog` with name confirmation input (confirm button disabled until `input.trim().toLowerCase() === keyName.toLowerCase()`), invalidates list on 204 in `frontend/src/app/(dashboard)/gemini-keys/delete-gemini-key-dialog.tsx`
- [X] T051 [US4] Wire Delete action button in `GeminiKeysTableShell` to open `DeleteGeminiKeyDialog` with selected row data (`id` and `name`) in `frontend/src/app/(dashboard)/gemini-keys/gemini-keys-table-shell.tsx`

### Tests

- [X] T052 [P] [US4] Add `DeleteGeminiKeyCommandHandler` unit tests (success hard delete, NotFound) in `backend/tests/AutomationPortal.Application.UnitTests/GeminiKeys/DeleteGeminiKeyCommandHandlerTests.cs`
- [X] T053 [P] [US4] Add `DELETE /api/gemini-keys/{id}` integration tests (204 success, 404 not found, confirm record is physically removed from DB) in `backend/tests/AutomationPortal.API.IntegrationTests/GeminiKeys/GeminiKeysEndpointTests.cs`

**Checkpoint**: All 4 user stories fully functional — complete CRUD for Gemini Key management

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Final validation, documentation, and quality checks across all stories

- [ ] T054 [P] Verify all 4 endpoints appear in Scalar UI with correct `WithName()` values (`GetGeminiKeys`, `CreateGeminiKey`, `UpdateGeminiKey`, `DeleteGeminiKey`) and tag `GeminiKeys` — run app and open `/scalar`
- [ ] T055 Run full validation per `quickstart.md` — build solution, apply migration, execute all test suites (Domain.UnitTests, Application.UnitTests, Infrastructure.IntegrationTests, API.IntegrationTests)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 — **BLOCKS all user stories**
- **User Stories (Phases 3–6)**: All depend on Foundational completion
  - US1 and US2 (both P1) can proceed in parallel after Phase 2
  - US3 (P2) can start after Phase 2; reuses form components created in US2
  - US4 (P3) can start after Phase 2; reuses table shell wiring pattern from US1
- **Polish (Phase 7)**: Depends on all desired user stories being complete

### User Story Dependencies

- **US1 (P1)**: Independent after Foundational — delivers list page with table shell
- **US2 (P1)**: Independent after Foundational — creates shared form components (name-field, key-value-field, user-select-field) reused by US3
- **US3 (P2)**: Independent after Foundational — may start US3 dialog after US2 form components exist
- **US4 (P3)**: Independent after Foundational — delete dialog is self-contained

### Within Each User Story

- Command/Query definition [P] and Validator [P] can start simultaneously
- Handler depends on Command + Validator
- Endpoint depends on Handler
- Frontend API function [P] is independent of backend
- Form components [P] are independent of each other
- Dialog depends on form components and API function
- Table shell wiring depends on dialog

### Parallel Opportunities

- T002, T003, T004, T005 in Phase 2 can all start simultaneously
- T012, T013, T014 in Phase 2 are fully independent of backend work and can run in parallel with backend foundation
- Within US2: T024, T025, T028, T029, T030, T031 can all start simultaneously
- US1 and US2 can be worked simultaneously by two developers after Phase 2

---

## Parallel Example: User Story 2 (Create)

```bash
# These 6 tasks can all start simultaneously after Phase 2:
T024: CreateGeminiKeyCommand + CreateGeminiKeyResponse
T025: CreateGeminiKeyCommandValidator
T028: createGeminiKey.ts API function
T029: form/name-field.tsx
T030: form/key-value-field.tsx
T031: form/user-select-field.tsx

# Then run in parallel:
T026: CreateGeminiKeyCommandHandler  (waits for T024, T025)
T032: CreateGeminiKeyDialog          (waits for T028, T029, T030, T031)

# Then sequentially:
T027: CreateGeminiKeyEndpoint        (waits for T026)
T033: Wire "Add Key" button          (waits for T032)

# Tests can run after implementation:
T034: Handler unit tests
T035: Endpoint integration tests
```

---

## Implementation Strategy

### MVP First (User Stories 1 + 2 Only)

1. Complete Phase 1: Setup (baseline verification)
2. Complete Phase 2: Foundational (CRITICAL — blocks everything)
3. Complete Phase 3: User Story 1 (list page with masked keys)
4. Complete Phase 4: User Story 2 (create dialog with replace-existing flow)
5. **STOP and VALIDATE**: Working end-to-end create + list flow
6. Deploy/demo if ready

### Incremental Delivery

1. Setup + Foundational → domain, infra, and frontend API wiring in place
2. US1 → list page independently testable
3. US2 → create works, list refreshes (MVP!)
4. US3 → edit works, reuses form components from US2
5. US4 → delete works with name confirmation
6. Polish → Scalar docs verified, full test suite green

### Parallel Team Strategy

After Phase 2 completes:
- Developer A: US1 (list page, table shell, GET endpoint)
- Developer B: US2 (create dialog, form components, POST endpoint)
- Developer C: US3 after US2 form components exist (edit dialog, PUT endpoint)

---

## Notes

- `[P]` tasks = different files, no unmet dependencies within the phase
- `[Story]` label maps each task to its user story for traceability
- `key_value` must **NEVER** appear in any API response — only `maskedKey` (`"****" + last 4 chars`)
- Delete is **hard delete** — call repository `Remove()`, do NOT use `SoftDelete()` from `BaseEntity`
- `replaceExisting` flag applies to both Create and Update commands — see `research.md §3` for the 409 flow
- `GeminiKeysEndpointTests.cs` is built up incrementally across Phases 3–6 (one test class, separate test methods per HTTP operation)
- Commit after each task or logical group
- Stop at any checkpoint to validate the story independently before moving to the next
