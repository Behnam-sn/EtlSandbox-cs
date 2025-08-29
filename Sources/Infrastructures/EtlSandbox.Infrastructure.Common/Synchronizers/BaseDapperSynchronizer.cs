using Dapper;

using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.DbConnectionFactories;

namespace EtlSandbox.Infrastructure.Common.Synchronizers;

public abstract class BaseDapperSynchronizer<T> : ISynchronizer<T>
    where T : class, IEntity
{
    private readonly IDestinationDbConnectionFactory _dbConnectionFactory;

    protected BaseDapperSynchronizer(IDestinationDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    protected abstract string Sql { get; }

    public async Task SoftDeleteObsoleteRowsAsync(long from, long to)
    {
        var parameters = new
        {
            From = from,
            To = to,
        };
        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.ExecuteAsync(Sql, parameters);
    }
}