using Eclipse.Core.Attributes;
using Eclipse.Pipelines.Pipelines.MainMenu.Reports;

namespace Eclipse.Pipelines.Pipelines.MainMenu;

[Route("Menu:MainMenu:Reports")]
internal sealed class ReportsPipeline : ReportsPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Menu(ReportsMenuButtons, Localizer["Pipelines:Reports"]));
    }
}
