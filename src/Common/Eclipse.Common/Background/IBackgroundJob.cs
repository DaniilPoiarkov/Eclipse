namespace Eclipse.Common.Background;

public interface IBackgroundJob<TArgs>
{
    Task ExecuteAsync(TArgs args, CancellationToken cancellationToken = default);
}
