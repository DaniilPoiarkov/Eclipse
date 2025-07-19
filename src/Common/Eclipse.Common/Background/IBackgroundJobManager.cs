namespace Eclipse.Common.Background;

public interface IBackgroundJobManager
{
    Task EnqueueAsync<TBackgroundJob, TArgs>(TArgs args, CancellationToken cancellationToken = default)
        where TBackgroundJob : IBackgroundJob<TArgs>;

    Task EnqueueAsync<TBackgroundJob>(CancellationToken cancellationToken = default)
        where TBackgroundJob : IBackgroundJob;
}
