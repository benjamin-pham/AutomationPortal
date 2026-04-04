using AutomationPortal.Application.Features.GeminiKeys.DeleteGeminiKey;
using AutomationPortal.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AutomationPortal.API.Endpoints.GeminiKeys;

public sealed class DeleteGeminiKeyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/gemini-keys/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new DeleteGeminiKeyCommand(id);
            var result = await sender.Send(command, ct);

            if (result.IsSuccess)
            {
                return Results.NoContent();
            }

            if (result.Error == GeminiKeyErrors.NotFound)
            {
                return Results.NotFound(new ProblemDetails
                {
                    Title = result.Error.Code,
                    Detail = result.Error.Description,
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Description,
                statusCode: StatusCodes.Status422UnprocessableEntity);
        })
        .RequireAuthorization()
        .WithName("DeleteGeminiKey")
        .WithTags("GeminiKeys")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity);
    }
}
