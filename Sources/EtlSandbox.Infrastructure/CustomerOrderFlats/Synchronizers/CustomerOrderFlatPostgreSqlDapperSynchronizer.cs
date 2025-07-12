using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.Shared.Synchronizers;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Synchronizers;

public sealed class CustomerOrderFlatPostgreSqlDapperSynchronizer(ILogger<CustomerOrderFlatPostgreSqlDapperSynchronizer> logger, IUnitOfWork unitOfWork)
    : BaseDapperSynchronizer<CustomerOrderFlat>(logger, unitOfWork, Sql)
{
    private const string Sql = """
                               UPDATE "CustomerOrders" T
                               SET "IsDeleted" = '1'
                               FROM (
                                   SELECT "UniqId", MAX("RentalId") AS "MaxRentalId"
                                   FROM "CustomerOrders"
                                   WHERE "RentalId" BETWEEN @FromId AND @ToId
                                   GROUP BY "UniqId"
                               ) Latest
                               WHERE T."UniqId" = Latest."UniqId"
                               AND T."RentalId" < Latest."MaxRentalId"
                               AND T."IsDeleted" = '0'
                               """;
}