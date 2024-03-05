namespace Eclipse.Telegram.Application.Contracts;

[Serializable]
public sealed class SendMessageModel
{
    public string Message { get; set; } = string.Empty;

    public long ChatId { get; set; }
}
