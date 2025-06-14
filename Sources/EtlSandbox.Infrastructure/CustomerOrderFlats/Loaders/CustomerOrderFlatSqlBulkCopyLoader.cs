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
        table.Columns.Add("Category", typeof(string));
        table.Columns.Add("UniqId", typeof(int));
        table.Columns.Add("IsDeleted", typeof(bool));

        foreach (var item in data)
        {
            table.Rows.Add(
                item.RentalId,
                item.CustomerName,
                item.Amount,
                item.RentalDate,
                item.Category,
                item.UniqId,
                item.IsDeleted
            );
        }

        SqlBulkCopy bulkCopy;
        if (transaction?.Connection is SqlConnection sqlConnection)
        {
            bulkCopy = new(sqlConnection, SqlBulkCopyOptions.Default, (SqlTransaction)transaction);
        }
        else
        {
            bulkCopy = new(_destinationConnectionString);
        }

        using (bulkCopy)
        {
            bulkCopy.DestinationTableName = "CustomerOrders";
            await bulkCopy.WriteToServerAsync(table, cancellationToken);
        }
    }
}