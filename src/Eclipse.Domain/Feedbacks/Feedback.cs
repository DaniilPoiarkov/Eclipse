using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.Feedbacks;

namespace Eclipse.Domain.Feedbacks;

public sealed class Feedback : AggregateRoot
{
    public Guid UserId { get; init; }

    public string? Comment { get; init; }

    public FeedbackRate Rate { get; init; }

    private Feedback(Guid id, Guid userId, string? comment, FeedbackRate rate) : base(id)
    {
        UserId = userId;
        Comment = comment;
        Rate = rate;
    }

    private Feedback() { }

    public static Feedback Create(Guid userId, string? comment, FeedbackRate rate)
    {
        var feedback = new Feedback(Guid.NewGuid(), userId, comment, rate);
        feedback.AddEvent(new FeedbackSentEvent(feedback.Id, userId, comment, rate));
        return feedback;
    }
}
