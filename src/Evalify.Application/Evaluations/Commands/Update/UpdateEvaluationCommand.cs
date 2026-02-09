using Evalify.Application.Abstractions.Messaging;

namespace Evalify.Application.Evaluations.Commands.Update;

public sealed record UpdateEvaluationCommand(
    Guid Id,
    string Title,
    string Description,
    DateTime? StartDate,
    DateTime? EndDate) : ICommand<bool>;