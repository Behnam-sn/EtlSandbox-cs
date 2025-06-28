using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Presentation.Shared.Workers;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Presentation.CustomerOrderFlats.Workers;

public sealed class SoftDeleteCustomerOrderFlatWorker(ILogger<SoftDeleteCustomerOrderFlatWorker> logger, IServiceProvider serviceProvider)
    : SoftDeleteBaseWorker<CustomerOrderFlat>(logger, serviceProvider);