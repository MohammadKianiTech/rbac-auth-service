using Evalify.Application.Abstractions.Caching;
using Evalify.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Evalify.Infrastructure.Authorization;

internal sealed class AuthorizationService(ApplicationDbContext _dbContext, ICacheService _cacheService)
{
    public async Task<HashSet<string>> GetPermissionsForUserAsync(Guid userId, int roleId)
    {
        var cacheKey = $"auth:permissions-{userId}";
        var cachedPermissions = await _cacheService.GetAsync<HashSet<string>>(cacheKey);

        if (cachedPermissions is not null) { return cachedPermissions; }

        var permissionNames = await _dbContext
            .Set<Permission>()
            .Where(p => _dbContext.Set<RolePermission>().Any(x => x.RoleId == roleId && x.PermissionId == p.Id))
            .AsNoTracking()
            .Select(p => p.Name).ToHashSetAsync();

        await _cacheService.SetAsync(cacheKey, permissionNames);

        return permissionNames;
    }
}