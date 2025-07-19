using Eclipse.Common.Background;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Pipelines.Jobs.OneOffs;
using Eclipse.Pipelines.Jobs.OneOffs.ExportReminders;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Export;

[Route("Menu:AdminMenu:Export:Reminders", "/admin_export_reminders")]
internal sealed class ExportRemindersPipeline : AdminPipelineBase
{
    private readonly IBackgroundJobManager _jobManager;

    public ExportRemindersPipeline(IBackgroundJobManager jobManager)
    {
        _jobManager = jobManager;
    }

    protected override void Initialize()
    {
        RegisterStage(ExportAsync);
    }

    private async Task<IResult> ExportAsync(MessageContext context, CancellationToken cancellationToken)
    {
        await _jobManager.EnqueueAsync<ExportRemindersBackgroundJob, ExportToUserBackgroundJobArgs>(
            new ExportToUserBackgroundJobArgs { ChatId = context.ChatId },
            cancellationToken
        );

        return Text(Localizer["Pipelines:Admin:Export:Requested"]);
    }
}
