using AutomationPortal.Application.Features.GeminiKeys.UpdateGeminiKey;
using AutomationPortal.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AutomationPortal.API.Endpoints.GeminiKeys;

public sealed class UpdateGeminiKeyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/gemini-keys/{id:guid}", async (
            Guid id,
            UpdateGeminiKeyRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateGeminiKeyCommand(
                id,
                request.Name,
                request.KeyValue,
                request.UserId,
                request.ReplaceExisting);

            var result = await sender.Send(command, ct);

            if (result.IsSuccess)
            {
                return Results.NoContent();
            }

            if (result.Error == GeminiKeyErrors.NotFound)
            {
                return Results.Problem(
                    title: result.Error.Code,
                    detail: result.Error.Description,
                    statusCode: StatusCodes.Status404NotFound);
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
        .WithName("UpdateGeminiKey")
        .WithTags("GeminiKeys")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
        .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity);
    }
}

public sealed record UpdateGeminiKeyRequest(
    string Name,
    string? KeyValue,
    Guid UserId,
    bool ReplaceExisting = false);
