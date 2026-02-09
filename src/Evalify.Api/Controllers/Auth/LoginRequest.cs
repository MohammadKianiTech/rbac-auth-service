namespace Evalify.Api.Controllers.Auth;

public record LoginRequest(
    string Email,
    string Password);