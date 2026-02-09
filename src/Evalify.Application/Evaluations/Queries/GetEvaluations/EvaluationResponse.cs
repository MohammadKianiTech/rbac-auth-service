namespace Evalify.Application.Evaluations.Queries.GetEvaluations;

public record EvaluationResponse(
    Guid Id,
    string Title,
    string Description,
    string Status,
    DateTime StartDate,
    DateTime EndDate,
    DateTime CreatedAt
);