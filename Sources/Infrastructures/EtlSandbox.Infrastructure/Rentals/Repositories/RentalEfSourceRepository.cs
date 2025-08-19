using EtlSandbox.Domain.Common.Repositories;
using EtlSandbox.Domain.Rentals;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.Rentals.Repositories;

public sealed class RentalEfSourceRepository : ISourceRepository<Rental>
{
    private readonly DbSet<Rental> _dbSet;

    public RentalEfSourceRepository(DbContext dbContext)
    {
        _dbSet = dbContext.Set<Rental>();
    }

    public async Task<long> GetLastIdAsync(CancellationToken cancellationToken = default)
    {
        // Todo: change this into max
        var lastItem = await _dbSet
            .AsNoTracking()
            .OrderByDescending(item => item.rental_id)
            .FirstOrDefaultAsync(cancellationToken);
        return lastItem?.rental_id ?? 0;
    }
}