using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Presentation.Shared.Workers;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Presentation.CustomerOrderFlats.Workers;

public sealed class InsertCustomerOrderFlatWorker(ILogger<InsertCustomerOrderFlatWorker> logger, IServiceProvider serviceProvider) 
    : InsertBaseWorker<CustomerOrderFlat>(logger, serviceProvider);