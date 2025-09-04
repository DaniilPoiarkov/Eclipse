using Eclipse.Domain.Shared.Feedbacks;

namespace Eclipse.Application.Contracts.Feedbacks;

public record FeedbackDto(
    Guid Id,
    Guid UserId,
    string Comment,
    FeedbackRate Rate,
    DateTime CreatedAt
);
