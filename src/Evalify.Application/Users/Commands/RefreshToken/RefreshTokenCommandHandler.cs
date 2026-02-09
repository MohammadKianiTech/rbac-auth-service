using Evalify.Application.Abstractions.Authentication;
using Evalify.Application.Abstractions.Caching;
using Evalify.Application.Abstractions.Clock;
using Evalify.Application.Abstractions.Hashing;
using Evalify.Application.Abstractions.Messaging;
using Evalify.Domain.Abstractions;
using Evalify.Domain.Entities;
using Evalify.Domain.Enums;
using Evalify.Domain.Errors;
using Evalify.Domain.Repositories;

namespace Evalify.Application.Users.Commands.RefreshToken;

internal sealed class RefreshTokenCommandHandler(
    IUnitOfWork _unitOfWork,
    ITokenRepository _tokenRepository,
    IJwtService _jwtService,
    IPasswordHasher _passwordHasher,
    IDateTimeProvider _dateTimeProvider,
    ICacheService _cacheService) : ICommandHandler<RefreshTokenCommand, RefrshTokenResponse>
{
    public async Task<Result<RefrshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshTokenClaimsResult = await _jwtService.GetRefreshTokenClaimsAsync(request.RefreshToken);
        var token = await _tokenRepository.GetByRefreshTokenAsync(refreshTokenClaimsResult.Value.serialNumber, cancellationToken);
        if (token is null || token.UserId != refreshTokenClaimsResult.Value.userId)
        {
            return Result.Failure<RefrshTokenResponse>(TokenErrors.NotFound);
        }
        var tokenResult = _jwtService.CreateJwtTokens(token.User!);

        if (tokenResult.IsFailure)
        {
            return Result.Failure<RefrshTokenResponse>(UserErrors.InvalidCredentials);
        }

        var result = new RefrshTokenResponse(tokenResult.Value.accessToken, tokenResult.Value.refreshToken);

        var newToken = Token.Create(token.UserId, TokenClientType.Web, _passwordHasher.GetSha256Hash(result.AccessToken), _dateTimeProvider.UtcNow.AddMinutes(tokenResult.Value.AccessTokenExpirationMinutes), tokenResult.Value.refreshTokenSerial, _dateTimeProvider.UtcNow.AddMinutes(tokenResult.Value.RefreshTokenExpirationMinutes));

        await _tokenRepository.AddAsync(newToken);

        _tokenRepository.Remove(token);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        //INFO: Consider creating dedicated caching services for tokens and users.
        //Set token in the redis
        string cacheTokenKey = $"token:{newToken.UserId}";
        var cachedToken = new CachedTokenModel(newToken.UserId, newToken.AccessTokenHash, newToken.AccessTokenExpiresDateTime, newToken.RefreshTokenIdHash, newToken.RefreshTokenExpiresDateTime);
        await _cacheService.SetAsync(cacheTokenKey, cachedToken, TimeSpan.FromMinutes(tokenResult.Value.AccessTokenExpirationMinutes), cancellationToken);

        return Result.Success(result);
    }
}