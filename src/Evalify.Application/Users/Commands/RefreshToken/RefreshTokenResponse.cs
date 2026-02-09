namespace Evalify.Application.Users.Commands.RefreshToken;

public record RefrshTokenResponse(
    string AccessToken,
    string RefreshToken);