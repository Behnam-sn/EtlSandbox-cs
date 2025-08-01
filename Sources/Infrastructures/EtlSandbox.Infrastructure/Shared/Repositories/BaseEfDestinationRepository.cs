﻿using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Repositories;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.Shared.Repositories;

public abstract class BaseEfDestinationRepository<T> : IDestinationRepository<T>
    where T : class, IEntity
{
    protected readonly DbSet<T> _dbSet;

    protected BaseEfDestinationRepository(DbContext dbContext)
    {
        _dbSet = dbContext.Set<T>();
    }

    public abstract Task<long> GetLastInsertedImportantIdAsync();

    public async Task<long> GetLastSoftDeletedItemIdAsync()
    {
        var lastItem = await _dbSet
            .AsNoTracking()
            .Where(item => item.IsDeleted == true)
            .OrderByDescending(item => item.Id)
            .FirstOrDefaultAsync();
        return lastItem?.Id ?? 0;
    }

    public async Task<long> GetLastItemIdAsync()
    {
        var lastItem = await _dbSet
            .AsNoTracking()
            .OrderByDescending(item => item.Id)
            .FirstOrDefaultAsync();
        return lastItem?.Id ?? 0;
    }
}