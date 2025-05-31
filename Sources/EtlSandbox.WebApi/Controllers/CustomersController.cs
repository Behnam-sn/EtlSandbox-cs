using EtlSandbox.Domain;
using EtlSandbox.Infrastructure;

using Microsoft.AspNetCore.Mvc;

namespace EtlSandbox.WebApi.Controllers;

[Route("api/customers")]
public sealed class CustomersController : ControllerBase
{
    private readonly CustomerOrderFlatService _customerOrderFlatService;

    public CustomersController(CustomerOrderFlatService customerOrderFlatService)
    {
        _customerOrderFlatService = customerOrderFlatService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CustomerOrderFlat>>> GetAsync(DateTime since, CancellationToken cancellationToken)
    {
        var items = await _customerOrderFlatService.GetCustomerOrderFlatsAsync(since);
        return Ok(items);
    }
}
