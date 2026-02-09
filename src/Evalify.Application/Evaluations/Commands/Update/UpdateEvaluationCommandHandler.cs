using Evalify.Application.Abstractions.Messaging;
using Evalify.Domain.Abstractions;
using Evalify.Domain.Errors;
using Evalify.Domain.Repositories;
using Evalify.Domain.ValueObjects;

namespace Evalify.Application.Evaluations.Commands.Update;

internal sealed class UpdateEvaluationCommandHandler(
    IUnitOfWork _unitOfWork,
    IEvaluationRepository _evaluationRepository) : ICommandHandler<UpdateEvaluationCommand, bool>
{
    public async Task<Result<bool>> Handle(UpdateEvaluationCommand request, CancellationToken cancellationToken)
    {
        var evaluation = await _evaluationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (evaluation is null)
        {
            return Result.Failure<bool>(EvaluationErrors.NotFound);
        }
        var start = request.StartDate?.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(request.StartDate ?? DateTime.MinValue, DateTimeKind.Utc)
                    : request.StartDate?.ToUniversalTime();

        var end = request.EndDate?.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(request.EndDate ?? DateTime.MinValue, DateTimeKind.Utc)
            : request.EndDate?.ToUniversalTime();

        var duration = DateRange.Create(start ?? DateTime.MinValue, end ?? DateTime.MinValue);

        evaluation.Update(
            new Title(request.Title),
            new Description(request.Description),
            duration);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }
}