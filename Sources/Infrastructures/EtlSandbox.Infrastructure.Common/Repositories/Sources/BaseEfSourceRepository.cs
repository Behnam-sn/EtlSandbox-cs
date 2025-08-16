using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Repositories;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.Common.Repositories.Sources;

public abstract class BaseEfSourceRepository<T> : ISourceRepository<T>
    where T : class, IEntity
{
    private readonly DbSet<T> _dbSet;

    protected BaseEfSourceRepository(DbContext dbContext)
    {
        _dbSet = dbContext.Set<T>();
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