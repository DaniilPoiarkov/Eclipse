namespace Eclipse.Infrastructure.Telegram;

public class SendMessageModel
{
    public string Message { get; set; } = string.Empty;

    public long ChatId { get; set; }
}
