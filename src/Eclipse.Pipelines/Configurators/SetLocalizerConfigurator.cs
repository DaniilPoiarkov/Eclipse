using Eclipse.Core.Pipelines;
using Eclipse.Pipelines.Pipelines;

using Microsoft.Extensions.Localization;

using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Configurators;

public sealed class SetLocalizerConfigurator : IPipelinePreConfigurator
{
    private readonly IStringLocalizer<SetLocalizerConfigurator> _stringLocalizer;

    public SetLocalizerConfigurator(IStringLocalizer<SetLocalizerConfigurator> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
    }

    public void Configure(Update update, PipelineBase pipeline)
    {
        if (pipeline is EclipsePipelineBase eclipsePipeline)
        {
            eclipsePipeline.SetLocalizer(_stringLocalizer);
        }
    }
}
