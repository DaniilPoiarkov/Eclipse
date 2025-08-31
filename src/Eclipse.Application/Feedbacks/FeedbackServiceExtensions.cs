using Eclipse.Application.Contracts.Feedbacks;
using Eclipse.Domain.Feedbacks;

namespace Eclipse.Application.Feedbacks;

internal static class FeedbackServiceExtensions
{
    internal static FeedbackDto ToDto(this Feedback feedback)
    {
        return new FeedbackDto(
            feedback.Id,
            feedback.UserId,
            feedback.Comment,
            feedback.Rate,
            feedback.CreatedAt
        );
    }
}
