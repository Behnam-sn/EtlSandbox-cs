using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Repositories;
using EtlSandbox.Domain.CustomerOrderFlats.Entities;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories.Sources;

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