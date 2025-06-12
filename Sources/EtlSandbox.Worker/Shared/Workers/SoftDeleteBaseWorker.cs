using EtlSandbox.Domain.ApplicationStates.Enums;
using EtlSandbox.Domain.ApplicationStates.Repositories;
using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Worker.Shared.Workers;

public abstract class SoftDeleteBaseWorker<T> : BackgroundService
{
    private readonly ILogger _logger;

    private readonly IServiceProvider _serviceProvider;

    protected SoftDeleteBaseWorker(ILogger logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var applicationStateCommandRepository = scope.ServiceProvider.GetRequiredService<IApplicationStateCommandRepository>();
        var synchronizer = scope.ServiceProvider.GetRequiredService<ISynchronizer<T>>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var lastDeletedId = await applicationStateCommandRepository.GetLastProcessedIdAsync<T>(ActionType.Delete);
                var lastInsertedId = await applicationStateCommandRepository.GetLastProcessedIdAsync<T>(ActionType.Insert);
                var count = lastInsertedId - lastDeletedId;

                if (count > 0)
                {
                    await unitOfWork.OpenConnectionAsync(stoppingToken);
                    unitOfWork.BeginTransaction();

                    try
                    {
                        _logger.LogInformation("Deleting from {FromId} to {ToId}", lastDeletedId, lastInsertedId);
                        await synchronizer.SoftDeleteObsoleteRowsAsync(
                            fromId: lastDeletedId,
                            toId: lastInsertedId,
                            transaction: unitOfWork.Transaction
                        );
                        await applicationStateCommandRepository.UpdateLastProcessedIdAsync<T>(
                            actionType: ActionType.Delete,
                            lastProcessedId: lastInsertedId,
                            transaction: unitOfWork.Transaction
                        );
                        unitOfWork.Commit();
                        _logger.LogInformation("Delete completed");
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                }
                else
                {
                    _logger.LogInformation("No new data to process");
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Soft delete failed: {Message}", e.Message);
            }
        }
    }
}