namespace AutomationPortal.Application.Features.Users.GetUserById;

public sealed record UserDetailResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Username,
    string? Email,
    string? Phone,
    DateOnly? Birthday);
