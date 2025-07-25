using Dapper;

using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Infrastructure.Shared.Synchronizers;

public abstract class BaseDapperSynchronizer<T> : ISynchronizer<T>
    where T : class, IEntity
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    protected BaseDapperSynchronizer(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    protected abstract string Sql { get; }

    public async Task SoftDeleteObsoleteRowsAsync(long fromId, long toId)
    {
        var parameters = new
        {
            FromId = fromId,
            ToId = toId,
        };

        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.ExecuteAsync(Sql, parameters);
    }
}