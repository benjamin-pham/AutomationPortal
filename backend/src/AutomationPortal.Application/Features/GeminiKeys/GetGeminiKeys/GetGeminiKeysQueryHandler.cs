using Dapper;
using AutomationPortal.Application.Abstractions.Data;
using AutomationPortal.Application.Abstractions.Messaging;
using AutomationPortal.Application.Shared;
using AutomationPortal.Domain.Abstractions;
using Microsoft.Extensions.Logging;

namespace AutomationPortal.Application.Features.GeminiKeys.GetGeminiKeys;

internal sealed class GetGeminiKeysQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    ILogger<GetGeminiKeysQueryHandler> logger)
    : IQueryHandler<GetGeminiKeysQuery, PagedList<GeminiKeyListItemResponse>>
{
    public async Task<Result<PagedList<GeminiKeyListItemResponse>>> Handle(
        GetGeminiKeysQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber ?? 1;
        var pageSize = request.PageSize ?? 10;
        var offset = (pageNumber - 1) * pageSize;

        using var connection = sqlConnectionFactory.CreateConnection();

        const string countSql = """
            SELECT COUNT(*)
            FROM gemini_keys
            WHERE is_deleted = false
            """;

        var totalItems = await connection.ExecuteScalarAsync<int>(countSql);

        const string dataSql = """
            SELECT
                gk.id           AS Id,
                gk.name         AS Name,
                gk.key_value    AS KeyValue,
                gk.user_id      AS AssignedUserId,
                u.username      AS AssignedUsername,
                gk.created_at   AS CreatedAt
            FROM gemini_keys gk
            LEFT JOIN users u ON u.id = gk.user_id AND u.is_deleted = false
            WHERE gk.is_deleted = false
            ORDER BY gk.created_at DESC
            LIMIT @PageSize OFFSET @Offset
            """;

        var rows = await connection.QueryAsync<GeminiKeyRow>(
            dataSql, new { PageSize = pageSize, Offset = offset });

        var items = rows.Select(r => new GeminiKeyListItemResponse(
            r.Id,
            r.Name,
            r.KeyValue.Length >= 4 ? "****" + r.KeyValue[^4..] : "****",
            r.AssignedUserId,
            r.AssignedUsername ?? string.Empty,
            r.CreatedAt)).ToList();

        logger.LogInformation(
            "GetGeminiKeys successful. TotalItems: {TotalItems}, Page: {Page}, PageSize: {PageSize}",
            totalItems,
            pageNumber,
            pageSize);

        return new PagedList<GeminiKeyListItemResponse>(
            items, pageNumber, pageSize, totalItems);
    }

    private sealed class GeminiKeyRow
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public string KeyValue { get; init; } = default!;
        public Guid AssignedUserId { get; init; }
        public string? AssignedUsername { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
