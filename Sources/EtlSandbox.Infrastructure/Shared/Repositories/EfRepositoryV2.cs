using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.Shared.Repositories;

public sealed class EfRepositoryV2<T>(ApplicationDbContext dbContext) : BaseEfRepository<T>(dbContext)
    where T : class, IEntity
{
    public override async Task<long> GetLastInsertedImportantIdAsync()
    {
        var lastItem = await _dbSet
            .AsNoTracking()
            .OrderByDescending(item => item.Id)
            .FirstOrDefaultAsync();
        return lastItem?.Id ?? 0;
    }
}