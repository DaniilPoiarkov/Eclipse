using Eclipse.Pipelines.Options.Languages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[Route("api/configuration")]
public class ConfigurationController : ControllerBase
{
    private readonly IOptions<LanguageList> _languageListOptions;

    private readonly IStringLocalizer<ConfigurationController> _stringLocalizer;

    public ConfigurationController(IOptions<LanguageList> languageListOptions, IStringLocalizer<ConfigurationController> stringLocalizer)
    {
        _languageListOptions = languageListOptions;
        _stringLocalizer = stringLocalizer;
    }

    [HttpGet("cultures")]
    public IActionResult GetCultures()
    {
        return Ok(_languageListOptions.Value
            .Select(i => new LanguageInfo
            {
                Code = i.Code,
                Language = _stringLocalizer[i.Language]
            }));
    }
}
