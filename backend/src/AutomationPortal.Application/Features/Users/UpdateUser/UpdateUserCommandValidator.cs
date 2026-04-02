using FluentValidation;

namespace AutomationPortal.Application.Features.Users.UpdateUser;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Id người dùng không được để trống.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Họ không được để trống.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Tên không được để trống.");
    }
}
