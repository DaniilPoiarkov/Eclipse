namespace Eclipse.Pipelines.Culture;

// TODO: Review.
public interface ICultureTracker
{
    Task ResetAsync(long chatId, string culture, CancellationToken cancellationToken = default);

    ValueTask<string> GetAsync(long chatId, CancellationToken cancellationToken = default);
}
