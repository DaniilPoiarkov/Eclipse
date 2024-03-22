using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class PingController : ControllerBase
{
    [HttpGet]
    public IActionResult Ping() => Ok("Ping");
}
