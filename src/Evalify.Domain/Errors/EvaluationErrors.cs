namespace Evalify.Domain.Errors;

public static class EvaluationErrors
{
    public static readonly Error NotFound = new(
        "Evaluation.Found",
        "The evaluation with the specified identifier was not found");
}