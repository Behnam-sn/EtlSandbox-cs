using MediatR;

namespace EtlSandbox.Application.Shared.Abstractions.Messaging;

public interface ICommand<TResponse> : IRequest<TResponse>
{
}
