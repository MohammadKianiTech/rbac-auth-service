namespace Evalify.Application.Roles.Queries.Get;

public sealed class RoleResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
    public List<PermissionResponse> Permissions { get; set; } = new List<PermissionResponse>();
}

public sealed class PermissionResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}