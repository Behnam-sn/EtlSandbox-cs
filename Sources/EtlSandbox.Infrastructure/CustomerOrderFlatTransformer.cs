using EtlSandbox.Domain;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Infrastructure;

public class CustomerOrderFlatTransformer : ITransformer<CustomerOrderFlat>
{
    private readonly ILogger<CustomerOrderFlatTransformer> _logger;

    public CustomerOrderFlatTransformer(ILogger<CustomerOrderFlatTransformer> logger)
    {
        _logger = logger;
    }

    public CustomerOrderFlat Transform(CustomerOrderFlat input)
    {
        var transformed = new CustomerOrderFlat
        {
            RentalId = input.RentalId,
            CustomerName = input.CustomerName.ToUpperInvariant(),
            Amount = input.Amount,
            RentalDate = input.RentalDate,
            Category = input.Category.ToLowerInvariant()
        };

        _logger.LogDebug("Transformed RentalId {RentalId}: {Original} => {Transformed}", input.RentalId, input, transformed);
        return transformed;
    }
}