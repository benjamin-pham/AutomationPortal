using AutomationPortal.Application.Abstractions.Messaging;
using AutomationPortal.Domain.Abstractions;
using AutomationPortal.Domain.Errors;
using AutomationPortal.Domain.Repositories;

namespace AutomationPortal.Application.Features.Users.UpdateUser;

internal sealed class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateUserCommand>
{
    public async Task<Result> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        user.UpdateProfile(
            command.FirstName,
            command.LastName,
            command.Email,
            command.Phone,
            command.Birthday);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
