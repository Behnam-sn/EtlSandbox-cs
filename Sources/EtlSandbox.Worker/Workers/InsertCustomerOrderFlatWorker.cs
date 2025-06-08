using EtlSandbox.Domain;

namespace EtlSandbox.Worker.Workers;

public sealed class InsertCustomerOrderFlatWorker(ILogger<InsertCustomerOrderFlatWorker> logger, IServiceProvider serviceProvider) 
    : InsertBaseWorker<CustomerOrderFlat>(logger, serviceProvider);