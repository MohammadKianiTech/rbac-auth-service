using Evalify.Application.Abstractions.Messaging;

namespace Evalify.Application.Users.Queries.Get;

public sealed record GetUserQuery() : IQuery<UserResponse>;