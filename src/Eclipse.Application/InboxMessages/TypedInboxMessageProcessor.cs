using Eclipse.Application.Contracts.InboxMessages;
using Eclipse.Common.Clock;
using Eclipse.Common.Events;
using Eclipse.Domain.InboxMessages;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Eclipse.Application.InboxMessages;

internal sealed class TypedInboxMessageProcessor<TEvent, TEventHandler> : IInboxMessageProcessor<TEvent, TEventHandler>
    where TEvent : IDomainEvent
    where TEventHandler : IEventHandler<TEvent>
{
    private readonly IInboxMessageRepository _repository;

    private readonly TEventHandler _eventHandler;

    private readonly ITimeProvider _timeProvider;

    private readonly ILogger<TypedInboxMessageProcessor<TEvent, TEventHandler>> _logger;

    public TypedInboxMessageProcessor(
        IInboxMessageRepository repository,
        TEventHandler eventHandler,
        ITimeProvider timeProvider,
        ILogger<TypedInboxMessageProcessor<TEvent, TEventHandler>> logger)
    {
        _repository = repository;
        _eventHandler = eventHandler;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<ProcessInboxMessagesResult> ProcessAsync(int count, CancellationToken cancellationToken = default)
    {
        var inboxMessages = await _repository.GetPendingAsync(count, _eventHandler.GetType().FullName, cancellationToken);

        if (inboxMessages.IsNullOrEmpty())
        {
            return ProcessInboxMessagesResult.Empty;
        }

        foreach (var message in inboxMessages)
        {
            message.SetInProcess();
        }

        await _repository.UpdateRangeAsync(inboxMessages, cancellationToken);

        foreach (var inboxMessage in inboxMessages)
        {
            try
            {
                var payload = JsonConvert.DeserializeObject<TEvent>(inboxMessage.Payload, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                });

                if (payload is null)
                {
                    _logger.LogError("Cannot deserialize payload type \'{Type}\' from {Message} with Id \'{Id}\'", inboxMessage.Type, nameof(InboxMessage), inboxMessage.Id);
                    inboxMessage.SetError("Cannot deserialize the payload.", _timeProvider.Now);
                    continue;
                }

                await _eventHandler.Handle(payload, cancellationToken);

                inboxMessage.SetProcessed(_timeProvider.Now);
            }
            catch (Exception ex)
            {
                inboxMessage.SetError(JsonConvert.SerializeObject(ex), _timeProvider.Now);
            }
        }

        await _repository.UpdateRangeAsync(inboxMessages, cancellationToken);

        var errors = inboxMessages
            .Where(m => !m.Error.IsNullOrEmpty())
            .Select(m => m.Error!)
            .ToList();

        return new ProcessInboxMessagesResult(
            inboxMessages.Count,
            inboxMessages.Count - errors.Count,
            errors.Count,
            errors
        );
    }
}
