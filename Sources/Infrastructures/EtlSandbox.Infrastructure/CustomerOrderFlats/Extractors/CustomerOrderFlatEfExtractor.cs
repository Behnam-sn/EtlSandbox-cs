using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Infrastructure.Shared.Extractors;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;

public sealed class CustomerOrderFlatEfExtractor(DbContext dbContext)
    : BaseEfExtractor<CustomerOrderFlat>(dbContext);