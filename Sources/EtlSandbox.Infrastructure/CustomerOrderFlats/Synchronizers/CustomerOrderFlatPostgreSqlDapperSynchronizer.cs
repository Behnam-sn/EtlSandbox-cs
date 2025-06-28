using System.Data;

using Dapper;

using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Synchronizers;

public sealed class CustomerOrderFlatPostgreSqlDapperSynchronizer : ISynchronizer<CustomerOrderFlat>
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerOrderFlatPostgreSqlDapperSynchronizer(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task SoftDeleteObsoleteRowsAsync(int fromId, int toId, IDbTransaction? transaction = null)
    {
        const string sql = """
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

        var parameters = new
        {
            FromId = fromId,
            ToId = toId,
        };

        if (transaction is null)
        {
            var connection = _unitOfWork.Connection;
            await connection.ExecuteAsync(sql, parameters);
        }
        else
        {
            var connection = transaction.Connection ?? throw new ArgumentNullException(nameof(transaction), "Transaction must have a valid connection.");
            await connection.ExecuteAsync(sql, parameters, transaction);
        }
    }
}