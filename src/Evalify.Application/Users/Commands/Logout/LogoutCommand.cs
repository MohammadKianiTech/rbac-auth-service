using Evalify.Application.Abstractions.Messaging;

namespace Evalify.Application.Users.Commands.Logout;

public sealed record LogoutCommand(
    string RefreshToken) : ICommand;