using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.Shared.ApiClient;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;

public sealed class CustomerOrderFlatRestApiExtractor : IExtractor<CustomerOrderFlat>
{
    private readonly ILogger<CustomerOrderFlatRestApiExtractor> _logger;

    private readonly IApiClient _apiClient;
    private readonly string _baseUrl;
    private readonly string _path = "/api/customers";

    public CustomerOrderFlatRestApiExtractor(ILogger<CustomerOrderFlatRestApiExtractor> logger, IApiClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
        _baseUrl = "http://localhost:5050"; // Optionally inject this
    }

    public async Task<IReadOnlyList<CustomerOrderFlat>> ExtractAsync(int lastProcessedId, int batchSize, CancellationToken cancellationToken)
    {
        var queryParams = new { lastProcessedId, batchSize };
        var customers = await _apiClient.GetAsync<List<CustomerOrderFlat>>(_baseUrl, _path, queryParams, cancellationToken);
        return customers ?? [];
    }
}