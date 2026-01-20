using Eclipse.Core.Routing;

namespace Eclipse.Pipelines.Pipelines.AdminMenu;

[Route("Menu:Admin:SwitchToUserMode", "/user_mode")]
internal sealed class UserModePipeline : AdminPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Menu(MainMenuButtons, Localizer["Pipelines:Admin:UserMode"]));
    }
}
