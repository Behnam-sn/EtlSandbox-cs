using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.Shared.Synchronizers;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Synchronizers;

public sealed class CustomerOrderFlatSqlServerDapperSynchronizer(IDbConnectionFactory dbConnectionFactory)
    : BaseDapperSynchronizer<CustomerOrderFlat>(dbConnectionFactory)
{
    protected override string Sql => """
                                     UPDATE T
                                     SET IsDeleted = 1
                                     FROM CustomerOrderFlats T
                                     INNER JOIN (
                                         SELECT CustomerName, MAX(Id) AS MaxId
                                         FROM CustomerOrderFlats
                                         WHERE Id BETWEEN @FromId AND @ToId
                                         GROUP BY CustomerName
                                     ) Latest ON T.CustomerName = Latest.CustomerName AND T.Id < Latest.MaxId
                                     AND T.IsDeleted = 0
                                     """;
}