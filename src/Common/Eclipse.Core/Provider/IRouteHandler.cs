using Eclipse.Core.Pipelines;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Core.Provider;

internal interface IRouteHandler
{
    UpdateType Type { get; }

    PipelineBase? Get(Update update);
}
