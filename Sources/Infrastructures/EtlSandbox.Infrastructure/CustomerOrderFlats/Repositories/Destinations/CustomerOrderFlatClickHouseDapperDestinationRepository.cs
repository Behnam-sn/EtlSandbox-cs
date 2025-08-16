using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Infrastructure.Common.Repositories.Destinations;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories.Destinations;

public sealed class CustomerOrderFlatClickHouseDapperDestinationRepository(IDbConnectionFactory dbConnectionFactory)
    : BaseClickHouseDapperDestinationRepository<CustomerOrderFlat>(dbConnectionFactory)
{
    protected override string TableName => "SakilaFlat.CustomerOrderFlats";
}
