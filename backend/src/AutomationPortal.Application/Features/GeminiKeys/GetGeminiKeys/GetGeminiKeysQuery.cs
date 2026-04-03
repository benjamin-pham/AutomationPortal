using AutomationPortal.Application.Abstractions.Messaging;
using AutomationPortal.Application.Shared;

namespace AutomationPortal.Application.Features.GeminiKeys.GetGeminiKeys;

public sealed record GetGeminiKeysQuery : PagedListSearch, IQuery<PagedList<GeminiKeyListItemResponse>>;

public sealed record GeminiKeyListItemResponse(
    Guid Id,
    string Name,
    string MaskedKey,
    Guid AssignedUserId,
    string AssignedUsername,
    DateTime CreatedAt);
