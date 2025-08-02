using EtlSandbox.Domain.Shared;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.Shared.Repositories;

public sealed class EfDestinationRepositoryV1<T>(DbContext dbContext) : BaseEfDestinationRepository<T>(dbContext)
    where T : class, IEntity
{
    public override async Task<long> GetLastInsertedImportantIdAsync()
    {
        var lastItem = await _dbSet
            .AsNoTracking()
            .OrderByDescending(item => item.Id)
            .FirstOrDefaultAsync();
        return lastItem?.ImportantId ?? 0;
    }
}