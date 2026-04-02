# Feature Specification: User Management

**Feature Branch**: `001-user-management`  
**Created**: 2026-04-02  
**Status**: Draft  
**Input**: User description: "tôi đang có sẵn entity User rồi, tôi muốn xây dựng chức năng quản lý User theo entity User hiện có"

## Clarifications

### Session 2026-04-02

- Q: Phạm vi truy cập khi bỏ phân quyền — ai có thể thực hiện các thao tác CRUD? → A: Mọi user đã đăng nhập có toàn quyền CRUD (list, create, edit, delete, reset password)
- Q: Chính sách mật khẩu khi tạo mới và đặt lại? → A: Tối thiểu 8 ký tự, có chữ hoa và số
- Q: Email có cần duy nhất không? → A: Không — username là định danh duy nhất, email không có ràng buộc uniqueness
- Q: Username format hợp lệ? → A: Chỉ chữ cái và số `[a-zA-Z0-9]`, tối thiểu 6, tối đa 50 ký tự (theo UsernameValidator hiện có)
- Q: Xử lý concurrent edit (2 người sửa cùng lúc)? → A: Last write wins — không kiểm tra conflict

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Danh sách và tìm kiếm người dùng (Priority: P1)

Người dùng đã đăng nhập cần xem danh sách tất cả người dùng trong hệ thống, có khả năng tìm kiếm và lọc để nhanh chóng tìm được người dùng cụ thể.

**Why this priority**: Đây là điểm khởi đầu của mọi tác vụ quản lý — không thể thao tác với người dùng nếu không thể tìm thấy họ. Cung cấp nền tảng cho tất cả các user story còn lại.

**Independent Test**: Có thể kiểm thử độc lập bằng cách truy cập màn hình danh sách người dùng, tìm kiếm theo tên/username/email và xác nhận kết quả hiển thị đúng — mang lại giá trị quản lý ngay cả khi chỉ có chức năng đọc.

**Acceptance Scenarios**:

1. **Given** người dùng đã đăng nhập, **When** truy cập trang quản lý người dùng, **Then** hệ thống hiển thị danh sách người dùng dạng bảng với các cột: Họ tên, Username, Email, Số điện thoại, Ngày sinh
2. **Given** danh sách người dùng đang hiển thị, **When** nhập từ khóa vào ô tìm kiếm (theo tên, username, hoặc email), **Then** danh sách được lọc theo từ khóa và hiển thị kết quả phù hợp
3. **Given** có hơn 20 người dùng trong hệ thống, **When** xem danh sách, **Then** dữ liệu được phân trang và người dùng có thể điều hướng giữa các trang
4. **Given** danh sách đang hiển thị, **When** nhấn tiêu đề cột, **Then** danh sách được sắp xếp theo cột đó (tăng/giảm dần)

---

### User Story 2 - Xem chi tiết người dùng (Priority: P2)

Người dùng đã đăng nhập cần xem đầy đủ thông tin của một người dùng cụ thể, bao gồm tất cả các trường trong entity User.

**Why this priority**: Cần thiết để kiểm tra thông tin trước khi thực hiện các tác vụ chỉnh sửa hoặc ra quyết định quản lý. Đây là bước thường xuyên nhất sau khi tìm kiếm.

**Independent Test**: Có thể kiểm thử bằng cách nhấn vào một người dùng trong danh sách và xác nhận trang chi tiết hiển thị đầy đủ thông tin — cung cấp giá trị tra cứu thông tin ngay cả không có tính năng chỉnh sửa.

**Acceptance Scenarios**:

1. **Given** người dùng đã đăng nhập đang xem danh sách người dùng, **When** nhấn vào một người dùng, **Then** hệ thống hiển thị trang chi tiết với toàn bộ thông tin: Họ, Tên, Username, Email, Số điện thoại, Ngày sinh
2. **Given** trang chi tiết người dùng đang hiển thị, **When** xem thông tin, **Then** các trường nhạy cảm như mật khẩu không được hiển thị
3. **Given** trang chi tiết người dùng đang hiển thị, **When** nhấn nút "Quay lại", **Then** người dùng được điều hướng về danh sách người dùng với trạng thái tìm kiếm/lọc được giữ nguyên

---

### User Story 3 - Tạo người dùng mới (Priority: P2)

Người dùng đã đăng nhập có thể tạo tài khoản người dùng mới trực tiếp từ giao diện quản lý, bao gồm thiết lập thông tin cơ bản và mật khẩu ban đầu.

**Why this priority**: Cho phép tạo tài khoản thủ công, quan trọng cho các hệ thống không có self-registration hoặc khi cần onboard người dùng theo lô.

**Independent Test**: Có thể kiểm thử bằng cách điền form tạo mới và xác nhận người dùng xuất hiện trong danh sách — độc lập hoàn toàn với các tính năng edit/delete.

**Acceptance Scenarios**:

1. **Given** người dùng đã đăng nhập đang ở trang quản lý người dùng, **When** nhấn nút "Thêm người dùng", **Then** hệ thống hiển thị form tạo mới với các trường: Họ (*), Tên (*), Username (*), Mật khẩu (*), Email, Số điện thoại, Ngày sinh
2. **Given** form tạo mới đang mở, **When** điền đầy đủ thông tin hợp lệ và nhấn "Lưu", **Then** người dùng mới được tạo thành công và xuất hiện trong danh sách
3. **Given** form tạo mới đang mở, **When** nhập username đã tồn tại trong hệ thống, **Then** hệ thống hiển thị thông báo lỗi "Username đã được sử dụng"
4. **Given** form tạo mới đang mở, **When** bỏ trống các trường bắt buộc (Họ, Tên, Username, Mật khẩu) và nhấn "Lưu", **Then** hệ thống hiển thị thông báo lỗi tương ứng cho từng trường

---

### User Story 4 - Chỉnh sửa thông tin người dùng (Priority: P2)

Người dùng đã đăng nhập có thể cập nhật thông tin hồ sơ của một người dùng, bao gồm Họ, Tên, Email, Số điện thoại, Ngày sinh.

**Why this priority**: Dữ liệu người dùng thay đổi theo thời gian (thay đổi email, số điện thoại, v.v.) và cần khả năng chỉnh sửa để duy trì tính chính xác.

**Independent Test**: Có thể kiểm thử bằng cách chỉnh sửa thông tin một người dùng và xác nhận thay đổi được lưu — không phụ thuộc vào tính năng xóa hay reset mật khẩu.

**Acceptance Scenarios**:

1. **Given** người dùng đã đăng nhập đang xem chi tiết hoặc danh sách người dùng, **When** nhấn nút "Chỉnh sửa" cho một người dùng, **Then** hệ thống hiển thị form chỉnh sửa với thông tin hiện tại được điền sẵn
2. **Given** form chỉnh sửa đang mở, **When** thay đổi thông tin hợp lệ và nhấn "Lưu", **Then** thông tin được cập nhật thành công và phản ánh ngay trong danh sách/chi tiết
3. **Given** form chỉnh sửa đang mở, **When** xóa trắng trường bắt buộc (Họ hoặc Tên) và nhấn "Lưu", **Then** hệ thống hiển thị lỗi validation và không lưu thay đổi
4. **Given** form chỉnh sửa đang mở, **When** nhấn "Hủy", **Then** không có thay đổi nào được lưu và người dùng trở về trang trước

---

### User Story 5 - Xóa người dùng (Priority: P3)

Người dùng đã đăng nhập có thể xóa một người dùng khỏi hệ thống sau khi xác nhận hành động.

**Why this priority**: Ít được thực hiện hơn các thao tác đọc/sửa, nhưng cần thiết để duy trì tính sạch sẽ của dữ liệu khi tài khoản không còn cần thiết.

**Independent Test**: Có thể kiểm thử bằng cách xóa một người dùng và xác nhận người dùng đó không còn xuất hiện trong danh sách.

**Acceptance Scenarios**:

1. **Given** người dùng đã đăng nhập đang xem danh sách hoặc chi tiết người dùng, **When** nhấn nút "Xóa" cho một người dùng, **Then** hệ thống hiển thị hộp thoại xác nhận với tên người dùng cụ thể
2. **Given** hộp thoại xác nhận đang hiển thị, **When** xác nhận xóa, **Then** người dùng bị xóa và không còn xuất hiện trong danh sách
3. **Given** hộp thoại xác nhận đang hiển thị, **When** nhấn "Hủy", **Then** không có thay đổi nào được thực hiện
4. **Given** người dùng đã đăng nhập cố gắng xóa tài khoản của chính mình, **When** thực hiện hành động xóa, **Then** hệ thống từ chối và hiển thị thông báo không thể xóa tài khoản đang đăng nhập

---

### User Story 6 - Đặt lại mật khẩu người dùng (Priority: P3)

Người dùng đã đăng nhập có thể đặt lại mật khẩu cho một người dùng khi họ quên mật khẩu hoặc cần can thiệp bảo mật.

**Why this priority**: Tính năng hỗ trợ quan trọng nhưng ít khẩn cấp hơn các thao tác CRUD cơ bản.

**Independent Test**: Có thể kiểm thử bằng cách reset mật khẩu và xác nhận người dùng có thể đăng nhập bằng mật khẩu mới.

**Acceptance Scenarios**:

1. **Given** người dùng đã đăng nhập đang xem chi tiết người dùng, **When** nhấn "Đặt lại mật khẩu", **Then** hệ thống hiển thị form nhập mật khẩu mới với xác nhận mật khẩu
2. **Given** form đặt lại mật khẩu đang mở, **When** nhập mật khẩu mới hợp lệ và xác nhận khớp, **Then** mật khẩu được cập nhật và các phiên đăng nhập hiện tại bị vô hiệu hóa
3. **Given** form đặt lại mật khẩu đang mở, **When** nhập mật khẩu xác nhận không khớp, **Then** hệ thống hiển thị lỗi và không thực hiện thay đổi

---

### Edge Cases

- Điều gì xảy ra khi tìm kiếm không trả về kết quả nào? → Hiển thị thông báo "Không tìm thấy người dùng nào phù hợp"
- Điều gì xảy ra khi xóa người dùng đang có phiên đăng nhập hoạt động? → Phiên bị hủy ngay lập tức
- Điều gì xảy ra khi nhập email không đúng định dạng? → Hệ thống hiển thị lỗi validation
- Điều gì xảy ra khi nhập số điện thoại không hợp lệ? → Hệ thống hiển thị lỗi validation
- Điều gì xảy ra khi cố tạo người dùng với username chứa ký tự đặc biệt hoặc không đúng format? → Hệ thống hiển thị lỗi: "Username chỉ được chứa chữ cái và số, tối thiểu 6 ký tự, tối đa 50 ký tự"

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Hệ thống PHẢI hiển thị danh sách người dùng với thông tin: Họ tên đầy đủ, Username, Email, Số điện thoại, Ngày sinh
- **FR-002**: Hệ thống PHẢI hỗ trợ tìm kiếm người dùng theo tên (Họ hoặc Tên), username, và email
- **FR-003**: Hệ thống PHẢI hỗ trợ phân trang cho danh sách người dùng
- **FR-004**: Hệ thống PHẢI hỗ trợ sắp xếp danh sách theo từng cột
- **FR-005**: Hệ thống PHẢI cho phép tạo người dùng mới với các trường bắt buộc: Họ, Tên, Username, Mật khẩu; và các trường tùy chọn: Email, Số điện thoại, Ngày sinh
- **FR-005a**: Mật khẩu PHẢI thỏa mãn: tối thiểu 8 ký tự, có ít nhất 1 chữ hoa và 1 chữ số — áp dụng cho cả tạo mới lẫn đặt lại mật khẩu
- **FR-006**: Hệ thống PHẢI validate username là duy nhất khi tạo người dùng mới; email không có ràng buộc uniqueness
- **FR-007**: Hệ thống PHẢI cho phép chỉnh sửa thông tin hồ sơ người dùng: Họ, Tên, Email, Số điện thoại, Ngày sinh (không bao gồm Username)
- **FR-008**: Hệ thống PHẢI yêu cầu xác nhận trước khi xóa người dùng
- **FR-009**: Hệ thống PHẢI ngăn người dùng tự xóa tài khoản của chính mình (dựa trên identity của phiên đăng nhập hiện tại, không phân biệt vai trò)
- **FR-010**: Hệ thống PHẢI cho phép đặt lại mật khẩu người dùng và vô hiệu hóa tất cả phiên đăng nhập hiện tại của người dùng đó sau khi đặt lại
- **FR-011**: Hệ thống PHẢI không hiển thị các trường nhạy cảm (hash mật khẩu, refresh token) trên giao diện

### Key Entities

- **User**: Đại diện cho tài khoản người dùng trong hệ thống với các thuộc tính: Họ (bắt buộc), Tên (bắt buộc), Username (bắt buộc, duy nhất, không thể thay đổi sau khi tạo, chỉ `[a-zA-Z0-9]`, 6–50 ký tự), Email (tùy chọn, không unique), Số điện thoại (tùy chọn), Ngày sinh (tùy chọn). Mật khẩu được lưu an toàn và không bao giờ được hiển thị trực tiếp.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Người dùng có thể tìm thấy bất kỳ người dùng nào trong hệ thống trong vòng dưới 30 giây
- **SC-002**: Người dùng có thể hoàn thành tác vụ tạo người dùng mới trong dưới 2 phút
- **SC-003**: Người dùng có thể hoàn thành tác vụ chỉnh sửa thông tin người dùng trong dưới 1 phút
- **SC-004**: Danh sách người dùng tải và hiển thị kết quả trong vòng dưới 2 giây ngay cả với hơn 10.000 người dùng
- **SC-005**: 100% các thao tác xóa yêu cầu xác nhận — không có tình huống xóa nhầm không cố ý
- **SC-006**: Người dùng hoàn thành các tác vụ CRUD cơ bản mà không cần hỗ trợ kỹ thuật

## Assumptions

- Mọi người dùng đã xác thực (đã đăng nhập) đều có toàn quyền truy cập và thực hiện tất cả các thao tác CRUD trên trang quản lý User — không áp dụng phân quyền theo vai trò
- Username không thể thay đổi sau khi tài khoản được tạo (để đảm bảo tính nhất quán của dữ liệu)
- Xóa người dùng là xóa vĩnh viễn (hard delete), không phải soft delete
- Chính sách mật khẩu: tối thiểu 8 ký tự, có ít nhất 1 chữ hoa và 1 chữ số — áp dụng cho cả tạo tài khoản lẫn đặt lại mật khẩu
- Concurrent edit không được kiểm soát — last write wins; optimistic locking không áp dụng trong phiên bản đầu tiên
- Giao diện quản lý chỉ dành cho desktop/web, không cần tối ưu cho mobile trong phiên bản đầu tiên
- Entity User hiện có (với các phương thức Create, UpdateProfile, SetRefreshToken, RevokeRefreshToken) sẽ được tái sử dụng trực tiếp
