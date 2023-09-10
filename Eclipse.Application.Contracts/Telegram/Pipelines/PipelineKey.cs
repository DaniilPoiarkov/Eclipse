namespace Eclipse.Application.Contracts.Telegram.Pipelines;

public class PipelineKey
{
    public long ChatId { get; }

    public PipelineKey(long chatId)
    {
        ChatId = chatId;
    }
}
