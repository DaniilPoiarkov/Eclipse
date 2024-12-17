using Asp.Versioning;

using Eclipse.IntegrationEvents.Users;
using Eclipse.WebAPI.Constants;

using MassTransit;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers.Ping.V1;

[ApiController]
[Route("api/v{version:apiVersion}/ping")]
[ApiVersion(ApiVersions.V1.Version, Deprecated = ApiVersions.V1.Deprecated)]
public sealed class PingController : ControllerBase
{
    private readonly IPublishEndpoint _endpoint;

    public PingController(IPublishEndpoint endpoint)
    {
        _endpoint = endpoint;
    }

    [HttpGet]
    public IActionResult Ping() => Ok("Ping");

    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        await _endpoint.Publish(
            new NewUserRegisteredIntegrationEvent(Guid.NewGuid(), "test", "test", "test", 0)
        );

        return NoContent();
    }
}
