using MediatR;

namespace EtlSandbox.Application.Common.Abstractions.Messaging;

public interface ICommand<TResponse> : IRequest<TResponse>
{
}
