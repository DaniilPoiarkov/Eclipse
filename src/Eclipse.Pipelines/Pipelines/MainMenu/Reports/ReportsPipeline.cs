using Eclipse.Core.Attributes;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Reports;

[Route("Menu:MainMenu:Reports")]
internal sealed class ReportsPipeline : ReportsPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Menu(ReportsMenuButtons, Localizer["Pipelines:Reports"]));
    }
}
