using AutomationPortal.Application.Abstractions.Authentication;
using AutomationPortal.Application.Abstractions.Messaging;
using AutomationPortal.Domain.Abstractions;
using AutomationPortal.Domain.Entities;
using AutomationPortal.Domain.Errors;
using AutomationPortal.Domain.Repositories;

namespace AutomationPortal.Application.Features.Users.CreateUser;

internal sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher)
    : ICommandHandler<CreateUserCommand, CreateUserResponse>
{
    public async Task<Result<CreateUserResponse>> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByUsernameAsync(command.Username, cancellationToken);
        if (existingUser is not null)
            return Result.Failure<CreateUserResponse>(UserErrors.UsernameAlreadyExists);

        var passwordHash = passwordHasher.Hash(command.Password);

        var user = User.Create(
            command.FirstName,
            command.LastName,
            command.Username,
            passwordHash,
            command.Email,
            command.Phone,
            command.Birthday);

        userRepository.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateUserResponse(user.Id, user.Username);
    }
}
