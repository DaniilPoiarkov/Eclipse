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
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _suggestionsService.GetWithUserInfo(cancellationToken));
    }
}
