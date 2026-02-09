using Evalify.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Evalify.Infrastructure.Authorization;

internal sealed class PermissionAuthorizationHandler(
    IServiceProvider _serviceProvider,
    IUserContext _userContext) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Identity is not { IsAuthenticated: true }) { return; }
        using var scope = _serviceProvider.CreateScope();
        var authorizationService = scope.ServiceProvider.GetRequiredService<AuthorizationService>();
        var permissions = await authorizationService.GetPermissionsForUserAsync(_userContext.UserId, _userContext.RoleId);
        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}