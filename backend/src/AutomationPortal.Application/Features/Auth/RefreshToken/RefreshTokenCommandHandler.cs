using AutomationPortal.Application.Abstractions.Authentication;
using AutomationPortal.Application.Abstractions.Messaging;
using AutomationPortal.Application.Features.Auth.Login;
using AutomationPortal.Domain.Abstractions;
using AutomationPortal.Domain.Repositories;
using Microsoft.Extensions.Logging;
using AutomationPortal.Application.Shared.Dtos;

namespace AutomationPortal.Application.Features.Auth.RefreshToken;

internal sealed class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    IJwtTokenService jwtTokenService,
    IUnitOfWork unitOfWork,
    ILogger<RefreshTokenCommandHandler> logger)
    : ICommandHandler<RefreshTokenCommand, TokenResponse>
{
    private static readonly Error InvalidRefreshToken =
        new("User.InvalidRefreshToken", "Refresh token không hợp lệ hoặc đã hết hạn.");

    public async Task<Result<TokenResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var hashedToken = jwtTokenService.HashToken(request.RefreshToken);

        var user = await userRepository.GetByHashedRefreshTokenAsync(hashedToken, cancellationToken);

        if (user is null
            || user.RefreshTokenExpiresAt is null
            || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
        {
            return Result.Failure<TokenResponse>(InvalidRefreshToken);
        }

        var newAccessToken = jwtTokenService.GenerateAccessToken(user.Id);
        var newRefreshToken = jwtTokenService.GenerateRefreshToken();
        var newHashedToken = jwtTokenService.HashToken(newRefreshToken);

        user.SetRefreshToken(newHashedToken, DateTime.UtcNow.AddDays(7));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Refresh token rotated. UserId: {UserId}, Timestamp: {Timestamp}",
            user.Id,
            DateTime.UtcNow);

        return new TokenResponse(newAccessToken, newRefreshToken, 86400, "Bearer");
    }
}
