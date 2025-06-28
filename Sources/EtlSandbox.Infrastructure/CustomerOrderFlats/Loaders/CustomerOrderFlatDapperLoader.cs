using System.Data;

using Dapper;

using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Npgsql;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;

public sealed class CustomerOrderFlatDapperLoader : ILoader<CustomerOrderFlat>
{
    private readonly string _destinationDatabaseConnectionString;

    private readonly ILogger<CustomerOrderFlatDapperLoader> _logger;

    public CustomerOrderFlatDapperLoader(IOptions<DatabaseConnections> options, ILogger<CustomerOrderFlatDapperLoader> logger)
    {
        _destinationDatabaseConnectionString = options.Value.SqlServer;
        _logger = logger;
    }

    public async Task LoadAsync(List<CustomerOrderFlat> data, CancellationToken cancellationToken, IDbTransaction? transaction = null)
    {
        if (data.Count == 0)
        {
            _logger.LogInformation("No data to load");
            return;
        }

        const string sql = """
            INSERT INTO "CustomerOrders" 
                ("RentalId", "CustomerName", "Amount", "RentalDate", "Category", "UniqId", "IsDeleted") 
            VALUES 
                (@RentalId, @CustomerName, @Amount, @RentalDate, @Category, @UniqId, @IsDeleted)
            """;

        if (transaction is null)
        {
            await using var connection = new NpgsqlConnection(_destinationDatabaseConnectionString);
            await connection.ExecuteAsync(sql, data);
        }
        else
        {
            var connection = transaction.Connection ?? throw new ArgumentNullException(nameof(transaction), "Transaction must have a valid connection.");
            await connection.ExecuteAsync(sql, data, transaction);
        }
    }
}