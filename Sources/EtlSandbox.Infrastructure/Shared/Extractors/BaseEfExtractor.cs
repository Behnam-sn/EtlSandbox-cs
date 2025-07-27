using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.Shared.Extractors;

public abstract class BaseEfExtractor<T> : IExtractor<T>
    where T : class, IEntity
{
    private readonly DbSet<T> _dbSet;

    protected BaseEfExtractor(ApplicationDbContext applicationDbContext)
    {
        _dbSet = applicationDbContext.Set<T>();
    }

    public async Task<List<T>> ExtractAsync(long lastProcessedId, int batchSize, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(i => i.Id > lastProcessedId)
            .OrderBy(i => i.Id)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }
}