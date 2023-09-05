namespace Eclipse.Application.Contracts.Telegram;

public class PipelineKey
{
    public long ChatId { get; }

    public PipelineKey(long chatId)
    {
        ChatId = chatId;
    }
}
