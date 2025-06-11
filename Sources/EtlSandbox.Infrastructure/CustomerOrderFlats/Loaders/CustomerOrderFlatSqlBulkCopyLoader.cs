using System.Data;

using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;

public class CustomerOrderFlatSqlBulkCopyLoader : ILoader<CustomerOrderFlat>
{
    private readonly string _destinationConnectionString;
    private readonly ILogger<CustomerOrderFlatSqlBulkCopyLoader> _logger;

    public CustomerOrderFlatSqlBulkCopyLoader(IOptions<DatabaseConnections> options, ILogger<CustomerOrderFlatSqlBulkCopyLoader> logger)
    {
        _destinationConnectionString = options.Value.SqlServer;
        _logger = logger;
    }

    public async Task LoadAsync(List<CustomerOrderFlat> data, CancellationToken cancellationToken, IDbTransaction? transaction = null)
    {
        var table = new DataTable();
        table.Columns.Add("RentalId", typeof(int));
        table.Columns.Add("CustomerName", typeof(string));
        table.Columns.Add("Amount", typeof(decimal));
        table.Columns.Add("RentalDate", typeof(DateTime));

        foreach (var item in data)
        {
            table.Rows.Add(item.RentalId, item.CustomerName, item.Amount, item.RentalDate);
        }

        SqlBulkCopy bulkCopy;
        if (transaction?.Connection is SqlConnection sqlConnection)
        {
            bulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, (SqlTransaction)transaction);
        }
        else
        {
            bulkCopy = new SqlBulkCopy(_destinationConnectionString);
        }

        using (bulkCopy)
        {
            bulkCopy.DestinationTableName = "CustomerOrders";
            _logger.LogInformation("Loading {Count} rows", data.Count);
            await bulkCopy.WriteToServerAsync(table, cancellationToken);
            _logger.LogInformation("Load completed");
        }
    }
}