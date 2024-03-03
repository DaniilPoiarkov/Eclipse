using Eclipse.Common.EventBus;
using Eclipse.Suggestions.Domain;
using Eclipse.Suggestions.IntegrationEvents;

using MediatR;

namespace Eclipse.Suggestions.Application;

internal sealed class NewSuggestionSentEventHandler : INotificationHandler<NewSuggestionSentDomainEvent>
{
    private readonly IEventBus _eventBus;

    public NewSuggestionSentEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task Handle(NewSuggestionSentDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new NewSuggestionSentIntegrationEvent(
            notification.SuggestionId,
            notification.ChatId,
            notification.Text);

        return _eventBus.Publish(@event, cancellationToken);
    }
}
