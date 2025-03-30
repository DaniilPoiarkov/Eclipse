using Eclipse.Core.Pipelines;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Core.Provider.Handlers;

internal sealed class UnknownHandler : IProviderHandler
{
    public UpdateType Type => UpdateType.Unknown;

    public PipelineBase? Get(Update update) => null;
}
