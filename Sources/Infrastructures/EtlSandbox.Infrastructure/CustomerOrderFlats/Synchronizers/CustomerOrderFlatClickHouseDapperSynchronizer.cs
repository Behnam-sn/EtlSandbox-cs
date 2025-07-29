using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.Shared.Synchronizers;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Synchronizers;

public sealed class CustomerOrderFlatClickHouseDapperSynchronizer(IDbConnectionFactory dbConnectionFactory)
    : BaseDapperSynchronizer<CustomerOrderFlat>(dbConnectionFactory)
{
    protected override string Sql => """
                                     ALTER TABLE SakilaFlat.CustomerOrderFlats
                                     UPDATE IsDeleted = 1
                                     WHERE (IsDeleted = 0)
                                     AND (CustomerName, Id) IN (
                                        SELECT T.CustomerName, T.Id
                                        FROM SakilaFlat.CustomerOrderFlats AS T
                                            INNER JOIN (
                                                SELECT CustomerName, MAX(Id) AS MaxId
                                                FROM SakilaFlat.CustomerOrderFlats
                                                WHERE Id BETWEEN @FromId AND @ToId
                                                GROUP BY CustomerName
                                            ) AS Latest ON T.CustomerName = Latest.CustomerName
                                        WHERE T.Id < Latest.MaxId
                                     )
                                     """;
}