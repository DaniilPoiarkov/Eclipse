using Telegram.Bot.Types;

namespace Eclipse.Core.Models;

public class TelegramUser
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Username { get; set; } = string.Empty;

    public TelegramUser(Update update)
    {
        var message = update.Message!;

        Id = message.Chat.Id;

        var from = message.From!;

        Name = $"{from.FirstName} {from.LastName}";
        Username = from.Username;
    }
}
