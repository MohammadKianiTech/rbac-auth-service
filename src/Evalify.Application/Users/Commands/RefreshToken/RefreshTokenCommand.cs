using Evalify.Application.Abstractions.Messaging;

namespace Evalify.Application.Users.Commands.RefreshToken;

public sealed record RefreshTokenCommand(
    string RefreshToken) : ICommand<RefrshTokenResponse>;