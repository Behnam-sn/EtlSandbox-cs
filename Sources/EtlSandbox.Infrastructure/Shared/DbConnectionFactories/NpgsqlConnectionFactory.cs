using System.Data;

using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Extensions.Options;

using Npgsql;

namespace EtlSandbox.Infrastructure.Shared.DbConnectionFactories;

public class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    
    public NpgsqlConnectionFactory(IOptions<DatabaseConnections> options)
    {
        _connectionString = options.Value.SqlServer;
    }
    
    public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}