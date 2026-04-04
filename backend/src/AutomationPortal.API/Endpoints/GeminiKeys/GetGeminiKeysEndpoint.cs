using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutomationPortal.Application.Features.GeminiKeys.GetGeminiKeys;
using AutomationPortal.Application.Shared;

namespace AutomationPortal.API.Endpoints.GeminiKeys;

internal sealed class GetGeminiKeysEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/gemini-keys", async (
            [AsParameters] GetGeminiKeysQuery query,
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
        .WithName("GetGeminiKeys")
        .WithTags("GeminiKeys")
        .Produces<PagedList<GeminiKeyListItemResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity);
    }
}
