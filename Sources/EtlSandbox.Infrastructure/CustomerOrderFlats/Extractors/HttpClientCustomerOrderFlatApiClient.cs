using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

using EtlSandbox.Domain.CustomerOrderFlats;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;

public class HttpClientCustomerOrderFlatApiClient : ICustomerOrderFlatApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public HttpClientCustomerOrderFlatApiClient(HttpClient httpClient, string baseUrl)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public async Task<IReadOnlyList<CustomerOrderFlat>> GetCustomerOrderFlatsAsync(int lastProcessedId, int batchSize, CancellationToken cancellationToken)
    {
        var url = $"{_baseUrl}/api/customers?lastProcessedId={lastProcessedId}&batchSize={batchSize}";
        var customers = await _httpClient.GetFromJsonAsync<List<CustomerOrderFlat>>(url, cancellationToken);
        return customers ?? new List<CustomerOrderFlat>();
    }
}
