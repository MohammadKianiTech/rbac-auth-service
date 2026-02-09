using Evalify.Application.Abstractions.Messaging;
using Evalify.Domain.ValueObjects;

namespace Evalify.Application.Evaluations.Commands.Create;

public sealed record CreateEvaluationCommand(
    string Title,
    string Description,
    DateTime? StartDate,
    DateTime? EndDate
) : ICommand<Guid>;