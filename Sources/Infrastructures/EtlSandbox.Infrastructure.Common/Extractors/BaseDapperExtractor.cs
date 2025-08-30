using Dapper;

using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.DbConnectionFactories;

namespace EtlSandbox.Infrastructure.Common.Extractors;

public abstract class BaseDapperExtractor<T> : IExtractor<T>
    where T : class, IEntity
{
    private readonly ISourceDbConnectionFactory _dbConnectionFactory;

    protected BaseDapperExtractor(ISourceDbConnectionFactory dbConnectionFactory)
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