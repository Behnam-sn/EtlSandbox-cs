using EtlSandbox.Application.Common.Commands;
using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Options;
using EtlSandbox.Domain.Common.Options.WorkerSettings;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Presentation.Common.Workers;

public abstract class BaseSoftDeleteWorker<TWorker, TSource> : BackgroundService
    where TWorker : BaseSoftDeleteWorker<TWorker, TSource>
    where TSource : class, IEntity
{
    private readonly ILogger _logger;

    private readonly IServiceProvider _serviceProvider;

    protected BaseSoftDeleteWorker(ILogger<BaseSoftDeleteWorker<TWorker, TSource>> logger, IServiceProvider serviceProvider)
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

            var globalSettingsOptions = serviceScope.ServiceProvider.GetRequiredService<IOptions<GlobalSettings>>();
            var globalSettings = globalSettingsOptions.Value;

            var batchSize = workerSettings.BatchSize ?? globalSettings.BatchSize;
            var delay = workerSettings.DelayInMilliSeconds ?? globalSettings.DelayInMilliSeconds;

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var command = new SoftDeleteCommand<TSource>(
                    BatchSize: batchSize
                );
                await mediator.Send(command, stoppingToken);

                await Task.Delay(delay, stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Soft delete failed: {Message}", e.Message);
        }
    }
}