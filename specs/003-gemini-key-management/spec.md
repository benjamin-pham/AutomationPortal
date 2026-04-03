# Feature Specification: Quản lý Gemini Key

**Feature Branch**: `003-gemini-key-management`  
**Created**: 2026-04-03  
**Status**: Draft  
**Input**: User description: "tôi muốn xây dựng chức năng quản lý key gemini, môi user sẽ gắn với 1 key"

## Clarifications

### Session 2026-04-03

- Q: Trong hộp thoại xác nhận xóa, người dùng cần điền gì? → A: Gõ lại tên (label) riêng của key — GeminiKey có trường Tên riêng biệt với giá trị key
- Q: Tên Key có cần duy nhất trong toàn hệ thống không? → A: Có — Tên Key phải duy nhất toàn hệ thống
- Q: Khi chỉnh sửa GeminiKey, có thể thay đổi người dùng được gán không? → A: Có — form sửa cho phép đổi cả Tên Key, Giá trị Key và Người dùng được gán
- Q: Danh sách có hỗ trợ tìm kiếm/lọc không? → A: Không — phân trang là đủ, không cần tìm kiếm
- Q: Hộp thoại xóa so sánh tên key theo kiểu phân biệt hoa thường không? → A: Không — so sánh case-insensitive để thân thiện hơn với người dùng

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Danh sách Gemini Key (Priority: P1)

Người dùng đã đăng nhập xem danh sách tất cả Gemini Key trong hệ thống, mỗi key hiển thị kèm tên key và thông tin người dùng được gán.

**Why this priority**: Đây là điểm khởi đầu của mọi tác vụ quản lý — cần xem được danh sách trước khi thực hiện bất kỳ thao tác nào khác.

**Independent Test**: Có thể kiểm thử bằng cách truy cập trang quản lý Gemini Key và xác nhận danh sách hiển thị đúng thông tin.

**Acceptance Scenarios**:

1. **Given** người dùng đã đăng nhập, **When** truy cập trang quản lý Gemini Key, **Then** hệ thống hiển thị danh sách key dạng bảng với các cột: Tên Key, Username của người dùng được gán, Key (ẩn một phần), Ngày tạo
2. **Given** danh sách đang hiển thị, **When** có hơn 20 bản ghi, **Then** dữ liệu được phân trang và người dùng có thể điều hướng giữa các trang

---

### User Story 2 - Tạo Gemini Key mới (Priority: P1)

Người dùng đã đăng nhập tạo một Gemini Key mới (có tên riêng) và gán cho một tài khoản người dùng trong hệ thống.

**Why this priority**: Chức năng tạo mới là nền tảng — không có tạo mới thì không có dữ liệu để quản lý.

**Independent Test**: Có thể kiểm thử bằng cách điền form tạo mới và xác nhận key xuất hiện trong danh sách với đúng tên và người dùng được gán.

**Acceptance Scenarios**:

1. **Given** người dùng đã đăng nhập đang ở trang danh sách key, **When** nhấn nút "Thêm Key", **Then** hệ thống hiển thị form tạo mới với các trường: Tên Key (*), Chọn người dùng (*), Giá trị Key (*)
2. **Given** form tạo mới đang mở, **When** điền đầy đủ thông tin hợp lệ và nhấn "Lưu", **Then** key được tạo thành công và xuất hiện trong danh sách với tên đã nhập
3. **Given** form tạo mới đang mở, **When** chọn một người dùng đã có key, **Then** hệ thống hiển thị cảnh báo "Người dùng này đã có Gemini Key, lưu sẽ thay thế key cũ"
4. **Given** form tạo mới đang mở, **When** để trống trường bắt buộc và nhấn "Lưu", **Then** hệ thống hiển thị thông báo lỗi tương ứng cho từng trường

---

### User Story 3 - Chỉnh sửa Gemini Key (Priority: P2)

Người dùng đã đăng nhập cập nhật tên hoặc giá trị Gemini Key của một người dùng khi key cũ hết hạn hoặc cần thay đổi.

**Why this priority**: Key có thể cần cập nhật định kỳ; chỉnh sửa trực tiếp hiệu quả hơn xóa rồi tạo lại.

**Independent Test**: Có thể kiểm thử bằng cách chỉnh sửa một key và xác nhận thay đổi được lưu trong danh sách.

**Acceptance Scenarios**:

1. **Given** người dùng đã đăng nhập đang xem danh sách key, **When** nhấn nút "Sửa" cho một bản ghi, **Then** hệ thống hiển thị form chỉnh sửa với Tên Key, Người dùng được gán và Giá trị Key được điền sẵn (giá trị key ẩn một phần)
2. **Given** form chỉnh sửa đang mở, **When** thay đổi thông tin hợp lệ (bao gồm đổi người dùng được gán) và nhấn "Lưu", **Then** key được cập nhật thành công và phản ánh trong danh sách
2a. **Given** form chỉnh sửa đang mở, **When** đổi sang một người dùng đã có key khác, **Then** hệ thống hiển thị cảnh báo "Người dùng này đã có Gemini Key, lưu sẽ thay thế key cũ"
3. **Given** form chỉnh sửa đang mở, **When** xóa trắng trường bắt buộc và nhấn "Lưu", **Then** hệ thống hiển thị lỗi validation và không lưu thay đổi
4. **Given** form chỉnh sửa đang mở, **When** nhấn "Hủy", **Then** không có thay đổi nào được lưu và người dùng trở về danh sách

---

### User Story 4 - Xóa Gemini Key (Priority: P3)

Người dùng đã đăng nhập xóa Gemini Key của một người dùng khi key không còn cần thiết. Để tránh xóa nhầm, người dùng phải gõ lại đúng tên key trước khi xác nhận.

**Why this priority**: Ít được thực hiện hơn các thao tác tạo/sửa, nhưng cần thiết để dọn dẹp dữ liệu khi key không còn dùng.

**Independent Test**: Có thể kiểm thử bằng cách xóa một key, xác nhận hộp thoại yêu cầu gõ tên, và xác nhận bản ghi không còn xuất hiện trong danh sách sau khi xóa thành công.

**Acceptance Scenarios**:

1. **Given** người dùng đã đăng nhập đang xem danh sách key, **When** nhấn nút "Xóa" cho một bản ghi, **Then** hệ thống hiển thị hộp thoại xác nhận với tên key và hướng dẫn "Gõ tên key để xác nhận xóa"
2. **Given** hộp thoại xác nhận đang hiển thị, **When** người dùng gõ đúng tên key và nhấn "Xác nhận xóa", **Then** key bị xóa vĩnh viễn và không còn xuất hiện trong danh sách
3. **Given** hộp thoại xác nhận đang hiển thị, **When** người dùng gõ sai tên key, **Then** nút "Xác nhận xóa" bị vô hiệu hóa và không thể thực hiện xóa
4. **Given** hộp thoại xác nhận đang hiển thị, **When** nhấn "Hủy", **Then** không có thay đổi nào được thực hiện

---

### Edge Cases

- Điều gì xảy ra khi tạo key cho một người dùng đã có key? → Hiển thị cảnh báo, nếu xác nhận thì thay thế key cũ
- Điều gì xảy ra khi giá trị key được nhập có khoảng trắng đầu hoặc cuối? → Hệ thống tự động cắt bỏ khoảng trắng trước khi lưu
- Điều gì xảy ra khi người dùng gõ tên key trong hộp thoại xóa nhưng khác hoa thường? → Hệ thống chấp nhận (so sánh case-insensitive), nhưng khoảng trắng thừa đầu/cuối vẫn bị cắt bỏ trước khi so sánh

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Hệ thống PHẢI hiển thị danh sách Gemini Key với thông tin: Tên Key, Username của người dùng được gán, Key (ẩn một phần), Ngày tạo
- **FR-002**: Hệ thống PHẢI hỗ trợ phân trang cho danh sách key
- **FR-003**: Hệ thống PHẢI cho phép tạo Gemini Key mới với các trường bắt buộc: Tên Key, Chọn người dùng, Giá trị Key
- **FR-004**: Hệ thống PHẢI đảm bảo mỗi người dùng chỉ có tối đa một Gemini Key; tạo key mới cho người dùng đã có key sẽ thay thế key cũ sau khi xác nhận
- **FR-005**: Hệ thống PHẢI validate các trường bắt buộc (Tên Key, Giá trị Key, Người dùng) không được để trống trước khi lưu
- **FR-005a**: Hệ thống PHẢI validate Tên Key là duy nhất trong toàn hệ thống; khi tạo hoặc sửa trùng tên đã tồn tại, hệ thống hiển thị lỗi "Tên Key đã được sử dụng"
- **FR-006**: Hệ thống PHẢI cho phép chỉnh sửa Tên Key, Giá trị Key và Người dùng được gán của một bản ghi; nếu người dùng mới được chọn đã có key khác, hệ thống phải cảnh báo trước khi thay thế
- **FR-007**: Hệ thống PHẢI yêu cầu người dùng gõ lại Tên Key trong hộp thoại xác nhận trước khi cho phép xóa; so sánh không phân biệt hoa thường (case-insensitive) và tự động cắt khoảng trắng đầu/cuối; nút xác nhận bị vô hiệu hóa cho đến khi tên khớp
- **FR-008**: Hệ thống PHẢI không hiển thị giá trị key đầy đủ trong giao diện — chỉ hiển thị dạng ẩn một phần (ví dụ: `****xyz`)

### Key Entities

- **GeminiKey**: Đại diện cho một API Key được liên kết với người dùng. Thuộc tính: Tên Key (bắt buộc, duy nhất toàn hệ thống, dùng để định danh trong giao diện và xác nhận xóa), giá trị key (bắt buộc, lưu trữ an toàn, không hiển thị đầy đủ), người dùng được gán (quan hệ một-một, bắt buộc), ngày tạo
- **User**: Tài khoản người dùng hiện có trong hệ thống; có quan hệ một-một tùy chọn với GeminiKey

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Người dùng có thể tạo mới hoặc cập nhật Gemini Key cho một tài khoản trong vòng dưới 60 giây
- **SC-002**: Người dùng có thể hoàn thành thao tác xóa key (bao gồm gõ tên xác nhận) trong vòng dưới 60 giây
- **SC-003**: 100% các thao tác xóa yêu cầu gõ đúng tên key — không thể xóa khi tên gõ không khớp
- **SC-004**: 100% giá trị key được che trong mọi giao diện — không có key nào bị lộ dưới dạng văn bản thô

## Assumptions

- Mọi người dùng đã xác thực (đã đăng nhập) đều có toàn quyền thực hiện các thao tác CRUD trên trang quản lý Gemini Key — không áp dụng phân quyền theo vai trò
- Mỗi người dùng chỉ được có tối đa một Gemini Key tại một thời điểm (quan hệ một-một)
- Xóa Gemini Key là xóa vĩnh viễn (hard delete)
- Hệ thống xác thực và quản lý người dùng hiện có sẽ được tái sử dụng; tính năng này mở rộng thêm vào đó
- Giao diện quản lý chỉ dành cho desktop/web, không cần tối ưu cho mobile trong phiên bản đầu tiên
