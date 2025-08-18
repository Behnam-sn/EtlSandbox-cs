using EtlSandbox.Domain.Common.DbConnectionFactories;
using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Infrastructure.Common.Synchronizers;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Synchronizers;

public sealed class CustomerOrderFlatClickHouseDapperSynchronizer(IDestinationDbConnectionFactory dbConnectionFactory)
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
                                                WHERE Id BETWEEN @From AND @To
                                                GROUP BY CustomerName
                                            ) AS Latest ON T.CustomerName = Latest.CustomerName
                                        WHERE T.Id < Latest.MaxId
                                     )
                                     """;
}