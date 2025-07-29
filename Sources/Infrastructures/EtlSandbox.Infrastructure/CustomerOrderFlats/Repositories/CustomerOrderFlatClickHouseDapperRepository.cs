using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.Shared.Repositories;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories;

public sealed class CustomerOrderFlatClickHouseDapperRepository(IDbConnectionFactory dbConnectionFactory)
    : BaseClickHouseDapperRepository<CustomerOrderFlat>(dbConnectionFactory)
{
    protected override string TableName => "SakilaFlat.CustomerOrderFlats";
}
