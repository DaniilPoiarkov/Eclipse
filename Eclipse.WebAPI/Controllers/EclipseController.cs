using Eclipse.WebAPI.Filters.Authorization;

using Microsoft.AspNetCore.Mvc;

using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Eclipse.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[TelegramBotApiSecretTokenAuthorize]
public class EclipseController : ControllerBase
{
    private readonly IUpdateHandler _updateHandler;

    private readonly ITelegramBotClient _botClient;

    public EclipseController(IUpdateHandler updateHandler, ITelegramBotClient botClient)
    {
        _updateHandler = updateHandler;
        _botClient = botClient;
    }

    [HttpPost("_handle")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> HandleUpdate([FromBody] Update update, CancellationToken cancellationToken)
    {
        await _updateHandler.HandleUpdateAsync(_botClient, update, cancellationToken);
        return NoContent();
    }
}
