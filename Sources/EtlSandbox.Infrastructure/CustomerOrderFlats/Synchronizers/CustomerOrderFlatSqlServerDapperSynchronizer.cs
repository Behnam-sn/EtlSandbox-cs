using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.Shared.Synchronizers;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Synchronizers;

public sealed class CustomerOrderFlatSqlServerDapperSynchronizer(ILogger<CustomerOrderFlatSqlServerDapperSynchronizer> logger, IUnitOfWork unitOfWork)
    : BaseDapperSynchronizer<CustomerOrderFlat>(logger, unitOfWork, Sql)
{
    private const string Sql = @"
                UPDATE T
                SET IsDeleted = 1
                FROM CustomerOrders T
                INNER JOIN (
                    SELECT CustomerName, MAX(Id) AS MaxId
                    FROM CustomerOrders
                    WHERE Id BETWEEN @FromId AND @ToId
                    GROUP BY CustomerName
                ) Latest ON T.CustomerName = Latest.CustomerName
                WHERE T.Id < Latest.MaxId
                AND T.IsDeleted = 0";
}