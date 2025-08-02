using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Infrastructure.Shared.Repositories;

public abstract class BaseClickHouseDapperDestinationRepository<T>(IDbConnectionFactory dbConnectionFactory)
    : BaseDapperDestinationRepository<T>(dbConnectionFactory)
    where T : class, IEntity
{
    protected override string GetLastInsertedImportantIdSql => $"SELECT max(Id) FROM {TableName}";

    protected override string GetLastSoftDeletedItemIdSql => $"""
                                                              SELECT max(Id)
                                                              FROM {TableName}
                                                              WHERE IsDeleted = 1;
                                                              """;

    protected override string GetLastItemIdSql => $"SELECT max(Id) FROM {TableName}";
}