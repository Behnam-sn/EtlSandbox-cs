using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.Shared.Repositories;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories;

public sealed class CustomerOrderFlatClickHouseDapperDestinationRepository(IDbConnectionFactory dbConnectionFactory)
    : BaseClickHouseDapperDestinationRepository<CustomerOrderFlat>(dbConnectionFactory)
{
    protected override string TableName => "SakilaFlat.CustomerOrderFlats";
}
