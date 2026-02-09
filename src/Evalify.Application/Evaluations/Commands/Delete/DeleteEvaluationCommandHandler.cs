using Evalify.Application.Abstractions.Messaging;
using Evalify.Domain.Abstractions;
using Evalify.Domain.Errors;
using Evalify.Domain.Repositories;

namespace Evalify.Application.Evaluations.Commands.Delete;

internal sealed class DeleteEvaluationCommandHandler(
    IUnitOfWork _unitOfWork,
    IEvaluationRepository _evaluationRepository) : ICommandHandler<DeleteEvaluationCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteEvaluationCommand request, CancellationToken cancellationToken)
    {
        var evaluation = await _evaluationRepository.GetByIdAsync(request.Id, cancellationToken);

        if (evaluation is null)
        {
            return Result.Failure<bool>(EvaluationErrors.NotFound);
        }

        _evaluationRepository.Remove(evaluation);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}