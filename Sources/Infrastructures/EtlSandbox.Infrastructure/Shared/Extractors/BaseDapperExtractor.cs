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

    public async Task<List<T>> ExtractAsync(long from, long to, CancellationToken cancellationToken = default)
    {
        var parameters = new
        {
            From = from,
            To = to
        };
        using var connection = _dbConnectionFactory.CreateConnection();
        var items = await connection.QueryAsync<T>(Sql, parameters);
        return items.ToList();
    }
}