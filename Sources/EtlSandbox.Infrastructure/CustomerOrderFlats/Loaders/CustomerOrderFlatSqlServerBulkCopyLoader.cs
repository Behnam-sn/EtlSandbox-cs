using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared.Options;
using EtlSandbox.Infrastructure.Shared.Loaders;

using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;

public sealed class CustomerOrderFlatSqlServerBulkCopyLoader(IOptions<DatabaseConnections> databaseConnectionsOptions)
    : BaseSqlBulkCopyLoader<CustomerOrderFlat>(databaseConnectionsOptions, tableName: "CustomerOrders");