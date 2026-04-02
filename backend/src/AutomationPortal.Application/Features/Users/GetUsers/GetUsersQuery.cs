using AutomationPortal.Application.Abstractions.Messaging;
using AutomationPortal.Application.Shared.Dtos;

namespace AutomationPortal.Application.Features.Users.GetUsers;

public sealed record GetUsersQuery(
    string? Search,
    string? SortColumn,
    string? SortDirection,
    int Page,
    int PageSize) : IQuery<PagedResponse<UserListItemResponse>>;
