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

    public async Task<List<T>> ExtractAsync(long lastProcessedId, int batchSize, CancellationToken cancellationToken)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        var parameters = new
        {
            LastProcessedId = lastProcessedId,
            BatchSize = batchSize
        };

        var result = await connection.QueryAsync<T>(Sql, parameters);
        var items = result.ToList();
        return items;
    }
}