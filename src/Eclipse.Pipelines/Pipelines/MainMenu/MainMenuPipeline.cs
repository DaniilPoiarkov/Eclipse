using Eclipse.Core.Routing;

namespace Eclipse.Pipelines.Pipelines.MainMenu;

[Route("Menu:MainMenu", "/main_menu")]
internal sealed class MainMenuPipeline : EclipsePipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Menu(MainMenuButtons, Localizer["Pipelines:MainMenu"]));
    }
}
