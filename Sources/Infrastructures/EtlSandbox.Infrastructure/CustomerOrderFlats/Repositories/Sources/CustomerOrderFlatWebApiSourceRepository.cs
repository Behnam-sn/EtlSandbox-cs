using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Repositories;
using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Infrastructure.Common.Repositories.Sources;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories.Sources;

public sealed class CustomerOrderFlatWebApiSourceRepository(string baseUrl, IRestApiClient restApiClient)
    : BaseWebApiSourceRepository<CustomerOrderFlat>(baseUrl, restApiClient)
{
    protected override string Path => "CustomerOrderFlats";
}