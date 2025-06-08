using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Persistence;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories;

public sealed class CustomerOrderFlatEfCommandRepository : ICommandRepository<CustomerOrderFlat>
{
    private readonly ApplicationDbContext _context;

    public CustomerOrderFlatEfCommandRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetLastIdAsync()
    {
        return await _context
            .CustomerOrders
            .MaxAsync(i => (int?)i.RentalId) ?? 0;
    }
}