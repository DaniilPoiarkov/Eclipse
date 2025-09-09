namespace Eclipse.Application.Jobs;

internal interface ISchedulerOptions
{
    Guid UserId { get; }

    TimeSpan Gmt { get; }
}
