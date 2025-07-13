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
        input.CustomerName = input.CustomerName?.ToUpperInvariant();
        input.Category = input.Category?.ToLowerInvariant();

        // _logger.LogDebug("Transformed RentalId {RentalId}: {Original} => {Transformed}", input.RentalId, input, transformed);
        return input;
    }
}