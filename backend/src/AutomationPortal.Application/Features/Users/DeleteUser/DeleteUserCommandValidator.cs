using FluentValidation;

namespace AutomationPortal.Application.Features.Users.DeleteUser;

public sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Id người dùng không được để trống.");
    }
}
