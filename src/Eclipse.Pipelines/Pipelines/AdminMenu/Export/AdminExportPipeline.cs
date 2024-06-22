using Eclipse.Core.Attributes;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Export;

[Route("Menu:AdminMenu:Export", "/admin_export")]
internal sealed class AdminExportPipeline : AdminPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Menu(ExportButtons, Localizer["Pipelines:AdminMenu:Export"]));
    }
}
