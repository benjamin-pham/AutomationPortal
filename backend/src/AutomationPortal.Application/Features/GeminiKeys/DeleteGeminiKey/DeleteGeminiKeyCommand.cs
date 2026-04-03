using AutomationPortal.Domain.Abstractions;
using MediatR;

namespace AutomationPortal.Application.Features.GeminiKeys.DeleteGeminiKey;

public sealed record DeleteGeminiKeyCommand(Guid Id) : IRequest<Result>;
