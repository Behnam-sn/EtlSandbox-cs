using EtlSandbox.Application.Common.Abstractions.Messaging;
using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Repositories;
using EtlSandbox.Domain.Common.Resolvers;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Application.Common.Commands;

public sealed class SoftDeleteCommandHandler<TDestination> : ICommandHandler<SoftDeleteCommand<TDestination>>
    where TDestination : class, IEntity
{
    private readonly ILogger _logger;

    private readonly IDestinationRepository<TDestination> _destinationRepository;

    private readonly ISoftDeleteStartingPointResolver<TDestination> _startingPointResolver;

    private readonly ISynchronizer<TDestination> _synchronizer;

    public SoftDeleteCommandHandler(
        ILogger<SoftDeleteCommandHandler<TDestination>> logger,
        IDestinationRepository<TDestination> destinationRepository,
        ISoftDeleteStartingPointResolver<TDestination> startingPointResolver,
        ISynchronizer<TDestination> synchronizer
    )
    {
        _logger = logger;
        _destinationRepository = destinationRepository;
        _startingPointResolver = startingPointResolver;
        _synchronizer = synchronizer;
    }

    public async Task Handle(SoftDeleteCommand<TDestination> request, CancellationToken cancellationToken)
    {
        var destinationTypeName = typeof(TDestination).Name;
        var lastId = await _destinationRepository.GetMaxIdOrDefaultAsync(cancellationToken);
        var from = _startingPointResolver.StartingPoint;
        var to = from + request.BatchSize < lastId
            ? from + request.BatchSize
            : lastId;

        if (from >= to)
        {
            return;
        }

        _logger.LogInformation("Soft deleting {Type}", destinationTypeName);
        await _synchronizer.SoftDeleteObsoleteRowsAsync(from, to);
        // await _destinationRepository.SoftDeleteObsoleteRowsAsync(from, to, cancellationToken);
        _logger.LogInformation("Soft deleted {Type} from {From} to {To}", destinationTypeName, from, to);

        _startingPointResolver.StartingPoint = to;
    }
}