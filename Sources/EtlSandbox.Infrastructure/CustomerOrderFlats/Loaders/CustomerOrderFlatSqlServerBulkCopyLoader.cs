using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.Shared.Loaders;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;

public sealed class CustomerOrderFlatSqlServerBulkCopyLoader(IDbConnectionFactory dbConnectionFactory)
    : BaseSqlBulkCopyLoader<CustomerOrderFlat>(dbConnectionFactory)
{
    protected override string TableName => "CustomerOrderFlats";
}