using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Infrastructure.Common.Loaders;

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