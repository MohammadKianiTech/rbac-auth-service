using Evalify.Application.Abstractions.Messaging;

namespace Evalify.Application.Roles.Queries.Get;

public sealed record GetRolesQuery() : IQuery<IReadOnlyList<RoleResponse>>;