using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutomationPortal.Application.Features.Users.UpdateUser;
using AutomationPortal.Domain.Errors;

namespace AutomationPortal.API.Endpoints.Users;

internal sealed class UpdateUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/users/{id:guid}", async (
            Guid id,
            UpdateUserRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateUserCommand(
                id,
                request.FirstName,
                request.LastName,
                request.Email,
                request.Phone,
                request.Birthday);

            var result = await sender.Send(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            if (result.Error == UserErrors.NotFound)
                return Results.Problem(
                    title: result.Error.Code,
                    detail: result.Error.Description,
                    statusCode: StatusCodes.Status404NotFound);

            return Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Description,
                statusCode: StatusCodes.Status422UnprocessableEntity);
        })
        .RequireAuthorization()
        .WithName("UpdateUser")
        .WithTags("Users")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity);
    }
}

internal sealed record UpdateUserRequest(
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    DateOnly? Birthday);
