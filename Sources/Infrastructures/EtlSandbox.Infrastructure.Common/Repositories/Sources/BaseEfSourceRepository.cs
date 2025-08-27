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

    public async Task<long> GetMaxIdOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .MaxAsync(entity => (long?)entity.Id, cancellationToken) ?? 0;
    }
}