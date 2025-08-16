using System.Data;

using EtlSandbox.Domain.Common.DbConnectionFactories;

using Npgsql;

namespace EtlSandbox.Infrastructure.Common.DbConnectionFactories.Bases;

public abstract class BaseNpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    protected BaseNpgsqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}