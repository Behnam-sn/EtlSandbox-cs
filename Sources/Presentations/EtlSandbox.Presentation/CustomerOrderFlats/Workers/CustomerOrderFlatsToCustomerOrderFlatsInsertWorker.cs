using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Presentation.Common.Workers;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Presentation.CustomerOrderFlats.Workers;

public sealed class CustomerOrderFlatsToCustomerOrderFlatsInsertWorker(ILogger<CustomerOrderFlatsToCustomerOrderFlatsInsertWorker> logger, IServiceProvider serviceProvider)
    : BaseInsertWorker<CustomerOrderFlatsToCustomerOrderFlatsInsertWorker, CustomerOrderFlat, CustomerOrderFlat>(logger, serviceProvider);