namespace AutomationPortal.Application.Features.Users.GetUsers;

public sealed record UserListItemResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Username,
    string? Email,
    string? Phone,
    DateOnly? Birthday);
