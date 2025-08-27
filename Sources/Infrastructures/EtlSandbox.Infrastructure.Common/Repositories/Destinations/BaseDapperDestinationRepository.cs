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

    protected abstract string GetMaxSourceIdSql { get; }

    protected abstract string GetMaxIdSql { get; }

    public async Task<long> GetMaxSourceIdOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<long?>(GetMaxSourceIdSql);
        return result ?? 0;
    }

    public async Task<long> GetMaxIdOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<long?>(GetMaxIdSql);
        return result ?? 0;
    }

    public Task SoftDeleteObsoleteRowsAsync(long from, long to, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}