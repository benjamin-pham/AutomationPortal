using Dapper;
using AutomationPortal.Application.Abstractions.Data;
using AutomationPortal.Application.Abstractions.Messaging;
using AutomationPortal.Domain.Abstractions;
using AutomationPortal.Domain.Errors;

namespace AutomationPortal.Application.Features.Users.GetUserById;

internal sealed class GetUserByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetUserByIdQuery, UserDetailResponse>
{
    public async Task<Result<UserDetailResponse>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                id         AS UserId,
                first_name AS FirstName,
                last_name  AS LastName,
                username   AS Username,
                email      AS Email,
                phone      AS Phone,
                birthday   AS Birthday
            FROM users
            WHERE id = @UserId
              AND is_deleted = false
            """;

        var user = await connection.QuerySingleOrDefaultAsync<UserDetailResponse>(
            sql, new { request.UserId });

        if (user is null)
            return Result.Failure<UserDetailResponse>(UserErrors.NotFound);

        return user;
    }
}
