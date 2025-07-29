using EtlSandbox.Application.ClickHouseUtils;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace EtlSandbox.DeltaWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class ClickHouseUtilsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClickHouseUtilsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet(Name = "GetCreateTableQuery")]
    public async Task<ActionResult<string>> GetCreateTableQuery(string tableName, CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new GetCreateTableQuery(
                TableName: tableName
            );
            var response =  await _mediator.Send(command, cancellationToken);
            
            return Ok(response);
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }
}