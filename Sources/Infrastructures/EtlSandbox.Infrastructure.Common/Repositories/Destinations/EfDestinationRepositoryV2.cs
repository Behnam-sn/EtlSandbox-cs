using EtlSandbox.Domain.Common;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.Common.Repositories.Destinations;

public sealed class EfDestinationRepositoryV2<T>(DbContext dbContext) : BaseEfDestinationRepository<T>(dbContext)
    where T : class, IEntity
{
    public override async Task<long> GetLastInsertedSourceIdAsync()
    {
        var lastItem = await _dbSet
            .AsNoTracking()
            .OrderByDescending(item => item.Id)
            .FirstOrDefaultAsync();
        return lastItem?.Id ?? 0;
    }
}