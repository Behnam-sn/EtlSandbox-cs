using Dapper;

using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.DbConnectionFactories;
using EtlSandbox.Domain.Common.Repositories;

namespace EtlSandbox.Infrastructure.Common.Repositories.Destinations;

public abstract class BaseDapperDestinationRepository<T> : IDestinationRepository<T>
    where T : class, IEntity
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    protected BaseDapperDestinationRepository(IDestinationDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    protected abstract string TableName { get; }

    protected abstract string GetLastInsertedSourceIdSql { get; }

    protected abstract string GetLastSoftDeletedItemIdSql { get; }

    protected abstract string GetLastItemIdSql { get; }

    public async Task<long> GetLastSourceIdAsync()
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<long?>(GetLastInsertedSourceIdSql);
        return result ?? 0;
    }

    public async Task<long> GetLastSoftDeletedIdAsync()
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<long?>(GetLastSoftDeletedItemIdSql);
        return result ?? 0;
    }

    public async Task<long> GetLastIdAsync()
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<long?>(GetLastItemIdSql);
        return result ?? 0;
    }
}