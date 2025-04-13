namespace Eclipse.Common.Background;

public interface IBackgroundJob<TArgs>
{
    Task ExecuteAsync(TArgs args, CancellationToken cancellationToken = default);
}

public interface IBackgroundJob
{
    Task Execute(CancellationToken cancellationToken = default);
}
