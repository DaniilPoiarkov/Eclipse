using Eclipse.Pipelines.Options.Languages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Eclipse.WebAPI.Configurations;

[ApiController]
[Route("api/configuration")]
public class ConfigurationController : ControllerBase
{
    private readonly IOptions<LanguageList> _languageListOptions;

    public ConfigurationController(IOptions<LanguageList> languageListOptions)
    {
        _languageListOptions = languageListOptions;
    }

    [HttpGet("cultures")]
    public IActionResult GetCultures()
    {
        return Ok(_languageListOptions.Value);
    }
}
