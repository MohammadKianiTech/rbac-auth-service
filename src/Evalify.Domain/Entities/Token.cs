namespace Evalify.Domain.Entities;

public sealed class Token : Entity
{
    private Token(Guid id, Guid userId, TokenClientType client, string accessTokenHash, DateTime accessTokenExpiresDateTime, string refreshTokenIdHash, DateTime refreshTokenExpiresDateTime) : base(id)
    {
        UserId = userId;
        Client = client;
        AccessTokenHash = accessTokenHash;
        AccessTokenExpiresDateTime = accessTokenExpiresDateTime;
        RefreshTokenIdHash = refreshTokenIdHash;
        RefreshTokenExpiresDateTime = refreshTokenExpiresDateTime;
    }
    private Token() { }
    public Guid UserId { get; private set; }
    public TokenClientType Client { get; private set; }
    public string AccessTokenHash { get; private set; } = default!;
    public DateTime AccessTokenExpiresDateTime { get; private set; }
    public string RefreshTokenIdHash { get; private set; } = default!;
    public DateTime RefreshTokenExpiresDateTime { get; private set; }
    public User? User { get; private set; }
    public static Token Create(Guid userId, TokenClientType client, string accessTokenHash, DateTime accessTokenExpiresDateTime, string refreshTokenIdHash, DateTime refreshTokenExpiresDateTime)
    {
        return new Token(
            Guid.NewGuid(),
            userId,
            client,
            accessTokenHash,
            accessTokenExpiresDateTime,
            refreshTokenIdHash,
            refreshTokenExpiresDateTime);
    }
}