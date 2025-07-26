using EtlSandbox.Application.Shared.Abstractions.Messaging;
using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Application.Shared.Commands;

public sealed record InsertCommand<T>(int BatchSize) : ICommand
    where T : class, IEntity;