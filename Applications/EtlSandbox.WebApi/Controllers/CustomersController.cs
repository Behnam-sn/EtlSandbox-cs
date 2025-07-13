using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;

using Microsoft.AspNetCore.Mvc;

namespace EtlSandbox.WebApi.Controllers;

[Route("api/customers")]
public sealed class CustomersController : ControllerBase
{
    private readonly ILogger<CustomersController> _logger;

    private readonly IExtractor<CustomerOrderFlat> _extractor;

    public CustomersController(ILogger<CustomersController> logger, IExtractor<CustomerOrderFlat> extractor)
    {
        _logger = logger;
        _extractor = extractor;
    }

    [HttpGet]
    public async Task<ActionResult<List<CustomerOrderFlat>>> GetAsync(int lastProcessedId, int batchSize, CancellationToken cancellationToken)
    {
        var items = await _extractor.ExtractAsync(lastProcessedId, batchSize, cancellationToken);
        return Ok(items);
    }
}