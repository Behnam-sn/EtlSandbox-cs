using EtlSandbox.Domain;

namespace EtlSandbox.Worker;

public sealed class EtlWorker : BackgroundService
{
    private readonly ILogger<EtlWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    // private readonly IExtractor<CustomerOrderFlat> _extractor;
    // private readonly ITransformer<CustomerOrderFlat> _transformer;
    // private readonly ILoader<CustomerOrderFlat> _loader;

    public EtlWorker(
        ILogger<EtlWorker> logger,
        // IExtractor<CustomerOrderFlat> extractor,
        // ITransformer<CustomerOrderFlat> transformer,
        // ILoader<CustomerOrderFlat> loader,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        // _extractor = extractor;
        // _transformer = transformer;
        // _loader = loader;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ETL Worker started");

        using var scope = _serviceProvider.CreateScope();
        var extractor = scope.ServiceProvider.GetRequiredService<IExtractor<CustomerOrderFlat>>();
        var transformer = scope.ServiceProvider.GetRequiredService<ITransformer<CustomerOrderFlat>>();
        var loader = scope.ServiceProvider.GetRequiredService<ILoader<CustomerOrderFlat>>();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var since = await extractor.GetLastProcessedTimestampAsync();
                var data = await extractor.ExtractAsync(since, stoppingToken);

                if (data.Any())
                {
                    var transformed = data.Select(transformer.Transform).ToList();
                    await loader.LoadAsync(transformed, stoppingToken);
                    await extractor.UpdateLastProcessedTimestampAsync(data.Max(x => x.RentalDate));
                }
                else
                {
                    _logger.LogInformation("No new data to process");
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ETL failed: {Message}", ex.Message);
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }


        _logger.LogInformation("ETL Worker stopped");
    }
}
