using Telegram.Bot.Types;

namespace Eclipse.Core.Pipelines;

/// <summary>
/// Allows to configure pipeline before it's execution.
/// </summary>
public interface IPipelinePreConfigurator
{
    void Configure(Update update, IPipeline pipeline);
}
