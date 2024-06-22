using Asp.Versioning;

using Eclipse.WebAPI.Constants;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers.Ping.V2;

[ApiController]
[Route("api/ping")]
[ApiVersion(ApiVersions.V2.Version, Deprecated = ApiVersions.V2.Deprecated)]
public sealed class PingController : ControllerBase
{
    [HttpGet]
    public IActionResult Ping() => Ok("Pong");
}
