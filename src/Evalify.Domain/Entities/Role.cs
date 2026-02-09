namespace Evalify.Domain.Entities;

public sealed class Role
{
    private readonly List<RolePermission> _rolePermissions = new();

    public int Id { get; init; }
    public string Name { get; init; }
    public bool IsActive { get; init; }
    public IReadOnlyCollection<RolePermission> Permissions => _rolePermissions.AsReadOnly();
    public Role(int id, string name)
    {
        Id = id;
        Name = name;
    }
    public void AddPermission(Permission permission)
    {
        if (_rolePermissions.TrueForAll(x => x.PermissionId != permission.Id))
        {
            _rolePermissions.Add(new RolePermission
            {
                RoleId = Id,
                PermissionId = permission.Id,
                Permission = permission
            });
        }
    }

    public void RemovePermission(Permission permission)
    {
        var item = _rolePermissions.Find(x => x.PermissionId == permission.Id);
        if (item != null)
        {
            _rolePermissions.Remove(item);
        }
    }
}