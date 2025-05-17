using System.Data;
using EtlSandbox.Domain;
using Microsoft.Data.SqlClient;

namespace EtlSandbox.Infrastructure;

public class SqlServerLoader : ILoader<CustomerOrderFlat>
{
    private readonly string _connectionString;

    public SqlServerLoader(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task LoadAsync(IEnumerable<CustomerOrderFlat> data, CancellationToken cancellationToken)
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

        using var bulkCopy = new SqlBulkCopy(_connectionString)
        {
            DestinationTableName = "CustomerOrderFlat"
        };

        await bulkCopy.WriteToServerAsync(table, cancellationToken);
    }
}