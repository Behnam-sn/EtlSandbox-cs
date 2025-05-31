using EtlSandbox.Domain;

namespace EtlSandbox.Worker;

public sealed class EtlWorker : BackgroundService
{
    private readonly ILogger<EtlWorker> _logger;
    private readonly IExtractor<CustomerOrderFlat> _extractor;
    private readonly ITransformer<CustomerOrderFlat> _transformer;
    private readonly ILoader<CustomerOrderFlat> _loader;

    public EtlWorker(
        ILogger<EtlWorker> logger,
        IExtractor<CustomerOrderFlat> extractor,
        ITransformer<CustomerOrderFlat> transformer,
        ILoader<CustomerOrderFlat> loader)
    {
        _logger = logger;
        _extractor = extractor;
        _transformer = transformer;
        _loader = loader;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ETL Worker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var since = await _extractor.GetLastProcessedTimestampAsync();
                var data = await _extractor.ExtractAsync(since, stoppingToken);

                if (data.Any())
                {
                    var transformed = data.Select(_transformer.Transform).ToList();
                    await _loader.LoadAsync(transformed, stoppingToken);
                    await _extractor.UpdateLastProcessedTimestampAsync(data.Max(x => x.RentalDate));
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
