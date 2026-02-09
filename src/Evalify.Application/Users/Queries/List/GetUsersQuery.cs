using Evalify.Application.Abstractions.Messaging;

namespace Evalify.Application.Users.Queries.List;

public sealed record GetUsersQuery() : IQuery<IReadOnlyList<UserResponse>>;