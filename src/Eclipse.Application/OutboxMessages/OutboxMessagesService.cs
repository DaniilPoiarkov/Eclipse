using Eclipse.Application.Contracts.OutboxMessages;
using Eclipse.Common.Clock;
using Eclipse.Common.EventBus;
using Eclipse.Common.Events;
using Eclipse.Domain.OutboxMessages;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Eclipse.Application.OutboxMessages;

internal sealed class OutboxMessagesService : IOutboxMessagesService
{
    private readonly IOutboxMessageRepository _repository;

    private readonly IEventBus _eventBus;

    private readonly ITimeProvider _timeProvider;

    private readonly ILogger<OutboxMessagesService> _logger;

    public OutboxMessagesService(
        IOutboxMessageRepository repository,
        IEventBus eventBus,
        ITimeProvider timeProvider,
        ILogger<OutboxMessagesService> logger)
    {
        _repository = repository;
        _eventBus = eventBus;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public Task DeleteSuccessfullyProcessedAsync(CancellationToken cancellationToken = default)
    {
        return _repository.DeleteSuccessfullyProcessedAsync(cancellationToken);
    }

    public async Task<ProcessOutboxMessagesResult> ProcessAsync(int count, CancellationToken cancellationToken = default)
    {
        var outboxMessages = await _repository.GetNotProcessedAsync(count, cancellationToken);

        foreach (var outboxMessage in outboxMessages)
        {
            await ProcessMessageAsync(outboxMessage, cancellationToken);
        }

        await _repository.UpdateRangeAsync(outboxMessages, cancellationToken);

        var failed = outboxMessages.Where(m => m.Error is not null);

        return new ProcessOutboxMessagesResult(
            outboxMessages.Count,
            outboxMessages.Where(m => m.Error is null).Count(),
            failed.Count(),
            failed.Select(m => m.Error!)
        );
    }

    private async Task ProcessMessageAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
    {
        try
        {
            var message = JsonConvert.DeserializeObject<IDomainEvent>(outboxMessage.JsonContent, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
            });

            if (message is null)
            {
                _logger.LogError("Error during publishing event:\n\r{error}", "event is null");
                return;
            }

            await _eventBus.Publish(message, cancellationToken);
            outboxMessage.SetProcessed(_timeProvider.Now);
        }
        catch (Exception ex)
        {
            outboxMessage.SetError(ex.Message, _timeProvider.Now);
            _logger.LogError(ex, "Error during publishing event:\n\r{error}", ex.Message);
        }
    }
}
