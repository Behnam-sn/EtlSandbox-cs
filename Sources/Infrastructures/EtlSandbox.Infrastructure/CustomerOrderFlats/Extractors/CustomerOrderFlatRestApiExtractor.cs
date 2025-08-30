using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Options;
using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Infrastructure.Common.Extractors;

using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;

public sealed class CustomerOrderFlatRestApiExtractor(IOptions<ConnectionStrings> options, IRestApiClient restApiClient)
    : BaseRestApiExtractor<CustomerOrderFlat>(options, restApiClient)
{
    protected override string Path => "CustomerOrderFlats";
}