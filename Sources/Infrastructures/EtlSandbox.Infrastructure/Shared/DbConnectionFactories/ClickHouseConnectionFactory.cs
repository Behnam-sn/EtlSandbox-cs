using System.Data;

using ClickHouse.Client.ADO;

using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Infrastructure.Shared.DbConnectionFactories;

public sealed class ClickHouseConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public ClickHouseConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection() => new ClickHouseConnection(_connectionString);
}