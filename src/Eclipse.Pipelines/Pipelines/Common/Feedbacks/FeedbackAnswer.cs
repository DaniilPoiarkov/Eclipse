using Eclipse.Domain.Shared.Feedbacks;

namespace Eclipse.Pipelines.Pipelines.Common.Feedbacks;

internal sealed record FeedbackAnswer(FeedbackRate? Rate, string Message);
