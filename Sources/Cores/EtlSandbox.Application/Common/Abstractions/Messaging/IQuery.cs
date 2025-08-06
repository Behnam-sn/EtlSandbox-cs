using MediatR;

namespace EtlSandbox.Application.Common.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<TResponse>
{
}
