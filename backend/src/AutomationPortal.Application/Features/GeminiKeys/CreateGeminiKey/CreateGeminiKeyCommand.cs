using AutomationPortal.Domain.Abstractions;
using MediatR;

namespace AutomationPortal.Application.Features.GeminiKeys.CreateGeminiKey;

public sealed record CreateGeminiKeyCommand(
    string Name,
    string KeyValue,
    Guid UserId,
    bool ReplaceExisting = false) : IRequest<Result<CreateGeminiKeyResponse>>;

public sealed record CreateGeminiKeyResponse(Guid Id, string Name);
