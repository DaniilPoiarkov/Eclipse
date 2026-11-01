using Eclipse.Application.Contracts.OutboxMessages;

using Quartz;

namespace Eclipse.Application.OutboxMessages;

internal sealed class DeleteSuccessfullyProcessedOutboxMessagesJob : IJob
{
    private readonly IOutboxMessagesService _outboxMessagesService;

    public DeleteSuccessfullyProcessedOutboxMessagesJob(IOutboxMessagesService outboxMessagesService)
    {
        _outboxMessagesService = outboxMessagesService;
    }

    public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        await _outboxMessagesService.DeleteSuccessfullyProcessedAsync(cancellationToken);
    }
}
