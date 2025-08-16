using EtlSandbox.Domain.Common.Repositories;
using EtlSandbox.Domain.CustomerOrderFlats.Entities;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories.Sources;

public sealed class CustomerOrderFlatEfSourceRepository : ISourceRepository<CustomerOrderFlat>
{
    private readonly DbSet<CustomerOrderFlat> _dbSet;

    public CustomerOrderFlatEfSourceRepository(DbContext dbContext)
    {
        _dbSet = dbContext.Set<CustomerOrderFlat>();
    }

    public async Task<long> GetLastItemIdAsync(CancellationToken cancellationToken = default)
    {
        // Todo: change this into max
        var lastItemId = await _dbSet
            .AsNoTracking()
            .OrderByDescending(item => item.Id)
            .Select(item => (long?)item.Id)
            .FirstOrDefaultAsync(cancellationToken);
        return lastItemId ?? 0;
    }
}