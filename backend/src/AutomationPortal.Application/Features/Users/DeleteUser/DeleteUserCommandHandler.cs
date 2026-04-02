using AutomationPortal.Application.Abstractions.Messaging;
using AutomationPortal.Domain.Abstractions;
using AutomationPortal.Domain.Errors;
using AutomationPortal.Domain.Repositories;

namespace AutomationPortal.Application.Features.Users.DeleteUser;

internal sealed class DeleteUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext)
    : ICommandHandler<DeleteUserCommand>
{
    public async Task<Result> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        if (command.UserId == userContext.UserId)
            return Result.Failure(UserErrors.CannotDeleteSelf);

        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        userRepository.Remove(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
