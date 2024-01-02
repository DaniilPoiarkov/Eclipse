using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.WebAPI.Filters;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[EclipseApiKeyAuthorize]
public class TelegramController : ControllerBase
{
    private readonly ITelegramService _telegramService;

    private readonly IIdentityUserService _userService;

    public TelegramController(ITelegramService telegramService, IIdentityUserService userService)
    {
        _telegramService = telegramService;
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] SendMessageModel message, CancellationToken cancellationToken)
    {
        await _telegramService.Send(message, cancellationToken);
        return NoContent();
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
    {
        return Ok(await _userService.GetAllAsync(cancellationToken));
    }
}
