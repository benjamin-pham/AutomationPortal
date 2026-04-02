using AutomationPortal.Application.Abstractions.Messaging;

namespace AutomationPortal.Application.Features.Users.UpdateUser;

public sealed record UpdateUserCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    DateOnly? Birthday) : ICommand;
