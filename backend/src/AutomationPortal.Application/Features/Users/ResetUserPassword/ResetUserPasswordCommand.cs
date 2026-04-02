using AutomationPortal.Application.Abstractions.Messaging;

namespace AutomationPortal.Application.Features.Users.ResetUserPassword;

public sealed record ResetUserPasswordCommand(
    Guid UserId,
    string NewPassword,
    string ConfirmPassword) : ICommand;
