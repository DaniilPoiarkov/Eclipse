using Eclipse.Common.Background;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Pipelines.Jobs.OneOffs;
using Eclipse.Pipelines.Jobs.OneOffs.ExportUsers;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Export;

[Route("Menu:AdminMenu:Export:Users", "/admin_export_users")]
public sealed class ExportUsersPipeline : AdminPipelineBase
{
    private readonly IBackgroundJobManager _jobManager;

    public ExportUsersPipeline(IBackgroundJobManager jobManager)
    {
        _jobManager = jobManager;
    }

    protected override void Initialize()
    {
        RegisterStage(RequestExportUsersJob);
    }

    private async Task<IResult> RequestExportUsersJob(MessageContext context, CancellationToken cancellationToken)
    {
        await _jobManager.EnqueueAsync<ExportUsersBackgroundJob, ExportToUserBackgroundJobArgs>(
            new ExportToUserBackgroundJobArgs { ChatId = context.ChatId },
            cancellationToken
        );

        return Text(Localizer["Pipelines:Admin:Export:Requested"]);
    }
}
