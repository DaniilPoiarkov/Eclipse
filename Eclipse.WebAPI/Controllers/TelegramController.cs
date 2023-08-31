using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TelegramController : ControllerBase
{
    private readonly ITelegramBotClient _botClient;

    private readonly ILogger _logger;

    public TelegramController(ITelegramBotClient botClient, ILogger logger)
    {
        _botClient = botClient;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Handle([FromBody] Update update, CancellationToken cancellationToken)
    {
        if (update.Type != Telegram.Bot.Types.Enums.UpdateType.Message)
        {
            _logger.LogInformation("Update is not type of message");
            return NoContent();
        }

        _logger.LogInformation("Recieved message from {chatId} (chatId)", update.Message!.Chat.Id);

        await _botClient.SendTextMessageAsync(update.Message!.Chat.Id, "Hello! I'm Eclipse. Right now I'm having a rest, so see you later", cancellationToken: cancellationToken);
        return NoContent();
    }
}
