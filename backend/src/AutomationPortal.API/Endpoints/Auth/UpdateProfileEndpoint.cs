using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutomationPortal.Application.Features.Auth.UpdateProfile;

namespace AutomationPortal.API.Endpoints.Auth;

internal sealed class UpdateProfileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/auth/profile", async (
            UpdateProfileCommand command,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.Problem(
                    title: result.Error.Code,
                    detail: result.Error.Description,
                    statusCode: StatusCodes.Status404NotFound);
        })
        .RequireAuthorization()
        .WithName("UpdateProfile")
        .WithTags("Auth")
        .Produces(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
    }
}
