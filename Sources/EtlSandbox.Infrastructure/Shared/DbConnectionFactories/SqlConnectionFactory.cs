using System.Data;

using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Shared.DbConnectionFactories;

public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    
    public SqlConnectionFactory(IOptions<DatabaseConnections> options)
    {
        _connectionString = options.Value.SqlServer;
    }
    
    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}