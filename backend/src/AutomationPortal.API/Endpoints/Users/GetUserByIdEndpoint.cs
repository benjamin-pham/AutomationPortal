using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutomationPortal.Application.Features.Users.GetUserById;

namespace AutomationPortal.API.Endpoints.Users;

internal sealed class GetUserByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetUserByIdQuery(id), ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(
                    title: result.Error.Code,
                    detail: result.Error.Description,
                    statusCode: StatusCodes.Status404NotFound);
        })
        .RequireAuthorization()
        .WithName("GetUserById")
        .WithTags("Users")
        .Produces<UserDetailResponse>()
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
    }
}
