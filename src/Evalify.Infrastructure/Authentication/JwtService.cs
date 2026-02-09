using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Evalify.Application.Abstractions.Authentication;
using Evalify.Domain.Abstractions;
using Evalify.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Evalify.Infrastructure.Authentication;

public sealed class JwtService(IOptions<JwtOptions> _configuration) : IJwtService
{
    public Result<(string accessToken, string refreshToken, string refreshTokenSerial, int AccessTokenExpirationMinutes, int RefreshTokenExpirationMinutes)> CreateJwtTokens(User user)
    {
        var result = GenerateToken(user);
        var (refreshTokenValue, refreshTokenSerial) = GenerateRefreshToken(user);
        return (result.accessToken, refreshTokenValue, refreshTokenSerial, result.AccessTokenExpirationMinutes, result.RefreshTokenExpirationMinutes);
    }
    public async Task<Result<(string serialNumber, Guid userId)>> GetRefreshTokenClaimsAsync(string refreshToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Result.Success((serialNumber: string.Empty, userId: Guid.Empty));
            }

            var handler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = _configuration.Value.Issuer,
                ValidAudience = _configuration.Value.Audience,
                RequireExpirationTime = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.Key)),
                ClockSkew = TimeSpan.Zero
            };

            var result = await handler.ValidateTokenAsync(refreshToken, validationParameters);

            if (!result.IsValid || result.Exception != null)
            {
                return Result.Success((serialNumber: string.Empty, userId: Guid.Empty));
            }

            var principal = result.ClaimsIdentity;

            string serialNumber = principal?.FindFirst(ClaimTypes.SerialNumber)?.Value ?? string.Empty;

            var userIdString = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            if (!Guid.TryParse(userIdString, out Guid userId) || string.IsNullOrEmpty(serialNumber))
            {
                return Result.Success((serialNumber: string.Empty, userId: Guid.Empty));
            }
            return Result.Success((serialNumber, userId));
        }
        catch (Exception)
        {
            return Result.Success((serialNumber: string.Empty, userId: Guid.Empty));
        }
    }

    private (string accessToken, int AccessTokenExpirationMinutes, int RefreshTokenExpirationMinutes) GenerateToken(User user)
    {
        var claims = new List<Claim>{
            new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iss,_configuration.Value.Issuer),
            new(JwtRegisteredClaimNames.Iat,DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture),ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
            // to invalidate the cookie
            new(ClaimTypes.SerialNumber, user.SerialNumber.Value.ToString(),ClaimValueTypes.String),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.RoleId.Value.ToString(CultureInfo.InvariantCulture))
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.Key));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        DateTime now = DateTime.UtcNow;
        DateTime expiryDate = now.AddMinutes(_configuration.Value.AccessTokenExpirationMinutes);
        var token = new JwtSecurityToken(
            issuer: _configuration.Value.Issuer,
            audience: _configuration.Value.Audience,
            claims: claims,
            notBefore: now,
            expires: expiryDate,
            signingCredentials: signingCredentials
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return (accessToken, _configuration.Value.AccessTokenExpirationMinutes, _configuration.Value.RefreshTokenExpirationMinutes);
    }
    private (string RefreshTokenValue, string RefreshTokenSerial) GenerateRefreshToken(User user)
    {
        var refreshTokenSerial = Guid.NewGuid().ToString().Replace("-", "", StringComparison.Ordinal);
        var claims = new List<Claim>{
            new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iss,_configuration.Value.Issuer),
            new(JwtRegisteredClaimNames.Iat,DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture),ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
            new(ClaimTypes.SerialNumber,refreshTokenSerial),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var now = DateTime.UtcNow;
        var token = new JwtSecurityToken(
                                         _configuration.Value.Issuer,
                                        _configuration.Value.Audience,
                                         claims,
                                         now,
                                         now.AddMinutes(_configuration.Value.RefreshTokenExpirationMinutes),
                                         creds);
        var refreshTokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return (refreshTokenValue, refreshTokenSerial);
    }

}