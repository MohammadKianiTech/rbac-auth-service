using Dapper;
using Evalify.Application.Abstractions.Data;
using Evalify.Application.Abstractions.Messaging;
using Evalify.Domain.Abstractions;

namespace Evalify.Application.Users.Queries.List;

internal sealed class GetUsersQueryHandler(ISqlConnectionFactory _sqlConnectionFactory) : IQueryHandler<GetUsersQuery, IReadOnlyList<UserResponse>>
{
    public async Task<Result<IReadOnlyList<UserResponse>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
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
                END AS Status,
                r.name AS Role
            FROM users u
            LEFT JOIN roles r ON u.role_id = r.id
            ORDER BY u.last_name
        """;

        var users = await connection.QueryAsync<UserResponse>(sql);

        return users.ToList();
    }
}