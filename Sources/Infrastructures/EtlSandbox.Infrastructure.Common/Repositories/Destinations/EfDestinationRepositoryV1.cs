using EtlSandbox.Domain.Common;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.Common.Repositories.Destinations;

public sealed class EfDestinationRepositoryV1<T>(DbContext dbContext) : BaseEfDestinationRepository<T>(dbContext)
    where T : class, IEntity
{
    public override async Task<long> GetMaxSourceIdOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var lastItem = await _dbSet
            .AsNoTracking()
            .OrderByDescending(item => item.Id)
            .FirstOrDefaultAsync(cancellationToken);
        return lastItem?.SourceId ?? 0;
    }

    public override Task SoftDeleteObsoleteRowsAsync(long from, long to, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}