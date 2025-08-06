using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Repositories;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories;

public sealed class CustomerOrderFlatWebApiSourceRepository : ISourceRepository<CustomerOrderFlat>
{
    private readonly string _baseUrl;

    private readonly IRestApiClient _restApiClient;

    public CustomerOrderFlatWebApiSourceRepository(string baseUrl, IRestApiClient restApiClient)
    {
        _baseUrl = baseUrl;
        _restApiClient = restApiClient;
    }

    public async Task<long> GetLastItemIdAsync(CancellationToken cancellationToken = default)
    {
        var item = await _restApiClient.GetAsync<long>(
            baseUrl: _baseUrl,
            path: "CustomerOrderFlats/GetLastItemId",
            cancellationToken: cancellationToken
        );
        return item;
    }
}