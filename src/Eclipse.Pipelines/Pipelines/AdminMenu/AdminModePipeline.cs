using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.AdminMenu;

[Route("Menu:AdminMenu", "/admin_mode")]
internal sealed class AdminModePipeline : AdminPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(SendAdminMenu);
    }

    private IResult SendAdminMenu(MessageContext context)
    {
        return Menu(AdminMenuButtons, Localizer["Pipelines:AdminMenu"]);
    }
}
