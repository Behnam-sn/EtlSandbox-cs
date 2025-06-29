using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.Shared.Loaders;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;

public sealed class CustomerOrderFlatPostgreSqlDapperLoader(ILogger<CustomerOrderFlatPostgreSqlDapperLoader> logger, IUnitOfWork unitOfWork)
    : BaseDapperLoader<CustomerOrderFlat>(logger, unitOfWork, Sql)
{
    private const string Sql = """
                               INSERT INTO "CustomerOrders" 
                                   ("RentalId", "CustomerName", "Amount", "RentalDate", "Category", "UniqId", "IsDeleted") 
                               VALUES 
                                   (@RentalId, @CustomerName, @Amount, @RentalDate, @Category, @UniqId, @IsDeleted)
                               """;
}