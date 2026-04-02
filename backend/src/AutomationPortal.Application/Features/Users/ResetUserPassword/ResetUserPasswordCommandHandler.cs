using AutomationPortal.Application.Abstractions.Authentication;
using AutomationPortal.Application.Abstractions.Messaging;
using AutomationPortal.Domain.Abstractions;
using AutomationPortal.Domain.Errors;
using AutomationPortal.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace AutomationPortal.Application.Features.Users.ResetUserPassword;

internal sealed class ResetUserPasswordCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ILogger<ResetUserPasswordCommandHandler> logger)
    : ICommandHandler<ResetUserPasswordCommand>
{
    public async Task<Result> Handle(ResetUserPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning(
                "ResetUserPassword failed. UserId: {UserId}, Reason: NotFound",
                command.UserId);

            return Result.Failure(UserErrors.NotFound);
        }

        var newPasswordHash = passwordHasher.Hash(command.NewPassword);
        user.ResetPassword(newPasswordHash);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "ResetUserPassword successful. UserId: {UserId}",
            command.UserId);

        return Result.Success();
    }
}
