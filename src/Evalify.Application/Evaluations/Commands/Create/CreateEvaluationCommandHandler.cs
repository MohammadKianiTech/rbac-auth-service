using Evalify.Application.Abstractions.Authentication;
using Evalify.Application.Abstractions.Messaging;
using Evalify.Domain.Abstractions;
using Evalify.Domain.Entities;
using Evalify.Domain.Repositories;
using Evalify.Domain.ValueObjects;

namespace Evalify.Application.Evaluations.Commands.Create;

internal sealed class CreateEvaluationCommandHandler(
    IUnitOfWork _unitOfWork,
    IEvaluationRepository _evaluationRepository,
    IUserContext _userContext) : ICommandHandler<CreateEvaluationCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateEvaluationCommand request, CancellationToken cancellationToken)
    {
        var start = DateTime.SpecifyKind(request.StartDate ?? DateTime.MinValue, DateTimeKind.Utc);
        var end = DateTime.SpecifyKind(request.EndDate ?? DateTime.MinValue, DateTimeKind.Utc);
        var duration = DateRange.Create(start, end);
        var evaluation = Evaluation.Create(new Title(request.Title), new Description(request.Description), duration, _userContext.UserId);
        await _evaluationRepository.AddAsync(evaluation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return evaluation.Id;
    }
}