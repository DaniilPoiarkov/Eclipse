using Eclipse.Application.Contracts.InboxMessages;
using Eclipse.Common.Clock;
using Eclipse.Common.Events;
using Eclipse.Domain.InboxMessages;
using Eclipse.Domain.OutboxMessages;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Eclipse.Application.InboxMessages;

internal sealed class InboxMessageConvertor : IInboxMessageConvertor
{
    private readonly IOutboxMessageRepository _outboxMessageRepository;

    private readonly IInboxMessageRepository _inboxMessageRepository;

    private readonly IServiceProvider _serviceProvider;

    private readonly ITimeProvider _timeProvider;

    private readonly ILogger<InboxMessageConvertor> _logger;

    public InboxMessageConvertor(
        IOutboxMessageRepository outboxMessageRepository,
        IInboxMessageRepository inboxMessageRepository,
        IServiceProvider serviceProvider,
        ITimeProvider timeProvider,
        ILogger<InboxMessageConvertor> logger)
    {
        _outboxMessageRepository = outboxMessageRepository;
        _inboxMessageRepository = inboxMessageRepository;
        _serviceProvider = serviceProvider;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<OutboxToInboxConversionResult> ConvertAsync(int count, CancellationToken cancellationToken = default)
    {
        var outboxMessages = await _outboxMessageRepository.GetNotProcessedAsync(count, cancellationToken);

        if (outboxMessages.IsNullOrEmpty())
        {
            return OutboxToInboxConversionResult.Empty;
        }

        var converted = new List<InboxMessage>();

        using var scope = _serviceProvider.CreateAsyncScope();

        foreach (var outboxMessage in outboxMessages)
        {
            var payloadType = Type.GetType(outboxMessage.Type);

            if (payloadType is null)
            {
                _logger.LogError("Cannot resolve payload type \'{Type}\' from {Message} with Id \'{Id}\'", outboxMessage.Type, nameof(OutboxMessage), outboxMessage.Id);
                outboxMessage.SetError("Cannot resolve payload type during converting to inbox message.", _timeProvider.Now);
                continue;
            }

            var type = typeof(IEventHandler<>).MakeGenericType(payloadType);

            var inboxMessages = scope.ServiceProvider.GetServices(type)
                .Select(handler => handler?.GetType().FullName)
                .Where(handlerName => !handlerName.IsNullOrEmpty())
                .Select(handlerName =>
                    InboxMessage.Create(
                        Guid.CreateVersion7(),
                        outboxMessage.Id,
                        handlerName!,
                        outboxMessage.JsonContent,
                        outboxMessage.Type,
                        outboxMessage.OccuredAt
                    )
                );

            converted.AddRange(inboxMessages);

            outboxMessage.SetProcessed(_timeProvider.Now);
        }

        await _outboxMessageRepository.UpdateRangeAsync(outboxMessages, cancellationToken);
        await _inboxMessageRepository.CreateRangeAsync(converted, cancellationToken);

        var errors = outboxMessages
            .Where(m => !m.Error.IsNullOrEmpty())
            .Select(m => m.Error!)
            .ToList();

        return new OutboxToInboxConversionResult(
            outboxMessages.Count,
            outboxMessages.Where(m => m.Error.IsNullOrEmpty()).Count(),
            errors.Count,
            errors
        );
    }
}
