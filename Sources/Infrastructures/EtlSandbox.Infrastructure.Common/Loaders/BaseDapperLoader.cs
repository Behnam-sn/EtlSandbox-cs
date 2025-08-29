using Dapper;

using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.DbConnectionFactories;

namespace EtlSandbox.Infrastructure.Common.Loaders;

// Todo: replace IDbConnectionFactory with IDestinationDbConnectionFactory
public abstract class BaseDapperLoader<T> : ILoader<T>
    where T : class, IEntity
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    protected BaseDapperLoader(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    protected abstract string Sql { get; }

    public async Task LoadAsync(List<T> items, CancellationToken cancellationToken)
    {
        if (items.Count == 0)
        {
            return;
        }

        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.ExecuteAsync(Sql, items);
    }
}