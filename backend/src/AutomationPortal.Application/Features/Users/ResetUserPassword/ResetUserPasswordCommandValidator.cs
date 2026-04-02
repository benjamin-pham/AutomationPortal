using FluentValidation;
using AutomationPortal.Application.Shared.RuleValidator;

namespace AutomationPortal.Application.Features.Users.ResetUserPassword;

public sealed class ResetUserPasswordCommandValidator : AbstractValidator<ResetUserPasswordCommand>
{
    public ResetUserPasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Id người dùng không được để trống.");

        RuleFor(x => x.NewPassword).SetValidator(new PasswordValidator());

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword).WithMessage("Xác nhận mật khẩu không khớp.");
    }
}
