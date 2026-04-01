using AutomationPortal.Application.Abstractions.Authentication;
using AutomationPortal.Application.Abstractions.Messaging;
using AutomationPortal.Domain.Abstractions;
using AutomationPortal.Domain.Entities;
using AutomationPortal.Domain.Repositories;

namespace AutomationPortal.Application.Features.Auth.Register;

internal sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher)
    : ICommandHandler<RegisterUserCommand, RegisterUserResponse>
{
    private static readonly Error UsernameAlreadyTaken =
        new("User.UsernameAlreadyTaken", "Username đã được sử dụng.");

    public async Task<Result<RegisterUserResponse>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var existing = await userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (existing is not null)
            return Result.Failure<RegisterUserResponse>(UsernameAlreadyTaken);

        var passwordHash = passwordHasher.Hash(request.Password);

        var user = User.Create(
            request.FirstName,
            request.LastName,
            request.Username,
            passwordHash,
            request.Email,
            request.Phone,
            request.Birthday);

        userRepository.Add(user);

        return new RegisterUserResponse(user.Id, user.Username, user.FirstName, user.LastName);
    }
}
