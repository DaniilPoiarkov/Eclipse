using Eclipse.Common.Background;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Pipelines.Jobs.OneOffs;
using Eclipse.Pipelines.Jobs.OneOffs.ExportTodoItems;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Export;

[Route("Menu:AdminMenu:Export:TodoItems", "/admin_export_todoitems")]
internal sealed class ExportTodoItemsPipeline : AdminPipelineBase
{
    private readonly IBackgroundJobManager _jobManager;

    public ExportTodoItemsPipeline(IBackgroundJobManager jobManager)
    {
        _jobManager = jobManager;
    }

    protected override void Initialize()
    {
        RegisterStage(ExportAsync);
    }

    private async Task<IResult> ExportAsync(MessageContext context, CancellationToken cancellationToken)
    {
        await _jobManager.EnqueueAsync<ExportTodoItemsBackgroundJob, ExportToUserBackgroundJobArgs>(
            new ExportToUserBackgroundJobArgs { ChatId = context.ChatId },
            cancellationToken
        );

        return Text(Localizer["Pipelines:Admin:Export:Requested"]);
    }
}
