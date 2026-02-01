using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;

namespace Eclipse.Pipelines.Pipelines.AdminMenu;

[Route("Menu:Admin", "/admin_mode")]
internal sealed class AdminModePipeline : AdminPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(SendAdminMenu);
    }

    private IResult SendAdminMenu(MessageContext context)
    {
        return Menu(AdminMenuButtons, Localizer["Pipelines:Admin"]);
    }
}
