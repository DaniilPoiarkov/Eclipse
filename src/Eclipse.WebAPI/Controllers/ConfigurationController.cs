using Eclipse.Application.Contracts.Configuration;
using Eclipse.WebAPI.Constants;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[Route("api/configuration")]
public sealed class ConfigurationController : ControllerBase
{
    private readonly IConfigurationService _configurationService;

    public ConfigurationController(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    [OutputCache]
    [HttpGet("cultures")]
    [EnableRateLimiting(RateLimiterPolicies.IpAddressFiveMinutes)]
    public IActionResult GetCultures()
    {
        return Ok(_configurationService.GetCultures());
    }
}
