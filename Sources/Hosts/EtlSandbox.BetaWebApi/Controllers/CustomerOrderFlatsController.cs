using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Repositories;

using Microsoft.AspNetCore.Mvc;

namespace EtlSandbox.BetaWebApiService.Controllers;

[Route("api/CustomerOrderFlats")]
public sealed class CustomerOrderFlatsController : ControllerBase
{
    private readonly IExtractor<CustomerOrderFlat> _extractor;

    private readonly ISourceRepository<CustomerOrderFlat> _sourceRepository;

    public CustomerOrderFlatsController(IExtractor<CustomerOrderFlat> extractor, ISourceRepository<CustomerOrderFlat> sourceRepository)
    {
        _extractor = extractor;
        _sourceRepository = sourceRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<CustomerOrderFlat>>> GetAsync(long from, long to, CancellationToken cancellationToken = default)
    {
        var items = await _extractor.ExtractAsync(from, to, cancellationToken);
        return Ok(items);
    }

    [HttpGet("GetLastItemId")]
    public async Task<ActionResult<long>> GetLastItemIdAsync(CancellationToken cancellationToken = default)
    {
        var item = await _sourceRepository.GetLastItemIdAsync(cancellationToken);
        return Ok(item);
    }
}