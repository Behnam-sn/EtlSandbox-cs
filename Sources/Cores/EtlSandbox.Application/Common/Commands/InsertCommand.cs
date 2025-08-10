using EtlSandbox.Application.Common.Abstractions.Messaging;
using EtlSandbox.Domain.Common;

namespace EtlSandbox.Application.Common.Commands;

public sealed record InsertCommand<TSource, TDestination>(long StartingPointId, int BatchSize) : ICommand
    where TSource : class
    where TDestination : class, IEntity;