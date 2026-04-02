using AutomationPortal.Application.Abstractions.Messaging;
using AutomationPortal.Domain.Abstractions;
using AutomationPortal.Domain.Errors;
using AutomationPortal.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace AutomationPortal.Application.Features.Users.DeleteUser;

internal sealed class DeleteUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    ILogger<DeleteUserCommandHandler> logger)
    : ICommandHandler<DeleteUserCommand>
{
    public async Task<Result> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        if (command.UserId == userContext.UserId)
        {
            logger.LogWarning(
                "DeleteUser failed. UserId: {UserId}, Reason: CannotDeleteSelf",
                command.UserId);

            return Result.Failure(UserErrors.CannotDeleteSelf);
        }

        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning(
                "DeleteUser failed. UserId: {UserId}, Reason: NotFound",
                command.UserId);

            return Result.Failure(UserErrors.NotFound);
        }

        userRepository.Remove(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "DeleteUser successful. UserId: {UserId}",
            command.UserId);

        return Result.Success();
    }
}
