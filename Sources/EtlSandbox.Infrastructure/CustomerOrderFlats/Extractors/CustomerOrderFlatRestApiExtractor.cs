using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;
using EtlSandbox.Infrastructure.Shared.Extractors;

using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;

public sealed class CustomerOrderFlatRestApiExtractor(string baseUrl, IRestApiClient restApiClient)
    : BaseRestApiExtractor<CustomerOrderFlat>(baseUrl, restApiClient)
{
    protected override string Path => "CustomerOrderFlats";
}