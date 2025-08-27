using EtlSandbox.Application.Common.Abstractions.Messaging;
using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Repositories;
using EtlSandbox.Domain.Common.Resolvers;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Application.Common.Commands;

public sealed class SoftDeleteCommandHandler<T> : ICommandHandler<SoftDeleteCommand<T>>
    where T : class, IEntity
{
    private readonly ILogger _logger;

    private readonly IDestinationRepository<T> _destinationRepository;

    private readonly ISoftDeleteStartingPointResolver<T> _startingPointResolver;

    private readonly ISynchronizer<T> _synchronizer;

    public SoftDeleteCommandHandler(
        ILogger<SoftDeleteCommandHandler<T>> logger,
        IDestinationRepository<T> destinationRepository,
        ISoftDeleteStartingPointResolver<T> startingPointResolver,
        ISynchronizer<T> synchronizer
    )
    {
        _logger = logger;
        _destinationRepository = destinationRepository;
        _startingPointResolver = startingPointResolver;
        _synchronizer = synchronizer;
    }

    public async Task Handle(SoftDeleteCommand<T> request, CancellationToken cancellationToken)
    {
        var destinationTypeName = typeof(T).Name;
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