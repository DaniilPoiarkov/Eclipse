using Eclipse.Common.Background;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Pipelines.Pipelines.MainMenu.Reports.ExportMoodReport;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Reports;

[Route("Menu:Reports:Mood", "/reports_mood")]
internal sealed class MoodReportPipeline : ReportsPipelineBase
{
    private readonly IBackgroundJobManager _jobManager;

    public MoodReportPipeline(IBackgroundJobManager jobManager)
    {
        _jobManager = jobManager;
    }

    protected override void Initialize()
    {
        RegisterStage(EnqueueMoodReportExportAsync);
    }

    private async Task<IResult> EnqueueMoodReportExportAsync(MessageContext context, CancellationToken cancellationToken)
    {
        await _jobManager.EnqueueAsync<ExportMoodReportBackgroundJob, ExportMoodReportBackgroundJobArgs>(new ExportMoodReportBackgroundJobArgs
        {
            ChatId = context.ChatId
        }, cancellationToken);

        return Text(Localizer["Pipelines:Reports:Mood:InProgress"]);
    }
}
