namespace Evalify.Api.Controllers.User;

public sealed record RegisterUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password);