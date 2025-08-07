using System.Data;

using ClickHouse.Client.ADO;

using EtlSandbox.Domain.Common;

namespace EtlSandbox.Infrastructure.Common.DbConnectionFactories;

public sealed class ClickHouseConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public ClickHouseConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection() => new ClickHouseConnection(_connectionString);
}