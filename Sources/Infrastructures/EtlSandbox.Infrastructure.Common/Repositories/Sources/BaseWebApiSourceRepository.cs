using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Repositories;

namespace EtlSandbox.Infrastructure.Common.Repositories.Sources;

public abstract class BaseWebApiSourceRepository<T> : ISourceRepository<T>
    where T : class, IEntity
{
    private readonly string _baseUrl;

    private readonly IRestApiClient _restApiClient;

    protected BaseWebApiSourceRepository(string baseUrl, IRestApiClient restApiClient)
    {
        _baseUrl = baseUrl;
        _restApiClient = restApiClient;
    }

    protected abstract string Path { get; }

    public async Task<long> GetLastItemIdAsync(CancellationToken cancellationToken = default)
    {
        var item = await _restApiClient.GetAsync<long>(
            baseUrl: _baseUrl,
            path: $"{Path}/GetLastItemId",
            cancellationToken: cancellationToken
        );
        return item;
    }
}