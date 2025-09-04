namespace Eclipse.Application.Feedbacks.Collection;

public interface IFeedbackCollector
{
    Task CollectAsync(Guid userId, CancellationToken cancellationToken = default);
}
