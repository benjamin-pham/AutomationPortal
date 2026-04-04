using AutomationPortal.Domain.Abstractions;
using MediatR;

namespace AutomationPortal.Application.Features.GeminiKeys.UpdateGeminiKey;

public sealed record UpdateGeminiKeyCommand(
    Guid Id,
    string Name,
    string? KeyValue,
    Guid UserId,
    bool ReplaceExisting = false) : IRequest<Result>;
