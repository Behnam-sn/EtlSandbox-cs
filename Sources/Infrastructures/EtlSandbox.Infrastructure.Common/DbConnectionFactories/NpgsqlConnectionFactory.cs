using System.Data;

using EtlSandbox.Domain.Common;

using Npgsql;

namespace EtlSandbox.Infrastructure.Common.DbConnectionFactories;

public sealed class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public NpgsqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}