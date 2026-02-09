using Evalify.Domain.Entities;

namespace Evalify.Domain.Repositories;

public interface IEvaluationRepository
{
    Task<Evaluation?> GetByIdAsync(Guid Id, CancellationToken cancellationToken);
    Task AddAsync(Evaluation evaluation);
    void Remove(Evaluation evaluation);
}