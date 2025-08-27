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

    public abstract Task<long> GetMaxSourceIdOrDefaultAsync(CancellationToken cancellationToken = default);

    public async Task<long> GetMaxIdOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .MaxAsync(entity => (long?)entity.Id, cancellationToken) ?? 0;
    }
}