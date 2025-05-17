using EtlSandbox.Domain;

namespace EtlSandbox.Infrastructure;

public class CustomerOrderFlatTransformer : ITransformer<CustomerOrderFlat>
{
    public CustomerOrderFlat Transform(CustomerOrderFlat input)
    {
        input.CustomerName = input.CustomerName.ToUpperInvariant();
        input.Category = input.Category.ToLowerInvariant();
        return input;
    }
}