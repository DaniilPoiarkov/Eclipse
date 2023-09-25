using Eclipse.Core.Attributes;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.View;

[Route("Menu:AdminMenu:View", "/admin_view")]
internal class AdminViewPipeline : AdminPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Menu(ViewButtons, Localizer["Pipelines:AdminMenu:View"]));
    }
}
