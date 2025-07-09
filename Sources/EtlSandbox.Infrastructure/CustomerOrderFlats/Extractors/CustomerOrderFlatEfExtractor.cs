using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;

public sealed class CustomerOrderFlatEfExtractor : IExtractor<CustomerOrderFlat>
{
    private readonly ApplicationDbContext _applicationDbContext;

    public CustomerOrderFlatEfExtractor(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<List<CustomerOrderFlat>> ExtractAsync(int lastProcessedId, int batchSize, CancellationToken cancellationToken)
    {
        return await _applicationDbContext
            .CustomerOrders
            .AsNoTracking()
            .Where(i => i.RentalId > lastProcessedId)
            .OrderBy(i => i.RentalId)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }
}