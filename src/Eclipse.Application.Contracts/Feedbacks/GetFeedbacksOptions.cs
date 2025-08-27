namespace Eclipse.Application.Contracts.Feedbacks;

public sealed class GetFeedbacksOptions
{
    public IEnumerable<Guid> UserIds { get; init; } = [];

    public DateTime? From { get; init; }

    public DateTime? To { get; init; }
}
