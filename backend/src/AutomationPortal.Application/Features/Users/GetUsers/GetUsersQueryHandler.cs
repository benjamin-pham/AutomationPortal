using Dapper;
using AutomationPortal.Application.Abstractions.Data;
using AutomationPortal.Application.Abstractions.Messaging;
using AutomationPortal.Application.Shared.Dtos;
using AutomationPortal.Domain.Abstractions;
using Microsoft.Extensions.Logging;
using AutomationPortal.Application.Shared;

namespace AutomationPortal.Application.Features.Users.GetUsers;

internal sealed class GetUsersQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    ILogger<GetUsersQueryHandler> logger)
    : IQueryHandler<GetUsersQuery, PagedList<UserListItemResponse>>
{
    private static readonly HashSet<string> AllowedSortColumns =
    [
        "first_name", "last_name", "username", "email", "phone", "birthday"
    ];

    public async Task<Result<PagedList<UserListItemResponse>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var safeColumn = AllowedSortColumns.Contains(request.SortBy ?? "")
            ? request.SortBy!
            : "username";
        var safeDirection = request.SortDirection?.ToUpperInvariant() == "DESC" ? "DESC" : "ASC";
        var offset = (request.PageNumber - 1) * request.PageSize;
        var searchParam = string.IsNullOrWhiteSpace(request.Search) ? null : request.Search;

        using var connection = sqlConnectionFactory.CreateConnection();

        const string countSql = """
            SELECT COUNT(*)
            FROM users
            WHERE is_deleted = false
              AND (@Search IS NULL OR
                   first_name ILIKE '%' || @Search || '%' OR
                   last_name  ILIKE '%' || @Search || '%' OR
                   username   ILIKE '%' || @Search || '%' OR
                   email      ILIKE '%' || @Search || '%')
            """;

        var totalItems = await connection.ExecuteScalarAsync<int>(
            countSql, new { Search = searchParam });

        var dataSql = $"""
            SELECT
                id         AS Id,
                first_name AS FirstName,
                last_name  AS LastName,
                username   AS Username,
                email      AS Email,
                phone      AS Phone,
                birthday   AS Birthday
            FROM users
            WHERE is_deleted = false
              AND (@Search IS NULL OR
                   first_name ILIKE '%' || @Search || '%' OR
                   last_name  ILIKE '%' || @Search || '%' OR
                   username   ILIKE '%' || @Search || '%' OR
                   email      ILIKE '%' || @Search || '%')
            ORDER BY {safeColumn} {safeDirection}
            LIMIT @PageSize OFFSET @Offset
            """;

        var items = await connection.QueryAsync<UserListItemResponse>(
            dataSql, new { Search = searchParam, PageSize = request.PageSize, Offset = offset });

        var totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

        logger.LogInformation(
            "GetUsers successful. TotalItems: {TotalItems}, Page: {Page}, PageSize: {PageSize}",
            totalItems,
            request.PageNumber,
            request.PageSize);

        return new PagedList<UserListItemResponse>(
            items.ToList(), request.PageNumber, request.PageSize, totalItems);
    }
}
