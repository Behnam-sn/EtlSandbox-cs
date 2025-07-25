using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Shared.Extractors;

public abstract class BaseRestApiExtractor<T> : IExtractor<T>
    where T : class, IEntity
{
    private readonly string _baseUrl;

    private readonly IRestApiClient _restApiClient;

    protected BaseRestApiExtractor(string baseUrl, IRestApiClient restApiClient)
    {
        _baseUrl = baseUrl;
        _restApiClient = restApiClient;
    }
    
    protected abstract string Path { get; }
    
    public async Task<List<T>> ExtractAsync(long lastProcessedId, int batchSize, CancellationToken cancellationToken)
    {
        var items = await _restApiClient.GetAsync<List<T>>(
            baseUrl: _baseUrl,
            path: Path,
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