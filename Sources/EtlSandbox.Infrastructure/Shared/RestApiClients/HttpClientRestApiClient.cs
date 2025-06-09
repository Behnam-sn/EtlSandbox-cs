using System.Net.Http.Json;
using System.Reflection;

using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Infrastructure.Shared.ApiClient;

public sealed class HttpClientRestApiClient : IRestApiClient
{
    private readonly HttpClient _httpClient;

    public HttpClientRestApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T?> GetAsync<T>(string baseUrl, string path, object? queryParams = null, CancellationToken cancellationToken = default)
    {
        var dict = queryParams != null ? ToDictionary(queryParams) : null;
        var url = BuildUrl(baseUrl, path, dict);
        return await _httpClient.GetFromJsonAsync<T>(url, cancellationToken);
    }

    private static IDictionary<string, object?> ToDictionary(object obj)
    {
        var dict = new Dictionary<string, object?>();
        foreach (var prop in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            dict[prop.Name] = prop.GetValue(obj);
        }
        return dict;
    }

    private static string BuildUrl(string baseUrl, string path, IDictionary<string, object?>? queryParams)
    {
        var uriBuilder = new UriBuilder(baseUrl.TrimEnd('/') + "/" + path.TrimStart('/'));
        if (queryParams != null && queryParams.Count > 0)
        {
            var query = string.Join("&", queryParams
                .Where(kvp => kvp.Value != null)
                .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value!.ToString()!)}"));
            uriBuilder.Query = query;
        }
        return uriBuilder.ToString();
    }
}
