using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Infrastructure.Common.Repositories;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories;

public sealed class CustomerOrderFlatClickHouseDapperDestinationRepository(IDbConnectionFactory dbConnectionFactory)
    : BaseClickHouseDapperDestinationRepository<CustomerOrderFlat>(dbConnectionFactory)
{
    protected override string TableName => "SakilaFlat.CustomerOrderFlats";
}
