using MediatR;

namespace Evalify.Domain.Abstractions;

public record DomainEvent(Guid Id) : INotification;