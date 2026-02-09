using Evalify.Domain.Entities;

namespace Evalify.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task AddAsync(User user);
    Task<bool> IsEmailUniqueAsync(Guid id, Email email, CancellationToken cancellationToken = default);
}