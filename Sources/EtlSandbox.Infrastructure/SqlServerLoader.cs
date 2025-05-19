using System.Data;

using EtlSandbox.Domain;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace EtlSandbox.Infrastructure;

public class SqlServerLoader : ILoader<CustomerOrderFlat>
{
    private readonly string _connectionString;
    private readonly ILogger<SqlServerLoader> _logger;

    public SqlServerLoader(string connectionString, ILogger<SqlServerLoader> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
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

        _logger.LogInformation("Loading {Count} rows into SQL Server", data.Count());
        await bulkCopy.WriteToServerAsync(table, cancellationToken);
        _logger.LogInformation("Load completed");
    }
}