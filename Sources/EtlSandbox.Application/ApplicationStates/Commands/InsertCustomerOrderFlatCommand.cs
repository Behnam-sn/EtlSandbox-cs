using EtlSandbox.Application.Shared.Abstractions.Messaging;
using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Application.ApplicationStates.Commands;

public sealed record InsertCustomerOrderFlatCommand<T>(int BatchSize) : ICommand
    where T : IEntity;
