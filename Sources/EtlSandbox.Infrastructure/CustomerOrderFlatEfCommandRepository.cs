using EtlSandbox.Domain;
using EtlSandbox.Persistence;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure;

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