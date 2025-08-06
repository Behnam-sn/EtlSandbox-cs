using EtlSandbox.Application.Shared.Commands;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Presentation.Shared.Workers;

public class InsertBaseWorker<TSource, TDestination> : BackgroundService
    where TSource : class
    where TDestination : class, IEntity
{
    private readonly ILogger _logger;

    private readonly IServiceProvider _serviceProvider;

    public InsertBaseWorker(ILogger<InsertBaseWorker<TSource, TDestination>> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected int? BatchSize { get; set; }

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

                var command = new InsertCommand<TSource, TDestination>(
                    BatchSize: batchSize
                );
                await mediator.Send(command, stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(delayInSeconds), stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Insert failed: {Message}", e.Message);
        }
    }
}