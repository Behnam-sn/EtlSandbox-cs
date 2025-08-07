using EtlSandbox.Domain.Common;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.Common.Extractors;

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
            .Where(i => from < i.Id && i.Id <= to)
            .OrderBy(i => i.Id)
            .ToListAsync(cancellationToken);
    }
}