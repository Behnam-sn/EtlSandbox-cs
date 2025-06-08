using EtlSandbox.Domain.Shared;

using Flurl;
using Flurl.Http;

namespace EtlSandbox.Infrastructure.Shared.ApiClient;

public class FlurlRestApiClient : IRestApiClient
{
    public async Task<T?> GetAsync<T>(string baseUrl, string path, object? queryParams = null, CancellationToken cancellationToken = default)
    {
        return await baseUrl
            .AppendPathSegment(path)
            .SetQueryParams(queryParams)
            .GetJsonAsync<T>(cancellationToken: cancellationToken);
    }
}