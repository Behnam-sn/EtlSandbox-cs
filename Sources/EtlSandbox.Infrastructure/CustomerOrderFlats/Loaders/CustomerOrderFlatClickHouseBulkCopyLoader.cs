using System.Data;

using ClickHouse.Client.Copy;

using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;

public sealed class CustomerOrderFlatClickHouseBulkCopyLoader : ILoader<CustomerOrderFlat>
{
    private readonly string _connectionString;

    public CustomerOrderFlatClickHouseBulkCopyLoader(IOptions<DatabaseConnections> databaseConnectionsOptions)
    {
        _connectionString = databaseConnectionsOptions.Value.Destination;
    }

    public async Task LoadAsync(List<CustomerOrderFlat> items, CancellationToken cancellationToken)
    {
        // var table = DataTableConverter.ToDataTable(data);
        var table = new DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("RentalId", typeof(int));
        table.Columns.Add("CustomerName", typeof(string));
        table.Columns.Add("Amount", typeof(decimal));
        table.Columns.Add("RentalDate", typeof(DateTime));
        table.Columns.Add("Category", typeof(string));
        table.Columns.Add("IsDeleted", typeof(bool));

        foreach (var item in items)
        {
            table.Rows.Add(
                item.Id,
                item.RentalId,
                item.CustomerName,
                item.Amount,
                item.RentalDate,
                item.Category,
                item.IsDeleted
            );
        }

        using var bulkCopyInterface = new ClickHouseBulkCopy(_connectionString)
        {
            DestinationTableName = "SakilaFlat.CustomerOrderFlats"
        };

        await bulkCopyInterface.InitAsync();
        await bulkCopyInterface.WriteToServerAsync(table, cancellationToken);
    }
}