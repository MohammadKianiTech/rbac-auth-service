using Evalify.Application.Abstractions.Authentication;
using Evalify.Application.Abstractions.Caching;
using Evalify.Application.Abstractions.Messaging;
using Evalify.Domain.Abstractions;
using Evalify.Domain.Repositories;

namespace Evalify.Application.Users.Commands.Logout;

internal sealed class LogoutCommandHanler(
    IUnitOfWork _unitOfWork,
    ITokenRepository _tokenRepository,
    IJwtService _jwtService,
    ICacheService _cacheService) : ICommandHandler<LogoutCommand>
{

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var refreshTokenClaimsResult = await _jwtService.GetRefreshTokenClaimsAsync(request.RefreshToken);
        var token = await _tokenRepository.GetByRefreshTokenAsync(refreshTokenClaimsResult.Value.serialNumber, cancellationToken);
        if (token != null)
        {
            //Remove token & user from redis
            string cacheTokenKey = $"token:{token.UserId}";
            await _cacheService.RemoveAsync(cacheTokenKey, cancellationToken);
            string cacheUserKey = $"user:{token.UserId}";
            await _cacheService.RemoveAsync(cacheUserKey, cancellationToken);

            _tokenRepository.Remove(token);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return Result.Success();
    }
}