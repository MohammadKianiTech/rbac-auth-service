namespace Evalify.Api.Controllers.Evaluations;

public record CreateEvaluationRequest(
    string Title,
    string Description,
    DateTime? StartDate,
    DateTime? EndDate);