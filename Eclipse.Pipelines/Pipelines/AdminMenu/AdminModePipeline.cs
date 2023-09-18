using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.AdminMenu;

[Route("", "/admin_mode")]
internal class AdminModePipeline : AdminPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(SendAdminMenu);
    }

    private IResult SendAdminMenu(MessageContext context)
    {
        return Menu(AdminMenuButtons, "Switched to admin mode");
    }
}
