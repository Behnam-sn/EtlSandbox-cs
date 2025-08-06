using EtlSandbox.Application.Common.Abstractions.Messaging;
using EtlSandbox.Domain.Common;

namespace EtlSandbox.Application.Common.Commands;

public sealed record SoftDeleteCommand<T>(int BatchSize) : ICommand
    where T : class, IEntity;