using Eclipse.Application.Contracts.OutboxMessages;
using Eclipse.Domain.OutboxMessages;

namespace Eclipse.Application.OutboxMessages;

internal sealed class OutboxMessagesService : IOutboxMessagesService
{
    private readonly IOutboxMessageRepository _repository;

    public OutboxMessagesService(IOutboxMessageRepository repository)
    {
        _repository = repository;
    }

    public Task DeleteSuccessfullyProcessedAsync(CancellationToken cancellationToken = default)
    {
        return _repository.DeleteSuccessfullyProcessedAsync(cancellationToken);
    }
}
