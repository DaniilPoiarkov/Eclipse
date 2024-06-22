namespace Eclipse.Common.Background;

public interface IBackgroundJob<TArgs>
{
    Task ExecureAsync(TArgs args, CancellationToken cancellationToken = default);
}
