using Eclipse.Application.Contracts.OutboxMessages;
using Quartz;

namespace Eclipse.Application.OutboxMessages.DeleteSuccessfullyProcessedMessages;

internal sealed class DeleteSuccessfullyProcessedOutboxMessagesJob : IJob
{
    private readonly IOutboxMessagesService _outboxMessagesService;

    public DeleteSuccessfullyProcessedOutboxMessagesJob(IOutboxMessagesService outboxMessagesService)
    {
        _outboxMessagesService = outboxMessagesService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _outboxMessagesService.DeleteSuccessfullyProcessedAsync(context.CancellationToken);
    }
}
