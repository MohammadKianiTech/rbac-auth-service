using System.Security.Claims;
using Evalify.Application.Abstractions.Caching;
using Evalify.Domain.Enums;
using Evalify.Domain.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Evalify.Infrastructure.Authentication;

public interface ITokenValidatorService
{
    Task ValidateAsync(TokenValidatedContext context);
}
public class TokenValidatorService(
    IUserRepository _usersService,
    ITokenRepository _tokenStoreService,
    ICacheService _cacheService) : ITokenValidatorService
{
    public async Task ValidateAsync(TokenValidatedContext context)
    {
        var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
        if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
        {
            context.Fail("This is not our issued token. It has no claims.");
            return;
        }

        var serialNumberClaim = claimsIdentity.FindFirst(ClaimTypes.SerialNumber);
        if (serialNumberClaim == null)
        {
            context.Fail("This is not our issued token. It has no serial.");
            return;
        }

        var userIdString = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            context.Fail("This is not our issued token. It has no user-id.");
            return;
        }
        //1) USER VALIDATION (REDIS FIRST)
        string userKey = $"user:{userId}";
        var cachedUser = await _cacheService.GetAsync<CachedUserModel>(userKey);
        if (cachedUser is not null)
        {
            // user found in Redis → no need DB
            if (cachedUser.SerialNumber.ToString() != serialNumberClaim.Value)
            {
                context.Fail("Serial mismatch. Please login again.");
                return;
            }
        }
        else
        {
            var user = await _usersService.GetByIdAsync(userId);
            //***INFO => User must be saved in redis
            if (user == null || user.SerialNumber.Value.ToString() != serialNumberClaim.Value || user.Status != UserStatus.Active)
            {
                // user has changed his/her password/roles/stat/IsActive
                context.Fail("This token is expired. Please login again.");
            }
        }

        if (context.SecurityToken is not JsonWebToken)
        {
            context.Fail("This token is not in our database.");
            return;
        }
        var rawToken = context.Request.Headers["Authorization"].ToString()
                .Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(rawToken) || !await _tokenStoreService.IsValidTokenAsync(rawToken, userId))
        {
            context.Fail("This token is not in our database.");
            return;
        }

        //INFO => Update User Last Activity Date 
    }
}