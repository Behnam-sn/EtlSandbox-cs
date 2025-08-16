using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Infrastructure.Common.Repositories;
using EtlSandbox.Infrastructure.Common.Repositories.Sources;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories.Sources;

public sealed class CustomerOrderFlatEfSourceRepository(DbContext dbContext) : BaseEfSourceRepository<CustomerOrderFlat>(dbContext);