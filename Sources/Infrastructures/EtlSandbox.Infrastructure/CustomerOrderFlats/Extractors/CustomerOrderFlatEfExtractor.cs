using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Infrastructure.DbContexts;
using EtlSandbox.Infrastructure.Shared.Extractors;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;

public sealed class CustomerOrderFlatEfExtractor(ApplicationDbContext applicationDbContext)
    : BaseEfExtractor<CustomerOrderFlat>(applicationDbContext);