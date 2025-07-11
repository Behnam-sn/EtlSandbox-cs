using EtlSandbox.Domain.ApplicationStates.Enums;
using EtlSandbox.Domain.ApplicationStates.Repositories;
using EtlSandbox.Domain.Shared;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EtlSandbox.Presentation.Shared.Workers;

public abstract class SoftDeleteBaseWorker<T> : BackgroundService
{
    private const int BatchSize = 100_000;

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
        
        unitOfWork.Connection.Open();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var lastDeletedId = await applicationStateCommandRepository.GetLastProcessedIdAsync<T>(ProcessType.Delete);
                var lastInsertedId = await applicationStateCommandRepository.GetLastProcessedIdAsync<T>(ProcessType.Insert);
                var count = lastInsertedId - lastDeletedId;

                if (count > 0)
                {
                    unitOfWork.BeginTransaction();

                    try
                    {
                        var toId = count > BatchSize ? lastDeletedId + BatchSize : lastInsertedId;

                        _logger.LogInformation("Deleting from {FromId} to {ToId}", lastDeletedId, toId);

                        await synchronizer.SoftDeleteObsoleteRowsAsync(
                            fromId: lastDeletedId,
                            toId: toId,
                            transaction: unitOfWork.Transaction
                        );

                        await applicationStateCommandRepository.UpdateLastProcessedIdAsync<T>(
                            processType: ProcessType.Delete,
                            lastProcessedId: toId,
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