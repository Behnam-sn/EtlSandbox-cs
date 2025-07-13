using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Transformers;

public sealed class CustomerOrderFlatTransformer : ITransformer<CustomerOrderFlat>
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
            Id = input.Id,
            RentalId = input.RentalId,
            CustomerName = input.CustomerName.ToUpperInvariant(),
            Amount = input.Amount,
            RentalDate = input.RentalDate,
            Category = input.Category.ToLowerInvariant(),
            UniqId = 0,
            IsDeleted = false,
        };

        _logger.LogDebug("Transformed RentalId {RentalId}: {Original} => {Transformed}", input.RentalId, input, transformed);
        return transformed;
    }
}