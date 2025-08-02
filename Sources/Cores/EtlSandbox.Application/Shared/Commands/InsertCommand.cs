using EtlSandbox.Application.Shared.Abstractions.Messaging;
using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Application.Shared.Commands;

public sealed record InsertCommand<TSource, TDestination>(int BatchSize) : ICommand
    where TSource : class
    where TDestination : class, IEntity;