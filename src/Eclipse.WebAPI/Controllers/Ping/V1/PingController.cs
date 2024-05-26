using Asp.Versioning;

using Eclipse.WebAPI.Constants;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers.Ping.V1;

[ApiController]
[Route("api/[controller]")]
[ApiVersion(ApiVersions.V1.Version, Deprecated = ApiVersions.V1.Deprecated)]
public sealed class PingController : ControllerBase
{
    [HttpGet]
    public IActionResult Ping() => Ok("Ping");
}
