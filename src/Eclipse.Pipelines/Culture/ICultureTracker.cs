namespace Eclipse.Pipelines.Culture;

public interface ICultureTracker
{
    Task ResetAsync(long chatId, string culture, CancellationToken cancellationToken = default);

    Task<string?> GetAsync(long chatId, CancellationToken cancellationToken = default);
}
