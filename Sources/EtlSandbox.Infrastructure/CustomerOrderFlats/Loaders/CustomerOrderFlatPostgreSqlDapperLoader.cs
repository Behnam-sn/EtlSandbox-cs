using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.Shared.Loaders;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;

public sealed class CustomerOrderFlatPostgreSqlDapperLoader(IUnitOfWork unitOfWork)
    : BaseDapperLoader<CustomerOrderFlat>(unitOfWork)
{
    protected override string Sql => """
                                     INSERT INTO "CustomerOrderFlats" 
                                         ("Id", "RentalId", "CustomerName", "Amount", "RentalDate", "Category", "IsDeleted") 
                                     VALUES 
                                         (@Id, @RentalId, @CustomerName, @Amount, @RentalDate, @Category, @IsDeleted)
                                     """;
}