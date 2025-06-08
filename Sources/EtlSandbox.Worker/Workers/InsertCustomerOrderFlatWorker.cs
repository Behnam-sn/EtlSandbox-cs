using EtlSandbox.Domain;

namespace EtlSandbox.Worker.Workers;

public sealed class InsertCustomerOrderFlatWorker : BackgroundService
{
    private readonly ILogger<InsertCustomerOrderFlatWorker> _logger;

    private readonly IServiceProvider _serviceProvider;

    public InsertCustomerOrderFlatWorker(ILogger<InsertCustomerOrderFlatWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ETL Worker started");

        using var scope = _serviceProvider.CreateScope();

        var etlStateCommandRepository = scope.ServiceProvider.GetRequiredService<IEtlStateCommandRepository>();
        var extractor = scope.ServiceProvider.GetRequiredService<IExtractor<CustomerOrderFlat>>();
        var transformer = scope.ServiceProvider.GetRequiredService<ITransformer<CustomerOrderFlat>>();
        var loader = scope.ServiceProvider.GetRequiredService<ILoader<CustomerOrderFlat>>();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var lastProcessedId = await etlStateCommandRepository.GetLastProcessedIdAsync();
                var data = await extractor.ExtractAsync(lastProcessedId, stoppingToken);

                if (data.Any())
                {
                    var transformed = data.Select(transformer.Transform).ToList();
                    await loader.LoadAsync(transformed, stoppingToken);
                    await etlStateCommandRepository.UpdateLastProcessedIdAsync(data.Max(x => x.RentalId));
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