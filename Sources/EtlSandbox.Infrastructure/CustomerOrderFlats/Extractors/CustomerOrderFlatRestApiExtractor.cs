using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;

public sealed class CustomerOrderFlatRestApiExtractor : IExtractor<CustomerOrderFlat>
{
    private readonly ILogger<CustomerOrderFlatRestApiExtractor> _logger;

    private readonly IRestApiClient _restApiClient;

    private readonly string _baseUrl;

    public CustomerOrderFlatRestApiExtractor(ILogger<CustomerOrderFlatRestApiExtractor> logger, IRestApiClient restApiClient, IOptions<RestApiConnections> options)
    {
        _logger = logger;
        _restApiClient = restApiClient;
        _baseUrl = options.Value.WebApi;
    }

    public async Task<List<CustomerOrderFlat>> ExtractAsync(int lastProcessedId, int batchSize, CancellationToken cancellationToken)
    {
        var items = await _restApiClient.GetAsync<List<CustomerOrderFlat>>(
            baseUrl: _baseUrl,
            path: "customers",
            queryParams: new
            {
                lastProcessedId,
                batchSize
            },
            cancellationToken: cancellationToken
        );
        return items ?? [];
    }
}