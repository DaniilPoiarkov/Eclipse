using Eclipse.Application.Contracts.Telegram;
using Eclipse.WebAPI.Filters.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiKeyAuthorize]
public sealed class TelegramController : ControllerBase
{
    private readonly ITelegramService _service;

    public TelegramController(ITelegramService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] SendMessageModel message, CancellationToken cancellationToken)
    {
        var result = await _service.Send(message, cancellationToken);
        return result.ToActionResult(NoContent);
    }
}
