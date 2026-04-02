using FluentValidation;
using AutomationPortal.Application.Shared.RuleValidator;

namespace AutomationPortal.Application.Features.Users.CreateUser;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Họ không được để trống.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Tên không được để trống.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Tên đăng nhập không được để trống.")
            .MinimumLength(3).WithMessage("Tên đăng nhập phải có ít nhất 3 ký tự.")
            .MaximumLength(50).WithMessage("Tên đăng nhập không vượt quá 50 ký tự.")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới.");

        RuleFor(x => x.Password).SetValidator(new PasswordValidator());
    }
}
