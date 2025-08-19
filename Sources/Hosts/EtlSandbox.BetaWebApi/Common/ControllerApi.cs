using Microsoft.AspNetCore.Mvc;

namespace EtlSandbox.BetaWebApiService.Common;

[ApiController]
[Route("api/[controller]")]
public abstract class ControllerApi : ControllerBase
{
}