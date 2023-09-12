using Eclipse.Application.Contracts.Google.Sheets;
using Eclipse.WebAPI.Filters;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiKeyAuthorize]
public class SuggestionsController : ControllerBase
{
    private readonly IEclipseSheetsService _sheetsService;

    public SuggestionsController(IEclipseSheetsService sheetsService)
    {
        _sheetsService = sheetsService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_sheetsService.GetSuggestions());
    }
}
