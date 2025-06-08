using MediatR;

namespace EtlSandbox.Application.Shared.Abstractions.Messaging;

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
}
