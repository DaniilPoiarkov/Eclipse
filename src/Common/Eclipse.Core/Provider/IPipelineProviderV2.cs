using Eclipse.Core.Pipelines;

using Telegram.Bot.Types;

namespace Eclipse.Core.Provider;

public interface IPipelineProviderV2
{
    PipelineBase Get(Update update);
}
