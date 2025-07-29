using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.Shared.Extractors;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;

public sealed class CustomerOrderFlatRestApiExtractor(string baseUrl, IRestApiClient restApiClient)
    : BaseRestApiExtractor<CustomerOrderFlat>(baseUrl, restApiClient)
{
    protected override string Path => "CustomerOrderFlats";
}