using AutomationPortal.Application.Features.GeminiKeys.CreateGeminiKey;
using AutomationPortal.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AutomationPortal.API.Endpoints.GeminiKeys;

public sealed class CreateGeminiKeyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/gemini-keys", async (
            CreateGeminiKeyCommand command,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);

            if (result.IsSuccess)
            {
                return Results.Created($"/api/gemini-keys/{result.Value.Id}", result.Value);
            }

            if (result.Error == GeminiKeyErrors.UserAlreadyHasKey)
            {
                return Results.Conflict(new ProblemDetails
                {
                    Title = result.Error.Code,
                    Detail = result.Error.Description,
                    Status = StatusCodes.Status409Conflict
                });
            }

            return Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Description,
                statusCode: StatusCodes.Status422UnprocessableEntity);
        })
        .RequireAuthorization()
        .WithName("CreateGeminiKey")
        .WithTags("GeminiKeys")
        .Produces<CreateGeminiKeyResponse>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
        .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity);
    }
}
