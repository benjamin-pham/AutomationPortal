using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutomationPortal.Application.Features.Auth.GetProfile;

namespace AutomationPortal.API.Endpoints.Auth;

internal sealed class GetProfileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/profile", async (
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetProfileQuery(), ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(
                    title: result.Error.Code,
                    detail: result.Error.Description,
                    statusCode: StatusCodes.Status404NotFound);
        })
        .RequireAuthorization()
        .WithName("GetProfile")
        .WithTags("Auth")
        .Produces<UserProfileResponse>()
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
    }
}
