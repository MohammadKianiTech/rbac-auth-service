namespace Evalify.Application.Abstractions.Caching;

public record CachedUserModel(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    Guid SerialNumber,
    int RoleId
);