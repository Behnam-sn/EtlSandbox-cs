using EtlSandbox.Domain.Shared;

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

    public async Task<List<T>> ExtractAsync(long from, long to, CancellationToken cancellationToken = default)
    {
        var items = await _restApiClient.GetAsync<List<T>>(
            baseUrl: _baseUrl,
            path: Path,
            queryParams: new
            {
                from,
                to
            },
            cancellationToken: cancellationToken
        );
        return items ?? [];
    }
}