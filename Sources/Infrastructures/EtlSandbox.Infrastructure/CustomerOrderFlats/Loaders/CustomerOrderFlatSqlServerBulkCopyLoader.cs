using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Infrastructure.Shared.Loaders;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;

public sealed class CustomerOrderFlatSqlServerBulkCopyLoader(string connectionString)
    : BaseSqlBulkCopyLoader<CustomerOrderFlat>(connectionString)
{
    protected override string TableName => "CustomerOrderFlats";
}