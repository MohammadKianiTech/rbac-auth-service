using FluentValidation;

namespace Evalify.Application.Evaluations.Commands.Delete;

public sealed class DeleteEvaluationCommandValidator : AbstractValidator<DeleteEvaluationCommand>
{
    public DeleteEvaluationCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Evaluation ID is required.");
    }
}
