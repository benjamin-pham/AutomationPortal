using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutomationPortal.Application.Features.Users.GetUsers;
using AutomationPortal.Application.Shared.Dtos;

namespace AutomationPortal.API.Endpoints.Users;

internal sealed class GetUsersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users", async (
            [AsParameters] GetUsersQueryParams queryParams,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new GetUsersQuery(
                queryParams.Search,
                queryParams.SortColumn,
                queryParams.SortDirection,
                queryParams.Page ?? 1,
                queryParams.PageSize ?? 20);

            var result = await sender.Send(query, ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(
                    title: result.Error.Code,
                    detail: result.Error.Description,
                    statusCode: StatusCodes.Status422UnprocessableEntity);
        })
        .RequireAuthorization()
        .WithName("GetUsers")
        .WithTags("Users")
        .Produces<PagedResponse<UserListItemResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity);
    }
}

internal sealed class GetUsersQueryParams
{
    public string? Search { get; set; }
    public string? SortColumn { get; set; }
    public string? SortDirection { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}
