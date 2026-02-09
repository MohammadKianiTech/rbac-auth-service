using Evalify.Application.Abstractions.Messaging;

namespace Evalify.Application.Users.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : ICommand<LoginResponse>;