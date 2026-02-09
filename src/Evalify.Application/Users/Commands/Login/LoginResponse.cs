namespace Evalify.Application.Users.Commands.Login;

public record LoginResponse(
    string AccessToken,
    string RefreshToken);