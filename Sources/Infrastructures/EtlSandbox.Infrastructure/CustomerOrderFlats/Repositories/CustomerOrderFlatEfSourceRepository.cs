using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared.Repositories;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories;

public sealed class CustomerOrderFlatEfSourceRepository : ISourceRepository<CustomerOrderFlat>
{
    private readonly DbSet<CustomerOrderFlat> _dbSet;

    public CustomerOrderFlatEfSourceRepository(DbContext dbContext)
    {
        _dbSet = dbContext.Set<CustomerOrderFlat>();
    }

    public async Task<long> GetLastItemIdAsync()
    {
        var lastItem = await _dbSet
            .AsNoTracking()
            .OrderByDescending(item => item.Id)
            .FirstOrDefaultAsync();
        return lastItem?.Id ?? 0;
    }
}