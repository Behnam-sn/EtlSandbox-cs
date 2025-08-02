using EtlSandbox.Application.Shared.Abstractions.Messaging;
using EtlSandbox.Domain.Shared;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Application.Shared.Commands;

public sealed class SoftDeleteCommandHandler<T> : ICommandHandler<SoftDeleteCommand<T>>
    where T : class, IEntity
{
    private readonly ILogger _logger;

    private readonly ISoftDeleteStartingPointResolver<T> _softDeleteStartingPointResolver;

    private readonly ISynchronizer<T> _synchronizer;

    public SoftDeleteCommandHandler(ILogger<SoftDeleteCommandHandler<T>> logger, ISoftDeleteStartingPointResolver<T> softDeleteStartingPointResolver, ISynchronizer<T> synchronizer)
    {
        _logger = logger;
        _softDeleteStartingPointResolver = softDeleteStartingPointResolver;
        _synchronizer = synchronizer;
    }

    public async Task Handle(SoftDeleteCommand<T> request, CancellationToken cancellationToken)
    {
        var fromId = await _softDeleteStartingPointResolver.GetLastSoftDeletedIdAsync(request.BatchSize);
        var toId = fromId + request.BatchSize;

        _logger.LogInformation("Soft deleting");
        await _synchronizer.SoftDeleteObsoleteRowsAsync(
            fromId: fromId,
            toId: toId
        );
        _logger.LogInformation("Soft deleted from {LastDeletedId} to {ToId}", fromId, toId);
    }
}