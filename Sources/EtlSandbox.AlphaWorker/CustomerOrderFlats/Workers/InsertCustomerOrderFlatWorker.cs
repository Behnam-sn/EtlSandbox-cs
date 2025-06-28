using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Worker.Shared.Workers;

namespace EtlSandbox.Worker.CustomerOrderFlats.Workers;

public sealed class InsertCustomerOrderFlatWorker(ILogger<InsertCustomerOrderFlatWorker> logger, IServiceProvider serviceProvider) 
    : InsertBaseWorker<CustomerOrderFlat>(logger, serviceProvider);