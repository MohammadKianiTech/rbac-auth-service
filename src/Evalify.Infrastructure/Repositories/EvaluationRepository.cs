using Evalify.Domain.Entities;
using Evalify.Domain.Repositories;

namespace Evalify.Infrastructure.Repositories;

internal sealed class EvaluationRepository : Repository<Evaluation>, IEvaluationRepository
{
    public EvaluationRepository(ApplicationDbContext dbContext)
    : base(dbContext)
    {
    }
}