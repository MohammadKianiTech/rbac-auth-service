using Evalify.Application.Abstractions.Messaging;

namespace Evalify.Application.Users.Commands.Register;

public sealed record UserRegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password
) : ICommand<Guid>;