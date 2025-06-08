using MediatR;

namespace EtlSandbox.Application.Shared.Abstractions.Messaging;

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand
{
}
