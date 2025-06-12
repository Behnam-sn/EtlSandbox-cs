using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories;

public sealed class CustomerOrderFlatEfCommandRepository : ICommandRepository<CustomerOrderFlat>
{
    private readonly ApplicationDbContext _context;

    public CustomerOrderFlatEfCommandRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<int>> GetIdsAsync(int from, int to)
    {
        return await _context
            .CustomerOrders
            .Where(item => item.RentalId > from && item.RentalId <= to)
            .Select(item => item.RentalId)
            .ToListAsync();
    }
}