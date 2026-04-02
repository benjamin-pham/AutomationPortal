namespace AutomationPortal.Application.Shared.Dtos;

public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int TotalItems,
    int TotalPages,
    int Page,
    int PageSize);
