namespace Evalify.Application.Users.Queries.Get;

public record UserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Status);