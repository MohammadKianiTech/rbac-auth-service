using Evalify.Domain.Abstractions;
using Evalify.Domain.Entities;

namespace Evalify.Application.Abstractions.Authentication;

public interface IJwtService
{
    Result<(string accessToken, string refreshToken, string refreshTokenSerial, int AccessTokenExpirationMinutes, int RefreshTokenExpirationMinutes)> CreateJwtTokens(User user);
    Task<Result<(string serialNumber, Guid userId)>> GetRefreshTokenClaimsAsync(string refreshToken);
}