using AutomationPortal.Application.Abstractions.Messaging;
using AutomationPortal.Application.Shared;
using AutomationPortal.Application.Shared.Dtos;

namespace AutomationPortal.Application.Features.Users.GetUsers;

public sealed record GetUsersQuery(
    string? Search) : PagedListSearch, IQuery<PagedList<UserListItemResponse>>;
