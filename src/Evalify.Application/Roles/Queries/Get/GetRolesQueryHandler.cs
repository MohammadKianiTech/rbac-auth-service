using Dapper;
using Evalify.Application.Abstractions.Data;
using Evalify.Application.Abstractions.Messaging;
using Evalify.Domain.Abstractions;

namespace Evalify.Application.Roles.Queries.Get;

internal sealed class GetRolesQueryHandler(ISqlConnectionFactory _sqlConnectionFactory) : IQueryHandler<GetRolesQuery, IReadOnlyList<RoleResponse>>
{
    public async Task<Result<IReadOnlyList<RoleResponse>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                r.id AS Id,
                r.name AS Name,
                r.is_active AS IsActive,
                p.id AS Id,
                p.name AS Name
            FROM roles r
            LEFT JOIN role_permissions rp ON r.id = rp.role_id
            LEFT JOIN permissions p ON rp.permission_id = p.id
            ORDER BY r.id
        """;

        var roleDict = new Dictionary<int, RoleResponse>();

        await connection.QueryAsync<RoleResponse, PermissionResponse, RoleResponse>(
            sql,
            (role, permission) =>
            {
                if (!roleDict.TryGetValue(role.Id, out var existingRole))
                {
                    existingRole = role;
                    roleDict.Add(role.Id, existingRole);
                }

                if (permission != null && permission.Id > 0)
                {
                    existingRole.Permissions.Add(permission);
                }

                return existingRole;
            },
            splitOn: "Id"
        );

        return Result.Success<IReadOnlyList<RoleResponse>>(roleDict.Values.ToList());
    }
}