using System;
using System.Threading;
using System.Threading.Tasks;

using Flurl;
using Flurl.Http;

namespace EtlSandbox.Infrastructure.Shared.ApiClient;

public class FlurlApiClient : IApiClient
{
    public async Task<T?> GetAsync<T>(string baseUrl, string path, object? queryParams = null, CancellationToken cancellationToken = default)
    {
        var url = baseUrl.TrimEnd('/') + "/" + path.TrimStart('/');
        var flurlRequest = url.SetQueryParams(queryParams);
        return await flurlRequest.GetJsonAsync<T>(cancellationToken: cancellationToken);
    }
    // You can implement PostAsync, PutAsync, etc. as needed
}