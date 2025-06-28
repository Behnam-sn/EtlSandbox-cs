using EtlSandbox.AlphaWorker.Shared.Workers;
using EtlSandbox.Domain.CustomerOrderFlats;

namespace EtlSandbox.AlphaWorker.CustomerOrderFlats.Workers;

public sealed class InsertCustomerOrderFlatWorker(ILogger<InsertCustomerOrderFlatWorker> logger, IServiceProvider serviceProvider) 
    : InsertBaseWorker<CustomerOrderFlat>(logger, serviceProvider);