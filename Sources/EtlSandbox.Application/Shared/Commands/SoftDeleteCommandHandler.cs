using EtlSandbox.Application.Shared.Abstractions.Messaging;
using EtlSandbox.Domain.Shared;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Application.Shared.Commands;

public sealed class SoftDeleteCommandHandler<T> : ICommandHandler<SoftDeleteCommand<T>>
    where T : class, IEntity
{
    private readonly ILogger _logger;

    private readonly ISynchronizerUtils<T> _synchronizerUtils;

    private readonly ISynchronizer<T> _synchronizer;

    public SoftDeleteCommandHandler(ILogger<SoftDeleteCommandHandler<T>> logger, ISynchronizer<T> synchronizer, ISynchronizerUtils<T> synchronizerUtils)
    {
        _logger = logger;
        _synchronizer = synchronizer;
        _synchronizerUtils = synchronizerUtils;
    }

    public async Task Handle(SoftDeleteCommand<T> request, CancellationToken cancellationToken)
    {
        var fromId = await _synchronizerUtils.GetLastSoftDeletedIdAsync();
        var toId = fromId + request.BatchSize;

        _logger.LogInformation("Soft deleting");
        await _synchronizer.SoftDeleteObsoleteRowsAsync(
            fromId: fromId,
            toId: toId
        );
        _logger.LogInformation("Soft deleted from {LastDeletedId} to {ToId}", fromId, toId);
    }
}