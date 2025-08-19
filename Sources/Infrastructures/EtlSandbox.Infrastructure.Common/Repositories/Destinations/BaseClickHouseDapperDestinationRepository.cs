using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.DbConnectionFactories;

namespace EtlSandbox.Infrastructure.Common.Repositories.Destinations;

public abstract class BaseClickHouseDapperDestinationRepository<T>(IDestinationDbConnectionFactory dbConnectionFactory)
    : BaseDapperDestinationRepository<T>(dbConnectionFactory)
    where T : class, IEntity
{
    protected override string GetLastInsertedSourceIdSql => $"SELECT max(Id) FROM {TableName}";

    protected override string GetLastSoftDeletedItemIdSql => $"""
                                                              SELECT max(Id)
                                                              FROM {TableName}
                                                              WHERE IsDeleted = 1;
                                                              """;

    protected override string GetLastItemIdSql => $"SELECT max(Id) FROM {TableName}";
}