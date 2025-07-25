using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.Shared.Loaders;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;

public sealed class CustomerOrderFlatPostgreSqlDapperLoader(IDbConnectionFactory dbConnectionFactory)
    : BaseDapperLoader<CustomerOrderFlat>(dbConnectionFactory)
{
    protected override string Sql => """
                                     INSERT INTO "CustomerOrderFlats" 
                                         ("Id", "RentalId", "CustomerName", "Amount", "RentalDate", "Category", "IsDeleted") 
                                     VALUES 
                                         (@Id, @RentalId, @CustomerName, @Amount, @RentalDate, @Category, @IsDeleted)
                                     """;
}