using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Infrastructure.DbContexts;
using EtlSandbox.Infrastructure.Shared.Repositories;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories;

public sealed class CustomerOrderFlatEfRepository(ApplicationDbContext dbContext)
    : BaseEfRepository<CustomerOrderFlat>(dbContext);