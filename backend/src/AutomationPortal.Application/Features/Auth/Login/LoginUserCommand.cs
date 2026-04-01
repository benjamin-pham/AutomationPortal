using AutomationPortal.Application.Abstractions.Messaging;
using AutomationPortal.Application.Shared.Dtos;

namespace AutomationPortal.Application.Features.Auth.Login;

public sealed record LoginUserCommand(
    string Username,
    string Password) : ICommand<TokenResponse>;


