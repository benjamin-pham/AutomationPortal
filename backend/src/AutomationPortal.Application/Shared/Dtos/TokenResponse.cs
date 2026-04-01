using System;

namespace AutomationPortal.Application.Shared.Dtos;

public sealed record TokenResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string TokenType);