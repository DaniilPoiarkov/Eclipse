using Eclipse.Application.Contracts.InboxMessages;
using Eclipse.Common.Clock;
using Eclipse.Common.Events;
using Eclipse.Domain.InboxMessages;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Eclipse.Application.InboxMessages;

internal sealed class InboxMessageProcessor : IInboxMessageProcessor
{
    private readonly IInboxMessageRepository _repository;

    private readonly ITimeProvider _timeProvider;

    private readonly IEnumerable<IEventHandler<IDomainEvent>> _handlers;

    private readonly ILogger<InboxMessageProcessor> _logger;

    public InboxMessageProcessor(
        IInboxMessageRepository repository,
        ITimeProvider timeProvider,
        IEnumerable<IEventHandler<IDomainEvent>> handlers,
        ILogger<InboxMessageProcessor> logger)
    {
        _repository = repository;
        _timeProvider = timeProvider;
        _handlers = handlers;
        _logger = logger;
    }

    public async Task<ProcessInboxMessagesResult> ProcessAsync(int count, CancellationToken cancellationToken = default)
    {
        var inboxMessages = await _repository.GetPendingAsync(count, cancellationToken);

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
            var payload = JsonConvert.DeserializeObject<IDomainEvent>(inboxMessage.Payload, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
            });

            if (payload is null)
            {
                _logger.LogError("Cannot deserialize payload type \'{Type}\' from {Message} with Id \'{Id}\'", inboxMessage.Type, nameof(InboxMessage), inboxMessage.Id);
                inboxMessage.SetError("Cannot deserialize the payload.", _timeProvider.Now);
                continue;
            }

            var handler = _handlers.FirstOrDefault(h => h.GetType().FullName == inboxMessage.HandlerName);

            if (handler is null)
            {
                _logger.LogError("Cannot retrieve handler \'{Handler}\' for {Message} with Id \'{Id}\'", inboxMessage.HandlerName, nameof(InboxMessage), inboxMessage.Id);
                inboxMessage.SetError("Cannot retrieve handler.", _timeProvider.Now);
                continue;
            }

            await handler.Handle(payload, cancellationToken);

            inboxMessage.SetProcessed(_timeProvider.Now);
        }

        await _repository.UpdateRangeAsync(inboxMessages, cancellationToken);

        var errors = inboxMessages
            .Where(m => !m.Error.IsNullOrEmpty())
            .Select(m => m.Error!)
            .ToList();

        return new ProcessInboxMessagesResult(
            inboxMessages.Count,
            inboxMessages.Where(m => m.Error.IsNullOrEmpty()).Count(),
            errors.Count,
            errors
        );
    }
}
