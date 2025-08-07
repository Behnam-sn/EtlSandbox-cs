using System.Data;

using EtlSandbox.Domain.Common;

using MySql.Data.MySqlClient;

namespace EtlSandbox.Infrastructure.Common.DbConnectionFactories;

public sealed class MySqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public MySqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection() => new MySqlConnection(_connectionString);
}