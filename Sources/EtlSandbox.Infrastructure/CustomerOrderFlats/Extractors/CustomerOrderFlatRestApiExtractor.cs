using System.Net.Http.Json;

using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Domain.Shared;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;

public sealed class CustomerOrderFlatRestApiExtractor : IExtractor<CustomerOrderFlat>
{
    private readonly ILogger<CustomerOrderFlatRestApiExtractor> _logger;

    private readonly HttpClient _httpClient;

    public CustomerOrderFlatRestApiExtractor(ILogger<CustomerOrderFlatRestApiExtractor> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<IReadOnlyList<CustomerOrderFlat>> ExtractAsync(int lastProcessedId, int batchSize, CancellationToken cancellationToken)
    {
        var url = $"http://localhost:5050/api/customers?lastProcessedId={lastProcessedId}&batchSize={batchSize}";
        var customers = await _httpClient.GetFromJsonAsync<List<CustomerOrderFlat>>(url, cancellationToken);
        return customers ?? [];
    }
}