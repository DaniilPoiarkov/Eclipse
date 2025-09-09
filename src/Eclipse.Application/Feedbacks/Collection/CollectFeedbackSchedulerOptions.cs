using Eclipse.Application.Jobs;

namespace Eclipse.Application.Feedbacks.Collection;

internal sealed record CollectFeedbackSchedulerOptions(Guid UserId, TimeSpan Gmt) : ISchedulerOptions;
