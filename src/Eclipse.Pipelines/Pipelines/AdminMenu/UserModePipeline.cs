using Eclipse.Core.Attributes;

namespace Eclipse.Pipelines.Pipelines.AdminMenu;

[Route("Menu:AdminMenu:SwitchToUserMode", "/user_mode")]
internal sealed class UserModePipeline : AdminPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Menu(MainMenuButtons, Localizer["Pipelines:AdminMenu:UserMode"]));
    }
}
