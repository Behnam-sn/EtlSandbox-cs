using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Presentation.Common.Workers;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Presentation.CustomerOrderFlats.Workers;

public sealed class CustomerOrderFlatsSoftDeleteWorker(ILogger<CustomerOrderFlatsSoftDeleteWorker> logger, IServiceProvider serviceProvider)
    : BaseSoftDeleteWorker<CustomerOrderFlatsSoftDeleteWorker, CustomerOrderFlat>(logger, serviceProvider);