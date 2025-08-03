using Eclipse.Application.Contracts.InboxMessages;
using Eclipse.Domain.InboxMessages;
using Eclipse.Domain.Shared.InboxMessages;

namespace Eclipse.Application.InboxMessages;

internal sealed class InboxMessageService : IInboxMessageService
{
    private readonly IInboxMessageRepository _repository;

    public InboxMessageService(IInboxMessageRepository repository)
    {
        _repository = repository;
    }

    public Task DeleteProcessedAsync(CancellationToken cancellationToken = default)
    {
        return _repository.DeleteSuccessfullyProcessedAsync(cancellationToken);
    }

    public async Task ResetFailedAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _repository.GetByExpressionAsync(m => m.Status == InboxMessageStatus.Failed, cancellationToken);

        foreach (var message in messages)
        {
            message.Reset();
        }

        await _repository.UpdateRangeAsync(messages, cancellationToken);
    }
}
