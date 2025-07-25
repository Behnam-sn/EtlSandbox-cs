using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.Shared.Repositories;

public abstract class BaseEfRepository<T> : IRepository<T>
    where T : class, IEntity
{
    protected readonly DbSet<T> _dbSet;

    protected BaseEfRepository(ApplicationDbContext dbContext)
    {
        _dbSet = dbContext.Set<T>();
    }

    public abstract Task<long> GetLastProcessedImportantIdAsync();

    public async Task<long> GetLastSoftDeletedItemIdAsync()
    {
        var lastItem = await _dbSet
            .AsNoTracking()
            .Where(item => item.IsDeleted == true)
            .OrderByDescending(item => item.Id)
            .FirstOrDefaultAsync();
        return lastItem?.Id ?? 0;
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