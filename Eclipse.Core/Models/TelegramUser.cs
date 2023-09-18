using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Core.Models;

public class TelegramUser
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Username { get; set; } = string.Empty;

    public TelegramUser(long id, string name, string username)
    {
        Id = id;
        Name = name;
        Username = username;
    }

    public static TelegramUser Create(Update update)
    {
        if (update.Type == UpdateType.Message)
        {
            var message = update.Message!;

            var id = message.Chat.Id;

            var from = message.From!;

            var name = $"{from.FirstName} {from.LastName}".Trim();
            var username = from.Username ?? string.Empty;

            return new TelegramUser(id, name, username);
        }

        var callback = update.CallbackQuery!;

        var callbackFrom = callback.From;
        var callbackFromName = $"{callbackFrom.FirstName} {callbackFrom.LastName}".Trim();

        return new TelegramUser(callbackFrom.Id, callbackFromName, callbackFrom.Username ?? string.Empty);
    }
}
