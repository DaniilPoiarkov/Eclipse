namespace Eclipse.Common.Telegram;

public sealed class TelegramOptions
{
    public required string Token { get; set; }

    public long Chat { get; set; }
}
