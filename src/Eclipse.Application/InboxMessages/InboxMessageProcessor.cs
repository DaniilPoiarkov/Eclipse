using Eclipse.Application.Contracts.InboxMessages;
using Eclipse.Common.Clock;
using Eclipse.Common.Events;
using Eclipse.Domain.InboxMessages;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Eclipse.Application.InboxMessages;

internal sealed class InboxMessageProcessor : IInboxMessageProcessor
{
    private readonly IInboxMessageRepository _repository;

    private readonly ITimeProvider _timeProvider;

    private readonly IServiceProvider _serviceProvider;

    private readonly ILogger<InboxMessageProcessor> _logger;

    public InboxMessageProcessor(
        IInboxMessageRepository repository,
        ITimeProvider timeProvider,
        IServiceProvider serviceProvider,
        ILogger<InboxMessageProcessor> logger)
    {
        _repository = repository;
        _timeProvider = timeProvider;
        _serviceProvider = serviceProvider;
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

        var handlers = new Dictionary<Type, IEnumerable<IEventHandler<IDomainEvent>>>();

        using var scope = _serviceProvider.CreateAsyncScope();

        foreach (var inboxMessage in inboxMessages)
        {
            var payloadType = Type.GetType(inboxMessage.Type);

            if (payloadType is null)
            {
                _logger.LogError("Cannot resolve payload type \'{Type}\' from {Message} with Id \'{Id}\'", inboxMessage.Type, nameof(InboxMessage), inboxMessage.Id);
                inboxMessage.SetError("Cannot resolve payload type during converting to inbox message.", _timeProvider.Now);
                continue;
            }

            var payload = JsonConvert.DeserializeObject<IDomainEvent>(inboxMessage.Payload, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
            });

            if (payload is null)
            {
                inboxMessage.SetError("Cannot deserialize the payload.", _timeProvider.Now);
                continue;
            }

            var type = typeof(IEventHandler<>).MakeGenericType(payloadType);

            if (!handlers.TryGetValue(type, out _))
            {
                handlers[type] = scope.ServiceProvider.GetServices(type).Cast<IEventHandler<IDomainEvent>>();
            }

            var handler = handlers[type].FirstOrDefault(handler => handler.GetType().FullName == inboxMessage.HandlerName);

            if (handler is null)
            {
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
