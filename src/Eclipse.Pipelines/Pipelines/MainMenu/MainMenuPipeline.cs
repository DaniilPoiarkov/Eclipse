using Eclipse.Core.Attributes;

namespace Eclipse.Pipelines.Pipelines.MainMenu;

[Route("Menu:MainMenu", "/main_menu")]
internal sealed class MainMenuPipeline : EclipsePipelineBase
{
    private static readonly string _message = "Pipelines:MainMenu";

    protected override void Initialize()
    {
        RegisterStage(_ => Menu(MainMenuButtons, Localizer[_message]));
    }
}
