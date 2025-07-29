using MediatR;

namespace EtlSandbox.Application.Shared.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<TResponse>
{
}
