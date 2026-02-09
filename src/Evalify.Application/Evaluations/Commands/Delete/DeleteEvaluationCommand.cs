using Evalify.Application.Abstractions.Messaging;

namespace Evalify.Application.Evaluations.Commands.Delete;

public sealed record DeleteEvaluationCommand(
    Guid Id) : ICommand<bool>;