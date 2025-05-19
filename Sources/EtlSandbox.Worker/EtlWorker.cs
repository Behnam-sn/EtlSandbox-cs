using EtlSandbox.Domain;

namespace EtlSandbox.Worker;

public class EtlWorker : BackgroundService
{
    private readonly IExtractor<CustomerOrderFlat> _extractor;
    private readonly ILoader<CustomerOrderFlat> _loader;
    private readonly ITransformer<CustomerOrderFlat> _transformer;
    private readonly ILogger<EtlWorker> _logger;

    public EtlWorker(
        IExtractor<CustomerOrderFlat> extractor,
        ILoader<CustomerOrderFlat> loader,
        ITransformer<CustomerOrderFlat> transformer,
        ILogger<EtlWorker> logger)
    {
        _extractor = extractor;
        _loader = loader;
        _transformer = transformer;
        _logger = logger;
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
