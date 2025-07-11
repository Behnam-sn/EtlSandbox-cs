using EtlSandbox.Domain.ApplicationStates.Enums;
using EtlSandbox.Domain.ApplicationStates.Repositories;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Presentation.Shared.Workers;

public abstract class InsertBaseWorker<T> : BackgroundService
    where T : IEntity
{
    private readonly ILogger _logger;

    private readonly IServiceProvider _serviceProvider;

    protected InsertBaseWorker(ILogger logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected int? BatchSize { get; set; }

    protected int? DelayInSeconds { get; set; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var applicationSettings = scope.ServiceProvider.GetRequiredService<IOptions<ApplicationSettings>>();
        var applicationStateCommandRepository = scope.ServiceProvider.GetRequiredService<IApplicationStateCommandRepository>();
        var extractor = scope.ServiceProvider.GetRequiredService<IExtractor<T>>();
        var transformer = scope.ServiceProvider.GetRequiredService<ITransformer<T>>();
        var loader = scope.ServiceProvider.GetRequiredService<ILoader<T>>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var batchSize = BatchSize ?? applicationSettings.Value.BatchSize;
        var delayInSeconds = DelayInSeconds ?? applicationSettings.Value.DelayInSeconds;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var lastProcessedId = await applicationStateCommandRepository.GetLastProcessedIdAsync<T>(ProcessType.Insert);

                var data = await extractor.ExtractAsync(
                    lastProcessedId,
                    batchSize,
                    stoppingToken
                );

                if (data.Count != 0)
                {
                    var transformed = data.Select(transformer.Transform).ToList();

                    unitOfWork.Connection.Open();
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
                    finally
                    {
                        unitOfWork.Connection.Close();
                    }
                }
                else
                {
                    _logger.LogInformation("No new data to process");
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Insert failed: {Message}", e.Message);
            }

            await Task.Delay(TimeSpan.FromSeconds(delayInSeconds), stoppingToken);
        }
    }
}