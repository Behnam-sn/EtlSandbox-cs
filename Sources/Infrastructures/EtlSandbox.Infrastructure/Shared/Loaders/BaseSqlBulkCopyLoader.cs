using System.Data;

using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.Shared.Converters;

using Microsoft.Data.SqlClient;

namespace EtlSandbox.Infrastructure.Shared.Loaders;

public abstract class BaseSqlBulkCopyLoader<T> : ILoader<T>
    where T : class, IEntity
{
    private readonly string _connectionString;

    protected BaseSqlBulkCopyLoader(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected abstract string TableName { get; }

    public async Task LoadAsync(List<T> items, CancellationToken cancellationToken)
    {
        var dataTable = DataTableConverter.ToDataTable(items);

        using var bulkCopy = new SqlBulkCopy(_connectionString);
        bulkCopy.DestinationTableName = TableName;

        foreach (DataColumn column in dataTable.Columns)
        {
            bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
        }

        await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
    }
}