# Feature Specification: Cập Nhật Mật Khẩu Cá Nhân

**Feature Branch**: `002-update-password`  
**Created**: 2026-04-02  
**Status**: Draft  
**Input**: User description: "tôi muốn xây dựng chức năng cập nhật mật khẩu cá nhân"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Thay Đổi Mật Khẩu Thành Công (Priority: P1)

Người dùng đã đăng nhập muốn đổi mật khẩu hiện tại sang mật khẩu mới để tăng cường bảo mật tài khoản.

**Why this priority**: Đây là luồng chính của tính năng, cung cấp giá trị cốt lõi. Không có luồng này, tính năng không hoạt động.

**Independent Test**: Có thể kiểm tra độc lập bằng cách điền mật khẩu hiện tại, mật khẩu mới, xác nhận mật khẩu mới và nhấn lưu — hệ thống cập nhật mật khẩu thành công và người dùng có thể đăng nhập bằng mật khẩu mới.

**Acceptance Scenarios**:

1. **Given** người dùng đã đăng nhập và truy cập trang đổi mật khẩu, **When** nhập đúng mật khẩu hiện tại, mật khẩu mới hợp lệ và xác nhận mật khẩu trùng khớp rồi nhấn lưu, **Then** hệ thống lưu mật khẩu mới, hiển thị thông báo thành công và người dùng vẫn ở trạng thái đăng nhập.
2. **Given** người dùng đã đổi mật khẩu thành công, **When** đăng xuất và đăng nhập lại bằng mật khẩu mới, **Then** đăng nhập thành công.
3. **Given** người dùng đã đổi mật khẩu thành công, **When** thử đăng nhập bằng mật khẩu cũ, **Then** đăng nhập thất bại với thông báo mật khẩu không đúng.

---

### User Story 2 - Xác Thực Dữ Liệu Đầu Vào (Priority: P2)

Hệ thống ngăn người dùng đặt mật khẩu không hợp lệ hoặc nhập sai thông tin, đảm bảo tính toàn vẹn và bảo mật.

**Why this priority**: Ngăn chặn lỗi người dùng và đảm bảo tiêu chuẩn bảo mật tối thiểu. Không có xác thực này, tính năng có thể bị lạm dụng.

**Independent Test**: Kiểm tra bằng cách nhập các giá trị không hợp lệ và xác nhận thông báo lỗi phù hợp xuất hiện mà không thực hiện thay đổi.

**Acceptance Scenarios**:

1. **Given** người dùng đang trên trang đổi mật khẩu, **When** nhập mật khẩu hiện tại sai, **Then** hệ thống hiển thị thông báo lỗi rõ ràng và không thực hiện thay đổi.
2. **Given** người dùng đang trên trang đổi mật khẩu, **When** mật khẩu mới và xác nhận mật khẩu không trùng khớp, **Then** hệ thống hiển thị thông báo lỗi và không lưu.
3. **Given** người dùng đang trên trang đổi mật khẩu, **When** mật khẩu mới có ít hơn 8 ký tự, **Then** hệ thống hiển thị yêu cầu mật khẩu và không lưu.
4. **Given** người dùng đang trên trang đổi mật khẩu, **When** để trống bất kỳ trường nào và nhấn lưu, **Then** hệ thống hiển thị thông báo yêu cầu điền đầy đủ.
5. **Given** người dùng đang trên trang đổi mật khẩu, **When** nhập mật khẩu mới giống mật khẩu hiện tại, **Then** hệ thống từ chối và yêu cầu chọn mật khẩu khác.

---

### User Story 3 - Bảo Mật Phiên Sau Khi Đổi Mật Khẩu (Priority: P3)

Sau khi đổi mật khẩu thành công, hệ thống vô hiệu hóa các phiên đăng nhập khác của người dùng để bảo vệ tài khoản khỏi truy cập trái phép.

**Why this priority**: Tăng cường bảo mật nhưng không ảnh hưởng đến luồng cơ bản. Người dùng vẫn có thể đổi mật khẩu mà không cần tính năng này.

**Independent Test**: Đăng nhập từ hai thiết bị, đổi mật khẩu trên một thiết bị, và xác nhận phiên trên thiết bị kia bị vô hiệu hóa yêu cầu đăng nhập lại.

**Acceptance Scenarios**:

1. **Given** người dùng đang đăng nhập trên nhiều thiết bị, **When** đổi mật khẩu thành công trên một thiết bị, **Then** các phiên đăng nhập khác bị vô hiệu hóa và yêu cầu đăng nhập lại.
2. **Given** người dùng vừa đổi mật khẩu, **When** tiếp tục sử dụng phiên hiện tại, **Then** phiên hiện tại vẫn hoạt động bình thường.

---

### Edge Cases

- Điều gì xảy ra khi phiên đăng nhập hết hạn trong lúc người dùng đang điền form đổi mật khẩu?
- Hệ thống xử lý thế nào khi có nhiều yêu cầu đổi mật khẩu đồng thời từ cùng một tài khoản?
- Điều gì xảy ra khi mạng bị ngắt giữa chừng khi gửi yêu cầu?
- Điều gì xảy ra nếu người dùng nhấn nút lưu nhiều lần liên tiếp?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Hệ thống PHẢI yêu cầu người dùng nhập mật khẩu hiện tại trước khi thay đổi mật khẩu mới.
- **FR-002**: Hệ thống PHẢI xác minh mật khẩu hiện tại là đúng trước khi cho phép cập nhật.
- **FR-003**: Hệ thống PHẢI yêu cầu người dùng nhập mật khẩu mới hai lần (mật khẩu mới và xác nhận mật khẩu mới).
- **FR-004**: Hệ thống PHẢI xác nhận mật khẩu mới và xác nhận mật khẩu trùng khớp trước khi lưu.
- **FR-005**: Hệ thống PHẢI áp dụng chính sách mật khẩu tối thiểu: ít nhất 8 ký tự.
- **FR-006**: Hệ thống PHẢI từ chối mật khẩu mới giống mật khẩu hiện tại.
- **FR-007**: Hệ thống PHẢI hiển thị thông báo thành công sau khi cập nhật mật khẩu.
- **FR-008**: Hệ thống PHẢI hiển thị thông báo lỗi rõ ràng khi xác thực thất bại.
- **FR-009**: Hệ thống PHẢI cho phép người dùng ẩn/hiện nội dung trong các ô nhập mật khẩu.
- **FR-010**: Hệ thống PHẢI vô hiệu hóa các phiên đăng nhập khác sau khi đổi mật khẩu thành công.
- **FR-011**: Hệ thống PHẢI giữ nguyên phiên đăng nhập hiện tại sau khi đổi mật khẩu thành công.

### Key Entities

- **Người dùng (User)**: Chủ tài khoản thực hiện thao tác đổi mật khẩu; phải ở trạng thái đã xác thực.
- **Mật khẩu (Password)**: Chuỗi bí mật dùng để xác thực; được lưu dưới dạng mã hóa một chiều, không bao giờ lưu dạng văn bản thường.
- **Phiên đăng nhập (Session)**: Phiên làm việc của người dùng; cần được quản lý sau khi đổi mật khẩu thành công.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Người dùng hoàn thành quá trình đổi mật khẩu trong vòng dưới 2 phút.
- **SC-002**: 95% người dùng hoàn thành đổi mật khẩu thành công ngay lần thử đầu tiên khi nhập đúng thông tin.
- **SC-003**: Thông báo lỗi xuất hiện trong vòng dưới 1 giây sau khi người dùng gửi form với dữ liệu không hợp lệ.
- **SC-004**: Không có trường hợp nào mật khẩu được cập nhật mà không xác thực đúng mật khẩu hiện tại.
- **SC-005**: Số lượng yêu cầu hỗ trợ liên quan đến tài khoản bị truy cập trái phép giảm 20% sau khi triển khai tính năng vô hiệu hóa phiên.

## Assumptions

- Người dùng đã đăng nhập vào hệ thống trước khi truy cập tính năng đổi mật khẩu (không hỗ trợ đặt lại mật khẩu khi quên — đây là tính năng riêng biệt nằm ngoài phạm vi).
- Chính sách mật khẩu tối thiểu là 8 ký tự; yêu cầu phức tạp hơn (chữ hoa, số, ký tự đặc biệt) có thể được bổ sung trong phiên bản sau.
- Hệ thống xác thực hiện tại đã hoạt động và sẽ được tái sử dụng để xác minh mật khẩu hiện tại.
- Tính năng đổi mật khẩu nằm trong trang cài đặt tài khoản cá nhân của người dùng.
- Ứng dụng hỗ trợ cả giao diện web desktop và mobile (responsive).
- Thông báo email khi mật khẩu thay đổi là tính năng tùy chọn, nằm ngoài phạm vi của đặc tả này.
- Người dùng có thể xem/ẩn mật khẩu trong quá trình nhập để tránh nhầm lẫn.
