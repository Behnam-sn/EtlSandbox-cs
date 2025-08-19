using EtlSandbox.BetaWebApiService.Common;
using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Repositories;
using EtlSandbox.Domain.CustomerOrderFlats.Entities;

using Microsoft.AspNetCore.Mvc;

namespace EtlSandbox.BetaWebApiService.Controllers;

public sealed class CustomerOrderFlatsController : ControllerApi
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

    [HttpGet("GetLastId")]
    public async Task<ActionResult<long>> GetLastIdAsync(CancellationToken cancellationToken = default)
    {
        var item = await _sourceRepository.GetLastIdAsync(cancellationToken);
        return Ok(item);
    }
}