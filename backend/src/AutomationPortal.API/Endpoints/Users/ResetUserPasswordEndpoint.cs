using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutomationPortal.Application.Features.Users.ResetUserPassword;
using AutomationPortal.Domain.Errors;

namespace AutomationPortal.API.Endpoints.Users;

internal sealed class ResetUserPasswordEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users/{id:guid}/reset-password", async (
            Guid id,
            ResetUserPasswordRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new ResetUserPasswordCommand(id, request.NewPassword, request.ConfirmPassword);
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
        .WithName("ResetUserPassword")
        .WithTags("Users")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity);
    }
}

internal sealed record ResetUserPasswordRequest(string NewPassword, string ConfirmPassword);
