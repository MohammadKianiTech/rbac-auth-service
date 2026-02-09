using FluentValidation;

namespace Evalify.Application.Evaluations.Commands.Update;

public sealed class UpdateEvaluationCommandValidator : AbstractValidator<UpdateEvaluationCommand>
{
    public UpdateEvaluationCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Evaluation ID is required.");

        RuleFor(c => c.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(c => c.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(c => c.StartDate)
            .Cascade(CascadeMode.Stop)
            .Must(date => date?.Year > 1)
            .WithMessage("Start date is required.")
            .LessThan(c => c.EndDate)
            .WithMessage("Start date must be before end date.");

        RuleFor(c => c.EndDate)
            .Cascade(CascadeMode.Stop)
            .Must(date => date?.Year > 1)
            .WithMessage("End date is required.")
            .GreaterThan(c => c.StartDate)
            .WithMessage("End date must be after start date.");
    }
}
