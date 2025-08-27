using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.DbConnectionFactories;

namespace EtlSandbox.Infrastructure.Common.Repositories.Destinations;

public abstract class BaseClickHouseDapperDestinationRepository<T>(IDestinationDbConnectionFactory dbConnectionFactory)
    : BaseDapperDestinationRepository<T>(dbConnectionFactory)
    where T : class, IEntity
{
    protected override string GetMaxSourceIdSql => $"SELECT max(Id) FROM {TableName}";

    protected override string GetMaxIdSql => $"SELECT max(Id) FROM {TableName}";
}