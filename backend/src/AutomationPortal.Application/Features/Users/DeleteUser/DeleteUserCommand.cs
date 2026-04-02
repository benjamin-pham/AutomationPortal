using AutomationPortal.Application.Abstractions.Messaging;

namespace AutomationPortal.Application.Features.Users.DeleteUser;

public sealed record DeleteUserCommand(Guid UserId) : ICommand;
