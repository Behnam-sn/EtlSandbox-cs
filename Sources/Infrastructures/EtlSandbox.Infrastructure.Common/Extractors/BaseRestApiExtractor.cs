using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Options;

using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Common.Extractors;

public abstract class BaseRestApiExtractor<T> : IExtractor<T>
    where T : class, IEntity
{
    private readonly ConnectionStrings _connectionStrings;

    private readonly IRestApiClient _restApiClient;

    protected BaseRestApiExtractor(IOptions<ConnectionStrings> options, IRestApiClient restApiClient)
    {
        _connectionStrings = options.Value;
        _restApiClient = restApiClient;
    }

    protected abstract string Path { get; }

    public async Task<List<T>> ExtractAsync(long from, long to, CancellationToken cancellationToken = default)
    {
        var items = await _restApiClient.GetAsync<List<T>>(
            baseUrl: _connectionStrings.Source,
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