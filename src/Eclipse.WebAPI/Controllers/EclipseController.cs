using Eclipse.Pipelines.UpdateHandler;
using Eclipse.WebAPI.Filters.Authorization;

using Microsoft.AspNetCore.Mvc;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[TelegramBotApiSecretTokenAuthorize]
public sealed class EclipseController : ControllerBase
{
    private readonly ITelegramBotClient _botClient;

    private readonly Dictionary<HandlerType, IEclipseUpdateHandler> _updateHandlers;

    public EclipseController(ITelegramBotClient botClient, IEnumerable<IEclipseUpdateHandler> updateHandlers)
    {
        _botClient = botClient;
        _updateHandlers = updateHandlers.ToDictionary(h => h.Type);
    }

    [HttpPost("_handle")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> HandleUpdate([FromBody] Update update, CancellationToken cancellationToken)
    {
        await _updateHandlers[HandlerType.Active].HandleUpdateAsync(_botClient, update, cancellationToken);
        return NoContent();
    }

    [HttpPost("_disabled")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> HandleDisabledUpdate([FromBody] Update update, CancellationToken cancellationToken)
    {
        await _updateHandlers[HandlerType.Disabled].HandleUpdateAsync(_botClient, update, cancellationToken);
        return NoContent();
    }
}
