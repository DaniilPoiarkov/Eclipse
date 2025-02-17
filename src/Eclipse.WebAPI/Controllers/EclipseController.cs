using Eclipse.Pipelines.UpdateHandler;
using Eclipse.WebAPI.Filters;

using Microsoft.AspNetCore.Mvc;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[Route("api/eclipse")]
[TelegramBotApiSecretTokenAuthorize]
public sealed class EclipseController : ControllerBase
{
    private readonly ITelegramBotClient _botClient;

    private readonly Dictionary<HandlerType, IEclipseUpdateHandler> _updateHandlers;

    private readonly ILogger<EclipseController> _logger;

    public EclipseController(ITelegramBotClient botClient, IEnumerable<IEclipseUpdateHandler> updateHandlers, ILogger<EclipseController> logger)
    {
        _botClient = botClient;
        _updateHandlers = updateHandlers.ToDictionary(h => h.Type);
        _logger = logger;
    }

    [HttpPost("_handle")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public Task<IActionResult> HandleActiveAsync([FromBody] Update update, CancellationToken cancellationToken)
    {
        return HandleAsync(HandlerType.Active, update, cancellationToken);
    }

    [HttpPost("_disabled")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public Task<IActionResult> HandleDisabledUpdate([FromBody] Update update, CancellationToken cancellationToken)
    {
        return HandleAsync(HandlerType.Disabled, update, cancellationToken);
    }

    private async Task<IActionResult> HandleAsync(HandlerType type, Update update, CancellationToken cancellationToken)
    {
        try
        {
            await _updateHandlers[type].HandleUpdateAsync(_botClient, update, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process update for {handler} handler.", type);
        }

        return NoContent();
    }
}
