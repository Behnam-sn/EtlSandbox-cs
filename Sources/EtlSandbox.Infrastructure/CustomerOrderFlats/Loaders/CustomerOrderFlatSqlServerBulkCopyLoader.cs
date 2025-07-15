using System.Data;

using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;
using EtlSandbox.Infrastructure.Shared.Converters;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;

public sealed class CustomerOrderFlatSqlServerBulkCopyLoader : ILoader<CustomerOrderFlat>
{
    private readonly string _destinationDatabaseConnectionString;

    private readonly ILogger<CustomerOrderFlatSqlServerBulkCopyLoader> _logger;

    public CustomerOrderFlatSqlServerBulkCopyLoader(IOptions<DatabaseConnections> options, ILogger<CustomerOrderFlatSqlServerBulkCopyLoader> logger)
    {
        _destinationDatabaseConnectionString = options.Value.Destination;
        _logger = logger;
    }

    public async Task LoadAsync(List<CustomerOrderFlat> data, CancellationToken cancellationToken, IDbTransaction? transaction = null)
    {
        var table = DataTableConverter.ToDataTable(data);

        SqlBulkCopy bulkCopy;
        if (transaction?.Connection is SqlConnection sqlConnection)
        {
            bulkCopy = new(sqlConnection, SqlBulkCopyOptions.Default, (SqlTransaction)transaction);
        }
        else
        {
            bulkCopy = new(_destinationDatabaseConnectionString);
        }

        using (bulkCopy)
        {
            bulkCopy.DestinationTableName = "CustomerOrders";
            await bulkCopy.WriteToServerAsync(table, cancellationToken);
        }
    }
}
