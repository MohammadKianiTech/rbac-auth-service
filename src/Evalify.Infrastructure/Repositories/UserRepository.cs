using Evalify.Domain.Entities;
using Evalify.Domain.Repositories;
using Evalify.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Evalify.Infrastructure.Repositories;


internal sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<User>().Where(c => c.Email == email).SingleOrDefaultAsync(cancellationToken);
    }
    public async Task<bool> IsEmailUniqueAsync(Guid id, Email email, CancellationToken cancellationToken = default)
    {
        return !await DbContext.Set<User>().Where(user => (id == Guid.Empty || user.Id != id) && user.Email == email).AnyAsync(cancellationToken);
    }
}