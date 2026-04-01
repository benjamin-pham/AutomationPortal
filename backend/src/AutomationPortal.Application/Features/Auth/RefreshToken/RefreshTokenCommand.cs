using AutomationPortal.Application.Abstractions.Messaging;
using AutomationPortal.Application.Features.Auth.Login;
using AutomationPortal.Application.Shared.Dtos;

namespace AutomationPortal.Application.Features.Auth.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<TokenResponse>;
