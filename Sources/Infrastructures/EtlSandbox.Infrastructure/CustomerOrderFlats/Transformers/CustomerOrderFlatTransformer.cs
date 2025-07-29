using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Transformers;

public sealed class CustomerOrderFlatTransformer : ITransformer<CustomerOrderFlat>
{
    public CustomerOrderFlat Transform(CustomerOrderFlat input)
    {
        input.CustomerName = input.CustomerName?.ToUpperInvariant();
        input.Category = input.Category?.ToLowerInvariant();

        return input;
    }
}