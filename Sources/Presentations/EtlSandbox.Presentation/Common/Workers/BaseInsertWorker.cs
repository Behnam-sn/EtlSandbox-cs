using EtlSandbox.Application.Common.Commands;
using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Options;
using EtlSandbox.Domain.Common.Options.WorkerSettings;
using EtlSandbox.Domain.Common.Resolvers;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Presentation.Common.Workers;

public abstract class BaseInsertWorker<TWorker, TSource, TDestination> : BackgroundService
    where TWorker : BaseInsertWorker<TWorker, TSource, TDestination>
    where TSource : class
    where TDestination : class, IEntity
{
    private readonly ILogger _logger;

    private readonly IServiceProvider _serviceProvider;

    protected BaseInsertWorker(ILogger<BaseInsertWorker<TWorker, TSource, TDestination>> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var serviceScope = _serviceProvider.CreateScope();

            var workerSettingsOptions = serviceScope.ServiceProvider.GetRequiredService<IOptions<InsertWorkerSettings<TWorker>>>();
            var workerSettings = workerSettingsOptions.Value;

            if (!workerSettings.Enable)
            {
                return;
            }

            var startingPointId = workerSettings.StartingPointId ?? 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                
                var batchSizeResolver = scope.ServiceProvider.GetRequiredService<IInsertWorkerBatchSizeResolver<TWorker, TSource, TDestination>>();
                var batchSize = await batchSizeResolver.GetBatchSizeAsync();
                
                var delayResolver = scope.ServiceProvider.GetRequiredService<IInsertWorkerDelayResolver<TWorker, TSource, TDestination>>();
                var delay = await delayResolver.GetDelayAsync();    
                
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var command = new InsertCommand<TSource, TDestination>(
                    StartingPointId: startingPointId,
                    BatchSize: batchSize
                );
                await mediator.Send(command, stoppingToken);

                await Task.Delay(delay, stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Insert failed: {Message}", e.Message);
        }
    }
}