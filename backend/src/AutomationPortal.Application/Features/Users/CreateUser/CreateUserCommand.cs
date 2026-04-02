using AutomationPortal.Application.Abstractions.Messaging;

namespace AutomationPortal.Application.Features.Users.CreateUser;

public sealed record CreateUserCommand(
    string FirstName,
    string LastName,
    string Username,
    string Password,
    string? Email,
    string? Phone,
    DateOnly? Birthday) : ICommand<CreateUserResponse>;
