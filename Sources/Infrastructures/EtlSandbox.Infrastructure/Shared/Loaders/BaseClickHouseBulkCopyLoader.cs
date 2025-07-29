using System.Data;

using ClickHouse.Client.Copy;

using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Infrastructure.Shared.Loaders;

public abstract class BaseClickHouseBulkCopyLoader<T> : ILoader<T>
    where T : class, IEntity
{
    private readonly string _connectionString;

    protected BaseClickHouseBulkCopyLoader(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected abstract string TableName { get; }

    public async Task LoadAsync(List<T> items, CancellationToken cancellationToken)
    {
        // Column order must match the database schema
        var dataTable = GetDataTable(items);

        using var bulkCopy = new ClickHouseBulkCopy(_connectionString)
        {
            DestinationTableName = TableName
        };
        
        await bulkCopy.InitAsync();
        await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
    }

    protected abstract DataTable GetDataTable(List<T> items);
}