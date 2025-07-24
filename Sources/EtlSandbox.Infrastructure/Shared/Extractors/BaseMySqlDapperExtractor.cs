using Dapper;

using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Extensions.Options;

using MySql.Data.MySqlClient;

namespace EtlSandbox.Infrastructure.Shared.Extractors;

public abstract class BaseMySqlDapperExtractor<T> : IExtractor<T>
    where T : class, IEntity
{
    private readonly string _sourceConnectionString;

    protected BaseMySqlDapperExtractor(IOptions<DatabaseConnections> options)
    {
        _sourceConnectionString = options.Value.Source;
    }

    protected abstract string Sql { get; }

    public async Task<List<T>> ExtractAsync(long lastProcessedId, int batchSize, CancellationToken cancellationToken)
    {
        await using var connection = new MySqlConnection(_sourceConnectionString);

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