using Dapper;

using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Infrastructure.Shared.Extractors;

public abstract class BaseDapperExtractor<T> : IExtractor<T>
    where T : class, IEntity
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    protected BaseDapperExtractor(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    protected abstract string Sql { get; }

    public async Task<List<T>> ExtractAsync(long lastProcessedId, int batchSize, CancellationToken cancellationToken = default)
    {
        var parameters = new
        {
            LastProcessedId = lastProcessedId,
            BatchSize = batchSize
        };
        using var connection = _dbConnectionFactory.CreateConnection();
        var items = await connection.QueryAsync<T>(Sql, parameters);
        return items.ToList();
    }
}