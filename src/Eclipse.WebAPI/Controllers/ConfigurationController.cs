using Eclipse.Application.Contracts.Configuration;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[Route("api/configuration")]
public class ConfigurationController : ControllerBase
{
    private readonly IConfigurationService _configurationService;

    public ConfigurationController(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    [HttpGet("cultures")]
    public IActionResult GetCultures()
    {
        return Ok(_configurationService.GetCultures());
    }
}
