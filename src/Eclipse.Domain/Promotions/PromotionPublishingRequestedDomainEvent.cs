using Eclipse.Common.Events;

namespace Eclipse.Domain.Promotions;

public sealed record PromotionPublishingRequestedDomainEvent(Guid PromotionId) : IDomainEvent;
