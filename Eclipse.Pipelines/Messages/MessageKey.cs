namespace Eclipse.Pipelines.Messages;

public class MessageKey
{
    public long ChatId { get; }

    internal MessageKey(long chatId)
    {
        ChatId = chatId;
    }
}
