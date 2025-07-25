using System.Data;

using EtlSandbox.Domain.Shared;

using Npgsql;

namespace EtlSandbox.Infrastructure.Shared.DbConnectionFactories;

public sealed class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public NpgsqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}