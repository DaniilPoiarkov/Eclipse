namespace Eclipse.Application.Contracts.Telegram.Messages;

public class MessageKey
{
    public long ChatId { get; }

    public MessageKey(long chatId)
    {
        ChatId = chatId;
    }
}
