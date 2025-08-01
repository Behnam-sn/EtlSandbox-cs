﻿using EtlSandbox.Domain.Rentals;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Repositories;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.Rentals;

public sealed class RentalEfRepository : ISourceRepository<Rental>
{
    private readonly DbSet<Rental> _dbSet;

    public RentalEfRepository(DbContext dbContext)
    {
        _dbSet = dbContext.Set<Rental>();
    }

    public async Task<long> GetLastItemIdAsync()
    {
        var lastItem = await _dbSet
            .AsNoTracking()
            .OrderByDescending(item => item.rental_id)
            .FirstOrDefaultAsync();
        return lastItem?.rental_id ?? 0;
    }
}