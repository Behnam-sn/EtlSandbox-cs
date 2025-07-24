using System.Data;

using ClickHouse.Client.ADO;

using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Shared.DbConnectionFactories;

public sealed class ClickHouseConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    
    public ClickHouseConnectionFactory(IOptions<DatabaseConnections> options)
    {
        _connectionString = options.Value.Destination;
    }
    
    public IDbConnection CreateConnection() => new ClickHouseConnection(_connectionString);
}