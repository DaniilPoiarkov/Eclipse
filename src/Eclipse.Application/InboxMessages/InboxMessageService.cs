using Eclipse.Application.Contracts.InboxMessages;
using Eclipse.Domain.InboxMessages;

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
}
