using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;

using Microsoft.AspNetCore.Mvc;

namespace EtlSandbox.BetaWebApiService.Controllers;

[Route("api/CustomerOrderFlats")]
public sealed class CustomerOrderFlatsController : ControllerBase
{
    private readonly IExtractor<CustomerOrderFlat> _extractor;

    public CustomerOrderFlatsController(IExtractor<CustomerOrderFlat> extractor)
    {
        _extractor = extractor;
    }

    [HttpGet]
    public async Task<ActionResult<List<CustomerOrderFlat>>> GetAsync(long lastProcessedId, int batchSize, CancellationToken cancellationToken)
    {
        var items = await _extractor.ExtractAsync(lastProcessedId, batchSize, cancellationToken);
        return Ok(items);
    }
}