using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutomationPortal.Application.Features.Users.DeleteUser;
using AutomationPortal.Domain.Errors;

namespace AutomationPortal.API.Endpoints.Users;

internal sealed class DeleteUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/users/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new DeleteUserCommand(id);
            var result = await sender.Send(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            if (result.Error == UserErrors.NotFound)
                return Results.Problem(
                    title: result.Error.Code,
                    detail: result.Error.Description,
                    statusCode: StatusCodes.Status404NotFound);

            if (result.Error == UserErrors.CannotDeleteSelf)
                return Results.Problem(
                    title: result.Error.Code,
                    detail: result.Error.Description,
                    statusCode: StatusCodes.Status409Conflict);

            return Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Description,
                statusCode: StatusCodes.Status422UnprocessableEntity);
        })
        .RequireAuthorization()
        .WithName("DeleteUser")
        .WithTags("Users")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
    }
}
