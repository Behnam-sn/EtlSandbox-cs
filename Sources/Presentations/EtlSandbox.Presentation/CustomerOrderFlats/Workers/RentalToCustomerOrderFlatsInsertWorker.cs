using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Rentals;
using EtlSandbox.Presentation.Common.Workers;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Presentation.CustomerOrderFlats.Workers;

public sealed class RentalToCustomerOrderFlatsInsertWorker(ILogger<RentalToCustomerOrderFlatsInsertWorker> logger, IServiceProvider serviceProvider)
    : BaseInsertWorker<RentalToCustomerOrderFlatsInsertWorker, Rental, CustomerOrderFlat>(logger, serviceProvider);