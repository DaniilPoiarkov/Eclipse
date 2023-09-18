using Eclipse.Application.Contracts.Suggestions;
using Eclipse.WebAPI.Filters;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiKeyAuthorize]
public class SuggestionsController : ControllerBase
{
    private readonly ISuggestionsService _suggestionsService;

    public SuggestionsController(ISuggestionsService suggestionsService)
    {
        _suggestionsService = suggestionsService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_suggestionsService.GetWithUserInfo());
    }
}
