using System.Data;

using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Shared.DbConnectionFactories;

public class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    
    public SqlServerConnectionFactory(IOptions<DatabaseConnections> options)
    {
        _connectionString = options.Value.Destination;
    }
    
    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}