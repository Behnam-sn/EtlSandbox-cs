using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using EtlSandbox.Domain.CustomerOrderFlats;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;

public interface ICustomerOrderFlatApiClient
{
    Task<IReadOnlyList<CustomerOrderFlat>> GetCustomerOrderFlatsAsync(int lastProcessedId, int batchSize, CancellationToken cancellationToken);
}
