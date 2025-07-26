using Dapper;

using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Infrastructure.Shared.Repositories;

public abstract class BaseDapperRepository<T> : IRepository<T>
    where T : class,IEntity
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    protected BaseDapperRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }
    
    protected abstract string TableName { get; }

    public async Task<long> GetLastProcessedImportantIdAsync()
    {
        var sql = $"SELECT max(Id) FROM {TableName}";
        using var connection = _dbConnectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<long?>(sql);
        return result ?? 0;
    }

    public async Task<long> GetLastSoftDeletedItemIdAsync()
    {
        var sql = $"""
                   SELECT max(Id)
                   FROM {TableName}
                   WHERE IsDeleted = 1;
                   """;
        using var connection = _dbConnectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<long?>(sql);
        return result ?? 0;
    }

    public async Task<long> GetLastItemIdAsync()
    {
        var sql = $"SELECT max(Id) FROM {TableName}";
        using var connection = _dbConnectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<long?>(sql);
        return result ?? 0;
    }
}