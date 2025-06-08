using EtlSandbox.Domain;

namespace EtlSandbox.Worker.Workers;

public abstract class InsertBaseWorker<T> : BackgroundService
{
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

        var commandRepository = scope.ServiceProvider.GetRequiredService<ICommandRepository<T>>();
        var extractor = scope.ServiceProvider.GetRequiredService<IExtractor<T>>();
        var transformer = scope.ServiceProvider.GetRequiredService<ITransformer<T>>();
        var loader = scope.ServiceProvider.GetRequiredService<ILoader<T>>();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var lastProcessedId = await commandRepository.GetLastIdAsync();

                var data = await extractor.ExtractAsync(
                    lastProcessedId,
                    stoppingToken
                );

                if (data.Any())
                {
                    var transformed = data.Select(transformer.Transform).ToList();
                    await loader.LoadAsync(transformed, stoppingToken);
                }
                else
                {
                    _logger.LogInformation("No new data to process");
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ETL failed: {Message}", e.Message);
            }
        }
    }
}