using EtlSandbox.Domain;

namespace EtlSandbox.Worker;

public class EtlWorker : BackgroundService
{
    private readonly IExtractor<CustomerOrderFlat> _extractor;
    private readonly ILoader<CustomerOrderFlat> _loader;
    private readonly ITransformer<CustomerOrderFlat> _transformer;

    public EtlWorker(
        IExtractor<CustomerOrderFlat> extractor,
        ILoader<CustomerOrderFlat> loader,
        ITransformer<CustomerOrderFlat> transformer
    )
    {
        _extractor = extractor;
        _loader = loader;
        _transformer = transformer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
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

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ETL failed: {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
