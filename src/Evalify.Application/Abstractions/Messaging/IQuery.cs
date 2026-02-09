using Evalify.Domain.Abstractions;
using MediatR;

namespace Evalify.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}