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

    public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        await _service.DeleteProcessedAsync(cancellationToken);
    }
}
