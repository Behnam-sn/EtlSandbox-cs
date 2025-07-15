using System.Data;

using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;
using EtlSandbox.Infrastructure.Shared.Converters;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Shared.Loaders;

public abstract class BaseSqlBulkCopyLoader<T> : ILoader<T>
{
    private readonly string _connectionString;
    private readonly string _tableName;

    protected BaseSqlBulkCopyLoader(IOptions<DatabaseConnections> databaseConnectionsOptions, string tableName)
    {
        _connectionString = databaseConnectionsOptions.Value.Destination;
        _tableName = tableName;
    }

    public async Task LoadAsync(List<T> data, CancellationToken cancellationToken, IDbTransaction? transaction = null)
    {
        var table = DataTableConverter.ToDataTable(data);

        SqlBulkCopy bulkCopy;
        if (transaction?.Connection is SqlConnection sqlConnection)
        {
            bulkCopy = new(sqlConnection, SqlBulkCopyOptions.Default, (SqlTransaction)transaction);
        }
        else
        {
            bulkCopy = new(_connectionString);
        }

        using (bulkCopy)
        {
            bulkCopy.DestinationTableName = _tableName;
            await bulkCopy.WriteToServerAsync(table, cancellationToken);
        }
    }
}