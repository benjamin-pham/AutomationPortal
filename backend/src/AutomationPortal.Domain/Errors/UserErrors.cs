using AutomationPortal.Domain.Abstractions;

namespace AutomationPortal.Domain.Errors;

public static class UserErrors
{
    public static readonly Error NotFound =
        new("User.NotFound", "Người dùng không tồn tại.");

    public static readonly Error UsernameAlreadyExists =
        new("User.UsernameAlreadyExists", "Tên đăng nhập đã được sử dụng.");

    public static readonly Error CannotDeleteSelf =
        new("User.CannotDeleteSelf", "Không thể xóa tài khoản đang đăng nhập.");
}
