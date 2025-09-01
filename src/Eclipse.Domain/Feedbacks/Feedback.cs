using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.Feedbacks;

namespace Eclipse.Domain.Feedbacks;

public sealed class Feedback : AggregateRoot, IHasCreatedAt
{
    public Guid UserId { get; init; }

    public string Comment { get; init; } = string.Empty;

    public FeedbackRate Rate { get; init; }

    public DateTime CreatedAt { get; init; }

    private Feedback(Guid id, Guid userId, string comment, FeedbackRate rate, DateTime createdAt) : base(id)
    {
        UserId = userId;
        Comment = comment;
        Rate = rate;
        CreatedAt = createdAt;
    }

    private Feedback() { }

    public static Feedback Create(Guid userId, string comment, FeedbackRate rate, DateTime createdAt)
    {
        var feedback = new Feedback(Guid.CreateVersion7(), userId, comment, rate, createdAt);
        feedback.AddEvent(new FeedbackSentEvent(feedback.Id, userId, comment, rate));
        return feedback;
    }
}
