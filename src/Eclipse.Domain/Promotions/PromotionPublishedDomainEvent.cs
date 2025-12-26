using Eclipse.Common.Events;

namespace Eclipse.Domain.Promotions;

public sealed record PromotionPublishedDomainEvent(Guid PromotionId) : IDomainEvent;
