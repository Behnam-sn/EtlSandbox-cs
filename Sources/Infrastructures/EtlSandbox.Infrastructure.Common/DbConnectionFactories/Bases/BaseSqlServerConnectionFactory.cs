using System.Data;

using EtlSandbox.Domain.Common.DbConnectionFactories;

using Microsoft.Data.SqlClient;

namespace EtlSandbox.Infrastructure.Common.DbConnectionFactories.Bases;

public abstract class BaseSqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    protected BaseSqlServerConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}