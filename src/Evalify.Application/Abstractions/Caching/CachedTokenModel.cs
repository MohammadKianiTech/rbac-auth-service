namespace Evalify.Application.Abstractions.Caching;

public record CachedTokenModel(
    Guid UserId,
    string AccessTokenHash,
    DateTime AccessTokenExpiresDateTime,
    string RefreshTokenIdHash,
    DateTime RefreshTokenExpiresDateTime
);