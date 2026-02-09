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
using Evalify.Domain.ValueObjects;

namespace Evalify.Application.Users.Commands.Login;

internal sealed class LoginCommandHandler(
    IUnitOfWork _unitOfWork,
    IUserRepository _userRepository,
    ITokenRepository _tokenRepository,
    IJwtService _jwtService,
    IPasswordHasher _passwordHasher,
    IDateTimeProvider _dateTimeProvider,
    ICacheService _cacheService) : ICommandHandler<LoginCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email);

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            return Result.Failure<LoginResponse>(UserErrors.InvalidCredentials);
        }

        var passwordIsOK = _passwordHasher.Verify(request.Password, user.PasswordHash.Value);

        if (!passwordIsOK)
        {
            return Result.Failure<LoginResponse>(UserErrors.InvalidCredentials);
        }

        var tokenResult = _jwtService.CreateJwtTokens(user);

        if (tokenResult.IsFailure)
        {
            return Result.Failure<LoginResponse>(UserErrors.InvalidCredentials);
        }

        var result = new LoginResponse(tokenResult.Value.accessToken, tokenResult.Value.refreshToken);

        var token = Token.Create(user.Id, TokenClientType.Web, _passwordHasher.GetSha256Hash(result.AccessToken), _dateTimeProvider.UtcNow.AddMinutes(tokenResult.Value.AccessTokenExpirationMinutes), tokenResult.Value.refreshTokenSerial, _dateTimeProvider.UtcNow.AddMinutes(tokenResult.Value.RefreshTokenExpirationMinutes));

        await _tokenRepository.AddAsync(token);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        //INFO: Consider creating dedicated caching services for tokens and users.
        //Set token in the redis
        string cacheTokenKey = $"token:{user.Id}";
        var cachedToken = new CachedTokenModel(user.Id, token.AccessTokenHash, token.AccessTokenExpiresDateTime, token.RefreshTokenIdHash, token.RefreshTokenExpiresDateTime);
        await _cacheService.SetAsync(cacheTokenKey, cachedToken, TimeSpan.FromMinutes(tokenResult.Value.AccessTokenExpirationMinutes), cancellationToken);
        //Set user in the redis
        string cacheUserKey = $"user:{user.Id}";
        var cachedUser = new CachedUserModel(user.Id, user.FirstName.Value, user.LastName.Value, user.Email.Value, SerialNumber: user.SerialNumber.Value, user.RoleId.Value);
        await _cacheService.SetAsync(cacheUserKey, cachedUser, null, cancellationToken);

        return Result.Success(result);
    }
}