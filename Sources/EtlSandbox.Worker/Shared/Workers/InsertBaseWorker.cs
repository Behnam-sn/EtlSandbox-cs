using EtlSandbox.Domain.ApplicationStates;
using EtlSandbox.Domain.ApplicationStates.Enums;
using EtlSandbox.Domain.ApplicationStates.Repositories;
using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Worker.Shared.Workers;

public abstract class InsertBaseWorker<T> : BackgroundService
    where T : IEntity
{
    private const int BatchSize = 100_000;

    private readonly ILogger _logger;

    private readonly IServiceProvider _serviceProvider;

    protected InsertBaseWorker(ILogger logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var applicationStateCommandRepository = scope.ServiceProvider.GetRequiredService<IApplicationStateCommandRepository>();
        var extractor = scope.ServiceProvider.GetRequiredService<IExtractor<T>>();
        var transformer = scope.ServiceProvider.GetRequiredService<ITransformer<T>>();
        var loader = scope.ServiceProvider.GetRequiredService<ILoader<T>>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var lastProcessedId = await applicationStateCommandRepository.GetLastProcessedIdAsync<T>(ProcessType.Insert);

                var data = await extractor.ExtractAsync(
                    lastProcessedId,
                    BatchSize,
                    stoppingToken
                );

                if (data.Any())
                {
                    var transformed = data.Select(transformer.Transform).ToList();

                    await unitOfWork.OpenConnectionAsync(stoppingToken);
                    unitOfWork.BeginTransaction();
                    try
                    {
                        _logger.LogInformation("Loading {Count} rows", data.Count);
                        await loader.LoadAsync(transformed, stoppingToken, unitOfWork.Transaction);
                        await applicationStateCommandRepository.UpdateLastProcessedIdAsync<T>(
                            processType: ProcessType.Insert,
                            lastProcessedId: transformed.Max(item => item.Id),
                            transaction: unitOfWork.Transaction
                        );
                        unitOfWork.Commit();
                        _logger.LogInformation("Load completed");
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
                _logger.LogError(e, "Insert failed: {Message}", e.Message);
            }
        }
    }
}