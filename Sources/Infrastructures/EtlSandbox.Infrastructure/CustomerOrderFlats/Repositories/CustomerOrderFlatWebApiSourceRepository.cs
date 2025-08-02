using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared.Repositories;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories;

public sealed class CustomerOrderFlatWebApiSourceRepository : ISourceRepository<CustomerOrderFlat>
{
    public Task<long> GetLastItemIdAsync()
    {
        throw new NotImplementedException();
    }
}