using EtlSandbox.Application.Common.Abstractions.Messaging;
using EtlSandbox.Domain.Common;

namespace EtlSandbox.Application.Common.Commands;

public sealed record SoftDeleteCommand<TDestination>(int BatchSize) : ICommand
    where TDestination : class, IEntity;