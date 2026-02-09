using Evalify.Application.Abstractions.Clock;
using Evalify.Application.Abstractions.Hashing;
using Evalify.Application.Abstractions.Messaging;
using Evalify.Domain.Abstractions;
using Evalify.Domain.Entities;
using Evalify.Domain.Errors;
using Evalify.Domain.Repositories;
using Evalify.Domain.ValueObjects;

namespace Evalify.Application.Users.Commands.Register;

internal sealed class UserRegisterCommandHandler(
    IUnitOfWork _unitOfWork,
    IUserRepository _userRepository,
    IDateTimeProvider _dateTimeProvider,
    IPasswordHasher _passwordHasher) : ICommandHandler<UserRegisterCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
    {
        bool isUnique = await _userRepository.IsEmailUniqueAsync(Guid.Empty, new Email(request.Email), cancellationToken);
        if (!isUnique)
        {
            return Result.Failure<Guid>(UserErrors.EmailAlreadyInUse);
        }

        var user = User.Create(new FirstName(request.FirstName), new LastName(request.LastName), new Email(request.Email), new Password(_passwordHasher.Hash(request.Password)), new RoleId(1), _dateTimeProvider.UtcNow, _dateTimeProvider.UtcNow);

        await _userRepository.AddAsync(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}