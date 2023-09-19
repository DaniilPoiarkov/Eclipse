using Eclipse.Core.Attributes;

namespace Eclipse.Pipelines.Pipelines.MainMenu;

[Route("Main menu", "/main_menu")]
internal class MainMenuPipeline : EclipsePipelineBase
{
    private static readonly string _message = "Yep, we are here";

    protected override void Initialize()
    {
        RegisterStage(_ => Menu(MainMenuButtons, _message));
    }
}
