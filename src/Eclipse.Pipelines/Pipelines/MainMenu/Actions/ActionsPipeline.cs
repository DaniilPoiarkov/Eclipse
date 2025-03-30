using Eclipse.Core.Routing;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Actions;

[Route("Menu:MainMenu:Actions", "/actions")]
internal sealed class ActionsPipeline : ActionsPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Menu(ActionsMenuButtons, Localizer["Pipelines:Actions"]));
    }
}
