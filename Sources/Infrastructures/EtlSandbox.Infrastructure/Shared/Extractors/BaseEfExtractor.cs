using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.Shared.Extractors;

public abstract class BaseEfExtractor<T> : IExtractor<T>
    where T : class, IEntity
{
    private readonly DbSet<T> _dbSet;

    protected BaseEfExtractor(DbContext dbContext)
    {
        _dbSet = dbContext.Set<T>();
    }

    public async Task<List<T>> ExtractAsync(long from, long to, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(i => i.Id > from && i.Id <= to)
            .OrderBy(i => i.Id)
            .ToListAsync(cancellationToken);
    }
}