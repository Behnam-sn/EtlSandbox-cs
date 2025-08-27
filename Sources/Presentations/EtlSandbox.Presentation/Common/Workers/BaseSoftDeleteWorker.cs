using EtlSandbox.Application.Common.Commands;
using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Options.WorkerSettings;
using EtlSandbox.Domain.Common.Resolvers;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Presentation.Common.Workers;

public abstract class BaseSoftDeleteWorker<TWorker, TDestination> : BackgroundService
    where TWorker : BaseSoftDeleteWorker<TWorker, TDestination>
    where TDestination : class, IEntity
{
    private readonly ILogger _logger;

    private readonly IServiceProvider _serviceProvider;

    protected BaseSoftDeleteWorker(ILogger<BaseSoftDeleteWorker<TWorker, TDestination>> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var serviceScope = _serviceProvider.CreateScope();

            var workerSettingsOptions = serviceScope.ServiceProvider.GetRequiredService<IOptions<SoftDeleteWorkerSettings<TWorker>>>();
            var workerSettings = workerSettingsOptions.Value;

            if (!workerSettings.Enable)
            {
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                
                var batchSizeResolver = scope.ServiceProvider.GetRequiredService<ISoftDeleteWorkerBatchSizeResolver<TWorker, TDestination>>();
                var batchSize = await batchSizeResolver.GetBatchSizeAsync();
                
                var delayResolver = scope.ServiceProvider.GetRequiredService<ISoftDeleteWorkerDelayResolver<TWorker, TDestination>>();
                var delay = await delayResolver.GetDelayAsync();
                
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var command = new SoftDeleteCommand<TDestination>(
                    BatchSize: batchSize
                );
                await mediator.Send(command, stoppingToken);

                await Task.Delay(delay, stoppingToken);
            }
        }
        catch (Exception e)
        {
            var destinationTypeName = typeof(TDestination).Name;
            _logger.LogError(e, "{Type} soft delete failed: {Message}", destinationTypeName, e.Message);
        }
    }
}