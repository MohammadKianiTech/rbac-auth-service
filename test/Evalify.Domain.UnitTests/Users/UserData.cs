using Evalify.Domain.ValueObjects;

namespace Evalify.Domain.UnitTests.Users;

internal static class UserData
{
    public static readonly FirstName FirstName = new("First");
    public static readonly LastName LastName = new("Last");
    public static readonly Email Email = new("test@test.com");
    public static readonly Password Password = new("000000Mk#");
    public static readonly RoleId RoleId = new(1);
}
