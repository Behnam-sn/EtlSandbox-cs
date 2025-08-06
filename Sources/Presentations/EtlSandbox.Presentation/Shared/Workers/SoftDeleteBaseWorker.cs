using EtlSandbox.Application.Shared.Commands;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Presentation.Shared.Workers;

public abstract class SoftDeleteBaseWorker<T> : BackgroundService
    where T : class, IEntity
{
    private readonly ILogger _logger;

    private readonly IServiceProvider _serviceProvider;

    protected SoftDeleteBaseWorker(ILogger logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected int? BatchSize { get; set; } = 50;

    protected int? DelayInSeconds { get; set; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();

                var applicationSettings = scope.ServiceProvider.GetRequiredService<IOptions<ApplicationSettings>>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var batchSize = BatchSize ?? applicationSettings.Value.BatchSize;
                var delayInSeconds = DelayInSeconds ?? applicationSettings.Value.DelayInSeconds;

                var command = new SoftDeleteCommand<T>(
                    BatchSize: batchSize
                );
                await mediator.Send(command, stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(delayInSeconds), stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Soft delete failed: {Message}", e.Message);
        }
    }
}