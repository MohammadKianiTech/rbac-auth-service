using Evalify.Domain.Entities;

namespace Evalify.Domain.Repositories;

public interface ITokenRepository
{
    Task<Token?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> IsValidTokenAsync(string accessToken, Guid userId);
    Task AddAsync(Token token);
    void Remove(Token token);
}