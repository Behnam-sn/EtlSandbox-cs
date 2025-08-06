using Dapper;

using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Repositories;

namespace EtlSandbox.Infrastructure.Common.Repositories;

public abstract class BaseDapperDestinationRepository<T> : IDestinationRepository<T>
    where T : class, IEntity
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    protected BaseDapperDestinationRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    protected abstract string TableName { get; }

    protected abstract string GetLastInsertedImportantIdSql { get; }

    protected abstract string GetLastSoftDeletedItemIdSql { get; }

    protected abstract string GetLastItemIdSql { get; }

    public async Task<long> GetLastInsertedImportantIdAsync()
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<long?>(GetLastInsertedImportantIdSql);
        return result ?? 0;
    }

    public async Task<long> GetLastSoftDeletedItemIdAsync()
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<long?>(GetLastSoftDeletedItemIdSql);
        return result ?? 0;
    }

    public async Task<long> GetLastItemIdAsync()
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<long?>(GetLastItemIdSql);
        return result ?? 0;
    }
}