using Eclipse.Application.Contracts.InboxMessages;

using Quartz;

namespace Eclipse.Application.InboxMessages;

internal sealed class DeleteProcessedInboxMessagesJob : IJob
{
    private readonly IInboxMessageService _service;

    public DeleteProcessedInboxMessagesJob(IInboxMessageService service)
    {
        _service = service;
    }

    public Task Execute(IJobExecutionContext context)
    {
        return _service.DeleteProcessedAsync(context.CancellationToken);
    }
}
