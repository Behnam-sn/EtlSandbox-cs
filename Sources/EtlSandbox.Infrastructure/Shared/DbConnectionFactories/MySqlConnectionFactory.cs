using System.Data;

using EtlSandbox.Domain.Shared;

using MySql.Data.MySqlClient;

namespace EtlSandbox.Infrastructure.Shared.DbConnectionFactories;

public sealed class MySqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public MySqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection() => new MySqlConnection(_connectionString);
}