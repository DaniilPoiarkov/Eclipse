using Eclipse.Domain.Shared.Events;

namespace Eclipse.Domain.Suggestions;

public sealed record NewSuggestionSentDomainEvent(Guid SuggestionId, long ChatId, string Text) : IDomainEvent;
