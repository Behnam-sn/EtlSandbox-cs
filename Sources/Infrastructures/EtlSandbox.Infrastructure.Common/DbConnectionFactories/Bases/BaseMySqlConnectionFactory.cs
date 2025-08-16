using System.Data;

using EtlSandbox.Domain.Common.DbConnectionFactories;

using MySql.Data.MySqlClient;

namespace EtlSandbox.Infrastructure.Common.DbConnectionFactories.Bases;

public abstract class BaseMySqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    protected BaseMySqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection() => new MySqlConnection(_connectionString);
}