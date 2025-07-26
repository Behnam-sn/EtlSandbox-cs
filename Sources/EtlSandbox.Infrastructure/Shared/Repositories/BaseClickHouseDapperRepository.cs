using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Infrastructure.Shared.Repositories;

public abstract class BaseClickHouseDapperRepository<T>(IDbConnectionFactory dbConnectionFactory)
    : BaseDapperRepository<T>(dbConnectionFactory)
    where T : class, IEntity
{
    protected override string GetLastProcessedImportantIdSql => $"SELECT max(Id) FROM {TableName}";

    protected override string GetLastSoftDeletedItemIdSql => $"""
                                                              SELECT max(Id)
                                                              FROM {TableName}
                                                              WHERE IsDeleted = 1;
                                                              """;

    protected override string GetLastItemIdSql => $"SELECT max(Id) FROM {TableName}";
}