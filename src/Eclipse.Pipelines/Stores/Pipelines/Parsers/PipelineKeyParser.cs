using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Stores.Pipelines.Parsers;

internal sealed class PipelineKeyParser : IPipelineKeyParser
{
    public string Parse(Update update)
    {
        if (update.Message is null)
        {
            throw new InvalidOperationException("Update does not contain a message.");
        }

        return $"pipeline:{update.Message.Chat.Id}";
    }
}
