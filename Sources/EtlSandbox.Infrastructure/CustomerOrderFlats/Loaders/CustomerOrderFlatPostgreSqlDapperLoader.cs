using System.Data;

using Dapper;

using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Npgsql;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;

public sealed class CustomerOrderFlatPostgreSqlDapperLoader : ILoader<CustomerOrderFlat>
{
    private readonly ILogger<CustomerOrderFlatPostgreSqlDapperLoader> _logger;

    private readonly IUnitOfWork _unitOfWork;

    public CustomerOrderFlatPostgreSqlDapperLoader(ILogger<CustomerOrderFlatPostgreSqlDapperLoader> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
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
            var connection = _unitOfWork.Connection;
            await connection.ExecuteAsync(sql, data);
        }
        else
        {
            var connection = transaction.Connection ?? throw new ArgumentNullException(nameof(transaction), "Transaction must have a valid connection.");
            await connection.ExecuteAsync(sql, data, transaction);
        }
    }
}