using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Stores.Pipelines;

internal interface IPipelineKeyParser
{
    string Parse(Update update);
}
