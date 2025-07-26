using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Infrastructure.Shared.Repositories;

public abstract class BaseClickHouseDapperRepository<T> : BaseDapperRepository<T>
    where T : class, IEntity
{
    protected BaseClickHouseDapperRepository(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
    {
    }

    protected override string GetLastProcessedImportantIdSql => $"SELECT max(Id) FROM {TableName}";

    protected override string GetLastSoftDeletedItemIdSql => $"""
                                                              SELECT max(Id)
                                                              FROM {TableName}
                                                              WHERE IsDeleted = 1;
                                                              """;

    protected override string GetLastItemIdSql => $"SELECT max(Id) FROM {TableName}";
}