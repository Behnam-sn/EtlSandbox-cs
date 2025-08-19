using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Repositories;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.Common.Repositories.Destinations;

public abstract class BaseEfDestinationRepository<T> : IDestinationRepository<T>
    where T : class, IEntity
{
    protected readonly DbSet<T> _dbSet;

    protected BaseEfDestinationRepository(DbContext dbContext)
    {
        _dbSet = dbContext.Set<T>();
    }

    public abstract Task<long> GetLastSourceIdAsync();

    public async Task<long> GetLastSoftDeletedIdAsync()
    {
        var lastItem = await _dbSet
            .AsNoTracking()
            .Where(item => item.IsDeleted == true)
            .OrderByDescending(item => item.Id)
            .FirstOrDefaultAsync();
        return lastItem?.Id ?? 0;
    }

    public async Task<long> GetLastIdAsync()
    {
        var lastItem = await _dbSet
            .AsNoTracking()
            .OrderByDescending(item => item.Id)
            .FirstOrDefaultAsync();
        return lastItem?.Id ?? 0;
    }
}