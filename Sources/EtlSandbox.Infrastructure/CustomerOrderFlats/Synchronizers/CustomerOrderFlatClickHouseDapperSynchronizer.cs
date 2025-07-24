using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.Shared.Synchronizers;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Synchronizers;

public sealed class CustomerOrderFlatClickHouseDapperSynchronizer(IUnitOfWork unitOfWork)
    : BaseDapperSynchronizer<CustomerOrderFlat>(unitOfWork)
{
    protected override string Sql => """
                                     ALTER TABLE SakilaFlat.CustomerOrderFlats
                                     UPDATE IsDeleted = 1
                                     WHERE (Id, IsDeleted) IN (
                                         SELECT 
                                             T.Id, 
                                             T.IsDeleted
                                         FROM SakilaFlat.CustomerOrderFlats T
                                         INNER JOIN (
                                             SELECT 
                                                 CustomerName, 
                                                 MAX(Id) AS MaxId
                                             FROM SakilaFlat.CustomerOrderFlats
                                             WHERE Id BETWEEN @FromId AND @ToId
                                             GROUP BY CustomerName
                                         ) Latest ON T.CustomerName = Latest.CustomerName
                                         WHERE 
                                             T.Id < Latest.MaxId
                                             AND T.Id < @ToId
                                             AND T.IsDeleted = 0
                                     );
                                     """;
}