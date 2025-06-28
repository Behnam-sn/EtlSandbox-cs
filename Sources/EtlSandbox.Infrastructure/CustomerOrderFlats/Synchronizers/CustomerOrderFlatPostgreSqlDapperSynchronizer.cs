using System.Data;

using Dapper;

using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

using Npgsql;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Synchronizers;

public sealed class CustomerOrderFlatPostgreSqlDapperSynchronizer : ISynchronizer<CustomerOrderFlat>
{
    private readonly string _destinationDatabaseConnectionString;

    public CustomerOrderFlatPostgreSqlDapperSynchronizer(IOptions<DatabaseConnections> options)
    {
        _destinationDatabaseConnectionString = options.Value.SqlServer;
    }

    public async Task SoftDeleteObsoleteRowsAsync(int fromId, int toId, IDbTransaction? transaction = null)
    {
        const string updateSql = """
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
            await using var connection = new NpgsqlConnection(_destinationDatabaseConnectionString);
            await connection.ExecuteAsync(updateSql, parameters);
        }
        else
        {
            var connection = transaction.Connection ?? throw new ArgumentNullException(nameof(transaction), "Transaction must have a valid connection.");
            await connection.ExecuteAsync(updateSql, parameters, transaction);
        }
    }
}