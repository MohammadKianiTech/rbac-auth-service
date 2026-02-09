using Evalify.Application.Abstractions.Authentication;
using Evalify.Application.Abstractions.Caching;
using Evalify.Application.Abstractions.Clock;
using Evalify.Application.Abstractions.Hashing;
using Evalify.Domain.Entities;
using Evalify.Domain.Enums;
using Evalify.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Evalify.Infrastructure.Repositories;


internal sealed class TokenRepository : Repository<Token>, ITokenRepository
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICacheService _cacheService;

    public TokenRepository(ApplicationDbContext dbContext, IPasswordHasher passwordHasher, IDateTimeProvider dateTimeProvider, ICacheService cacheService)
        : base(dbContext)
    {
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
        _cacheService = cacheService;
    }

    public async Task<Token?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<Token>().Include(c => c.User).FirstOrDefaultAsync(x => x.RefreshTokenIdHash == refreshToken, cancellationToken);
    }

    public async Task<bool> IsValidTokenAsync(string accessToken, Guid userId)
    {
        var accessTokenHash = _passwordHasher.GetSha256Hash(accessToken);
        //1) Token VALIDATION (REDIS FIRST)
        string tokenKey = $"token:{userId}";
        var cachedToken = await _cacheService.GetAsync<CachedTokenModel>(tokenKey);
        if (cachedToken != null)
        {
            return cachedToken.AccessTokenExpiresDateTime >= _dateTimeProvider.UtcNow;
        }
        var userToken = await DbContext.Set<Token>().FirstOrDefaultAsync(x => x.AccessTokenHash == accessTokenHash && x.UserId == userId && x.Client == TokenClientType.Web);
        return userToken?.AccessTokenExpiresDateTime >= _dateTimeProvider.UtcNow;
    }
}