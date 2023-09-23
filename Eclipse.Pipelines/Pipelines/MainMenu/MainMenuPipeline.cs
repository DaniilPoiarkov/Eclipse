using Eclipse.Core.Attributes;
using Eclipse.Localization.Localizers;

namespace Eclipse.Pipelines.Pipelines.MainMenu;

[Route("Menu:MainMenu", "/main_menu")]
internal class MainMenuPipeline : EclipsePipelineBase
{
    private static readonly string _message = "Pipelines:MainMenu";

    private readonly ILocalizer _localizer;

    public MainMenuPipeline(ILocalizer localizer)
    {
        _localizer = localizer;
    }

    protected override void Initialize()
    {
        RegisterStage(_ => Menu(MainMenuButtons, _localizer[_message]));
    }
}
