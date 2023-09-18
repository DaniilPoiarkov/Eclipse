using Eclipse.Core.Attributes;

namespace Eclipse.Pipelines.Pipelines.AdminMenu;

[Route("Switch to user mode", "/user_mode")]
internal class UserModePipeline : AdminPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Menu(MainMenuButtons, "Switched to user menu"));
    }
}
