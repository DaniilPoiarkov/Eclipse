using Eclipse.Domain.Shared.Feedbacks;

namespace Eclipse.Application.Contracts.Feedbacks;

public record CreateFeedbackModel(
    string Comment,
    FeedbackRate Rate
);
