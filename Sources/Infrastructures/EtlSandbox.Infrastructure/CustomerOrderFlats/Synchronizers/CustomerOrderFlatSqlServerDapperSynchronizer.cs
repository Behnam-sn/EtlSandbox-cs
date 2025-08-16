using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.DbConnectionFactories;
using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Infrastructure.Common.Synchronizers;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Synchronizers;

public sealed class CustomerOrderFlatSqlServerDapperSynchronizer(IDestinationDbConnectionFactory dbConnectionFactory)
    : BaseDapperSynchronizer<CustomerOrderFlat>(dbConnectionFactory)
{
    protected override string Sql => """
                                     UPDATE T
                                     SET IsDeleted = 1
                                     FROM CustomerOrderFlats T
                                     INNER JOIN (
                                         SELECT CustomerName, MAX(Id) AS MaxId
                                         FROM CustomerOrderFlats
                                         WHERE Id BETWEEN @From AND @To
                                         GROUP BY CustomerName
                                     ) Latest ON T.CustomerName = Latest.CustomerName AND T.Id < Latest.MaxId AND T.IsDeleted = 0
                                     """;
}