using System.Globalization;
using System.Security.Claims;
using Evalify.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace Evalify.Infrastructure.Authentication;

internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                ThrowNotAuthenticated();
            }
            var userIdClaim = user!.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? null;

            if (userIdClaim == null)
            {
                ThrowClaimNotFound("User ID");
            }
            return Guid.Parse(userIdClaim!);
        }
    }
    public int RoleId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                ThrowNotAuthenticated();
            }
            var roleIdClaim = user!.FindFirst(ClaimTypes.Role)?.Value ?? null;

            if (roleIdClaim == null)
            {
                ThrowClaimNotFound("Role ID");
            }
            return int.Parse(roleIdClaim!, CultureInfo.InvariantCulture);
        }
    }

    private void ThrowNotAuthenticated()
    {
        throw new ApplicationException("User is not authenticated");
    }

    private void ThrowClaimNotFound(string claimName)
    {
        throw new ApplicationException($"{claimName} claim not found");
    }

}