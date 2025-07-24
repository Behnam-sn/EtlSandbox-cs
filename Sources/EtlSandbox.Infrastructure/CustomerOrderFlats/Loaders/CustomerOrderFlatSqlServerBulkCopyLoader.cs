using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.Shared.Loaders;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;

public sealed class CustomerOrderFlatSqlServerBulkCopyLoader(IUnitOfWork unitOfWork)
    : BaseSqlBulkCopyLoader<CustomerOrderFlat>(unitOfWork)
{
    protected override string TableName => "CustomerOrderFlats";
}