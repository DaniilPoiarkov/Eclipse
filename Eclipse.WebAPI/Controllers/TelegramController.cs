using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Contracts.Telegram.TelegramUsers;
using Eclipse.WebAPI.Filters;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiKeyAuthorize]
public class TelegramController : ControllerBase
{
    private readonly ITelegramService _telegramService;

    private readonly ITelegramUserRepository _userRepositoy;

    public TelegramController(ITelegramService telegramService, ITelegramUserRepository userRepositoy)
    {
        _telegramService = telegramService;
        _userRepositoy = userRepositoy;
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] SendMessageModel message, CancellationToken cancellationToken)
    {
        await _telegramService.Send(message, cancellationToken);
        return NoContent();
    }

    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        return Ok(_userRepositoy.GetAll());
    }
}
