using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutomationPortal.Application.Features.Users.GetUsers;
using AutomationPortal.Application.Shared.Dtos;
using AutomationPortal.Application.Shared;

namespace AutomationPortal.API.Endpoints.Users;

internal sealed class GetUsersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users", async (
            [AsParameters] GetUsersQuery query,
            ISender sender,
            CancellationToken ct) =>
        {
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
        .Produces<PagedList<UserListItemResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity);
    }
}
