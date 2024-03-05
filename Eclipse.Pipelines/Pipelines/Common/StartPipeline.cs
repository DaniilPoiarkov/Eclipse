using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.Common;

[Route("Start", "/start")]
public sealed class StartPipeline : EclipsePipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(Start);
    }

    private IResult Start(MessageContext context)
    {
        var message = Localizer["Pipelines:Common:Start"]
            .Replace("{name}", context.User.Name.TrimEnd());

        return Menu(MainMenuButtons, message);
    }
}
