using Eclipse.Core.Routing;

namespace Eclipse.Pipelines.Pipelines.Common;

[Route("Menu:Guide", "/guide")]
internal sealed class GuidePipeline : EclipsePipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Menu(MainMenuButtons, Localizer["Guide"]));
    }
}
