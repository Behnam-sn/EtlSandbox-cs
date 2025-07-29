using System.Data;

using EtlSandbox.Domain.Shared;

using Microsoft.Data.SqlClient;

namespace EtlSandbox.Infrastructure.Shared.DbConnectionFactories;

public sealed class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlServerConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}