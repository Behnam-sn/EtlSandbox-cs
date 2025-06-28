using EtlSandbox.AlphaWorker.Shared.Workers;
using EtlSandbox.Domain.CustomerOrderFlats;

namespace EtlSandbox.AlphaWorker.CustomerOrderFlats.Workers;

public sealed class SoftDeleteCustomerOrderFlatWorker(ILogger<SoftDeleteCustomerOrderFlatWorker> logger, IServiceProvider serviceProvider)
    : SoftDeleteBaseWorker<CustomerOrderFlat>(logger, serviceProvider);