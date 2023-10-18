namespace Eclipse.Pipelines.Pipelines.Store;

public class PipelineKey
{
    public long ChatId { get; }

    public PipelineKey(long chatId)
    {
        ChatId = chatId;
    }
}
