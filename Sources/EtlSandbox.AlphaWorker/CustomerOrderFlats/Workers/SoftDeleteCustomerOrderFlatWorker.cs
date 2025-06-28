using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Worker.Shared.Workers;

namespace EtlSandbox.Worker.CustomerOrderFlats.Workers;

public sealed class SoftDeleteCustomerOrderFlatWorker(ILogger<SoftDeleteCustomerOrderFlatWorker> logger, IServiceProvider serviceProvider)
    : SoftDeleteBaseWorker<CustomerOrderFlat>(logger, serviceProvider);