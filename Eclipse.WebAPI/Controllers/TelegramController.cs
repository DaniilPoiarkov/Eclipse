using Eclipse.Application.Contracts.Telegram.Stores;
using Eclipse.Infrastructure.Telegram;
using Eclipse.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiKeyAuthorize]
public class TelegramController : ControllerBase
{
    private readonly ITelegramService _telegramService;

    private readonly IUserInfoStore _userStore;

    public TelegramController(ITelegramService telegramService, IUserInfoStore userStore)
    {
        _telegramService = telegramService;
        _userStore = userStore;
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
        return Ok(_userStore.GetUsers());
    }
}
