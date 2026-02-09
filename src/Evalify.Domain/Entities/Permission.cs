namespace Evalify.Domain.Entities;

public sealed class Permission
{
    public Permission(int id, string name)
    {
        Id = id;
        Name = name;
    }
    private Permission() { }
    public int Id { get; init; }
    public string Name { get; init; }
}