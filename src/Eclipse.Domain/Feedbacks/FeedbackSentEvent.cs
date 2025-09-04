using Eclipse.Common.Events;
using Eclipse.Domain.Shared.Feedbacks;

namespace Eclipse.Domain.Feedbacks;

public sealed record FeedbackSentEvent(
    Guid FeedbackId,
    Guid UserId,
    string Comment,
    FeedbackRate Rate
) : IDomainEvent;
