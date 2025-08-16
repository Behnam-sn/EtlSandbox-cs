using System.Data;

using ClickHouse.Client.ADO;

using EtlSandbox.Domain.Common.DbConnectionFactories;

namespace EtlSandbox.Infrastructure.Common.DbConnectionFactories.Bases;

public abstract class BaseClickHouseConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    protected BaseClickHouseConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection() => new ClickHouseConnection(_connectionString);
}