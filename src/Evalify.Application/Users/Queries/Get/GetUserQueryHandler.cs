using System.Data;
using Dapper;
using Evalify.Application.Abstractions.Authentication;
using Evalify.Application.Abstractions.Data;
using Evalify.Application.Abstractions.Messaging;
using Evalify.Domain.Abstractions;
using Evalify.Domain.Errors;

namespace Evalify.Application.Users.Queries.Get;

internal sealed class GetUserQueryHandler(ISqlConnectionFactory _sqlConnectionFactory, IUserContext _userContext) : IQueryHandler<GetUserQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();
        const string sql = """
            SELECT
                u.id AS Id,
                u.first_name AS FirstName,
                u.last_name AS LastName,
                u.email AS Email,
                CASE u.status
                    WHEN 1 THEN 'Active'
                    WHEN 2 THEN 'Inactive'
                    WHEN 3 THEN 'Pending'
                    WHEN 4 THEN 'Suspended'
                    ELSE 'Unknown'
                END AS Status
            FROM users AS u
            WHERE u.id = @id
        """;
        var parameters = new DynamicParameters();
        parameters.Add("@id", _userContext.UserId, DbType.Guid);
        var user = await connection.QuerySingleOrDefaultAsync<UserResponse>(sql, parameters);
        if (user is null || user.Id != _userContext.UserId)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound);
        }
        return Result.Success(user);
    }
}