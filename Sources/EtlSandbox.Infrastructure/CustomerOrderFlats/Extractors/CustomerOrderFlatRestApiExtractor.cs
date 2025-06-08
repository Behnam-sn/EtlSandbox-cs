using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Domain.Shared;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;

public sealed class CustomerOrderFlatRestApiExtractor : IExtractor<CustomerOrderFlat>
{
    private readonly ILogger<CustomerOrderFlatRestApiExtractor> _logger;

    private readonly IRestApiClient _restApiClient;

    private readonly string _baseUrl;

    public CustomerOrderFlatRestApiExtractor(ILogger<CustomerOrderFlatRestApiExtractor> logger, IRestApiClient restApiClient)
    {
        _logger = logger;
        _restApiClient = restApiClient;
        _baseUrl = "http://localhost:5050/api"; // Optionally inject this
    }

    public async Task<IReadOnlyList<CustomerOrderFlat>> ExtractAsync(int lastProcessedId, int batchSize, CancellationToken cancellationToken)
    {
        var items = await _restApiClient.GetAsync<List<CustomerOrderFlat>>(
            baseUrl: _baseUrl,
            path: "customers",
            queryParams: new { lastProcessedId, batchSize },
            cancellationToken: cancellationToken
        );
        return items ?? [];
    }
}