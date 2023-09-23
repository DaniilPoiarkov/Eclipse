using Eclipse.Core.Attributes;

namespace Eclipse.Pipelines.Pipelines.AdminMenu;

[Route("Menu:AdminMenu:SwitchToUserMode", "/user_mode")]
internal class UserModePipeline : AdminPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Menu(MainMenuButtons, "Switched to user mode"));
    }
}
