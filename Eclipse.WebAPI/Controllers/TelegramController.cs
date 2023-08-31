using Eclipse.WebAPI.Services.TelegramServices;
using Eclipse.WebAPI.Services.UserStores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Eclipse.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TelegramController : ControllerBase
{
    private readonly ITelegramService _telegramService;

    private readonly IUserStore _userStore;

    private readonly IOptions<TelegramOptions> _options;

    public TelegramController(ITelegramService telegramService, IUserStore userStore, IOptions<TelegramOptions> options)
    {
        _telegramService = telegramService;
        _userStore = userStore;
        _options = options;
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] SendMessageModel message, CancellationToken cancellationToken)
    {
        await _telegramService.Send(message, cancellationToken);
        return NoContent();
    }

    [HttpGet("users")]
    public IActionResult GetUsers([FromQuery] string eclipsetoken)
    {
        if (!_options.Value.EclipseToken.Equals(eclipsetoken))
        {
            return Unauthorized();
        }

        return Ok(_userStore.GetUsers());
    }
}
