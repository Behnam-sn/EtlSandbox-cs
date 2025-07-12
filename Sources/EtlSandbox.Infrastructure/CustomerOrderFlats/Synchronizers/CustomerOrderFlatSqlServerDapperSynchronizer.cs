using EtlSandbox.Domain.CustomerOrderFlats;
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
                    SELECT UniqId, MAX(RentalId) AS MaxRentalId
                    FROM CustomerOrders
                    WHERE RentalId BETWEEN @FromId AND @ToId
                    GROUP BY UniqId
                ) Latest ON T.UniqId = Latest.UniqId
                WHERE T.RentalId < Latest.MaxRentalId
                AND T.IsDeleted = 0";
}