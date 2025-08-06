using System.Data;

using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Infrastructure.Common.Loaders;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;

public sealed class CustomerOrderFlatClickHouseBulkCopyLoader(string connectionString)
    : BaseClickHouseBulkCopyLoader<CustomerOrderFlat>(connectionString)
{
    protected override string TableName => "SakilaFlat.CustomerOrderFlats";

    protected override DataTable GetDataTable(List<CustomerOrderFlat> items)
    {
        var dataTable = new DataTable();

        dataTable.Columns.Add("Id", typeof(long));
        dataTable.Columns.Add("RentalId", typeof(long));
        dataTable.Columns.Add("CustomerName", typeof(string));
        dataTable.Columns.Add("Amount", typeof(decimal));
        dataTable.Columns.Add("RentalDate", typeof(DateTime));
        dataTable.Columns.Add("Category", typeof(string));
        dataTable.Columns.Add("IsDeleted", typeof(bool));

        foreach (var item in items)
        {
            dataTable.Rows.Add(
                item.Id,
                item.RentalId,
                item.CustomerName,
                item.Amount,
                item.RentalDate,
                item.Category,
                item.IsDeleted
            );
        }

        return dataTable;
    }
}