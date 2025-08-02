using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Rentals;
using EtlSandbox.Presentation.Shared.Workers;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Presentation.CustomerOrderFlats.Workers;

public sealed class InsertCustomerOrderFlatWorker(ILogger<InsertCustomerOrderFlatWorker> logger, IServiceProvider serviceProvider)
    : InsertBaseWorker<Rental, CustomerOrderFlat>(logger, serviceProvider);