using System.Data;

using EtlSandbox.Domain.Common;

using Microsoft.Data.SqlClient;

namespace EtlSandbox.Infrastructure.Common.DbConnectionFactories;

public sealed class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlServerConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}